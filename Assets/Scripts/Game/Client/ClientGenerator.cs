using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClientGenerator : MonoBehaviour
{
    [Header("Parameters")]

    [SerializeField]
    private float moveDuration = 1f;
    
    [SerializeField]
    private float rotationSpeed = 260f;
    [SerializeField] 
    private float targetWalk = 0f;

    [SerializeField]
    private const float targetOut = -90f;
    
    [SerializeField]
    private const float timeBetween = 0.1f;

    [SerializeField]
    private const float timeToWait = 2;

    private List<Coroutine>[] activeCoroutines = new List<Coroutine>[KeyboardAndJostickController.MAXPLAYERS];
    private Coroutine[] lastCoroutine = new Coroutine[KeyboardAndJostickController.MAXPLAYERS];

    [Header("Monitor")]
    [SerializeField]
    private MonitorSelectGood _goods;

    [Header("Clients")]
    [SerializeField]
    private GameObject[] _clientsPrefabs;

    private GameObject[] _aktualClient = new GameObject[KeyboardAndJostickController.MAXPLAYERS];

    [Header("Way")]
    private Transform[] _aktualTransform = new Transform[KeyboardAndJostickController.MAXPLAYERS];
    private bool _isRotating = false;

    [SerializeField]
    private ClientGeneratorPlayerIterator[] _transformClientInfo = new ClientGeneratorPlayerIterator[KeyboardAndJostickController.MAXPLAYERS];


    public event Action OnClientDestroy;

    private void Start()
    {
        _goods.AfterPay += ClientPayed;

        for (int i = 0; i < activeCoroutines.Length; i++)
        {
            activeCoroutines[i] = new List<Coroutine>();
        }
    }

    public void SpawnClient(int index)
    {
        if (_aktualClient[index] == null)
        {
            _aktualClient[index] = Instantiate(_clientsPrefabs[UnityEngine.Random.Range(0, _clientsPrefabs.Length)],
                _transformClientInfo[index].GetSpawnPoint.position, _transformClientInfo[index].GetSpawnPoint.rotation);
            _aktualTransform[index] = _transformClientInfo[index].GetWayPoints[0];

            activeCoroutines[index].Add(StartCoroutine(MoveToPlayer(_aktualClient[index], index)));

        }
    }

    private IEnumerator MoveToPlayer(GameObject client, int index)
    {
        activeCoroutines[index].Add(StartCoroutine(MoveToTarget(client, _aktualTransform[index], index)));

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
        activeCoroutines[index].Add(StartCoroutine(RotateToTargetAngle(client, targetWalk)));


    }

    private IEnumerator MoveToOutSide(GameObject client, int index)
    {
        activeCoroutines[index].Add(StartCoroutine(RotateToTargetAngle(client, targetOut)));
        while (_isRotating)
        {
            yield return null;
        }
        _aktualTransform[index] = _transformClientInfo[index].GetWayPoints[1];
       
        activeCoroutines[index].Add(StartCoroutine(MoveToTarget(client, _aktualTransform[index], index)));

        if (client != null && client.GetComponent<Animator>() != null)
        {
            while ( client.GetComponent<Animator>().GetBool("Walk"))
            {
                yield return null;
            }
        } else
        {
            yield return new WaitForSeconds(timeToWait);
        }
        Destroy(client);
        yield return new WaitForSeconds(timeBetween);
        StopAllCoroutinesExceptCurrent(index);
        GameController.Instance.NextGenerete(index);
        StopCoroutine(lastCoroutine[index]);
    }

    private IEnumerator MoveToTarget(GameObject objToMove, Transform target, int index)
    {
        if(_aktualClient[index] != null && _aktualClient[index].GetComponent<Animator>() != null)
            _aktualClient[index].GetComponent<Animator>().SetBool("Walk", true);
        Vector3 startPosition = objToMove.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            objToMove.transform.position = Vector3.Lerp(startPosition, target.position, t);

            yield return null;
        }

        objToMove.transform.position = target.position;
        if (_aktualClient[index].GetComponent<Animator>() != null)
            _aktualClient[index].GetComponent<Animator>().SetBool("Walk", false);

    }


    private IEnumerator RotateToTargetAngle(GameObject objectRotate, float target)
    {
        _isRotating = true;
        Quaternion startRotation = objectRotate.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, target, 0f);
        float elapsedTime = 0f;

        while (objectRotate != null && elapsedTime < Mathf.Abs(target - objectRotate.transform.eulerAngles.y) / rotationSpeed)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / (Mathf.Abs(target - objectRotate.transform.eulerAngles.y) / rotationSpeed));

            objectRotate.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        if(objectRotate != null)
            objectRotate.transform.rotation = targetRotation;
        _isRotating = false;
    }
    private void StopAllCoroutinesExceptCurrent(int index)
    {
        foreach (Coroutine coroutine in activeCoroutines[index])
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        activeCoroutines[index].Clear();
    }

    public void ClientPayed(int index)
    {
        if (_aktualClient[index] != null)
            lastCoroutine[index] = StartCoroutine(MoveToOutSide(_aktualClient[index], index));
    }
}

[System.Serializable]
class ClientGeneratorPlayerIterator
{
    [SerializeField]
    private Transform[] _wayPoints;

    [SerializeField]
    private Transform _spawnPoint;

    public Transform[] GetWayPoints => _wayPoints;
    public Transform GetSpawnPoint => _spawnPoint;
}