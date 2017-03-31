using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR

public class SimpleTimer : EditorWindow
{
    GameObject myCanvas;
    [MenuItem("Cinematics Manager/Simple Timer")]

    

    static void ShowEditor()
    {

        SimpleTimer _setup = EditorWindow.GetWindow<SimpleTimer>();



    }

    

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {

        //Debug.Log(myCanvas.GetComponent<RectTransform>().rect.yMax);

        myCanvas = GameObject.Find("Canvas");
        if (myCanvas != null) {
            if (GUILayout.Button("Add Timer"))
            {
                       
                    AddText();
            }
            

            if(GUILayout.Button("Delete Timer"))
            {
                DestroyImmediate(GameObject.Find("myTimer").gameObject);
                DestroyImmediate(GameObject.Find("timeCounter").gameObject);
            }
        }
        else
        {
            if(GUILayout.Button("Add Canvas"))
            {
                AddCanvas();
            }
        }
    }

    void AddText()
    {
        GameObject _textTime = new GameObject();
        GameObject _time = new GameObject();

        _textTime.name = "myTimer";
        _textTime.transform.parent = myCanvas.transform;
        _textTime.AddComponent<Text>();
        _textTime.GetComponent<RectTransform>().anchoredPosition = new Vector2(myCanvas.GetComponent<RectTransform>().rect.xMax - 200, myCanvas.GetComponent<RectTransform>().rect.yMax - 100);
        _textTime.GetComponent<Text>().text = "Time:";
        _textTime.GetComponent<Text>().fontStyle = FontStyle.Bold;
        _textTime.GetComponent<Text>().fontSize = 25;


        //-----------------------------------------------

        _time.name = "timeCounter";
        _time.transform.parent = myCanvas.transform;
        _time.AddComponent<Text>();
        _time.GetComponent<RectTransform>().anchoredPosition = new Vector2(myCanvas.GetComponent<RectTransform>().rect.xMax - 100, myCanvas.GetComponent<RectTransform>().rect.yMax - 100);
        _time.GetComponent<Text>().text = "0.00";
        _time.GetComponent<Text>().fontStyle = FontStyle.Bold;
        _time.GetComponent<Text>().fontSize = 25;
    }

    void AddCanvas()
    {
        GameObject _canvas = new GameObject();
        _canvas.name = "Canvas";
        _canvas.AddComponent<Canvas>();
        _canvas.AddComponent<CanvasGroup>();
        _canvas.AddComponent<CanvasRenderer>();

        _canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.GetComponent<RectTransform>().position = new Vector2(0, 0);
    }
}
#endif