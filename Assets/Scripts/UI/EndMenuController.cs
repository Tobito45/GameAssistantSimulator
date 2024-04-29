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
    private const string pattern = @"\((.*?)\)";

    [SerializeField]
    private GameObject _panelEndInfoPrefab;
    [SerializeField]
    private MainController _mainController;

    [SerializeField]
    private EndMenuObjectsPlayerIterator[] _objectScene = new EndMenuObjectsPlayerIterator[KeyboardAndJostickController.MAXPLAYERS];

    public bool IsEndPanelActive(int index) => _objectScene[index].GetPanelEnd.activeInHierarchy;

    private void Start()
    {
        FunctionsExtensions.ForeachAllObjects(_objectScene.Where(n => n.GetPanelEnd != null).Select(n => n.GetPanelEnd).ToArray(), (obj) => obj.SetActive(false));
        GameController.Instance.OnStartNewGame += ClearScroll;

        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
        {
            int index = i;
            _objectScene[i].GetSaveButton.GetComponent<Button>().onClick.AddListener(() => {
                _mainController.ActivateMenuControllingJostic(index, _objectScene[index].GetPanelInputName);
            });

        }
    }

    public void ClearScroll()
    {
        foreach (GameObject scroll in _objectScene.Where(n => n.GetScrollContent != null).Select(n => n.GetScrollContent)) 
            foreach (Transform obj in scroll.transform)
                Destroy(obj.gameObject);

    }

    public void AddElementToEnd(int clientNumber, int countGoods, int correctGoods, float minusMoney, float plusMoney, float allSum, int index)
    {
        var obj = Instantiate(_panelEndInfoPrefab, _objectScene[index].GetScrollContent.transform);
        obj.transform.Find("ClientName").GetComponent<TextMeshProUGUI>().text = $"Shopper {clientNumber}";
        obj.transform.Find("GoodsNumbers").GetComponent<TextMeshProUGUI>().text = $"{correctGoods}/{countGoods}";
        obj.transform.Find("GoodMinusPrice").GetComponent<TextMeshProUGUI>().text = $"-{minusMoney:F2}";
        obj.transform.Find("GoodPlusPrice").GetComponent<TextMeshProUGUI>().text = $"+{plusMoney:F2}";
        obj.transform.Find("PriceTogether").GetComponent<TextMeshProUGUI>().text = $"{(plusMoney - minusMoney).ToString("F2")} ({allSum:F2})";
    }

    public void ActivePanel(int index)
    {
        _objectScene[index].GetPanelEnd.SetActive(true);
        _mainController.ActivateMenuControllingJostic(index, _objectScene[index].GetPanelEnd);
    }

    public void AktualText(float sum, int index) => _objectScene[index].GetTextScope.text = $"score: {sum.ToString("F2")}";
    public void ClosePanelEnd()
    {
        for (int i = 0; i < KeyboardAndJostickController.GetCountControllers(); i++)
            if (!IsEndPanelActive(i))
                return;

        FunctionsExtensions.ForeachAllObjects(_objectScene.Where(n => n.GetPanelEnd != null).Select(n => n.GetPanelEnd).ToArray(), (obj) => obj.SetActive(false));
        _mainController.OpenMenuAndCloseGame();
    }

    public void SaveDataInFirebase(int index)
    {
        if (string.IsNullOrEmpty(_objectScene[index].GetInputFieldName.text))
        {
            //show error message
            StartCoroutine(FunctionsExtensions.MakeActionAfterTime(
                () => _objectScene[index].GetWarningSymbol.SetActive(true),
                () => _objectScene[index].GetWarningSymbol.SetActive(false), 2));
            return;
        }

        GameController.Instance.FirebaseController.SaveUserScore(_objectScene[index].GetInputFieldName.text, GameController.Instance.AllSum(index), 
                            (int)GameController.Instance.MaxTime, DateTime.Now.ToString("dd.MM.yyyy H:mm:ss"));

        _objectScene[index].GetPanelInputName.SetActive(false);
    }
  
    public static string GetByPattern(string pattern, string input) => Regex.Match(input, pattern).Groups[1].Value;
}

[System.Serializable]
class EndMenuObjectsPlayerIterator
{
    [SerializeField]
    private GameObject _panelEnd, _panelInputName, _saveButton;
    [SerializeField]
    private GameObject _scrollContent;
    [SerializeField]
    private TMP_InputField _inputFieldsName;
    [SerializeField]
    private GameObject _warningSymbol;
    [SerializeField]
    private TextMeshProUGUI _textScope;

    public GameObject GetPanelEnd => _panelEnd;
    public GameObject GetPanelInputName => _panelInputName;
    public GameObject GetSaveButton => _saveButton;
    public GameObject GetScrollContent => _scrollContent;
    public TMP_InputField GetInputFieldName => _inputFieldsName;
    public GameObject GetWarningSymbol => _warningSymbol;
    public TextMeshProUGUI GetTextScope => _textScope;
}
