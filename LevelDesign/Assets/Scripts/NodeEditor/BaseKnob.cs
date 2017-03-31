using UnityEngine;
using System.Collections;
using UnityEditor;

public class BaseKnob : ScriptableObject
{
    public Rect windowRect;

    public string windowTitle = "";


    public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
    {

    }

    public virtual void DrawWindow()
    {
        //windowTitle = EditorGUILayout.TextField("Title", windowTitle);

    }

    

    public virtual void NodeDeleted(BaseNode node)
    {

    }

    public virtual BaseInputNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }

}
