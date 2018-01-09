using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class BaseKnob : ScriptableObject
{
    public Rect windowRect;

    public string windowTitle = "";

    public int nodeID;
    public int windowID;

    public virtual void SetInput(InputKnob input, OutputKnob output, Vector2 clickPos)
    {

    }
    

    public virtual void SetOutput(OutputKnob output, Vector2 clickPos) {
    }


    public virtual void SetYesInput(InputKnob input, YesOutputKnob output, Vector2 clickPos)
    {

    }

    public virtual void SetYesOutput(YesOutputKnob output, Vector2 clickPos)
    {

    }

    public virtual void SetNoInput(InputKnob input, NoOutputKnob output, Vector2 clickPos)
    {

    }

    public virtual void SetNoOutput(NoOutputKnob output, Vector2 clickPos)
    {

    }

    public virtual void DrawWindow(Rect pos, Texture2D tex)
    {

    }


    public abstract void DrawCurves();

    public virtual void NodeDeleted(BaseKnob node)
    {

    }

    public virtual InputKnob ClickedOnInput(Vector2 pos)
    {
        return null;
    }

    public virtual OutputKnob ClickedOnOutput(Vector2 pos)
    {
        return null;
    }

    public virtual YesInputKnob ClickedOnYesInput(Vector2 pos)
    {
        return null;
    }

    public virtual YesOutputKnob ClickedOnYesOutput(Vector2 pos)
    {
        return null;
    }


    public virtual NoInputKnob ClickedOnNoInput(Vector2 pos)
    {
        return null;
    }

    public virtual NoOutputKnob ClickedOnNoOutput(Vector2 pos)
    {
        return null;
    }



    public virtual Rect ReturnKnobRect()
    {
        return windowRect;
    }

    public virtual void SetNodeID(int id)
    {
        nodeID = id;
    }

    public virtual int ReturnWindowID()
    {
        return windowID;
    }
   
        

}
