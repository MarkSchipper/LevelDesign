using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_EDITOR

public class SoundTrackNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    private GameObject walkToNode;

    private HasSound _sound;

    private AudioClip _soundSource;

    public SoundTrackNode()
    {

        windowTitle = "Add a global soundtrack";
        hasInputs = false;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Soundtrack ");
        _soundSource = (AudioClip)EditorGUILayout.ObjectField(_soundSource, typeof(AudioClip));

        if (_soundSource != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetAudio(_soundSource);
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().AutoPlay(true);
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
        if (walkToNode != null)
            nodeResult = walkToNode.ToString();

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

    public void SetAudio(AudioClip _clip)
    {
        _soundSource = _clip;
    }

}
#endif