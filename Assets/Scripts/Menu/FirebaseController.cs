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

    // private Dictionary<string, UserData> _usersData = new Dictionary<string, UserData>();
    private List<UserData> _usersData = new List<UserData>();


    [Header("Objeccts from scene")]
    [SerializeField]
    private GameObject _content;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _prefabInfoGameObject;

    private int _typeOfSort = -1; // 0 - by name, 1 - by score, 2 - by time, 3 - by date
    private int _koefFilterRemoved = 0;
    private TypeDateFilter _filterDate = TypeDateFilter.All;
    public void SetTypeOfSorting(int i)
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        if (_typeOfSort == i)
        {
            _typeOfSort = -1;
            return;
        }
        _typeOfSort = i;
    }

    public void ChangeFilterDate(int type)
    {
        _filterDate = (TypeDateFilter)type; //Inspector Unity problem, cant detect enums
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
            for (int i = 0; i < args.Snapshot.Children.Count(); i++)//foreach (var childSnapshot in args.Snapshot.Children)
            {
                //   string userId = childSnapshot.Key;                 
                Dictionary<string, object> userData = (Dictionary<string, object>)args.Snapshot.Children.ElementAt(i).Value; //(Dictionary<string, object>)childSnapshot[i].Value;
                string username = userData["username"].ToString();
                float score = (float)Convert.ToDouble(userData["score"]);
                int time = Convert.ToInt32(userData["time"]);
                string date = userData["date"].ToString();

                //_usersData[userId] = new UserData(username, score, time, date);
                _usersData.Add(new UserData(username, score, time, date));
            }
        }
    }

    public void MakeDictionarySorted(ref List<UserData> userData)
    {
        switch (_typeOfSort)
        {
            case -1:
                break;

            case 0:
                userData = userData.OrderBy(n => n.username).ToList();
                break;
            case 1:
                userData = userData.OrderBy(n => n.score).Reverse().ToList();
                break;
            case 2:
                userData = userData.OrderBy(n => n.time).Reverse().ToList();
                break;
            case 3:
                userData = userData.OrderBy(n => n.date).Reverse().ToList();
                break;
        }
    }

    public List<UserData> MakeDictionaryNecessaryDate(List<UserData> userData)
    {
        int saveCount = userData.Count;
        List<UserData> savedUserData = null;

        switch (_filterDate)
        {
            case TypeDateFilter.All:
                savedUserData = new List<UserData>(userData);
                break;
            case TypeDateFilter.Today:
                savedUserData = userData.Where(n => DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date == DateTime.Today).ToList();
                break;
            case TypeDateFilter.Week:
                savedUserData = userData.Where((n) =>
                {
                    DateTime dateToCheck = DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date;
                    DayOfWeek dayOfWeek = dateToCheck.DayOfWeek;
                    DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    DateTime endOfWeek = startOfWeek.AddDays(6);
                    return dateToCheck >= startOfWeek && dateToCheck <= endOfWeek;
                }).ToList();
                break;
            case TypeDateFilter.Month:
                savedUserData = userData.Where((n) =>
                {
                    DateTime dateToCheck = DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Date;
                    return dateToCheck.Month == DateTime.Today.Month && dateToCheck.Year == DateTime.Today.Year;
                }).ToList();
                break;
            case TypeDateFilter.Year:
                savedUserData = userData.Where(n => DateTime.ParseExact(n.date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture).Year == DateTime.Today.Year).ToList();
                break;
        }
        _koefFilterRemoved = saveCount - userData.Count;
        return savedUserData;
    }

    void Update()
    {
        if (_content.transform.childCount + _koefFilterRemoved == _usersData.Count)
            return;

        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        MakeDictionarySorted(ref _usersData);
        var dateFilteredData = MakeDictionaryNecessaryDate(_usersData);
        foreach (var userData in dateFilteredData)
        {
            var obj = Instantiate(_prefabInfoGameObject, _content.transform);
            obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = userData.username;
            obj.transform.Find("TextScore").GetComponent<TextMeshProUGUI>().text = userData.score.ToString("F2");
            obj.transform.Find("TextMaxTime").GetComponent<TextMeshProUGUI>().text = userData.time.ToString();
            obj.transform.Find("TextDate").GetComponent<TextMeshProUGUI>().text = userData.date.ToString();
            //Debug.Log("UserID: " + userData.Key + ", Username: " + userData.Value.username + ", Score: " + userData.Value.score + ", Time: " + userData.Value.time + ", Date: " + userData.Value.date);
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