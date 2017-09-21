using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class OutputKnob : BaseKnob {

    private Texture2D _knob;
    private OutputKnob output1;
    private int _windowID;

    public OutputKnob(int windowID, float x, float y, float width, float height)
    {
        _windowID = windowID;
        windowRect = new Rect(x, y, width, height); 
    }

    public OutputKnob()
    {

    }

    public override void DrawWindow(Rect pos, Texture2D tex)
    {

        base.DrawWindow(windowRect, tex);
        GUI.DrawTexture(windowRect, tex);

        windowRect.position = new Vector2(pos.x + (pos.width / 2), pos.y + pos.height);
        
    }


    public override Rect ReturnKnobRect()
    {
        return windowRect;
    }

    public override void SetOutput(OutputKnob output, Vector2 clickPos)
    {


        //clickPos.x -= windowRect.x;
        //clickPos.y -= windowRect.y;

        if(windowRect.Contains(clickPos))
        {
            output1 = output;
            
        }
    }


    public override OutputKnob ClickedOnOutput(Vector2 pos)
    {
        OutputKnob retValue = null;

        //pos.x -= windowRect.x;
        //pos.y -= windowRect.y;

        if(windowRect.Contains(pos))
        {
            retValue = output1;
            output1 = null;
        }

        return retValue;
    }

    public override int ReturnWindowID()
    {
        return _windowID;
    }

    public override void DrawCurves()
    {
        if(output1 != null)
        {
            DialogueSystem.NodeEditor.DrawNodeCurve(windowRect, output1.windowRect);
        }
    }

}
