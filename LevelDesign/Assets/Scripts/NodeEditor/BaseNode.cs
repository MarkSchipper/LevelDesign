using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect windowRect;

    public bool hasInputs = false;

    public string windowTitle = "";

    private int _nodeID;

    public virtual void DrawWindow()
    {
        //windowTitle = EditorGUILayout.TextField("Title", windowTitle);

    }

    public abstract void DrawCurves();

    public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
    {

    }

    public virtual void  NodeDeleted(BaseNode node)
    {

    }

    public virtual BaseInputNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }


    public abstract void Tick(float deltaTime);

    public virtual void SetID(int _id)
    {
        if(_id < 0 && _nodeID > 0)
        {
            
            _nodeID += _id;
        }
        _nodeID = _id;
    }

    public virtual int ReturnID()
    {
        return _nodeID;
    }

}
