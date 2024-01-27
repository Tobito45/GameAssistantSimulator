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
    private KeyboardAndJostickController _jostickController;

    [SerializeField]
    private QCodeDetecter _qCodeDetecter;
    [SerializeField]
    private SplitController _splitController;
    public bool IsTimerEnded() => _timer < 0;

    public float MaxTime => _maxTime;
    public float AllSum => _allSum;

    private List<GoodInfo>[] _basicGoodsName; //= new List<GoodInfo>();
    private float _sum = 0, _plusSum = 0, _minusSum = 0, _allSum;
    private int _all = 0, _allCorect = 0;

    private int[] _countClients;

    public QCodeDetecter QCodeDetecter => _qCodeDetecter;
    public SplitController SplitController => _splitController;
    
    public event Action OnStartNewGame;

    private static GameController _instance;
    public static GameController Instance => _instance;

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
        OnStartNewGame += NextGenerete;
     
        _clientGenerator.OnClientDestroy += NextGenerete;
    }

    public void StartOfTheGame(float maxTime = 60f)
    {
        this._maxTime = maxTime; 
        OnStartNewGame();
    }

    private void NextGenerete()
    {
        for(int i = 0; i < _countClients.Length; i++)
        {
            if (_countClients[i] != 0)
                EndClient();
        }
        //if (_countClient != 0)
        //    EndClient();

        if (IsTimerEnded())
        {
            EndGame();
            return;
        }



        //_countClient++;
        for (int i = 0; i < _countClients.Length; i++)
            _countClients[i] ++;

        _basicGoodsName = new List<GoodInfo>[KeyboardAndJostickController.GetCountGamepads()];
        for(int i = 0; i < _countClients.Length; i++)
        {
            _basicGoodsName[i] = new List<GoodInfo>();
        }


        _sum = 0;
        _plusSum = 0;
        _minusSum = 0;
        _allCorect = 0;

        for (int j = 0; j < _countClients.Length; j++)
        {
            for (int i = UnityEngine.Random.Range(3, 7); i < 8; i++)
            {
                GoodInfo good = _goodsController.AddNewItem(j).GetComponentInChildren<GoodInfo>();
                _basicGoodsName[j].Add(good);
                _sum += good.Price;
            }
        }
        _all = _basicGoodsName[0].Count; //TODO

        for (int i = 0; i < _countClients.Length; i++)
        {
            _clientGenerator.SpawnClient(i);
        }
    }

    public void UpdateTimer(TextMeshProUGUI text)
    {
        if (IsTimerEnded())
        {
            text.text = $"Time is left! This is your last client";
            return;
        }

        _timer -= Time.deltaTime;
        text.text = $"Time left: {_timer.ToString("F2")}";
    }

    public void OnAddGood(GoodInfo good, int index = 0) //TODO
    {
        if (_basicGoodsName[index].Remove(this[good.GoodName]) == true)
        {
            _allCorect++;
            _plusSum += good.Price;
        } else
        {
            _minusSum += good.Price;
        }
    }

    private void EndClient(int index = 0) //TODO
    {
        foreach(GoodInfo good in _basicGoodsName[0])
        {
            _minusSum += good.Price;
        }

        _allSum += _plusSum - _minusSum;
        _endMenuController.AddElementToEnd(clientNumber: _countClients[0], countGoods: _all, correctGoods: _allCorect,
                                                    minusMoney: _minusSum, plusMoney: _plusSum, allSum: _sum); //TODO!
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
        _allSum = 0;
        _timer = _maxTime;
        _countClients = new int[KeyboardAndJostickController.GetCountGamepads()];
    }

    private void EndGame()
    {
        _endMenuController.AktualText(_allSum);
        _endMenuController.ActivePanel();
    }
}
