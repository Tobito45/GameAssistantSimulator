using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    private const float basicTime = 60f;

    [Header("Panels")]
    [SerializeField]
    private GameObject _panelMenu;
    [SerializeField]
    private GameObject _panelGame;

    [Header("InputFields")]
    [SerializeField]
    private TMP_InputField _inputField;

    [Header("Buttons")]
    [SerializeField]
    private Button _startGameButton;

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI textInfo;

    [SerializeField]
    private TextMeshProUGUI textTimer;

    [Header("Camers")]
    [SerializeField]
    private Camera _gameCamera;

    public Camera GameCamera => _gameCamera;
  
    [SerializeField]
    private Camera _mainCamera;

    private void Start()
    {
        _inputField.text = basicTime.ToString();
        _startGameButton.onClick.AddListener(() => GameController.Instance.StartOfTheGame(float.Parse(_inputField.text)));
        textInfo.text = $"Today: {DateTime.Now.Day}.{DateTime.Now.Month} \nVersion: {Application.version}";
        ActiveFirstDisactiveSecond(_mainCamera.gameObject, _gameCamera.gameObject);
        ActiveFirstDisactiveSecond(_panelMenu, _panelGame);
    }

    public void PlayGame()
    {
        ActiveFirstDisactiveSecond(_gameCamera.gameObject, _mainCamera.gameObject);
        ActiveFirstDisactiveSecond(_panelGame, _panelMenu);
    }

    private void Update()
    {
        GameController.Instance.UpdateTimer(textTimer);
    }

    public static void ActiveFirstDisactiveSecond(GameObject first, GameObject second) 
    {
        first.SetActive(true);
        second.SetActive(false); 
    }

    public void OpenMenuAndCloseGame()
    {
        ActiveFirstDisactiveSecond(_panelMenu, _panelGame);
        ActiveFirstDisactiveSecond(_mainCamera.gameObject,_gameCamera.gameObject);
        
    }
    public void Exit() => Application.Quit();
}
