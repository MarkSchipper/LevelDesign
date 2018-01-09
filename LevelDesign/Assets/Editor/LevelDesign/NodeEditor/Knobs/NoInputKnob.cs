using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class NoInputKnob : BaseKnob {

    private Texture2D _knob;
    private NoOutputKnob output1;
    private InputKnob input1;

    public NoInputKnob(float x, float y, float width, float height)
    {
        windowRect = new Rect(x, y, width, height); 
    }

    public NoInputKnob()
    {

    }

    public override void SetNoInput(InputKnob input, NoOutputKnob output, Vector2 clickPos)
    {
        if(windowRect.Contains(clickPos))
        {
            output1 = output;
            input1 = input;

            Debug.Log(output1 + " - " + input1);
        }
    }

    public override InputKnob ClickedOnInput(Vector2 pos)
    {
        InputKnob retValue = null;
                
        if (windowRect.Contains(pos))
        {
            retValue = input1;
            input1 = null;
        }

        return retValue;
    }

    public override void DrawWindow(Rect pos, Texture2D tex)
    {

        base.DrawWindow(new Rect(pos.x + (pos.width / 2), pos.y - 20, windowRect.width, windowRect.height), tex);
        GUI.DrawTexture(new Rect(pos.x + (pos.width / 2), pos.y - 20, windowRect.width, windowRect.height), tex);

        windowRect.position = new Vector2(pos.x + (pos.width / 2), pos.y - 20);
        
    }

    public override void DrawCurves()
    {
        if(output1 != null && input1 != null)
        {
            Debug.Log(" draw curves ");
            DialogueSystem.NodeEditor.DrawNodeCurve(input1.windowRect, output1.windowRect);
        }
    }

    public override Rect ReturnKnobRect()
    {
        return windowRect;
    }

}
