using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayDetect : MonoBehaviour
{
    private RotateController _rotateController;


    private const float _rayDuration = 1f;
    private const float _maxDistance = 5f;

    private bool isRayActive = false;

    Ray ray;
    RaycastHit hit;
    RaycastHit[] hits;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartRay();
        } else if (Input.GetMouseButtonUp(0))
        {
            isRayActive = false;
        }

        if(isRayActive)
        {
            hits = Physics.RaycastAll(ray, _maxDistance);
            if (hits.Length > 0)
            {
                Debug.Log(hits.Last().collider.gameObject);
            }
            if(hits.Length > 0 && hits.First().collider.gameObject.GetComponent<DragObject>() && 
                hits.First().collider.gameObject == _rotateController.ObjectRotate.GetComponent<DragObject>().GetQCodeObject)
            {
                Debug.Log("Луч достиг цели!");
            } 

            /*if (Physics.Raycast(ray, out hit, 5f))
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject.GetComponent<DragObject>())
                {
                    Debug.Log("Луч достиг цели!");
                    // Вызывайте нужный вам метод, когда луч достигает цели
                }
            }*/
        }
    }

    private void StartRay()
    {
        if (!isRayActive)
        {
            //StartCoroutine(ShootRay());
            isRayActive = true;
            ray = new Ray(transform.position, transform.forward);

            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, _rayDuration);
        }
    }

    private System.Collections.IEnumerator ShootRay()
    {
        while (Input.GetMouseButton(0))
        {
          
            yield return new WaitForSeconds(_rayDuration);

        }
    }
}
