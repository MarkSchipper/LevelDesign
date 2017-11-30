using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class InputKnob : BaseKnob {

    private Texture2D _knob;
    private OutputKnob output1;
    private InputKnob input1;

    private NoOutputKnob noOutput;
    private YesOutputKnob yesOutput;

    public InputKnob(float x, float y, float width, float height)
    {
        windowRect = new Rect(x, y, width, height); 
    }

    public InputKnob()
    {

    }

    public override void SetInput(InputKnob input, OutputKnob output, Vector2 clickPos)
    {
        
            output1 = output;
            input1 = input;
        
    }

    public override void SetNoInput(InputKnob input, NoOutputKnob output, Vector2 clickPos)
    {
        input1 = input;
        noOutput = output;
    }

    public override void SetYesInput(InputKnob input, YesOutputKnob output, Vector2 clickPos)
    {
        input1 = input;
        yesOutput = output;
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

        //base.DrawWindow(new Rect(pos.x + (pos.width / 2), pos.y - 20, windowRect.width, windowRect.height), tex);
        GUI.DrawTexture(new Rect(pos.x + (pos.width / 2), pos.y - 12 , windowRect.width, windowRect.height), tex);

        windowRect.position = new Vector2(pos.x + (pos.width / 2), pos.y - 12);
        
    }

    public override void DrawCurves()
    {
        if(output1 != null && input1 != null && noOutput == null && yesOutput == null)
        {
            DialogueSystem.NodeEditor.DrawNodeCurve(input1.windowRect, output1.windowRect);
        }

        if(noOutput != null && input1 != null && output1 == null && yesOutput == null)
        {
            DialogueSystem.NodeEditor.DrawNodeCurve(input1.windowRect, noOutput.windowRect);
        }

        if(yesOutput != null && input1 != null && output1 == null && noOutput == null)
        {
            DialogueSystem.NodeEditor.DrawNodeCurve(input1.windowRect, yesOutput.windowRect);
        }
    }

    public override Rect ReturnKnobRect()
    {
        return windowRect;
    }

}
