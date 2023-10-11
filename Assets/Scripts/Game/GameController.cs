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
    private JostickController _jostickController;
    public bool IsTimerEnded() => _timer < 0;

    public float MaxTime => _maxTime;
    public float AllSum => _allSum;

    private List<GoodInfo> _basicGoodsName = new List<GoodInfo>();
    private float _sum = 0, _plusSum = 0, _minusSum = 0, _allSum;
    private int _all = 0, _allCorect = 0, _countClient;

    public event Action OnStartNewGame;

    private static GameController _instance;
    public static GameController Instance => _instance;

    public GoodInfo this[string name]
    {
        get
        {
            var list = from good in _basicGoodsName  //_basicGoodsName.Where(n => n.GoodName == name);
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

    private void Update()
    {
        if(Gamepad.all.Count() > 0)
        {
            _jostickController.enabled = true;
        } else
        {
            _jostickController.enabled = false;
        }

    }

    public void StartOfTheGame(float maxTime = 60f)
    {
        this._maxTime = maxTime; 
        OnStartNewGame();
    }

    private void NextGenerete()
    {
        if (_countClient != 0)
            EndClient();

        if (IsTimerEnded())
        {
            EndGame();
            return;
        }

        
        
        _countClient++;

        _basicGoodsName.Clear();
        _sum = 0;
        _plusSum = 0;
        _minusSum = 0;
        _allCorect = 0;

        for (int i = UnityEngine.Random.Range(3, 7); i < 8; i++)
        {
            GoodInfo good = _goodsController.AddNewItem().GetComponentInChildren<GoodInfo>();
           _basicGoodsName.Add(good);
            _sum += good.Price;
        }
        _all = _basicGoodsName.Count;

        _clientGenerator.SpawnClient();
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

    public void OnAddGood(GoodInfo good)
    {
        if(_basicGoodsName.Remove(this[good.GoodName]) == true)
        {
            _allCorect++;
            _plusSum += good.Price;
        } else
        {
            _minusSum += good.Price;
        }
    }

    private void EndClient()
    {
        foreach(GoodInfo good in _basicGoodsName)
        {
            _minusSum += good.Price;
        }

        _allSum += _plusSum - _minusSum;
        _endMenuController.AddElementToEnd(clientNumber: _countClient, countGoods: _all, correctGoods: _allCorect,
                                                    minusMoney: _minusSum, plusMoney: _plusSum, allSum: _sum);
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
        _countClient = 0;
    }

    private void EndGame()
    {
        _endMenuController.AktualText(_allSum);
        _endMenuController.ActivePanel();
    }
}
