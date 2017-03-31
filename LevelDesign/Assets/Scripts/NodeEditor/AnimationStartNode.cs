using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

#if UNITY_EDITOR

public class AnimationStartNode : BaseInputNode {

    private GameObject startPos;
    private float idleDelay;

    private AnimationActions anims;
    private AnimationActions followAnims;
    public enum AnimationActions
    {
        None,
        Idle,
        Walk,
        Run,
        MeleeAttack,
        SpellAttack,
    }

    public AnimationStartNode()
    {
        windowTitle = "Animation Start Node";
        hasInputs = false;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Start Position");
        startPos = (GameObject)EditorGUILayout.ObjectField(startPos, typeof(GameObject), true);

        if(startPos != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetWayPoint(startPos);
        }
       

    }

    public void SetWayPoint(GameObject _waypoint)
    {
        startPos = _waypoint;
    }

    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    public override void Tick(float deltaTime)
    {
        
    }

}
#endif