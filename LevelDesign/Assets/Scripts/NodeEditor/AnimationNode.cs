using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

#if UNITY_EDITOR

public class AnimationNode : BaseInputNode {

    private GameObject wayPoint;
    private string idleDelay;

    private BaseInputNode input1;
    private Rect input1Rect;

    private BaseInputNode input2;
    private Rect input2Rect;

    private AnimationActions anims;
    public enum AnimationActions
    {
        None,
        Idle,
        Walk,
        Run,
        MeleeAttack,
        SpellAttack,
    }

    public AnimationNode()
    {
        windowTitle = "Animation Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        string input1Title = "None";

        if (input1 != null)
        {
            input1Title = input1.getResult();
        }

        GUILayout.Label("Input 1: " + input1Title);

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }

        string input2Title = "None";

        if (input2 != null)
        {
            input2Title = input2.getResult();
        }

        GUILayout.Label("Input 2: " + input2Title);

        if (e.type == EventType.Repaint)
        {
            input1Rect = GUILayoutUtility.GetLastRect();
        }


        wayPoint = (GameObject)EditorGUILayout.ObjectField(wayPoint, typeof(GameObject), true);
        if(wayPoint != null)
        {
            anims = (AnimationActions)EditorGUILayout.EnumPopup("Animation: ", anims);

            if(anims.ToString() == "Idle")
            {
                GUILayout.Label("How long idle?: ");
                idleDelay = EditorGUILayout.TextField("Time: ", idleDelay);
            }

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

    public override void NodeDeleted(BaseNode node)
    {
        if (node.Equals(input1))
        {
            input1 = null;
        }
    }

    public override BaseInputNode ClickedOnInput(Vector2 pos)
    {
        BaseInputNode retValue = null;
        pos.x -= windowRect.x;
        pos.y -= windowRect.y;

        if (input1Rect.Contains(pos))
        {
            retValue = input1;
            input1 = null;
        }

        return retValue;
    }

    public override void DrawCurves()
    {
        if (input1 != null)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y + input1Rect.height / 2;
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