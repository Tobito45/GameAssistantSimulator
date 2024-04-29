using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardAndJostickController : MonoBehaviour
{
    public const int MAXPLAYERS = 4;
    public static bool IsJosticConnected => Gamepad.all.Count > 0;

    //return 1 if count of gamepads is 0, because there is a keyboard as a controller
    public static int GetCountControllers()
    {
        if (Gamepad.all.Count == 0)
            return 1;
        else
            return Gamepad.all.Count;
    }

    public static (float horizontal, float vertical) GetMovement(int index)
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (!IsJosticConnected)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                horizontalInput = 1f;
            else if (Input.GetKey(KeyCode.RightArrow))
                horizontalInput = -1f;
            if (Input.GetKey(KeyCode.UpArrow))
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                verticalInput = -1f;

            return (horizontalInput, verticalInput);
        }
        else
            return (-Gamepad.all[index].leftStick.ReadValue().x, Gamepad.all[index].leftStick.ReadValue().y);
    }


    public static (float horizontal, float vertical) GetRotate(int index)
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;
        if (!IsJosticConnected)
        {
            if (Input.GetKey(KeyCode.A))
                horizontalInput = -1f;
            else if (Input.GetKey(KeyCode.D))
                horizontalInput = 1f;

            if (Input.GetKey(KeyCode.W))
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.S))
                verticalInput = -1f;
        }
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
                if (i == index)
                {
                    horizontalInput = Gamepad.all[i].rightStick.ReadValue().x;
                    verticalInput = Gamepad.all[i].rightStick.ReadValue().y;
                }
        }

        return (horizontalInput, verticalInput);
    }



    public static IEnumerable<int> GetAButton()
    {
        //creating list of indexes
        var list = new List<int>();
        //selecting of indexes
        list = Gamepad.all
               .Select((gamepad, index) => (gamepad, index))
               .Where(x => x.gamepad.aButton.wasPressedThisFrame)
               .Select(x => x.index)
               .ToList();

        return list;
    }
    public static IEnumerable<int> GetUpButton()
    {
        //creating list of indexes
        var list = new List<int>();
        if (IsJosticConnected)
        {
            //selecting of indexes
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.dpad.up.isPressed)
                   .Select(x => x.index)
                   .ToList();
        }
        else
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            list.Add(0);

        return list;
    }

    public static IEnumerable<int> GetDownButton()
    {
        //creating list of indexes
        var list = new List<int>();
        if (IsJosticConnected)
        {
            //selecting of indexes
            list = Gamepad.all
               .Select((gamepad, index) => (gamepad, index))
               .Where(x => x.gamepad.dpad.down.isPressed)
               .Select(x => x.index)
               .ToList();
        }
        else
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            list.Add(0);

        return list;
    }

    public static IEnumerable<int> GetLeftButton()
    {
        //creating list of indexes
        var list = new List<int>();
        //selecting of indexes
        list = Gamepad.all
               .Select((gamepad, index) => (gamepad, index))
               .Where(x => x.gamepad.dpad.left.isPressed)
               .Select(x => x.index)
               .ToList();
        return list;
    }


    public static IEnumerable<int> GetRigthButton()
    {
        //creating list of indexes
        var list = new List<int>();
        //selecting of indexes
        list = Gamepad.all
               .Select((gamepad, index) => (gamepad, index))
               .Where(x => x.gamepad.dpad.right.isPressed)
               .Select(x => x.index)
               .ToList();

        return list;
    }


    public static IEnumerable<int> GetBButton()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.K) ? new List<int> { 0 } : new List<int>();
        else
        {
            //creating list of indexes
            var list = new List<int>();
            //selecting of indexes
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.bButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
        }
    }

    public static IEnumerable<int> GetYButton()
    {
        //creating list of indexes
        var list = new List<int>();
        //selecting of indexes
        list = Gamepad.all
               .Select((gamepad, index) => (gamepad, index))
               .Where(x => x.gamepad.yButton.wasPressedThisFrame)
               .Select(x => x.index)
               .ToList();

        return list;
    }

    public static IEnumerable<int> GetButtonRT()
    {
        if (!IsJosticConnected)
            return Input.GetKey(KeyCode.Space) ? new List<int> { 0 } : new List<int>();
        else
        {
            //creating list of indexes
            var list = new List<int>();
            //selecting of indexes
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.rightTrigger.isPressed)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
    }

    public static IEnumerable<int> GetButtonLT()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.Tab) ? new List<int> { 0 } : new List<int>();
        else
        {
            //creating list of indexes
            var list = new List<int>();
            //selecting of indexes
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.leftTrigger.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
        }

    }
}
