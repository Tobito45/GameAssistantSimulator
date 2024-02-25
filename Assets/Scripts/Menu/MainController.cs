using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    private const float basicTime = 60f;

    [Header("Panels")]
    [SerializeField]
    private GameObject _panelMenu, _rankPanel;
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


    private List<GameObject>[] _outlines = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _outLineIndex = new int[KeyboardAndJostickController.MAXPLAYERS];
    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];

    public bool IsMenu => _panelMenu.activeInHierarchy;

    private void Start()
    {
        for (int i = 0; i < _outLineIndex.Length; i++)
            _outLineIndex[i] = -1;

        for (int i = 0; i < _outlines.Length; i++)
            _outlines[i] = new List<GameObject>();

        _inputField.text = basicTime.ToString();
        _startGameButton.onClick.AddListener(() => GameController.Instance.StartOfTheGame(float.Parse(_inputField.text)));
        textInfo.text = $"Today: {DateTime.Now.Day}.{DateTime.Now.Month} \nVersion: {Application.version}";
        _mainCamera.gameObject.SetActive(true);
        GameController.Instance.SplitController.SetActiveCamers(false);

        _panelMenu.SetActive(true);
        ForeachAllObjects(_panelGame, (obj) => { obj.SetActive(false); });

        ActivateMenuControllingJostic(0);
    }

    
    public void ActivateMenuControllingJostic(int index, GameObject parent = null)
    {
        if(parent == null)
            _outlines[index] = FindObjectsOfType<UnityEngine.UI.Outline>().Select(g => g.gameObject).OrderBy(x => x.transform.position.y).Reverse().ToList();
        else
            _outlines[index] = parent.GetComponentsInChildren<UnityEngine.UI.Outline>().Select(g => g.gameObject).OrderBy(x => x.transform.position.y).Reverse().ToList();

        _outLineIndex[index] = -1;
    }

    public void ClearMenuControllingJostic(int index)
    {
        if(_outLineIndex[index] != -1)
            _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = false;

        _outlines[index].Clear();
        _outLineIndex[index] = -1;

    }

    public void OpenRankMenu()
    {
        _rankPanel.SetActive(true);
        ClearMenuControllingJostic(0);
        ActivateMenuControllingJostic(0, _rankPanel);
    }
    public void CloseRankMenu()
    {
        ClearMenuControllingJostic(0);
        ActivateMenuControllingJostic(0, _panelMenu);
        _rankPanel.SetActive(false);
    }

    public void PlayGame()
    {
        foreach (var text in _textScore)
            text.text = "Score: 0";
        
        _mainCamera.gameObject.SetActive(false);
        GameController.Instance.SplitController.SetActiveCamers(true);

        _panelMenu.SetActive(false);
        ClearMenuControllingJostic(0);
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
        for (int index = 0; index < KeyboardAndJostickController.GetCountGamepads();index++)
        {
            if (timer[index] < 0)
            {
                if ( _outlines[index].Count > 0)
                {
                    var vertical = KeyboardAndJostickController.GetMovement(index).vertical;
                    if (vertical != 0)
                    {
                        if (_outLineIndex[index] == -1)
                        {
                            _outLineIndex[index] = 0;
                        }
                        else
                        {
                            _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = false;
                            if (vertical < 0)
                            {
                                timer[index] = 0.2f;
                                _outLineIndex[index]++;
                                if (_outLineIndex[index] > _outlines[index].Count - 1)
                                    _outLineIndex[index] = 0;
                            }
                            else if (vertical > 0)
                            {
                                timer[index] = 0.2f;
                                _outLineIndex[index]--;
                                if (_outLineIndex[index] < 0)
                                    _outLineIndex[index] = _outlines[index].Count - 1;
                            }

                            _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                        }
                    }
                }
            }
            else
            {
                timer[index] -= Time.deltaTime;
            }

            if (_outLineIndex[index] != -1)
            {
                if (KeyboardAndJostickController.GetAButton() != null && KeyboardAndJostickController.GetAButton().Contains(index))
                {
                    var selectedObj = _outlines[index][_outLineIndex[index]];

                    Button button = selectedObj.GetComponent<Button>();
                    TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>();
                    if (button)
                    {
                        button.onClick.Invoke();
                    }
                    else if (inputField)
                    {
                        if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                            inputField.text = (int.Parse(inputField.text) + 1).ToString();
                        else
                        {
                            int saveIndex = index;
                            GameController.Instance.KeyBoardForJostic.Active(index, true, () =>
                            {
                                ClearMenuControllingJostic(saveIndex);
                                GameController.Instance.KeyBoardForJostic.OnSelected[saveIndex] = null;
                                ActivateMenuControllingJostic(saveIndex, FindObjectOfType<TMP_InputField>().gameObject.transform.parent.gameObject);
                                GameController.Instance.KeyBoardForJostic.Active(saveIndex, false, null);
                            });
                            ClearMenuControllingJostic(index);
                            GameController.Instance.KeyBoardForJostic.OnSelected[index] += (c) =>
                            {
                                if (c == 'D')
                                {
                                    inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                                }
                                inputField.text += c.ToString();
                            };
                        }
                    }
                }
                else if (KeyboardAndJostickController.GetBButton() != null && KeyboardAndJostickController.GetBButton().Contains(0))
                {
                    var selectedObj = _outlines[index][_outLineIndex[index]];
                    TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>();

                    if (inputField)
                    {
                        if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                            inputField.text = (int.Parse(inputField.text) - 1).ToString();

                    }
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


        ActivateMenuControllingJostic(0);
        

        GameController.Instance.SplitController.SetActiveCamers(false);
        _mainCamera.gameObject.SetActive(true);
        
    }
    public void Exit() => Application.Quit();
}
