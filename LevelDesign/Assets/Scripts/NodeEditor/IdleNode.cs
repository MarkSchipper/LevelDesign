using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public enum HasSound
{
    No,
    Yes,
}
#if UNITY_EDITOR

public class IdleNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    [SerializeField]
    private float idleTime;

    private HasSound _sound;

    
    private AudioClip _soundSource;

    public IdleNode()
    {
        windowTitle = "Idle Node";
        hasInputs = true;
    }

   

    public override void DrawWindow()
    {

        base.DrawWindow();

        Event e = Event.current;


        GUILayout.Label("Idle Time: ");
        float.TryParse(EditorGUILayout.TextField("Idle for: ", idleTime.ToString()), out idleTime);

        if(idleTime > 0)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().setIdleWait(idleTime);
        }

        string input1Title = "None";


        if (input1 != null)
        {
            input1Title = input1.getResult();
        }

        _sound = (HasSound)EditorGUILayout.EnumPopup("Sound: ", _sound);

        if(_sound == HasSound.Yes)
        {
            _soundSource = (AudioClip)EditorGUILayout.ObjectField(_soundSource, typeof(AudioClip));

            if(_soundSource != null)
            {
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetAudio(_soundSource);
            }
        }

       // GUILayout.Label(base.ReturnID().ToString());

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }

    }

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {

            input1 = input;

        }

    }

    public void SetIdle(float _time)
    {
        idleTime = _time;
    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public void SetSound(string _hasSound )
    {
        if(_hasSound == "No")
        {
            _sound = HasSound.No;
        }
        if (_hasSound == "Yes")
        {
            _sound = HasSound.Yes;
        }
    }

    public override void DrawCurves()
    {
        if (input1)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y;
            rect.width = 1;
            rect.height = 1;

            NodeEditor.DrawNodeCurve(input1.windowRect, rect);
        }
    }
    }
#endif