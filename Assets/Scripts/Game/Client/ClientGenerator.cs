using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClientGenerator : MonoBehaviour
{
    private const float moveDuration = 1f;
    private const float rotationSpeed = 260f;
    private const float targetWalk = 0f;
    private const float targetOut = -90f;
    private const float timeBetween = 0.1f;
    private const float timeToWait = 2;

    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private Coroutine lastCoroutine;

    [Header("Monitor")]
    [SerializeField]
    private MonitorSelectGood _goods;

    [Header("Clients")]
    [SerializeField]
    private GameObject[] _clientsPrefabs;

    private GameObject[] _aktualClient = new GameObject[KeyboardAndJostickController.MAXPLAYERS];

    [Header("Way")]
    //public InspectorArray<Transform[]>[] wayPoints;
    private Transform[] _aktualTransform = new Transform[KeyboardAndJostickController.MAXPLAYERS];
    private bool _isRotating = false;

    //[SerializeField]
    //private Transform[] _spawnTransform;
    
    [SerializeField]
    private ClientGeneratorPlayerIterrator[] _transformClientInfo = new ClientGeneratorPlayerIterrator[KeyboardAndJostickController.MAXPLAYERS];


    public event Action OnClientDestroy;

    private void Start()
    {
        _goods.AfterPay += ClientPayed;
    }

    public void SpawnClient(int index)
    {
        if (_aktualClient[index] == null)
        {
            _aktualClient[index] = Instantiate(_clientsPrefabs[UnityEngine.Random.Range(0, _clientsPrefabs.Length)],
                _transformClientInfo[index].GetSpawnPoint.position, _transformClientInfo[index].GetSpawnPoint.rotation);
            _aktualTransform[index] = _transformClientInfo[index].GetWayPoints[0];

            activeCoroutines.Add(StartCoroutine(MoveToPlayer(_aktualClient[index], index)));

        }
    }

    private IEnumerator MoveToPlayer(GameObject client, int index)
    {
        activeCoroutines.Add(StartCoroutine(MoveToTarget(client, _aktualTransform[index], index)));

        if (client.GetComponent<Animator>() != null)
        {
            while (client.GetComponent<Animator>().GetBool("Walk"))
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(timeToWait);
        }
        activeCoroutines.Add(StartCoroutine(RotateToTargetAngle(client, targetWalk)));


    }

    private IEnumerator MoveToOutSide(GameObject client, int index)
    {
        activeCoroutines.Add(StartCoroutine(RotateToTargetAngle(client, targetOut)));
        while (_isRotating)
        {
            yield return null;
        }
        _aktualTransform[index] = _transformClientInfo[index].GetWayPoints[1];
       
        activeCoroutines.Add(StartCoroutine(MoveToTarget(client, _aktualTransform[index], index)));

        if (client.GetComponent<Animator>() != null)
        {
            while (client.GetComponent<Animator>().GetBool("Walk"))
            {
                yield return null;
            }
        } else
        {
            yield return new WaitForSeconds(timeToWait);
        }
        Destroy(client);
        yield return new WaitForSeconds(timeBetween);
        StopAllCoroutinesExceptCurrent();
        GameController.Instance.NextGenerete(index);
        StopCoroutine(lastCoroutine);
    }

    private IEnumerator MoveToTarget(GameObject objToMove, Transform target, int index)
    {
        if(_aktualClient[index].GetComponent<Animator>() != null)
            _aktualClient[index].GetComponent<Animator>().SetBool("Walk", true);
        Vector3 startPosition = objToMove.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            // Вычисляем прогресс перемещения в диапазоне от 0 до 1
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Интерполируем позицию объекта по ходу времени
            objToMove.transform.position = Vector3.Lerp(startPosition, target.position, t);

            yield return null;
        }

        // Убеждаемся, что объект точно достиг целевой точки
        objToMove.transform.position = target.position;
        if (_aktualClient[index].GetComponent<Animator>() != null)
            _aktualClient[index].GetComponent<Animator>().SetBool("Walk", false);

    }


    private IEnumerator RotateToTargetAngle(GameObject objectRotate, float target)
    {
        _isRotating = true;
        Quaternion startRotation = objectRotate.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, target, 0f); // Целевой поворот по заданному углу
        float elapsedTime = 0f;

        while (objectRotate != null && elapsedTime < Mathf.Abs(target - objectRotate.transform.eulerAngles.y) / rotationSpeed)
        {
            elapsedTime += Time.deltaTime;

            // Вычисляем прогресс поворота в диапазоне от 0 до 1
            float t = Mathf.Clamp01(elapsedTime / (Mathf.Abs(target - objectRotate.transform.eulerAngles.y) / rotationSpeed));

            // Интерполируем вращение объекта по ходу времени
            objectRotate.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }


        // Убеждаемся, что объект точно завершил поворот к целевому углу
        if(objectRotate != null)
        objectRotate.transform.rotation = targetRotation;
        _isRotating = false;
    }
    private void StopAllCoroutinesExceptCurrent()
    {
        foreach (Coroutine coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        activeCoroutines.Clear(); // Очистка списка активных корутин
    }

    public void ClientPayed(int index)
    {
        if (_aktualClient[index] != null)
            lastCoroutine = StartCoroutine(MoveToOutSide(_aktualClient[index], index));
    }
}

[System.Serializable]
public class InspectorArray<T>
{
    [SerializeField]
    private T elements;

    public T GetElements => elements;

}


[System.Serializable]
class ClientGeneratorPlayerIterrator
{
    [SerializeField]
    private Transform[] _wayPoints;

    [SerializeField]
    private Transform _spawnPoint;

    public Transform[] GetWayPoints => _wayPoints;
    public Transform GetSpawnPoint => _spawnPoint;
}