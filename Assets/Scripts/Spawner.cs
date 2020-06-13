using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Boid prefab;
    public Depredador prefabDepredador;
    public int numeroBoids = 20;
    public int numeroDepredadores = 5;
    public int radioSpawner = 5;
    public BoidSettings settings;
    public DepredadorSettings settingsDepredador;
    public RegionManager rM;

    //void Awake() {
    void Start() {
        //Lista de boids
        List<Boid> todosBoids = new List<Boid>();
        //Inicializamos los boids
        for (int i = 0; i < numeroBoids; i++) {
            Vector3 posRandom = transform.position + Random.insideUnitSphere * radioSpawner;
            var boid = Instantiate(prefab);
            //boid.transform.position = posRandom;
            boid.Inicializar(settings, posRandom);

            boid.transform.position = posRandom;
            boid.transform.forward = Random.insideUnitSphere;

            todosBoids.Add(boid);
        }
        //Ya no es necesario porque lo administra el RegionManager
        //Añadimos la lista de boids a cada boid para que saquen las distancias
        for (int i = 0; i < numeroBoids; i++) {
            todosBoids[i].todosBoids = todosBoids;
        }

        //DEPREDADOR
        //Inicializamos los boids
        Vector3 diferenciaD = new Vector3(3f,3f,3f);
        for (int i = 0; i < numeroDepredadores; i++) {
            Vector3 posRandom = transform.position + diferenciaD + Random.insideUnitSphere * radioSpawner;
            var dep = Instantiate(prefabDepredador);
            dep.Inicializar(settingsDepredador, posRandom);

            dep.transform.position = posRandom;
            dep.transform.forward = Random.insideUnitSphere;
        }
    }
}
