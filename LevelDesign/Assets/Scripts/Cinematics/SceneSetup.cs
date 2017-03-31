using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR

public class SceneSetup : EditorWindow {

    public GameObject _gameManager;
    public GameObject _lux;
    public GameObject _cinematicCharacter;

    [MenuItem("Cinematics Manager/Scene Setup")]

    static void ShowEditor()
    {

        SceneSetup _setup = EditorWindow.GetWindow<SceneSetup>();



    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {

        if(GUILayout.Button("Set up scene"))
        {

            GameObject _addGM = Instantiate(_gameManager, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            GameObject _addLux = Instantiate(_lux, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            GameObject _addCinematicChar = Instantiate(_cinematicCharacter, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

            GameObject _nodeParent = new GameObject();
            _nodeParent.tag = "NodeParent";
            _nodeParent.name = "NODES";

            GameObject _wayPointParent = new GameObject();

            _wayPointParent.name = "WAYPOINTS";


        }

        if (GUILayout.Button("Add Waypoint"))
        {

            GameObject _waypoint = new GameObject();
            _waypoint.name = "CinematicWaypoint";
            _waypoint.transform.parent = GameObject.Find("WAYPOINTS").transform;


        }

        if(GUILayout.Button("Add Make Epic bars"))
        {

            if (GameObject.Find("Canvas") == null)
            {
                GameObject _myCanvas = new GameObject();
                _myCanvas.name = "Canvas";
                _myCanvas.AddComponent<Canvas>();
                _myCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

                GameObject _lowerBars = new GameObject();
                _lowerBars.name = "LowerEpicBar";
                _lowerBars.transform.parent = _myCanvas.transform;

                _lowerBars.AddComponent<Image>();
                _lowerBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _lowerBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _lowerBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 50);

                GameObject _upperBars = new GameObject();
                _upperBars.name = "UpperEpicBar";
                _upperBars.transform.parent = _myCanvas.transform;

                _upperBars.AddComponent<Image>();
                _upperBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _upperBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _upperBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (_myCanvas.GetComponent<RectTransform>().rect.height / 2) - 50);

            }
            else
            {
                GameObject _myCanvas = GameObject.Find("Canvas");
                GameObject _lowerBars = new GameObject();
                _lowerBars.name = "LowerEpicBar";
                _lowerBars.transform.parent = _myCanvas.transform;

                _lowerBars.AddComponent<Image>();
                _lowerBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _lowerBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _lowerBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((_myCanvas.GetComponent<RectTransform>().rect.height / 2) * -1) + 50);

                GameObject _upperBars = new GameObject();
                _upperBars.name = "UpperEpicBar";
                _upperBars.transform.parent = _myCanvas.transform;

                _upperBars.AddComponent<Image>();
                _upperBars.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                _upperBars.GetComponent<RectTransform>().sizeDelta = new Vector2(_myCanvas.GetComponent<RectTransform>().rect.width, 100);
                _upperBars.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (_myCanvas.GetComponent<RectTransform>().rect.height / 2) - 50);
            }
        }



    }

}
#endif