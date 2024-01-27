using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class KeyboardAndJostickController : MonoBehaviour
{
    public  const int  MAXPLAYERS = 4;
    public static bool IsJosticConnected => Gamepad.all.Count > 0;


    public void Update()
    {
        //if (Gamepad.current.rightTrigger.isPressed)
        //  Debug.Log(Gamepad.current.displayName + " " + Gamepad.current.deviceId + " " + Gamepad.current.);

      /*  Gamepad[] gamepads = Gamepad.all.ToArray();

        for (int i = 0; i < gamepads.Length; i++)
        {
            if (gamepads[i].rightTrigger.isPressed)
            {
                Debug.Log(i);
            }
        }*/
    }



    public static int GetCountGamepads()
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
        } else
        {

            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (i == index)
                {
                    //horizontalInput = -Input.GetAxis("Horizontal");
                    //verticalInput = Input.GetAxis("Vertical");
                    horizontalInput = -Gamepad.all[i].leftStick.ReadValue().x;
                    verticalInput = Gamepad.all[i].leftStick.ReadValue().y;
                }
            }
        }

        return (horizontalInput, verticalInput);
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
        } else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if(i == index)
                {
                    horizontalInput = Gamepad.all[i].rightStick.ReadValue().x;
                    verticalInput = Gamepad.all[i].rightStick.ReadValue().y;
                }
            }

                
        }

        return (horizontalInput, verticalInput);
    }

    public static (bool isPressed, int index) MoveGoodsConveyor()
    {
        if (!IsJosticConnected)
            return (Input.GetKey(KeyCode.Space), 0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].rightTrigger.isPressed)
                {
                    return (true,  i); 
                }
            }
            return (false, 0);
        }
            //return Gamepad.current.rightTrigger.isPressed;
    }

    public static IEnumerable<int> SelectNextGoodOnMonitor()
    {
        if (!IsJosticConnected)
            return new List<int> { 0 }; //new List<( int index)> { (Input.GetKeyDown(KeyCode.H), 0) };
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.xButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
            /*) =). g.xButton.wasPressedThisFrame).Select(g => g.de)
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].xButton.wasPressedThisFrame)
                {
                    //return (true, i);
                    list.Add((true, i));
                }
            }
            return new List<int>(); //(false, 0);
        */
        }
    }
    public static (bool isPressed, int index) ConfirmPayment()
    {
        if (!IsJosticConnected)
            return (Input.GetKeyDown(KeyCode.N),0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].startButton.wasPressedThisFrame)
                {
                    return (true, i);
                }
            }
            return (false, 0);
        }
        //return Gamepad.current.startButton.wasPressedThisFrame;
    } 
    
    public static (bool isPressed, int index) SelectGoodOnMonitor()
    {
        if (!IsJosticConnected)
            return (Input.GetKeyDown(KeyCode.J), 0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].yButton.wasPressedThisFrame)
                {
                    return (true, i);
                }
            }
            return (false, 0);
        }
        //return Gamepad.current.yButton.wasPressedThisFrame;
    }

    public static (bool isPressed, int index) ChangeGoods()
    {
        if (!IsJosticConnected)
            return (Input.GetKeyDown(KeyCode.Tab), 0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].leftTrigger.wasPressedThisFrame)
                {
                    return (true, i);
                }
            }
            return (false, 0);
        }
    } 
    
    public static (bool isPressed, int index) TakeGood()
    {
        if (!IsJosticConnected)
            return (Input.GetKeyDown(KeyCode.L),0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].aButton.wasPressedThisFrame)
                {
                    return (true, i);
                }
            }
            return (false, 0);
        }
    }
    
    public static (bool isPressed, int index) LetsGoGood()
    {
        if (!IsJosticConnected)
            return (Input.GetKeyDown(KeyCode.K), 0);
        else
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (Gamepad.all[i].bButton.wasPressedThisFrame)
                {
                    return (true, i);
                }
            }
            return (false, 0);
        }
    }

    /*  [SerializeField]
      private float _speed = 5;

      [Flags]
      public enum MouseEventFlags
      {
          LeftDown = 0x00000002,
          LeftUp = 0x00000004,
          MiddleDown = 0x00000020,
          MiddleUp = 0x00000040,
          Move = 0x00000001,
          Absolute = 0x00008000,
          RightDown = 0x00000008,
          RightUp = 0x00000010
      }
      [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
      private static extern int SetCursorPos(int x, int y);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool GetCursorPos(out MousePoint lpMousePoint);

      [DllImport("user32.dll")]
      private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

      public static void SetCursorPosition(int X, int Y)
      {
          SetCursorPos(X, Y);
      }

      public static void SetCursorPosition(MousePoint point)
      {
          SetCursorPos(point.X, point.Y);
      }

      public static MousePoint GetCursorPosition()
      {
          MousePoint currentMousePoint;
          var gotPoint = GetCursorPos(out currentMousePoint);
          if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
          return currentMousePoint;
      }

      public int targetX = 20; // ����� X-���������� �������
      public int targetY = 20; // ����� Y-���������� �������


      private void Update()
      {
          //SetCursorPos(targetX, targetY);
           Vector2 rightStickInput = Gamepad.current.rightStick.ReadValue();
          //Vector2 movement = new Vector3(rightStickInput.x, rightStickInput.y) * _speed * Time.deltaTime;
          // SetCursorPos(movement.x + Mouse.current.position.value.x, movement.y + Mouse.current.position.value.y);
         // if (rightStickInput.x > 0.5f)
              targetX += (int)(rightStickInput.x * _speed * Time.deltaTime);
         // else if(rightStickInput.x < -0.5f)
         //     targetX += (int)(rightStickInput.x * 100 * Time.deltaTime / 2);

          //if (rightStickInput.y > 0.5f)
          //    targetY += (int)(rightStickInput.y * 100 * Time.deltaTime / 2);
          //else if (rightStickInput.y < -0.5f)
              targetY -= (int)(rightStickInput.y * _speed * Time.deltaTime);
          SetCursorPosition(targetX, targetY);

          if (Input.GetMouseButtonDown(0))
          {
              Debug.Log("Preset");
          }

          if (Input.GetMouseButtonUp(0))
          {
              Debug.Log("DontPresset");
          }

          if (Gamepad.current.aButton.isPressed)//Input.GetKeyDown(KeyCode.K))
          {
              JostickController.MouseEvent(JostickController.MouseEventFlags.LeftDown); // | JostickController.MouseEventFlags.LeftUp
          }
          if (Gamepad.current.aButton.wasReleasedThisFrame)//Input.GetKeyDown(KeyCode.K))
          {
              JostickController.MouseEvent(JostickController.MouseEventFlags.LeftUp); // | JostickController.MouseEventFlags.LeftUp
          }
          //Vector3 currentMousePosition = Input.mousePosition;
          //currentMousePosition.x += rightStickInput.x;
          //currentMousePosition.y += rightStickInput.y;
          //Cursor.lockState = CursorLockMode.Confined; // ������������� ������ ������ ����
          //Cursor.visible = false;                    // ������ ����������� ������

          //Mouse.current.position = 
          // ���������� ����� ������� �������
          //Cursor. position = currentMousePosition;
          //Debug.Log(Mouse.current.position.value + " " + movement);
          // _josrickMovementCursor.transform.Translate(movement);


          /*
           if (Mouse.current.leftButton.wasPressedThisFrame)
           {
               Ray ray = _mainController.GameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
               if (Physics.Raycast(ray, out RaycastHit hit))
               {
                   GameObject hitObject = hit.collider.gameObject;

                   Debug.Log(hitObject.name);
                   // �������� ����� ��� ���������� ��� ������� �� ������
                   // ��������, �� ������ ������� ������� OnClick �� ������� �������
                   //hitObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
               }
           }
      }

      private void OnEnable()
      {
          targetX = Screen.width;
          targetY = Screen.height;
      }
      public static void MouseEvent(MouseEventFlags value)
      {
          MousePoint position = GetCursorPosition();

          mouse_event
              ((int)value,
              position.X,
               position.Y,
               0,
               0)
               ;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct MousePoint
      {
          public int X;
          public int Y;

          public MousePoint(int x, int y)
          {
              X = x;
              Y = y;
          }

      }*/
}
