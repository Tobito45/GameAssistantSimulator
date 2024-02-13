using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using System;
using TMPro;

public class FirebaseController : MonoBehaviour
{
    private DatabaseReference _reference;

    private Dictionary<string, UserData> _usersData = new Dictionary<string, UserData>();


    [Header("Objeccts from scene")]
    [SerializeField]
    private GameObject _content;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _prefabInfoGameObject;

    private int _saveCountData;



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
            foreach (var childSnapshot in args.Snapshot.Children)
            {
                string userId = childSnapshot.Key;
                Dictionary<string, object> userData = (Dictionary<string, object>)childSnapshot.Value;
                string username = userData["username"].ToString();
                float score = (float)Convert.ToDouble(userData["score"]);
                int time = Convert.ToInt32(userData["time"]);
                string date = userData["date"].ToString();

                _usersData[userId] = new UserData(username, score, time, date);
            }
        }
    }

    void Update()
    {
        if (_saveCountData == _usersData.Count)
            return;

        _saveCountData = _usersData.Count;
        foreach(Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var userData in _usersData)
        {
            var obj = Instantiate(_prefabInfoGameObject, _content.transform);
            obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = userData.Value.username;
            obj.transform.Find("TextScore").GetComponent<TextMeshProUGUI>().text = userData.Value.score.ToString("F2");
            obj.transform.Find("TextMaxTime").GetComponent<TextMeshProUGUI>().text = userData.Value.time.ToString();
            obj.transform.Find("TextDate").GetComponent<TextMeshProUGUI>().text = userData.Value.date.ToString();
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