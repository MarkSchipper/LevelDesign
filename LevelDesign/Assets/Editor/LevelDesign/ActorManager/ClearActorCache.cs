using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NPCSystem.NPC))]
public class ClearActorCache : Editor {

    public override void OnInspectorGUI()
    {
        NPCSystem.NPC _target = (NPCSystem.NPC)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("CLEAR 'HAS MET PLAYER'"))
        {
            _target.ClearCache();
        }
    }
}
