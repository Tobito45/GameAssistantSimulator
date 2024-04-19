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
    private const float TIMERBOARDERFORMENUITERATION = 0.2f;
    [Header("Panels")]
    [SerializeField]
    private GameObject _panelMenu, _rankPanel;
   // [SerializeField]
   // private GameObject[] _panelGame;

    [Header("InputFields")]
    [SerializeField]
    private TMP_InputField _inputField;

    [Header("Buttons")]
    [SerializeField]
    private Button _startGameButton;

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI textInfo;

    //[SerializeField]
   // private TextMeshProUGUI[] _textTimer, _textScore;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private MainObjectsPlayerIterrator[] _playerObjects = new MainObjectsPlayerIterrator[KeyboardAndJostickController.MAXPLAYERS];

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
        ForeachAllObjects(_playerObjects.Where(n => n.GetPanelGame != null).Select(n => n.GetPanelGame).ToArray(), (obj) => { obj.SetActive(false); });
        ForeachAllObjects(_playerObjects.Where(n => n.GetCanvasPanel != null).Select(n => n.GetCanvasPanel).ToArray(), (obj) => { obj.SetActive(false); });
        GameController.Instance.SetCityActive(true);

        ActivateMenuControllingJostic(0);
    }

    
    public void ActivateMenuControllingJostic(int index, GameObject parent = null)
    {
        if (parent == null)
        {
            //finding all elements with component Outline
            _outlines[index] = 
                FindObjectsOfType<UnityEngine.UI.Outline>() 
                .Select(g => g.gameObject)
                .OrderBy(x => x.transform.position.y) //ordering by y coordinate
                .Reverse()
                .ToList();
        }
        else 
        {
            //finding all elements with component Outline from parrent
            _outlines[index] = 
                parent.GetComponentsInChildren<UnityEngine.UI.Outline>() 
                    .Select(g => g.gameObject).OrderBy(x => x.transform.position.y) //ordering by y coordinate
                    .Reverse()
                    .ToList();
        }
        //reseting iterator
        _outLineIndex[index] = -1; 
    }

    public void ClearMenuControllingJostic(int index)
    {
        if(_outLineIndex[index] != -1)
            //disabling Outline component
            _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = false; 

        //clearing the list
        _outlines[index].Clear();
        //reseting iterator
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
        foreach (var text in _playerObjects.Where(n => n.GetTextScore != null).Select(n => n.GetTextScore))
            text.text = "Score: 0";
        
        _mainCamera.gameObject.SetActive(false);
        GameController.Instance.SplitController.SetActiveCamers(true);

        _panelMenu.SetActive(false);
        ClearMenuControllingJostic(0);
        ForeachAllObjects(_playerObjects.Where(n => n.GetPanelGame != null).Select(n => n.GetPanelGame).ToArray(), (obj) => { obj.SetActive(true); });
        ForeachAllObjects(_playerObjects.Where(n => n.GetCanvasPanel != null).Select(n => n.GetCanvasPanel).ToArray(), (obj) => { obj.SetActive(true); });
        GameController.Instance.SetCityActive(false);


    }

    private void Update()
    {
        for(int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++) 
        {
            GameController.Instance.SetTextTimer(_playerObjects[i].GetTextTimer, i);
        }
        MenuControllerJostic();
        
    }

    private void MenuControllerJostic()
    {
        for (int index = 0; index < KeyboardAndJostickController.GetCountGamepads();index++)
        {
            //moving iterator
            MovingInMenuUsingJostick(index); 

            if (_outLineIndex[index] != -1)
            {
                if (KeyboardAndJostickController.GetAButton() != null && KeyboardAndJostickController.GetAButton().Contains(index))
                {
                    //getting selected gameobject
                    var selectedObj = _outlines[index][_outLineIndex[index]];

                    //getting component Buttom
                    Button button = selectedObj.GetComponent<Button>();
                    //getting component TMP_InputField
                    TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>(); 
                    if (button)
                    {
                        //execution of button method
                        button.onClick.Invoke(); 
                    }
                    else if (inputField)
                    {
                        if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                            //increasing inputfield value
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
                                if (c == 'D' && inputField.text.Length > 0)
                                {
                                    inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                                } else 
                                    inputField.text += c.ToString();
                            };
                        }
                    }
                }
                else if (KeyboardAndJostickController.GetBButton() != null && KeyboardAndJostickController.GetBButton().Contains(0))
                {
                    //getting selected gameobject
                    var selectedObj = _outlines[index][_outLineIndex[index]];
                    //getting component TMP_InputField
                    TMP_InputField inputField = selectedObj.GetComponentInChildren<TMP_InputField>();
                    if (inputField)
                    {
                        if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
                            //decreasing inputfield value
                            inputField.text = (int.Parse(inputField.text) - 1).ToString(); 

                    }
                }
            }
        }
    }

    private void MovingInMenuUsingJostick(int index)
    {
        if (timer[index] < 0)
        {
            if (_outlines[index].Count > 0)
            {
                //moving of stick
                var vertical = KeyboardAndJostickController.GetMovement(index).vertical; 
                if (vertical != 0)
                {
                    if (_outLineIndex[index] == -1)
                    {
                        //setting to first value of list
                        _outLineIndex[index] = 0; 
                    }
                    else
                    {
                        //disabling Outline
                        _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = false;
                        if (vertical < 0)
                        {
                            //setting limit to timer
                            timer[index] = TIMERBOARDERFORMENUITERATION;
                            //increasing iterrator
                            _outLineIndex[index]++; 
                            if (_outLineIndex[index] > _outlines[index].Count - 1)
                                _outLineIndex[index] = 0;
                        }
                        else if (vertical > 0)
                        {
                            //setting limit to timer 
                            timer[index] = TIMERBOARDERFORMENUITERATION;
                            //decreasing iterrator
                            _outLineIndex[index]--; 
                            if (_outLineIndex[index] < 0)
                                _outLineIndex[index] = _outlines[index].Count - 1;
                        }
                        //enabling Outline
                        _outlines[index][_outLineIndex[index]].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                    }
                }
            }
        }
        else
            // decreasing timer by real time
            timer[index] -= Time.deltaTime; 
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
        _playerObjects[index].GetTextScore.text = $"Score: {score:F2}";
        var colourSafe = _playerObjects[index].GetTextScore.color;
        if (isPlus)
        {
           StartCoroutine(MakeActionAfterTime(() => _playerObjects[index].GetTextScore.color = Color.green,
                                () => _playerObjects[index].GetTextScore.color = colourSafe, 2));
        } else
        {
            StartCoroutine(MakeActionAfterTime(() => _playerObjects[index].GetTextScore.color = Color.red,
                                () => _playerObjects[index].GetTextScore.color = colourSafe, 2));
        }
    }

    public void OpenMenuAndCloseGame()
    {
        _panelMenu.SetActive(true);
        ForeachAllObjects(_playerObjects.Where(n => n.GetPanelGame != null).Select(n => n.GetPanelGame).ToArray(), (obj) => { obj.SetActive(false); });
        ForeachAllObjects(_playerObjects.Where(n => n.GetCanvasPanel != null).Select(n => n.GetCanvasPanel).ToArray(), (obj) => { obj.SetActive(false); });
        GameController.Instance.SetCityActive(true);


        ActivateMenuControllingJostic(0);
        

        GameController.Instance.SplitController.SetActiveCamers(false);

        _mainCamera.gameObject.SetActive(true);
        
    }
    public void Exit() => Application.Quit();
}

[System.Serializable]
class MainObjectsPlayerIterrator
{
    [SerializeField]
    private GameObject _canvasGame;

    [SerializeField]
    private GameObject _panelGame;

    [SerializeField]
    private TextMeshProUGUI _textTimer, _textScore;


    public GameObject GetPanelGame => _panelGame;
    public GameObject GetCanvasPanel => _canvasGame;
    public TextMeshProUGUI GetTextTimer => _textTimer;
    public TextMeshProUGUI GetTextScore => _textScore;
}