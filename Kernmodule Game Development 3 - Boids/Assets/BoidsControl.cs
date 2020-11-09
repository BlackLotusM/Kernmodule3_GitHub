using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

//Om dit neer te kunnen zetten heb ik gebruik gemaakt van
//https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
//en 
//https://github.com/keijiro/Boids
public class BoidsControl : MonoBehaviour
{
    public BoidsSpawner valueController;
    float noiseOffset;
    
    public GameObject targ;
    public Material[] targMMat;

    Vector3 separation;
    Vector3 alignment;
    Vector3 cohesion;
    Collider[] nearbyBoids;

    private void Start()
    {
        noiseOffset = Random.value * 10.0f;
        int rand = Random.Range(0, valueController.targets.Length);
        targ = valueController.targets[rand];
        this.gameObject.GetComponent<MeshRenderer>().material = targMMat[rand];
    }
    
    void Update()
    {
        Vector3 boidPos = transform.position;
        Quaternion boidRot = transform.rotation;
        if (Vector3.Distance(boidPos, targ.transform.position) <= 4.4f)
        {
            changeTarg();
        }

        float noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        float velocity = valueController.boidSpeed * (1.0f + noise * valueController.boidSpeedVariation);

        separation = Vector3.zero;
        alignment = targ.transform.forward;
        cohesion = targ.transform.position;

        nearbyBoids = Physics.OverlapSphere(boidPos, valueController.boidDistance, valueController.layerSearch);

        foreach (Collider boid in nearbyBoids)
        {
            if (boid.gameObject == gameObject) continue;
            Transform t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.forward;
            cohesion += t.position;
        }

        var avg = 1.0f / nearbyBoids.Length;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - boidPos).normalized;

        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

        if (rotation != boidRot)
        {
            var ip = Mathf.Exp(-valueController.rotationCoeff * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(rotation, boidRot, ip);
        }

        transform.position = boidPos + transform.forward * (velocity * Time.deltaTime);
    }

    public void changeTarg()
    {
        int rand = Random.Range(0, valueController.targets.Length);
        targ = valueController.targets[rand];
        this.gameObject.GetComponent<MeshRenderer>().material = targMMat[rand];
    }

    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / valueController.boidDistance);
        return diff * (scaler / diffLen);
    }
}
