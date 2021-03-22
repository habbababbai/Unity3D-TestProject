using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] public GameObject spherePrefab;
    [SerializeField] private int numberOfSpheres;
    [SerializeField] private float timeBetweenSpawn;
    [SerializeField] private UICounter uiCounter;
    private int _counter;

    [SerializeField] private Vector3 size;
    void Start()
    {
        StartCoroutine(SpawnSpheres(numberOfSpheres));
        _counter = 0;
        Physics.IgnoreLayerCollision(3, 6);
    }

    private IEnumerator SpawnSpheres(int numberOfSpheres)
    {
        while (true)
        {
            Vector3 rndPosition = new Vector3(
                Random.Range( -size.x/2, size.x/2), 
                size.y/2, 
                Random.Range( -size.z/2, size.z/2));
            
            Instantiate(spherePrefab, rndPosition, transform.rotation);
            _counter++;
            uiCounter.UpdateUI(_counter.ToString());

            if (_counter == numberOfSpheres)
            {
                var sphereList = FindObjectsOfType<Sphere>();
                foreach (var sphere in sphereList)
                {
                    sphere.ChangeBehaviour();
                    StartCoroutine(sphere.StopCollision());
                }
               
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(gameObject.transform.position, size);
    }
    
}
