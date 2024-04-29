using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JosticScrollSelecter {

    private float[] _timer = new float[KeyboardAndJostickController.MAXPLAYERS];
    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];
    private List<GameObject>[] _objectsWillBeIterated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];

    public GameObject SelectedObject(int index) => _selectedObject[index];
    public void ResetSelectedObject(int index) => _selectedObject[index] = null;
    public void ResetIndexForIterator(int index) => _indexForIterator[index] = -1;
    public void AddObjectThatWillBeIterated(int index, GameObject obj) => _objectsWillBeIterated[index].Add(obj);
    public void SetObjectsThatWillBeIterated(int index, List<GameObject> objs) => _objectsWillBeIterated[index] = objs;
    public JosticScrollSelecter() {
        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
            _objectsWillBeIterated[i] = new List<GameObject>();
    }

    public void NextObjectSelect(int index, int countAdd = 1)
    {
        _indexForIterator[index] += countAdd;

        if (_indexForIterator[index] >= _objectsWillBeIterated[index].Count || _indexForIterator[index] < 0)
            _indexForIterator[index] -= countAdd;

        if (_indexForIterator[index] < 0)
            _indexForIterator[index] = 0;

        OnSelectObject(_objectsWillBeIterated[index][_indexForIterator[index]], index);
    }

    public void OnSelectObject(GameObject button, int index)
    {
        if (_selectedObject[index] != null)
            _selectedObject[index].transform.GetChild(0).gameObject.SetActive(false);

        _selectedObject[index] = button.transform.gameObject;
        _selectedObject[index].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void MoveIterator(int index, int koeficientMovement)
    {
        if (_timer[index] < 0)
        {
            var movement = KeyboardAndJostickController.GetMovement(index);

            if (movement.horizontal > 0.25f)
                NextObjectSelect(index, -1);
            else if (movement.horizontal < -0.25f)
                NextObjectSelect(index, 1);
            else if (movement.vertical > 0.25f)
                NextObjectSelect(index, -_objectsWillBeIterated[index].Count / koeficientMovement);
            else if (movement.vertical < -0.25f)
                NextObjectSelect(index, _objectsWillBeIterated[index].Count / koeficientMovement);

            _timer[index] = 0.05f;
        }
        else
            _timer[index] -= Time.deltaTime;
    }
}
