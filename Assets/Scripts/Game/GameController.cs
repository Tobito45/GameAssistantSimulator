using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private float _maxTime;

    private float _timer;

    [SerializeField]
    private ClientGenerator _clientGenerator;
    
    [SerializeField]
    private GoodsController _goodsController;

    [SerializeField]
    private EndMenuController _endMenuController;
    
    [SerializeField]
    private MainController  _mainController;

    [SerializeField]
    private KeyboardAndJostickController _jostickController;

    [SerializeField]
    private QCodeDetecter _qCodeDetecter;
    [SerializeField]
    private SplitController _splitController;
    [SerializeField]
    private FirebaseController _firebaseController;
    
    [SerializeField]
    private InputKeyboardController _keyboardForJostic;
    public bool IsTimerEnded() => _timer < 0;

    public float MaxTime => _maxTime;
    public float AllSum(int index) => _allSum[index];

    private List<GoodInfo>[] _basicGoodsName; //= new List<GoodInfo>();
    private float[] _sum = new float[KeyboardAndJostickController.MAXPLAYERS], 
        _plusSum = new float[KeyboardAndJostickController.MAXPLAYERS], 
        _minusSum = new float[KeyboardAndJostickController.MAXPLAYERS], 
        _allSum = new float[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _all = new int[KeyboardAndJostickController.MAXPLAYERS],
        _allCorect = new int[KeyboardAndJostickController.MAXPLAYERS];

    private int[] _countClients ;

    public QCodeDetecter QCodeDetecter => _qCodeDetecter;
    public SplitController SplitController => _splitController;

    public EndMenuController EndMenuController => _endMenuController;  
    public MainController MainController => _mainController;  
    public FirebaseController FirebaseController => _firebaseController;
    public InputKeyboardController KeyBoardForJostic => _keyboardForJostic;
    
    public event Action OnStartNewGame;
    public event Action<int> OnEndGame;

    private static GameController _instance;
    public static GameController Instance => _instance;

    public bool[] IsOpenedPanelUI { get; set; }

    public GoodInfo this[string name, int index = 0] //TODO
    {
        get
        {
            var list = from good in _basicGoodsName[index]  //_basicGoodsName.Where(n => n.GoodName == name);
                       where good.GoodName == name
                       select good;

            if (list.Count() == 0)
            {
                return null;
            }
            return list.First();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        OnStartNewGame += DeleteAllGoodsOnScene;
        OnStartNewGame += SetBasicInfo;
        OnEndGame += EndGame;

        IsOpenedPanelUI = new bool[KeyboardAndJostickController.MAXPLAYERS];
    }

    public void StartOfTheGame(float maxTime = 60f)
    {
        this._maxTime = maxTime;
        OnStartNewGame();
        for (int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++)
        {
            NextGenerete(i);
        }
    }

    public void NextGenerete(int index)
    {
        if (_countClients[index] != 0)
                EndClient(index);
        //if (_countClient != 0)
        //    EndClient();

        if (IsTimerEnded())
        {
            //EndGame(index);
            OnEndGame(index);
            return;
        }



        _countClients[index]++;

        _basicGoodsName[index] = new List<GoodInfo>();


        _sum[index] = 0;
        _plusSum[index] = 0;
        _minusSum[index] = 0;
        _allCorect[index] = 0;

        for (int i = UnityEngine.Random.Range(3, 7); i < 8; i++)
        {
            GoodInfo good = _goodsController.AddNewItem(index).GetComponentInChildren<GoodInfo>();
            _basicGoodsName[index].Add(good);
            _sum[index] += good.Price;
        }
        _all[index] = _basicGoodsName[index].Count;

        _clientGenerator.SpawnClient(index);
        
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
    }

    public void SetTextTimer(TextMeshProUGUI text, int index)
    {
        if (IsTimerEnded())
        {
            if (_countClients != null && _countClients.Count() > 0 && _countClients[index] == 0)
            {
                text.text = $"Time is left! Waiting for other players";
                return;
            }

            text.text = $"Time is left! This is your last client";
            return;
        }
        text.text = $"Time left: {_timer.ToString("F2")}";

    }


    public void OnAddGood(GoodInfo good, int index) 
    {
        if (_basicGoodsName[index].Remove(this[good.GoodName]) == true)
        {
            _allCorect[index]++;
            _plusSum[index] += good.Price;
        } else
        {
            _minusSum[index] += good.Price;
        }
    }

    public bool GetByCode(string code, int index, out GoodInfo goodInfo)
    {
        goodInfo = _basicGoodsName[index].Find(x => x.GetComponent<DragObject>().HasQrCode && x.GetComponent<DragObject>().GetCode == code);
        if (goodInfo)
        {
            _basicGoodsName[index].Remove(goodInfo);
            _allCorect[index]++;
            _plusSum[index] += goodInfo.Price;
            return true;
        }
        return false;
    }

    private void EndClient(int index)
    {
        var saveSum = _allSum[index];
        foreach (GoodInfo good in _basicGoodsName[index])
        {
            _minusSum[index] += good.Price;
        }

        _allSum[index] += _plusSum[index] - _minusSum[index];

        if (_allSum[index] > saveSum)
            _mainController.OnItemAdded(index, _allSum[index], true);
        else
            _mainController.OnItemAdded(index, _allSum[index], false);

        _endMenuController.AddElementToEnd(clientNumber: _countClients[index], countGoods: _all[index], correctGoods: _allCorect[index],
                                                    minusMoney: _minusSum[index], plusMoney: _plusSum[index], allSum: _sum[index], index: index); 
    }

    private void DeleteAllGoodsOnScene()
    {
        foreach (var good in FindObjectsOfType<GoodInfo>())
        {
            if (good is not GoodInfoSelect)
            {
                Destroy(good.gameObject);
            }
        }
    }
    private void SetBasicInfo()
    {

        for(int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
            _allSum[i] = 0; 

        _timer = _maxTime;
        _countClients = new int[KeyboardAndJostickController.GetCountGamepads()];
        _basicGoodsName = new List<GoodInfo>[KeyboardAndJostickController.GetCountGamepads()];

    }

    private void EndGame(int index)
    {
        _endMenuController.AktualText(_allSum[index], index);
        _endMenuController.ActivePanel(index);
    }
}
