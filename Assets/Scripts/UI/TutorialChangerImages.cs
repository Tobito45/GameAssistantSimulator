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

    void Awake() =>
        _image = GetComponent<Image>();

    private void Start() =>
        GameController.Instance.OnStartNewGame += OnEnable;

    private void OnEnable()
    {
        if (KeyboardAndJostickController.IsJosticConnected)
            _image.sprite = _josticImage;
        else
            _image.sprite = _keyBoardImage;

        if (_image.sprite == null)
            _image.color = new Color(1, 1, 1, 0);
        else
            _image.color = new Color(1, 1, 1, 1f);
    }
}
