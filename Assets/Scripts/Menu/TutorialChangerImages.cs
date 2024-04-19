using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialChangerImages : MonoBehaviour
{
    [SerializeField]
    private Sprite _josticImage;

    [SerializeField]
    private Sprite _keyBoardImage;

    private Image _image;

    [SerializeField]
    private bool testic;
    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>(); 
        GameController.Instance.OnStartNewGame += OnChangeImage;
    }

    private void OnChangeImage()
    {
        if(KeyboardAndJostickController.IsJosticConnected)
            _image.sprite = _josticImage;
        else
            _image.sprite = _keyBoardImage;
    }
}
