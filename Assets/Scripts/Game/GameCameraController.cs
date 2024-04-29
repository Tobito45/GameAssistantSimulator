using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
    private float _basicCameraValueZoom;
    private Camera _camera;
    private Vector3 _positionStart;
    private bool _gameWasEnd;

    [SerializeField]
    private int _index;

    [Header("Parameters")]
    [SerializeField]
    private float _speedZoom;
    [SerializeField]
    private float _speedMovement;
    [SerializeField]
    private float _minValueZoom;
    [SerializeField]
    private float _maxDistanceToMove;

    void Start()
    {
        GameController.Instance.OnStartNewGame += OnStartGame;
        GameController.Instance.OnEndGame += OnEndGame;

        _camera = GetComponent<Camera>();
        _basicCameraValueZoom = _camera.fieldOfView;
        _positionStart = _camera.gameObject.transform.position;
    }

    private void OnStartGame()
    {
        _camera.fieldOfView = _basicCameraValueZoom;
        _camera.gameObject.transform.position = _positionStart;
        _gameWasEnd = false;
    }

    private void OnEndGame(int index)
    {
        if (_index != index)
            return;

        _gameWasEnd = true;
    }

    void Update()
    {
        if (_gameWasEnd == true || GameController.Instance.MainController.IsMenu || GameController.Instance.IsOpenedPanelUI[_index])
            return;

        if (KeyboardAndJostickController.GetUpButton().Contains(_index) && _camera.fieldOfView > _minValueZoom)
                _camera.fieldOfView -= _speedZoom * Time.deltaTime;
        
        if (KeyboardAndJostickController.GetDownButton().Contains(_index) && _camera.fieldOfView < _basicCameraValueZoom)
            _camera.fieldOfView += _speedZoom * Time.deltaTime;

        if (KeyboardAndJostickController.GetLeftButton().Contains(_index) && _camera.transform.position.x < _positionStart.x + _maxDistanceToMove)
            _camera.transform.position += new Vector3(_speedMovement * Time.deltaTime, 0, 0);

        if (KeyboardAndJostickController.GetRigthButton().Contains(_index) &&  _camera.transform.position.x > _positionStart.x - _maxDistanceToMove)
            _camera.transform.position -= new Vector3(_speedMovement * Time.deltaTime, 0, 0);

    }
}
