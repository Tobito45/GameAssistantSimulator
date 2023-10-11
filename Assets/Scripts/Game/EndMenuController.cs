using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EndMenuController : MonoBehaviour
{
    private const string keySave = "IndexSavedGame";
    private const string pattern = @"\((.*?)\)";
    
    [Header("GameObjects")]
    [SerializeField]
    private GameObject _panelEnd;
    [SerializeField]
    private GameObject _scrollContent;
    [SerializeField]
    private GameObject _panelEndInfoPrefab;

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI _textScope;


    [Header("Scripts")]
    [SerializeField]
    private MainController _mainController;

    private void Start()
    {
        _panelEnd.SetActive(false);
        GameController.Instance.OnStartNewGame += ClearScroll;
    }

    public void AddElementToEnd(int clientNumber, int countGoods, int correctGoods, float minusMoney, float plusMoney, float allSum)
    {
        var obj = Instantiate(_panelEndInfoPrefab, _scrollContent.transform);
        obj.transform.Find("ClientName").GetComponent<TextMeshProUGUI>().text = $"{clientNumber} - Client";
        obj.transform.Find("GoodsNumbers").GetComponent<TextMeshProUGUI>().text = $"{correctGoods}/{countGoods}";
        obj.transform.Find("GoodMinusPrice").GetComponent<TextMeshProUGUI>().text = $"-{minusMoney}";
        obj.transform.Find("GoodPlusPrice").GetComponent<TextMeshProUGUI>().text = $"+{plusMoney}";
        obj.transform.Find("PriceTogether").GetComponent<TextMeshProUGUI>().text = $"{(plusMoney - minusMoney).ToString("F2")} ({allSum})";

    }

    public void ClearScroll()
    {
        foreach(Transform obj in _scrollContent.transform)
        {
            Destroy(obj.gameObject);
        }
    }

    public void ActivePanel() => _panelEnd.SetActive(true);

    public void AktualText(float sum) => _textScope.text = $"Your scope: {sum.ToString("F2")}"; 

    public void ClosePanelEnd()
    {
        _panelEnd.SetActive(false);
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

            foreach(Transform obj in _scrollContent.transform)
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
            writer.WriteLine($"\nYour scope is: {GameController.Instance.AllSum}");
            PlayerPrefs.SetInt(keySave, ++indexGame);

        }
    }

    public static string GetByPattern(string pattern, string input)
    {
        return Regex.Match(input, pattern).Groups[1].Value;
    }

}
