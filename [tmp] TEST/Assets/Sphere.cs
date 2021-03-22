using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sphere : MonoBehaviour
{
    [SerializeField] private float pullForce = 25;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private float disabledCollidersTime = 0.5f;
    [SerializeField] private float destroyPushForce = 350f;
    private float _pullRadius;
    private Rigidbody _sphereRb;
    private SphereCollider _sphereCollider;
    private bool _isPulling;
    public int NumberOfSpheres { get; private set; }
    void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereRb = GetComponent<Rigidbody>();
        _pullRadius = _sphereCollider.radius * 5;
        _isPulling = true;
        NumberOfSpheres = 1;
        _sphereRb.mass = (float) VolumeOfSphere(0.5);
    }

    //     radius 0,5 dla scale 1 -> promien wynosi polowe skali
    // domyślna kula ma skale 1 - promien wynosi 0.5 - pole powierzchni 3.14159265 - objetosc 0.523598776
    // masa wynosi 0.523598776 przy zalozeniu objetosci rownej 1
    // masa krytyczna = 26,1799388 ( masa 50 kul) < 17 kul
    // 
    
    public void SetSphereProperties(int newNumberOfSpheres)
    {
        NumberOfSpheres = newNumberOfSpheres;
        double newArea = 0;
        for (int i = 0; i < newNumberOfSpheres; i++)
        {
            newArea += AreaOfSphere(0.5f);
        }
        float newRadius = (float)RadiusOfSphere(newArea);
        _pullRadius = newRadius * 5;
        gameObject.transform.localScale = new Vector3(newRadius * 2, newRadius * 2, newRadius * 2);
        var rb = GetComponent<Rigidbody>();
        rb.mass = (float) VolumeOfSphere(newRadius);
    }
    //Metoda używana do ustawiania odpowiednich wartości pojedynczych kul podczas eksplozji
    public void SetDefaultSphere()
    {
        NumberOfSpheres = 1;
        gameObject.transform.localScale = Vector3.one;
        _pullRadius = 0.5f * 5;
        var rb = GetComponent<Rigidbody>();
        rb.mass = 0.523598776f;    //Wartość objętości dla promienia równego 0.5
    }
    
    
    private double AreaOfSphere(double radius)
    {
        return 4 * Math.PI * Math.Pow(radius, 2);
    }
    private double VolumeOfSphere(double radius)
    {
        return (4 / 3) * Math.PI * Math.Pow(radius, 3);
    }
    private double RadiusOfSphere(double area)
    {
        return Math.Sqrt(area / (4 * Math.PI));
    }
    private void FixedUpdate()
    {
        if (NumberOfSpheres >= 17)
        {
            DestroySphere();
        }
        foreach (Collider col in Physics.OverlapSphere(transform.position, _pullRadius))
        {
            if (col.CompareTag(gameObject.tag))
            {
                Vector3 forceDirection = transform.position - col.transform.position;
                if (_isPulling && col.gameObject.layer == 3)
                    col.attachedRigidbody.AddForce(forceDirection.normalized * pullForce);
                else if (!_isPulling)
                    col.attachedRigidbody.AddForce(-forceDirection.normalized * pullForce);
            }
        }
    }

    private void DestroySphere()
    {
        StartCoroutine(StopCollision());
        for (int i = 0; i < NumberOfSpheres; i++)
        {
            var gameObj = Instantiate(spherePrefab, Random.insideUnitSphere * transform.localScale.x/2 + transform.position, Quaternion.identity);
            var sphere = gameObj.GetComponent<Sphere>();
            sphere.SetDefaultSphere();
            StartCoroutine(sphere.StopCollision(disabledCollidersTime));
            sphere.ShootSphere();
        }
        Destroy(gameObject);
    }
    public void ShootSphere()
    {
        StartCoroutine(StopCollision(5.0f));
        Transform trnsfrm = transform;
        trnsfrm.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f).normalized;
        GetComponent<Rigidbody>().AddForce(trnsfrm.forward * destroyPushForce);
    }

    public IEnumerator StopCollision(float timeSpan = 0)
    {
        // 3 - layer z kolizja
        // 6 - layer bez kolizji
        var gObj = gameObject;
        if (timeSpan == 0)
        {
            gObj.layer = 6;
        }
        else
        {
            gObj.layer = 6;
            yield return  new WaitForSecondsRealtime(timeSpan);
            gObj.layer = 3;
        }
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag(gameObject.tag) && col.collider.gameObject.layer == 3)
        {
            Sphere otherSphere = col.collider.GetComponent<Sphere>();
            if (col.gameObject.GetInstanceID() <= GetInstanceID())
            {
                NumberOfSpheres += otherSphere.NumberOfSpheres;
                SetSphereProperties(NumberOfSpheres);
                Destroy(col.gameObject);
            }
        }
    }
    

    public void ChangeBehaviour()
    {
        _isPulling = !_isPulling;
    }
    
}
