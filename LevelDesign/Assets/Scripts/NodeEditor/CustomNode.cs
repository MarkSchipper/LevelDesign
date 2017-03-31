using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public enum AnimationType
{
    Default,
    Idle,
    Walk,
    Run,
    Melee,
    Ranged,
    Gesture,
}
#if UNITY_EDITOR
public class CustomNode : BaseInputNode
{


    private BaseInputNode input1;
    private Rect input1Rect;

    private AnimationClip _customAnim;
    
    
    private AnimationType _animType;

    private float idleTime;
    private GameObject generalNode;
    private GameObject runTo;
    private GameObject aimAt;

    private HasSound _sound;
    private AudioClip _soundSource;

    public CustomNode()
    {
        windowTitle = "Custom Animation Node";
        hasInputs = true;
        
    }


    public override void DrawWindow()
    {

        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Custom Animation: ");

        _animType = (AnimationType)EditorGUILayout.EnumPopup("Type:", _animType);

        if (_animType == AnimationType.Idle)
        {
            GUILayout.Label("Idle Time: ");
            float.TryParse(EditorGUILayout.TextField("Idle for: ", idleTime.ToString()), out idleTime);

            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Idle");
                        

            if (idleTime > 0)
            {
                
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().setIdleWait(idleTime);
            }
        }

        else if (_animType == AnimationType.Walk)
        {
            GUILayout.Label("Walk to: ");
            generalNode = (GameObject)EditorGUILayout.ObjectField(generalNode, typeof(GameObject), true);

            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Walk");

            if (generalNode != null)
            {
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Walk");
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(generalNode);
            }
        }

        else if (_animType == AnimationType.Run)
        {
            GUILayout.Label("Run to: ");
            generalNode = (GameObject)EditorGUILayout.ObjectField(generalNode, typeof(GameObject), true);

            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Run");

            if (generalNode != null)
            {
                
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(generalNode);
            }
        }

        else if (_animType == AnimationType.Ranged)
        {
            GUILayout.Label("Shoot at: ");
            generalNode = (GameObject)EditorGUILayout.ObjectField(generalNode, typeof(GameObject), true);

            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Ranged");

            if (generalNode != null)
            {
                
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(generalNode);
            }
        }

        else if (_animType == AnimationType.Gesture)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnimType("Gesture");
        }

        GUILayout.Label("Animation: ");
        _customAnim = (AnimationClip)EditorGUILayout.ObjectField(_customAnim, typeof(AnimationClip), true);
        

        if(_customAnim != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetCustomAnim(_customAnim);
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
    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
    }

    public void SetWayPoint(GameObject _go)
    {
        generalNode = _go;
    }

    public void SetIdle(float _time)
    {
        idleTime = _time;
    }

    public void SetCustomAnimation(AnimationClip _cAnim)
    {
        _customAnim = _cAnim;
    }

    public void SetAnimType(string _type)
    {
        if (_type == "Idle")
        {
            _animType = AnimationType.Idle;
        }
        if (_type == "Walk")
        {
            _animType = AnimationType.Walk;
        }
        if (_type == "Run")
        {
            _animType = AnimationType.Run;
        }
        if (_type == "Ranged")
        {
            _animType = AnimationType.Ranged;
        }
        if (_type == "Melee")
        {
            _animType = AnimationType.Melee;
        }
        if (_type == "Gesture")
        {
            _animType = AnimationType.Gesture;
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

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {

            input1 = input;

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