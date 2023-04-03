using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMeteor : MonoBehaviour
{
    public GameObject meteorPrefab;
    public Transform startPoint;
    [SerializeField] Transform cubeParent;
    List<Vector3> cubes = new();
    List<GameObject> meteorPool = new();
    [SerializeField] int poolSize = 10;
    [SerializeField] float spawnRate = 5f;



    private void Start()
    {
        SetupCubes();
        InitializeMeteors();

        StartCoroutine(SpawnMeteors());
    }
    void SetupCubes()
    {
        for (int i = 0; i < cubeParent.childCount; ++i)
        {
            cubes.Add(cubeParent.GetChild(i).position);
        }
    }
    void InitializeMeteors()
    {
        for (int i = 0; i < poolSize; ++i)
        {
            GameObject meteor = PhotonNetwork.Instantiate(meteorPrefab.name, startPoint.position, Quaternion.identity);
            meteor.SetActive(false);
            meteorPool.Add(meteor);
        }
    }
    private IEnumerator SpawnMeteors()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            GameObject meteor = GetMeteorFromPool();
            if (meteor != null)
            {
                meteor.transform.position = startPoint.position;
                var target = GetRandomTargetPoint();
                RotateTo(meteor, target);
                meteor.SetActive(true);
            }
        }
    }

    Vector3 GetRandomTargetPoint()
    {
        var index = Random.RandomRange(0, cubes.Count);
        Vector3 pos = cubes[index];
        cubes.RemoveAt(index);
        return pos;
    }
    GameObject GetMeteorFromPool()
    {
        for (int i = 0; i < meteorPool.Count; i++)
        {
            if (!meteorPool[i].activeInHierarchy)
            {
                return meteorPool[i];
            }
        }
        //all active
        return null;
    }

    void RotateTo(GameObject obj, Vector3 destination)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }
}