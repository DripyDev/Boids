using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nos permite crear el asset BoidSettings en el inspector de unity
[CreateAssetMenu]
///<summary>Valores base de todos los Boids.</summary>
public class BoidSettings : ScriptableObject {
    public float minSpeed = 2f;
    public float maxSpeed = 8f;
    
    //Importancia de las 3 reglas base. Cohesion, separacion y alineacion
    public float pesoCohesion = 1f;
    public float pesoSeparacion = 1f;
    public float pesoAlineacion = 1f;

    public float radioPercepcion = 5f;
}
