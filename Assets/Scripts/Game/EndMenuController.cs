using System;
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuController : MonoBehaviour
{
    private const string keySave = "IndexSavedGame";
    private const string pattern = @"\((.*?)\)";

    [Header("GameObjects")]
    [SerializeField]
    private GameObject[] _panelEnd, _panelInputName, _saveButtons;
    [SerializeField]
    private GameObject[] _scrollContent;
    [SerializeField]
    private GameObject _panelEndInfoPrefab;

    [SerializeField]
    private TMP_InputField[] _inputFieldsName;
    [SerializeField]
    private GameObject[] _warningSymbol;

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI[] _textScope;


    [Header("Scripts")]
    [SerializeField]
    private MainController _mainController;

    public bool IsEndPanelActive(int index) => _panelEnd[index].activeInHierarchy;

    private void Start()
    {
        MainController.ForeachAllObjects(_panelEnd, (obj) => obj.SetActive(false));
        GameController.Instance.OnStartNewGame += ClearScroll;

        for (int i = 0; i < _saveButtons.Length; i++)
        {
            int index = i; 
            _saveButtons[i].GetComponent<Button>().onClick.AddListener(() => {
                _mainController.ActivateMenuControllingJostic(index, _panelInputName[index]);
            });

        }
    }

    public void AddElementToEnd(int clientNumber, int countGoods, int correctGoods, float minusMoney, float plusMoney, float allSum, int index)
    {
        var obj = Instantiate(_panelEndInfoPrefab, _scrollContent[index].transform);
        obj.transform.Find("ClientName").GetComponent<TextMeshProUGUI>().text = $"{clientNumber:F2} - Client";
        obj.transform.Find("GoodsNumbers").GetComponent<TextMeshProUGUI>().text = $"{correctGoods:F2}/{countGoods:F2}";
        obj.transform.Find("GoodMinusPrice").GetComponent<TextMeshProUGUI>().text = $"-{minusMoney:F2}";
        obj.transform.Find("GoodPlusPrice").GetComponent<TextMeshProUGUI>().text = $"+{plusMoney:F2}";
        obj.transform.Find("PriceTogether").GetComponent<TextMeshProUGUI>().text = $"{(plusMoney - minusMoney).ToString("F2")} ({allSum:F2})";

    }

    public void ClearScroll()
    {
        foreach (GameObject scroll in _scrollContent) {
            foreach (Transform obj in scroll.transform)
            {
                Destroy(obj.gameObject);
            }
        }
    }

    public void ActivePanel(int index)
    {
        _panelEnd[index].SetActive(true);
        _mainController.ActivateMenuControllingJostic(index, _panelEnd[index]);
    }

    public void AktualText(float sum, int index)
    {
        _textScope[index].text = $"Your scope: {sum.ToString("F2")}";
    }
    public void ClosePanelEnd()
    {
        MainController.ForeachAllObjects(_panelEnd, (obj) => obj.SetActive(false));
        _mainController.OpenMenuAndCloseGame();

    }

    public void SaveData()
    {
        int indexGame = PlayerPrefs.GetInt(keySave, 1);
        string filePath = $"saveData{indexGame}.txt";

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"Saved game number{indexGame}:");
            writer.WriteLine($"Time was: {GameController.Instance.MaxTime} seconds");

            foreach(Transform obj in _scrollContent[0].transform) //TODO
            {
                writer.WriteLine("================================================");
                writer.WriteLine($"{obj.Find("ClientName").GetComponent<TextMeshProUGUI>().text}");
                writer.WriteLine($"Correct items was: {obj.Find("GoodsNumbers").GetComponent<TextMeshProUGUI>().text.Split("/").First()}");
                writer.WriteLine($"All items was: {obj.Find("GoodsNumbers").GetComponent<TextMeshProUGUI>().text.Split("/").Last()}");
                writer.WriteLine($"Profit: {obj.Find("GoodPlusPrice").GetComponent<TextMeshProUGUI>().text}");
                writer.WriteLine($"Lesion: {obj.Find("GoodMinusPrice").GetComponent<TextMeshProUGUI>().text}");
                writer.WriteLine($"How much can we get: {EndMenuController.GetByPattern(input: obj.Find("PriceTogether").GetComponent<TextMeshProUGUI>().text.Split(" ").Last(), pattern: pattern)}");
                writer.WriteLine($"How much we get: {obj.Find("PriceTogether").GetComponent<TextMeshProUGUI>().text.Split(" ").First()}");

            }
            writer.WriteLine($"\nYour scope is: {GameController.Instance.AllSum(0)}"); //TODO
            PlayerPrefs.SetInt(keySave, ++indexGame);

        }
    }

    public void SaveDataInFirebase(int index)
    {
        if (string.IsNullOrEmpty(_inputFieldsName[index].text))
        {
            StartCoroutine(MainController.MakeActionAfterTime(
                () => _warningSymbol[index].SetActive(true),
                () => _warningSymbol[index].SetActive(false), 2));
            return;
        }

        GameController.Instance.FirebaseController.SaveUserScore(_inputFieldsName[index].text, GameController.Instance.AllSum(index), 
                            (int)GameController.Instance.MaxTime, DateTime.Now.ToString("dd.MM.yyyy H:mm:ss"));

        _panelInputName[index].SetActive(false);
    }

  
    public static string GetByPattern(string pattern, string input)
    {
        return Regex.Match(input, pattern).Groups[1].Value;
    }

}
