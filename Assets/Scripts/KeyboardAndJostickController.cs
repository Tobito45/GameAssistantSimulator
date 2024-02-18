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

            return (horizontalInput, verticalInput);
        }
        else
        {
            return (-Gamepad.all[index].leftStick.ReadValue().x, Gamepad.all[index].leftStick.ReadValue().y);
           // var list = new List<(float horizontal, float vertical)>();
           // list = Gamepad.all
           //        .Select((gamepad, index) => (gamepad, index))
           //        .Select(x => (-x.gamepad.leftStick.ReadValue().x, -x.gamepad.leftStick.ReadValue().y))
           //        .ToList();

            // return list;
        }

    }
    public static IEnumerable<int> GetAButton()
    {
        if (IsJosticConnected)
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.aButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
        return null;
    }
    
    public static IEnumerable<int> GetBButton()
    {
        if (IsJosticConnected)
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.bButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
        return null;
    }

    public static IEnumerable<int> GetYButton()
    {
        if (IsJosticConnected)
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.yButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
        return null;
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

    public static IEnumerable<int> MoveGoodsConveyor()
    {
        if (!IsJosticConnected)
            return Input.GetKey(KeyCode.Space) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.rightTrigger.isPressed)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
    }

    public static IEnumerable<int> SelectNextGoodOnMonitor()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.H) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.xButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
    }
    public static IEnumerable<int> ConfirmPayment()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.N) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.startButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();

            return list;
        }
    } 
    
    public static IEnumerable<int> SelectGoodOnMonitor()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.J) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.yButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
        }
    }

    public static IEnumerable<int> ChangeGoods()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.Tab) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.leftTrigger.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
        }

    } 
    
    public static IEnumerable<int> TakeGood()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.L) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.aButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
        }

       
    }
    
    public static IEnumerable<int> LetsGoGood()
    {
        if (!IsJosticConnected)
            return Input.GetKeyDown(KeyCode.K) ? new List<int> { 0 } : new List<int>();
        else
        {
            var list = new List<int>();
            list = Gamepad.all
                   .Select((gamepad, index) => (gamepad, index))
                   .Where(x => x.gamepad.bButton.wasPressedThisFrame)
                   .Select(x => x.index)
                   .ToList();
            return list;
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

      public int targetX = 20; // Новая X-координата курсора
      public int targetY = 20; // Новая Y-координата курсора


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
          //Cursor.lockState = CursorLockMode.Confined; // Заблокировать курсор внутри окна
          //Cursor.visible = false;                    // Скрыть стандартный курсор

          //Mouse.current.position = 
          // Установить новую позицию курсора
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
                   // Вызываем метод или обработчик для нажатия на объект
                   // Например, вы можете вызвать событие OnClick на нажатом объекте
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
