using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Linq;

public enum ZoneTrigger
{
    None,
    Square,
    Spherical,
}
#if UNITY_EDITOR
public class ZoneManager : EditorWindow
{

    private bool _addZone;
    private bool _editZone;
    private bool _deleteZone;

    private bool _loadedGameObject = false;

    private string _zoneName;
    private string _zoneDescription;

    private ZoneTrigger _zoneTrigger;

    private GameObject[] _allZones;
    private string[] _zoneNames;
    private int _selectedZoneIndex;
    private bool[] _selectedZoneDelete;

    [MenuItem("Level Design/Managers/Zone Manager")]

    static void ShowEditor()
    {
        ZoneManager _zoneManager = EditorWindow.GetWindow<ZoneManager>();
    }

    void OnGUI()
    {

        GUILayout.Label("Welcome to the Zone Manager", EditorStyles.boldLabel);

        if (!_addZone && !_editZone && !_deleteZone)
        {
            _zoneName = "";
            _zoneDescription = "";
            _loadedGameObject = false;

            // IF THERE ARE NO ZONES
            if (GameObject.FindGameObjectsWithTag("Zone").Length == 0)
            {
                GUILayout.Label("Currently there are no Zones");
                if (GUILayout.Button("ADD ZONE"))
                {
                    _addZone = true;
                }
            }

            else
            {
                if (GUILayout.Button("ADD ZONE"))
                {
                    _addZone = true;
                }

                if (GUILayout.Button("EDIT ZONE"))
                {
                    _editZone = true;
                }
                if (GUILayout.Button("DELETE ZONE"))
                {
                    _deleteZone = true;
                }
            }
        }

        if (_addZone)
        {
            AddZone();
        }

        if (_editZone)
        {
            EditZone();
        }

        if (_deleteZone)
        {
            DeleteZone();
        }

    }


    void AddZone()
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

            _addZone = false;

        }
        if (GUILayout.Button("BACK"))
        {
            _addZone = false;
        }
        EditorGUILayout.EndHorizontal();
    }

    void EditZone()
    {
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
            _editZone = false;
        }
        if (GUILayout.Button("BACK"))
        {
            _editZone = false;
        }

        EditorGUILayout.EndHorizontal();

    }

    void DeleteZone()
    {
        if (!_loadedGameObject)
        {
            _allZones = GameObject.FindGameObjectsWithTag("Zone");
            _zoneNames = new string[_allZones.Length];
            _selectedZoneDelete = new bool[_allZones.Length];

            _loadedGameObject = true;
        }

        for (int i = 0; i < _allZones.Length; i++)
        {

            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
            _zoneNames[i] = _allZones[i].GetComponent<Quest.Zone>().ReturnName();

            _selectedZoneDelete[i] = EditorGUILayout.Toggle(_selectedZoneDelete[i]);

            Debug.Log(_selectedZoneDelete[i]);
            GUILayout.Label(_zoneNames[i]);
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("DELETE SELECTED"))
        {

            for (int i = 0; i < _selectedZoneDelete.Length; i++)
            {
                if (_selectedZoneDelete[i])
                {
                    DestroyImmediate(_allZones[i]);
                }
            }

            _deleteZone = false;
        }
        if (GUILayout.Button("BACK"))
        {
            _deleteZone = false;
        }

        EditorGUILayout.EndHorizontal();


    }
}
#endif