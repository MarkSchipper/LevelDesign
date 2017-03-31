using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public enum ParticleAction
{
    Nothing,
    Enable,
    Disable,
}
#if UNITY_EDITOR
public class ParticleNode : BaseInputNode
{

    private BaseInputNode input1;
    private Rect input1Rect;

    private ParticleAction _pAction;

    private HasSound _hasSound;
    private AudioClip _soundSource;

    [SerializeField]
    private ParticleSystem _pSystem;

    public ParticleNode()
    {
        windowTitle = "Particle System Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Which Particle System?");

        _pSystem = (ParticleSystem)EditorGUILayout.ObjectField(_pSystem, typeof(ParticleSystem), true);

        if(_pSystem != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetParticleSystem(_pSystem);
        }

        GUILayout.Label("What Action?");

        _pAction = (ParticleAction)EditorGUILayout.EnumPopup("Action", _pAction);

        if(_pAction != ParticleAction.Nothing)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetParticleAction(_pAction.ToString());
        }

        GUILayout.Label("Sound:");
        _hasSound = (HasSound)EditorGUILayout.EnumPopup("Sound", _hasSound);

        if (_hasSound == HasSound.Yes)
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

    public void SetParticleSystem(ParticleSystem _particles)
    {
        _pSystem = _particles;
    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
    }

    public void SetParticleAction(string _action)
    {
        if(_action == "Nothing")
        {
            _pAction = ParticleAction.Nothing;
        }
        else if (_action == "Enable")
        {
            _pAction = ParticleAction.Enable;
        }
        else
        {
            _pAction = ParticleAction.Disable;
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

    public override void Tick(float deltaTime)
    {

    }
}
#endif