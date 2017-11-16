using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ZoneTrigger
{
    None,
    Square,
    Spherical,
}

public class ZoneEditor : MonoBehaviour {

    private static string _zoneName;
    private static string _zoneDescription;
    private static ZoneTrigger _zoneTrigger;

    private static GameObject[] _allZones;
    private static string[] _zoneNames;
    private static int _selectedZoneIndex;
    private static bool _loadedGameObject;
    private static bool _deleteConfirmation;
    private static bool _deletedZone;
    private static int _deleteIndex;

    public static void ShowAddZone()
    {
        _zoneName = EditorGUILayout.TextField("Zone Name: ", _zoneName);
        _zoneDescription = EditorGUILayout.TextField("Zone Description: ", _zoneDescription);
        _zoneTrigger = (ZoneTrigger)EditorGUILayout.EnumPopup("What kind of shape?: ", _zoneTrigger);


        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("ADD ZONE"))
        {
            GameObject _zone = new GameObject();
            _zone.name = "Zone_" + _zoneName;
            _zone.tag = "Zone";
            _zone.layer = 2;

            if (GameObject.Find("ZONES") != null)
            {
                _zone.transform.parent = GameObject.Find("ZONES").transform;
            }
            else
            {
                GameObject _zoneParent = new GameObject();
                _zoneParent.name = "ZONES";
                _zone.transform.parent = _zoneParent.transform;
                _zoneParent.layer = 2;
            }

            if (_zoneTrigger == ZoneTrigger.Spherical)
            {
                _zone.AddComponent<SphereCollider>();
                _zone.GetComponent<SphereCollider>().radius = 10;
                _zone.GetComponent<SphereCollider>().isTrigger = true;
            }
            if (_zoneTrigger == ZoneTrigger.Square)
            {
                _zone.AddComponent<BoxCollider>();
                _zone.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
                _zone.GetComponent<BoxCollider>().isTrigger = true;
            }

            _zone.AddComponent<Quest.Zone>();
            _zone.GetComponent<Quest.Zone>().SetNames(_zoneName, _zoneDescription);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void ShowEditZone()
    {
        
        _deleteConfirmation = false;
        _deletedZone = false;

        _allZones = GameObject.FindGameObjectsWithTag("Zone");
        _zoneNames = new string[_allZones.Length];

        for (int i = 0; i < _allZones.Length; i++)
        {
            _zoneNames[i] = _allZones[i].GetComponent<Quest.Zone>().ReturnName();
        }

        _selectedZoneIndex = EditorGUILayout.Popup("Which Zone: ", _selectedZoneIndex, _zoneNames);

        if (!_loadedGameObject)
        {
            _zoneName = _allZones[_selectedZoneIndex].GetComponent<Quest.Zone>().ReturnName();
            _zoneDescription = _allZones[_selectedZoneIndex].GetComponent<Quest.Zone>().ReturnDescription();
            _loadedGameObject = true;
        }

        if (GUI.changed)
        {
            _loadedGameObject = false;
        }

        _zoneName = EditorGUILayout.TextField("Zone Name: ", _zoneName);
        _zoneDescription = EditorGUILayout.TextField("Zone Description: ", _zoneDescription);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("SAVE ZONE"))
        {

            _allZones[_selectedZoneIndex].GetComponent<Quest.Zone>().SetNames(_zoneName, _zoneDescription);
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void ShowDeleteZone()
    {
        if (!_loadedGameObject)
        {
            _allZones = GameObject.FindGameObjectsWithTag("Zone");
            _zoneNames = new string[_allZones.Length];

            _loadedGameObject = true;
        }

        if(!_deleteConfirmation) {
            for (int i = 0; i < _allZones.Length; i++)
            {
                _zoneNames[i] = _allZones[i].GetComponent<Quest.Zone>().ReturnName();
                if (GUILayout.Button("Delete zone " + _zoneNames[i]))
                { 
                    _deleteConfirmation = true;
                    _deleteIndex = i;
                }
            }
        }

        if(_deleteConfirmation && !_deletedZone)
        {
            if(GUILayout.Button("Are you sure you want to delete " + _zoneNames[_deleteIndex] + "?"))
            {
                DestroyImmediate(_allZones[_deleteIndex]);
                _deletedZone = true;
            }
        }

        if(_deletedZone)
        {
            GUILayout.Label("Zone has been deleted");
        }
    }
    
    public static void ClearValues()
    {
        _zoneName = "";
        _zoneDescription = "";
        _zoneTrigger = ZoneTrigger.None;
    }

}
