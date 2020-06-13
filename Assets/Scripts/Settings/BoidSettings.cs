using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nos permite crear el asset BoidSettings en el inspector de unity
[CreateAssetMenu]
///<summary>Valores base de todos los Boids.</summary>
public class BoidSettings : ScriptableObject {
    public float minSpeed = 2f;
    public float maxSpeed = 8f;
    public float radioPercepcion = 2.5f;
    ///<summary>Distancia de separacion entre boids</summary>
    public float dstSeparacion = 2.5f;

    [Header ("Pesos reglas")]
    //Importancia de las 3 reglas base. Cohesion, separacion y alineacion
    public float pesoCohesion = 1f;
    public float pesoSeparacion = 1f;
    public float pesoAlineacion = 1f;
    public float pesoHuirDepredador = 3f;
    public float pesoEvitarChoque = 10f;

    [Header("Colision")]
    ///<summary>Evitan todo obstaculo que este a esta distancia</summary>
    public float distanciaEvitarColision = 5f;
    public LayerMask mascaraObstaculos;
}
