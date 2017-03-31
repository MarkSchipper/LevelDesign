using UnityEngine;
using System.Collections;
using System;

public abstract class BaseInputNode : BaseNode {

    protected string nodeResult = "";

   

    public virtual string getResult()
    {
        return nodeResult;
    }

    public override void DrawCurves()
    {
       // throw new NotImplementedException();
    }

   
    

}
