using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEngine.UI;

public enum ImageMode
{
    Appear,
    Fade,
}
#if UNITY_EDITOR
public class ImageNode : BaseInputNode {

    private BaseInputNode input1;
    private Rect input1Rect;

    private HasSound _hasSound;
    private AudioClip _soundSource;

    private Sprite _userImage;

    private ImageMode _mode;

    private float _screenTime;

    public ImageNode()
    {
        windowTitle = "Image Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        GUILayout.Label("Which Image ( SPRITE )");
        _userImage = (Sprite)EditorGUILayout.ObjectField(_userImage, typeof(Sprite), true);

        if(_userImage != null)
        {
            GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetUserImage(_userImage);

            if(GameObject.Find("Canvas") != null)
            {
                if (GameObject.Find("userImage") == null)
                {
                    GameObject _go = new GameObject();
                    _go.name = "userImage";
                    _go.transform.parent = GameObject.Find("Canvas").transform;

                    _go.AddComponent<Image>();
                    _go.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                }
            }

            if(GameObject.Find("Canvas") == null)
            {
                GameObject _go = new GameObject();
                _go.name = "Canvas";
                _go.AddComponent<Canvas>();

                GameObject _goImg = new GameObject();
                _goImg.name = "userImage";
                _goImg.transform.parent = GameObject.Find("Canvas").transform;

                _goImg.AddComponent<Image>();
                _goImg.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }

            GUILayout.Label("How to display");
            _mode = (ImageMode)EditorGUILayout.EnumPopup("Action:", _mode);

            if(_mode != null)
            {
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetImageMode(_mode.ToString());

                GUILayout.Label("How long on screen");
                float.TryParse(EditorGUILayout.TextField("On screen for: ", _screenTime.ToString()), out _screenTime);

                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetImageTime(_screenTime);
            }
        }

        GUILayout.Label("Sound:");
        _hasSound = (HasSound)EditorGUILayout.EnumPopup("Sound:", _hasSound);
        if(_hasSound == HasSound.Yes)
        {
            _soundSource = (AudioClip)EditorGUILayout.ObjectField(_soundSource, typeof(AudioClip), true);
            if (_soundSource != null)
            {
                GameObject.Find("Node" + base.ReturnID()).GetComponent<NodeObject>().SetAudio(_soundSource);
            }
        }

    }

    public void SetImage(Sprite _img)
    {
        _userImage = _img;
    }

    public void SetImageMode(string _imgMode)
    {
        if(_imgMode == "Fade")
        {
            _mode = ImageMode.Fade;
        }
        else
        {
            _mode = ImageMode.Appear;
        }
    }

    public void SetScreenTime(float _time)
    {
        _screenTime = _time;
    }

    public void SetAudio(AudioClip _sound)
    {
        _hasSound = HasSound.Yes;
        _soundSource = _sound;
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        if (input1Rect.Contains(clickPos))
        {

            input1 = input;

        }

    }

    public override void DrawCurves()
    {
        if (input1)
        {
            Rect rect = windowRect;
            rect.x += input1Rect.x;
            rect.y += input1Rect.y;
            rect.width = 1;
            rect.height = 1;

            NodeEditor.DrawNodeCurve(input1.windowRect, rect);
        }
    }
}
#endif