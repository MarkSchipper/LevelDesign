using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelEditor
{
    namespace Utils
    {
        public class FloorCheck : MonoBehaviour
        {

            private static bool _isGroundLevel;

            private static GameObject _groundFloor;
            private static List<GameObject> _lowerLevels = new List<GameObject>();

            private static List<GameObject> _upperLevels = new List<GameObject>();

            private static bool _groundIsActive = true;
            private static List<bool> _lowerLevelsIsActive = new List<bool>();
            private static List<bool> _upperLevelsIsActive = new List<bool>();

            private static FloorObject[] _getFloors;
            private static List<string> _allFloors = new List<string>();
            private static int _floorObjectIndex = 0;
            private static List<GameObject> _activeFloors = new List<GameObject>();

            void OnEnable()
            {
                

            }

            void OnDisable()
            {
                _upperLevels.Clear();
                _upperLevelsIsActive.Clear();
                _lowerLevels.Clear();
                _lowerLevelsIsActive.Clear();
            }

            public static void GetAllFloors()
            {
                _getFloors = GameObject.FindObjectsOfType<FloorObject>();
                for (int i = 0; i < _getFloors.Length; i++)
                {

                    _allFloors.Add(_getFloors[i].name);

                    if (_getFloors[i].ReturnLocation() == 0)
                    {
                        _lowerLevels.Add(_getFloors[i].gameObject);
                        _lowerLevelsIsActive.Add(_getFloors[i].ReturnObjectActive());

                    }
                    if (_getFloors[i].ReturnLocation() == 1)
                    {
                        _upperLevels.Add(_getFloors[i].gameObject);
                        _upperLevelsIsActive.Add(_getFloors[i].ReturnObjectActive());
                    }
                    if (_getFloors[i].ReturnLocation() == -1)
                    {
                        _groundFloor = _getFloors[i].gameObject;
                        _groundIsActive = _getFloors[i].ReturnObjectActive();
                    }
                }
            }

            public static void CheckFloors()
            {
                if (GameObject.Find("GroundLevel") == null)
                {
                    if (GUILayout.Button("Add Ground Level"))
                    {

                        if (GameObject.Find("WORLD") == null)
                        {
                            GameObject _worldParent = new GameObject();
                            _worldParent.name = "WORLD";

                        }

                        if (GameObject.Find("WorldLevels") == null)
                        {
                            GameObject _levelsParent = new GameObject();
                            _levelsParent.name = "WorldLevels";
                            _levelsParent.transform.SetParent(GameObject.Find("WORLD").transform);
                        }

                        GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                        _plane.GetComponent<MeshRenderer>().enabled = true;
                        _plane.AddComponent<BoxCollider>();

                        _plane.transform.localScale = new Vector3(1000, 0, 1000);
                        _plane.name = "GroundLevel";
                        _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                        _plane.AddComponent<FloorObject>();
                        _plane.GetComponent<FloorObject>().SetLocation(-1);
                        _plane.GetComponent<FloorObject>().SetObjectActive(true);

                        _groundFloor = _plane;
                        _activeFloors.Add(_groundFloor);
                        _allFloors.Add(_groundFloor.name);

                    }
                }
                else {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Level Beneath"))
                    {
                        GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                        _plane.GetComponent<MeshRenderer>().enabled = true;
                        _plane.AddComponent<BoxCollider>();

                        _plane.transform.localScale = new Vector3(1000, 0, 1000);

                        _plane.transform.position = new Vector3(0, ((_lowerLevels.Count * 5) + 5) * -1, 0);

                        _plane.name = "LowerLevel_" + _lowerLevels.Count;
                        _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                        _plane.AddComponent<FloorObject>();
                        _plane.GetComponent<FloorObject>().SetObjectActive(true);
                        _plane.GetComponent<FloorObject>().SetLocation(0);

                        _lowerLevels.Add(_plane);
                        _lowerLevelsIsActive.Add(true);
                        _activeFloors.Add(_plane);
                        _allFloors.Add(_plane.name);



                    }

                    if (GUILayout.Button("Add Level Above"))
                    {
                        GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        _plane.GetComponent<MeshRenderer>().material = Resources.Load("SceneEditor/LevelEditorMaterial") as Material;
                        _plane.GetComponent<MeshRenderer>().enabled = true;
                        _plane.AddComponent<BoxCollider>();

                        _plane.transform.localScale = new Vector3(1000, 0, 1000);

                        _plane.transform.position = new Vector3(0, ((_upperLevels.Count * 5) + 5), 0);

                        _plane.name = "UpperLevels_" + _upperLevels.Count;
                        _plane.transform.SetParent(GameObject.Find("WorldLevels").transform);
                        _plane.AddComponent<FloorObject>();
                        _plane.GetComponent<FloorObject>().SetObjectActive(true);
                        _plane.GetComponent<FloorObject>().SetLocation(1);

                        _upperLevels.Add(_plane);
                        _upperLevelsIsActive.Add(true);
                        _activeFloors.Add(_plane);
                        _allFloors.Add(_plane.name);

                    }
                    EditorGUILayout.EndHorizontal();

                    GameObject _current = GameObject.Find(_allFloors[_floorObjectIndex]);

                    GUILayout.Label("Which Floor is Active");
                    _floorObjectIndex = EditorGUILayout.Popup(_floorObjectIndex, _allFloors.ToArray());

                    if (GameObject.Find(_allFloors[_floorObjectIndex]) != _current)
                    {
                        _current.GetComponent<FloorObject>().SetObjectActive(false);
                        _current = GameObject.Find(_allFloors[_floorObjectIndex]);
                        _current.GetComponent<FloorObject>().SetObjectActive(true);

                    }

                }
            }
        }
    }
}
