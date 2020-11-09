using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Om dit neer te kunnen zetten heb ik gebruik gemaakt van
//https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
//en 
//https://github.com/keijiro/Boids
public class BoidsSpawner : MonoBehaviour
{
    public int spawnAmount;
    public GameObject boidPrefab;
    public GameObject[] targets;
    [Range(0.1f, 20.0f)]
    public float boidSpeed = 6.0f;
    [Range(0.0f, 0.9f)]
    public float boidSpeedVariation = 0.5f;
    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;
    [Range(0.1f, 10.0f)]
    public float boidDistance = 2.0f;

    public LayerMask layerSearch;

    private void Start()
    {
        for (int i = 0; i < spawnAmount; i++) Spawn();
    }

    public GameObject Spawn()
    {
        Vector3 v3Pos = new Vector3(-0.15f, .5f, 10);
        v3Pos = Camera.main.ViewportToWorldPoint(v3Pos);

        Quaternion rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
        GameObject boid = Instantiate(boidPrefab, v3Pos, rotation) as GameObject;
        boid.GetComponent<BoidsControl>().valueController = this;
        return boid;
    }
}
