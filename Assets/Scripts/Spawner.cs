using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Boid prefab;
    public int numeroBoids = 20;
    public int radioSpawner = 5;
    public BoidSettings settings;
    public RegionManager rM;

    void Awake() {
        //Lista de boids
        List<Boid> todosBoids = new List<Boid>();
        //Inicializamos los boids
        for (int i = 0; i < numeroBoids; i++) {
            Vector3 posRandom = transform.position + Random.insideUnitSphere * radioSpawner;
            var boid = Instantiate(prefab);
            //boid.transform.position = posRandom;
            boid.Inicializar(settings, posRandom);

            boid.mapaRegiones = RegionManager.mapaRegiones;

            boid.transform.position = posRandom;
            boid.transform.forward = Random.insideUnitSphere;

            boid.numeroBoids = numeroBoids;
            todosBoids.Add(boid);
        }
        //Añadimos la lista de boids a cada boid para que saquen las distancias
        for (int i = 0; i < numeroBoids; i++) {
            todosBoids[i].todosBoids = todosBoids;
        }
    }
}
