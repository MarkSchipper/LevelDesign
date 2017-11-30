using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class YesInputKnob : BaseKnob {

    private Texture2D _knob;
    private YesOutputKnob output1;
    private YesInputKnob input1;

    public YesInputKnob(float x, float y, float width, float height)
    {
        windowRect = new Rect(x, y, width, height); 
    }

    public YesInputKnob()
    {

    }

    public override void SetYesInput(InputKnob input, YesOutputKnob output, Vector2 clickPos)
    {
        if(windowRect.Contains(clickPos))
        {
            output1 = output;
       //     input1 = input;
        }
    }

    public override YesInputKnob ClickedOnYesInput(Vector2 pos)
    {
        YesInputKnob retValue = null;
                
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

            DialogueSystem.NodeEditor.DrawNodeCurve(input1.windowRect, output1.windowRect);
        }
    }

    public override Rect ReturnKnobRect()
    {
        return windowRect;
    }

}
