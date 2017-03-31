using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_EDITOR
public class RangedNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    // in this case the waypoints serves as a "Fire at"
    private GameObject _wayPoint;

    

    private AudioClip _soundSource;
    private HasSound _sound;

    public RangedNode()
    {
        windowTitle = "Ranged Attack Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Fire at: ");


        _wayPoint = (GameObject)EditorGUILayout.ObjectField(_wayPoint, typeof(GameObject), true);

        if (_wayPoint != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(_wayPoint);
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

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {

            input1 = input;

        }

    }

    public void SetWayPoint(GameObject _obj)
    {
        _wayPoint = _obj;
    }

    public override void Tick(float deltaTime)
    {

    }

    public void SetAudio(AudioClip _sound)
    {
        _soundSource = _sound;
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