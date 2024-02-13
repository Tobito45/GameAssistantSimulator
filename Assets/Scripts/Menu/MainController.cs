using System;
using System.Collections;
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
    private TextMeshProUGUI[] _textTimer, _textScore;

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

    public void ActivateMenuControllingJostic(GameObject parent = null)
    {
        if(parent == null)
            _outlines = FindObjectsOfType<UnityEngine.UI.Outline>().Select(g => g.gameObject).OrderBy(x => x.transform.position.y).Reverse().ToList();
        else
            _outlines = parent.GetComponentsInChildren<UnityEngine.UI.Outline>().Select(g => g.gameObject).OrderBy(x => x.transform.position.y).Reverse().ToList();

        _outLineIndex = -1;
    }

    public void ClearMenuControllingJostic()
    {
        if(_outLineIndex != -1)
            _outlines[_outLineIndex].GetComponent<UnityEngine.UI.Outline>().enabled = false;

        _outlines.Clear();
        _outLineIndex = -1;

    }

    public void PlayGame()
    {
        _mainCamera.gameObject.SetActive(false);
        GameController.Instance.SplitController.SetActiveCamers(true);

        _panelMenu.SetActive(false);
        ClearMenuControllingJostic();
        ForeachAllObjects(_panelGame, (obj) => { obj.SetActive(true); });

    }

    private void Update()
    {
        for(int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++) 
        {
            GameController.Instance.SetTextTimer(_textTimer[i], i);
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

                Button button = selectedObj.GetComponent<Button>();
                TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>();
                if (button)
                {
                //   selectedObj.GetComponent<UnityEngine.UI.Outline>().enabled = false;
                   button.onClick.Invoke();
                 //  _outLineIndex = -1;         
                } else if(inputField)
                {
                    if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                        inputField.text = (int.Parse(inputField.text) + 1).ToString();
                    else
                    {
                        GameController.Instance.KeyBoardForJostic.Active(true, () =>
                        {
                            ClearMenuControllingJostic();
                            GameController.Instance.KeyBoardForJostic.OnSelected = null;
                            ActivateMenuControllingJostic(FindObjectOfType<TMP_InputField>().gameObject.transform.parent.gameObject);
                            GameController.Instance.KeyBoardForJostic.Active(false, null);
                        });
                        ClearMenuControllingJostic();
                        GameController.Instance.KeyBoardForJostic.OnSelected += (c) =>
                        {
                            if(c == 'D')
                            {
                                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                            }
                            inputField.text += c.ToString();
                        };
                    }
                }
            } else if (KeyboardAndJostickController.GetBButton() != null && KeyboardAndJostickController.GetBButton().Contains(0))
            {
                var selectedObj = _outlines[_outLineIndex];
                TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>();

                if (inputField)
                {
                    if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                        inputField.text = (int.Parse(inputField.text) - 1).ToString();
                    
                }
            }
        }
    }

    public static void ForeachAllObjects<T>(T[] objects, Action<T> action)
    {
        foreach(T obj in objects)
            action(obj);
    }

    public static IEnumerator MakeActionAfterTime(Action actionBefore, Action actionAfter, float duraction)
    {
        actionBefore();
        yield return new WaitForSeconds(duraction);
        actionAfter();
    }


    public void OnItemAdded(int index, float score, bool isPlus)
    {
        _textScore[index].text = $"Score: {score:F2}";
        var colourSafe = _textScore[index].color;
        if (isPlus)
        {
           StartCoroutine(MakeActionAfterTime(() => _textScore[index].color = Color.green,
                                () => _textScore[index].color = colourSafe, 2));
        } else
        {
            StartCoroutine(MakeActionAfterTime(() => _textScore[index].color = Color.red,
                                () => _textScore[index].color = colourSafe, 2));
        }
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
