using UnityEngine;

public class RayDetect : MonoBehaviour //can be deleted
{
    [SerializeField]
    private int index;

    [SerializeField]
    private RotateController _rotateController;


    private const float rayDuration = 1f;
    private const float maxDistance = 5f;

    private bool _isRayActive = false;

    private Ray _ray;

    public delegate void OnRayDetect(RayDetect ray, GoodInfo info);
    public event OnRayDetect OnRayDetectNesseryObject;

    public int Index => index;
    
    private void Update()
    {
     /*   if (Input.GetMouseButtonDown(0))
        {
         //   StartRay();
        } else if (Input.GetMouseButtonUp(0))
        {
            _isRayActive = false;
        }

        if(_isRayActive)
        {
            Debug.DrawRay(_ray.origin, _ray.direction * maxDistance, Color.red, rayDuration);

            RaycastHit hit;

            if (Physics.Raycast(ray: _ray, out hit, maxDistance: maxDistance))
            {
                if (_rotateController.ObjectRotate != null 
                    && hit.collider.gameObject == _rotateController.DragObject.GetQCodeObject
                    && _rotateController.GoodInfo.IsScaned == false)
                {
                    OnRayDetectNesseryObject(ray: this, info: _rotateController.GoodInfo);

                }
            }
   
        }*/
    }

    private void StartRay()
    {
        if (!_isRayActive)
        {
            _isRayActive = true;
            _ray = new Ray(transform.position, transform.forward);

        }
    }
}
