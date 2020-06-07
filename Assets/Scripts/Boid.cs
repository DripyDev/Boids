using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings settings;
    ///<summary>Es la direccion(vector3) por la velocidad(float)</summary>
    public Vector3 velocidad;
    public int numeroBoids;
    public List<Boid> todosBoids;
    
    //Inicializamos el Boid
    public void Inicializar(BoidSettings set, Vector3 pos){
        this.settings = set;
        this.transform.position = pos;
    }

    float Distancia(Vector3 destino, Vector3 origen){
        Vector3 offset = destino - origen;
        return Mathf.Sqrt(offset.x*offset.x + offset.y*offset.y +offset.z*offset.z);
    }

    //Devuelve una lista con los boids en el rango settings.radioPercepcion de nosotros
    List<Boid> PercibirBoids(){
        List<Boid> manada = new List<Boid>();
        foreach (var b in todosBoids){
            //Vector3 offset = b.transform.position - this.transform.position;
            //float distancia = Mathf.Sqrt(offset.x*offset.x + offset.y*offset.y +offset.z*offset.z);
            float distancia = Distancia(b.transform.position, this.transform.position);
            if(distancia < settings.radioPercepcion){
                manada.Add(b);
            }
        }
        return manada;
    }

    Vector3 Cohesion(List<Boid> manada){
        Vector3 pC = new Vector3(0,0,0);
        foreach (var b in manada) {
            if (b!=this){
                pC += b.transform.position;
            }
        }
        pC = (manada.Count) > 1? pC/(manada.Count-1) : pC;
        return (pC-this.transform.position)*settings.pesoCohesion;
        //return new Vector3(0,0,0);
    }
    Vector3 Separacion(List<Boid> manada){
        Vector3 c = new Vector3(0,0,0);
        foreach (var b in manada) {
            if (b!=this){
                if(Distancia(b.transform.position, this.transform.position) > 3f){
                    c-= b.transform.position - this.transform.position;
                }
            }
        }
        return c*settings.pesoSeparacion;
        //return new Vector3(0,0,0);
    }

    Vector3 Alineacion(List<Boid> manada){
        Vector3 pV = new Vector3(0,0,0);
        foreach (var b in manada) {
            if (b!=this){
                pV += b.velocidad;
            }
        }
        pV = (manada.Count) > 1? pV/(manada.Count-1): pV;
        return (pV - this.velocidad)*settings.pesoAlineacion;
        //return new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update(){
        List<Boid> manada = new List<Boid>();
        manada = PercibirBoids();
        
        //Las 3 reglas basicas
        Vector3 r1,r2,r3;
        r1 = Cohesion(manada);
        r2 = Separacion(manada);
        r3 = Alineacion(manada);
        //print("r1: " + r1 + " r2: " + r2 + " r3: " + r3);

        velocidad += r1+r2+r3;

        transform.position += velocidad * Time.deltaTime;
    }
}
