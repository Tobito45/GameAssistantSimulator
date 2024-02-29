using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitController : MonoBehaviour
{
    [SerializeField]
    private Camera[] camers;

    public void SetActiveCamers(bool set)
    {
        if(!set)
        {
            foreach (var item in camers)
            {
                item.gameObject.SetActive(false);
            }
        } else
        {
            int countCamers = KeyboardAndJostickController.GetCountGamepads();
            for(int i = 0; i < countCamers; i++)
            {
                camers[i].gameObject.SetActive(true);
            }


            switch(countCamers)
            {
                case 1:
                    camers[0].rect = new Rect(0,0,1,1); 
                    break;
                case 2:
                    camers[0].rect = new Rect(0.5f, 0, 0.5f, 1);
                    camers[1].rect = new Rect(0f, 0, 0.5f, 1);
                    break;
                case 3:
                    camers[0].rect = new Rect(0,0.5f, 0.5f, 0.5f);
                    camers[1].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    camers[2].rect = new Rect(0.5f,0.5f, 0.5f, 0.5f);
                    break;
                case 4:
                    camers[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    camers[0].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    camers[1].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    camers[2].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    break;

            }
        }

    }
}
