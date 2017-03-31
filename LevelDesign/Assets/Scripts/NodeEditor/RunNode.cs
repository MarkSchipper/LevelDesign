using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_EDITOR
public class RunNode : BaseInputNode
{
    private BaseInputNode input1;
    private Rect input1Rect;
    private GameObject runTo;

    private HasSound _sound;
    private AudioClip _soundSource;

    public RunNode()
    {
        windowTitle = "Run Node";
        hasInputs = true;
    }

   
    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Run to: ");
        runTo = (GameObject)EditorGUILayout.ObjectField(runTo, typeof(GameObject), true);

        if (runTo != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(runTo);
        }

        _sound = (HasSound)EditorGUILayout.EnumPopup("Sound: ", _sound);

        if (_sound == HasSound.Yes)
        {
            _soundSource = (AudioClip)EditorGUILayout.ObjectField(_soundSource, typeof(AudioClip));

            if (_soundSource != null)
            {
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetAudio(_soundSource);
            }
        }

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }


    }

    public void SetWayPoint(GameObject _waypoint)
    {
        runTo = _waypoint;
    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
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

    public void SetSound(string _hasSound)
    {
        if (_hasSound == "No")
        {
            _sound = HasSound.No;
        }
        if (_hasSound == "Yes")
        {
            _sound = HasSound.Yes;
        }
    }

    public override void Tick(float deltaTime)
    {

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