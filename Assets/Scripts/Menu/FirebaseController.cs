using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{
    private DatabaseReference _reference;

    private List<UserData> _usersData = new List<UserData>();


    [Header("Objeccts from scene")]
    [SerializeField]
    private GameObject _content;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _prefabInfoGameObject;

    private TypeColumnFilter _typeOfSort = TypeColumnFilter.None;
    private int _koefFilterRemoved = 0;
    private TypeDateFilter _filterDate = TypeDateFilter.All;
    public void SetTypeOfSorting(int i)
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        if (_typeOfSort == (TypeColumnFilter)i)
        {
            _typeOfSort = TypeColumnFilter.None;
            return;
        }
        _typeOfSort = (TypeColumnFilter)i;
    }

    public void ChangeFilterDate(int type)
    {
        //Inspector Unity problem, cant detect enums
        _filterDate = (TypeDateFilter)type; 
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }
    void Start()
    {
        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        _reference.Child("users").ValueChanged += HandleValueChanged;
    }

    public void SaveUserScore(string username, float score, int time, string date)
    {
        string userId = _reference.Child("users").Push().Key;

        UserData user = new UserData(username, score, time, date);
        string json = JsonUtility.ToJson(user);

        _reference.Child("users").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("Data saved");
    }
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        _usersData.Clear();

        if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0)
        {
            for (int i = 0; i < args.Snapshot.Children.Count(); i++)
            {
                Dictionary<string, object> userData = (Dictionary<string, object>)args.Snapshot.Children.ElementAt(i).Value;
                string username = userData["username"].ToString();
                float score = (float)Convert.ToDouble(userData["score"]);
                int time = Convert.ToInt32(userData["time"]);
                string date = userData["date"].ToString();

                _usersData.Add(new UserData(username, score, time, date));
            }
        }
    }

    private void MakeListSorted(ref List<UserData> userData)
    {
        switch (_typeOfSort)
        {
            case TypeColumnFilter.None:
                break;
            case TypeColumnFilter.Name:
                userData.Sort((n1, n2) => string.Compare(n1.username, n2.username)); 
                break;
            case TypeColumnFilter.Score:
                userData.Sort((n1, n2) => n2.score.CompareTo(n1.score));
                break;
            case TypeColumnFilter.Time:
                userData.Sort((n1, n2) => n2.time.CompareTo(n1.time));
                break;
            case TypeColumnFilter.Date:
                userData.Sort((n1, n2) => DateTime.Compare(DateTime.Parse(n2.date), DateTime.Parse(n1.date)));
                break;
        }
    }

    private List<UserData> MakeListNecessaryDateSorted(List<UserData> userData)
    {
        int saveCount = userData.Count;
        List<UserData> savedUserData = null;
        switch (_filterDate)
        {
            case TypeDateFilter.All:
                savedUserData = new List<UserData>(userData);
                break;
            case TypeDateFilter.Today:
                savedUserData = userData.Where(n => DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date == DateTime.Today)
                    .ToList();
                break;
            case TypeDateFilter.Week:
                savedUserData = userData.Where((n) =>
                {
                    //Calculation the day of week
                    DateTime dateToCheck = DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date;
                    DayOfWeek dayOfWeek = dateToCheck.DayOfWeek;
                    DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    DateTime endOfWeek = startOfWeek.AddDays(6);
                    return dateToCheck >= startOfWeek && dateToCheck <= endOfWeek;
                }).ToList();
                break;
            case TypeDateFilter.Month:
                int todayMonth = DateTime.Today.Month;
                int todayYear = DateTime.Today.Year;
                savedUserData = userData.Where((n) =>
                {
                    //Calculation the month of year
                    DateTime dateToCheck = DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date;
                    return dateToCheck.Month == todayMonth && dateToCheck.Year == todayYear;
                }).ToList();
                break;
            case TypeDateFilter.Year:
                savedUserData = userData.Where(n => DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Year
                        == DateTime.Today.Year).ToList();
                break;
        }
        _koefFilterRemoved = saveCount - userData.Count;
        return savedUserData;
    }

    void Update()
    {
        if (_content.transform.childCount + _koefFilterRemoved == _usersData.Count)
            return;

        //clearing already existing list of date
        foreach (Transform child in _content.transform) 
            Destroy(child.gameObject);

        //filtering by name/score/time/date
        MakeListSorted(ref _usersData);

        //filtering by time period
        var dateFilteredData = MakeListNecessaryDateSorted(_usersData); 

        //createting new one
        foreach (var userData in dateFilteredData) 
        {
            var obj = Instantiate(_prefabInfoGameObject, _content.transform);
            obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = userData.username;
            obj.transform.Find("TextScore").GetComponent<TextMeshProUGUI>().text = userData.score.ToString("F2");
            obj.transform.Find("TextMaxTime").GetComponent<TextMeshProUGUI>().text = userData.time.ToString();
            obj.transform.Find("TextDate").GetComponent<TextMeshProUGUI>().text = userData.date.ToString();
        }
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public float score;
    public int time;
    public string date;

    public UserData(string username, float score, int time, string date)
    {
        this.username = username;
        this.score = score;
        this.time = time;
        this.date = date;
    }
}


[System.Serializable]
public enum TypeDateFilter
{
    Today, Week, Month, Year, All
}

[System.Serializable]
public enum TypeColumnFilter
{
    None = -1, Name = 0, Score = 1, Time = 2, Date = 3
}