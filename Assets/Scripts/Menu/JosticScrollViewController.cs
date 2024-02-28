using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JosticScrollViewController : MonoBehaviour
{
    private bool _isEnabled = false;

    [SerializeField]
    private int _index = -1; //-1 is to everyone

    [SerializeField]
    private Scrollbar _horizontalScrollBar, _vericalScrollBar;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private bool _controllWithStics;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isEnabled)
            return;

        (float horizontal, float vertical) movement = (0,0);
        if (_controllWithStics)
            movement = KeyboardAndJostickController.GetMovement(_index);

       // Debug.Log(movement.horizontal + " " + );
        if (((_index == -1 && !_controllWithStics && KeyboardAndJostickController.GetUpButton().Count() > 0) ||
                KeyboardAndJostickController.GetUpButton().Contains(_index)) ||
                (_controllWithStics && movement.vertical > 0.25f))
        {
            if(_vericalScrollBar != null)
                _vericalScrollBar.value += _speed * Time.deltaTime;
        }

        //if ((_index == -1 && KeyboardAndJostickController.GetDownButton().Count() > 0) || KeyboardAndJostickController.GetDownButton().Contains(_index))
        if (((_index == -1 && !_controllWithStics && KeyboardAndJostickController.GetDownButton().Count() > 0) ||
                KeyboardAndJostickController.GetDownButton().Contains(_index)) ||
                (_controllWithStics && movement.vertical < -0.25f))
        {
            if (_vericalScrollBar != null)
                _vericalScrollBar.value -= _speed * Time.deltaTime;
        }

        //if ((_index == -1 && KeyboardAndJostickController.GetLeftButton().Count() > 0) || KeyboardAndJostickController.GetLeftButton().Contains(_index))
        if (((_index == -1 && !_controllWithStics && KeyboardAndJostickController.GetLeftButton().Count() > 0) || 
                KeyboardAndJostickController.GetLeftButton().Contains(_index)) ||
                (_controllWithStics && movement.horizontal > 0.25f))
        {
            if (_horizontalScrollBar != null)
                _horizontalScrollBar.value -= _speed * Time.deltaTime;
        }

        //if ((_index == -1 && KeyboardAndJostickController.GetRigthButton().Count() > 0) || KeyboardAndJostickController.GetRigthButton().Contains(_index))
        if (((_index == -1 && !_controllWithStics && KeyboardAndJostickController.GetRigthButton().Count() > 0) ||
                KeyboardAndJostickController.GetRigthButton().Contains(_index)) ||
                (_controllWithStics && movement.horizontal < -0.25f))
        {
            if (_horizontalScrollBar != null)
                _horizontalScrollBar.value += _speed * Time.deltaTime;
        }

    }

    private void OnEnable()
    {
        _isEnabled = true;
    }

    private void OnDisable()
    {
        _isEnabled = false;
    }
}
