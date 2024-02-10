using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    private GameObject[] _panelGame;

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
    private TextMeshProUGUI[] textTimer;

    [Header("Camers")]
    [SerializeField]
    private Camera[] _gameCamera;

    [SerializeField]
    private Camera _mainCamera;


    private List<GameObject> _outlines;
    private int _outLineIndex = -1;
    private float timer;

    public bool IsMenu => _panelMenu.activeInHierarchy;

    private void Start()
    {
        _inputField.text = basicTime.ToString();
        _startGameButton.onClick.AddListener(() => GameController.Instance.StartOfTheGame(float.Parse(_inputField.text)));
        textInfo.text = $"Today: {DateTime.Now.Day}.{DateTime.Now.Month} \nVersion: {Application.version}";
        _mainCamera.gameObject.SetActive(true);
        GameController.Instance.SplitController.SetActiveCamers(false);

        _panelMenu.SetActive(true);
        ForeachAllObjects(_panelGame, (obj) => { obj.SetActive(false); });

        ActivateMenuControllingJostic();
    }

    public void ActivateMenuControllingJostic()
    {
        _outlines = FindObjectsOfType<UnityEngine.UI.Outline>().Select(g => g.gameObject).ToList();
        _outLineIndex = -1;
    }

    public void PlayGame()
    {
        _mainCamera.gameObject.SetActive(false);
        GameController.Instance.SplitController.SetActiveCamers(true);

        _panelMenu.SetActive(false);
        _outlines.Clear();
        ForeachAllObjects(_panelGame, (obj) => { obj.SetActive(true); });

    }

    private void Update()
    {
        for(int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++) 
        {
            GameController.Instance.SetTextTimer(textTimer[i], i);
        }
        MenuControllerJostic();
        
    }

    private void MenuControllerJostic()
    {
        if (timer < 0)
        {
            if (_outlines.Count > 0)
            {
                var vertical = KeyboardAndJostickController.GetMovement(0).vertical;
                if (vertical != 0)
                {
                    if (_outLineIndex == -1)
                    {
                        _outLineIndex = 0;
                    }
                    else
                    {
                        _outlines[_outLineIndex].GetComponent<UnityEngine.UI.Outline>().enabled = false;
                        if (vertical < 0)
                        {
                            timer = 0.2f;
                            _outLineIndex++;
                            if (_outLineIndex > _outlines.Count - 1)
                                _outLineIndex = 0;
                        }
                        else if (vertical > 0)
                        {
                            timer = 0.2f;
                            _outLineIndex--;
                            if (_outLineIndex < 0)
                                _outLineIndex = _outlines.Count - 1;
                        }

                        _outlines[_outLineIndex].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                    }
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if ( _outLineIndex != -1 )
        {
            if (KeyboardAndJostickController.GetAButton() != null && KeyboardAndJostickController.GetAButton().Contains(0))
            {
                var selectedObj = _outlines[_outLineIndex];
                if (selectedObj.GetComponent<Button>())
                {
                   selectedObj.GetComponent<UnityEngine.UI.Outline>().enabled = false;
                   selectedObj.GetComponent<Button>().onClick.Invoke();
                   _outLineIndex = -1;         
                } else if(selectedObj.GetComponentInChildren<TMP_InputField>())
                {
                    selectedObj.GetComponentInChildren<TMP_InputField>().text = (int.Parse(selectedObj.GetComponentInChildren<TMP_InputField>().text) + 1).ToString();
                }
            } else if (KeyboardAndJostickController.GetBButton() != null && KeyboardAndJostickController.GetBButton().Contains(0))
            {
                var selectedObj = _outlines[_outLineIndex];
                if (selectedObj.GetComponentInChildren<TMP_InputField>())
                {
                    selectedObj.GetComponentInChildren<TMP_InputField>().text = (int.Parse(selectedObj.GetComponentInChildren<TMP_InputField>().text) - 1).ToString();
                }
            }
        }

    }

    public static void ForeachAllObjects<T>(T[] objects, Action<T> action)
    {
        foreach(T obj in objects)
            action(obj);
    }


    public void OpenMenuAndCloseGame()
    {
        _panelMenu.SetActive(true);
        ForeachAllObjects(_panelGame, (obj) => { obj.SetActive(false); });


        ActivateMenuControllingJostic();


        GameController.Instance.SplitController.SetActiveCamers(false);
        _mainCamera.gameObject.SetActive(true);
        
    }
    public void Exit() => Application.Quit();
}
