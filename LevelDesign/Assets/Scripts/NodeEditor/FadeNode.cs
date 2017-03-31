using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public enum FadeAction
{
    Nothing,
    Teleport,
}
public enum StartAnimation
{
    No,
    Yes,
}
#if UNITY_EDITOR
public class FadeNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    private GameObject teleportTo;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private float _solidTime;

    [SerializeField]
    private Color _fadeColor;

    private Texture2D _fadeTexture;

    private HasSound _sound;

    private AudioClip _soundSource;

    private FadeAction _fadeAction;

    private StartAnimation _startAnim;

    public FadeNode()
    {
        windowTitle = "Fade to Color";
        hasInputs = true;
    }

    public override void DrawWindow()
    {

        base.DrawWindow();

        Event e = Event.current;


        GUILayout.Label("Fade Time: ");
        float.TryParse(EditorGUILayout.TextField("Fade for: ", fadeTime.ToString()), out fadeTime);

        if (fadeTime > 0)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetFadeTime(fadeTime);
        }
        else
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetFadeTime(1);
        }

        GUILayout.Label("Solid colour time: ");
        float.TryParse(EditorGUILayout.TextField("Solid for: ", _solidTime.ToString()), out _solidTime);

        if (_solidTime > 0)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetSolidTime(_solidTime);
        }


        GUILayout.Label("Fade Colour: ");
        _fadeColor = EditorGUILayout.ColorField("New Colour", _fadeColor);

        if(GUILayout.Button("Set Colour"))
        {
            CreateTexture(_fadeColor);
        }

        GUILayout.Label("Action during fade:");
        _fadeAction = (FadeAction)EditorGUILayout.EnumPopup("Action", _fadeAction);

        if(_fadeAction == FadeAction.Teleport)
        {
            teleportTo = (GameObject)EditorGUILayout.ObjectField(teleportTo, typeof(GameObject), true);
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(teleportTo);
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetFadeAction("Teleport");
        }
        /*
        GUILayout.Label("Start next animation during fade?");
        _startAnim = (StartAnimation)EditorGUILayout.EnumPopup("Start", _startAnim);

        GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetFadeAnimStart(_startAnim.ToString());
        */

        


        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }

    }

    void CreateTexture(Color _color)
    {
        
        GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetFadeTexture(_color);
        GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().CanvasCheck();

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

    public void SetFade(float _time)
    {
        fadeTime = _time;
    }

    public void SetSolid(float _solid)
    {
        _solidTime = _solid;
    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
    }

    public void SetFadeAnimStart(string _animStart)
    {
        if(_animStart == "No")
        {
            _startAnim = StartAnimation.No;
        }
        else
        {
            _startAnim = StartAnimation.Yes;
        }
    }

    public override void Tick(float deltaTime)
    {

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

    public void SetWayPoint(GameObject _obj)
    {
        teleportTo = _obj;
    }

    public void SetAction(string _hasAction)
    {
        if(_hasAction == "Nothing")
        {
            _fadeAction = FadeAction.Nothing;
        }
        else
        {
            _fadeAction = FadeAction.Teleport;
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