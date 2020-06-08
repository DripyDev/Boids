using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidSettings settings;
    ///<summary>Direccion a la que nos dirigimos. direccion = velocidad/speed</summary>
    public Vector3 direccion;
    ///<summary>Velocidad de movimiento. speed = velocidad/direccion</summary>
    public float speed;
    ///<summary>Es la direccion(vector3) por la velocidad(float)</summary>
    public Vector3 velocidad;
    public int numeroBoids;
    public List<Boid> todosBoids;
    public List<Boid> manada;
    //TEMPORAL. BORRAR MAS TARDE
    public Vector3 r1,r2,r3;
    
    //Inicializamos el Boid
    public void Inicializar(BoidSettings set, Vector3 pos){
        settings = set;
        this.transform.position = pos;
        //Direccion inicial: adelante
        direccion = transform.forward;
        //Velocidad de movimiento inicial
        speed = (settings.maxSpeed+settings.minSpeed)/2;
        //Velocidad inicial
        velocidad = direccion*speed;
    }

    float Distancia(Vector3 destino, Vector3 origen){
        Vector3 offset = destino - origen;
        return Mathf.Sqrt(offset.x*offset.x + offset.y*offset.y +offset.z*offset.z);
    }

    //Devuelve una lista con los boids en el rango settings.radioPercepcion de nosotros
    List<Boid> PercibirBoids(){
        List<Boid> manadaAux = new List<Boid>();
        foreach (var b in todosBoids){
            //Vector3 offset = b.transform.position - this.transform.position;
            //float distancia = Mathf.Sqrt(offset.x*offset.x + offset.y*offset.y +offset.z*offset.z);
            float distancia = Distancia(b.transform.position, this.transform.position);
            //La segunda condicion es para no contarnos a nosotros mismos
            if(distancia < settings.radioPercepcion && distancia > 0){
                manadaAux.Add(b);
            }
        }
        return manadaAux;
    }

    ///<summary>R1. Devuelve el vector entre nosotros y el centro de la manada</summary>
    Vector3 Cohesion(List<Boid> manada){
        Vector3 pC = new Vector3(0,0,0);
        foreach (var b in manada) {
            pC += b.transform.position;
        }
        //NOTA: en la manada NO estamos nosotros nunca
        pC = (manada.Count) > 1? pC/manada.Count : pC;
        return (pC-this.transform.position)*settings.pesoCohesion;
    }
    Vector3 Separacion(List<Boid> manada){
        Vector3 c = new Vector3(0,0,0);
        foreach (var b in manada) {
            if(Distancia(b.transform.position, this.transform.position) < settings.dstSeparacion){
                c-= b.transform.position - this.transform.position;
            }
        }
        return c*settings.pesoSeparacion;
    }

    Vector3 Alineacion(List<Boid> manada){
        Vector3 pV = new Vector3(0,0,0);
        foreach (var b in manada) {
            pV += b.velocidad;
        }
        pV = (manada.Count) > 1? pV/manada.Count: pV;
        return (pV - this.velocidad)*settings.pesoAlineacion;
    }

    //NOTA: Como es un update independiente, no sirve inicializar valores porque entra en el update antes de que se llame a Inicializar.
    //por eso settings tiene que ser referencia desde el principio
    // Update is called once per frame
    void Update(){
        manada = PercibirBoids();
        
        //Las 3 reglas basicas
        //Vector3 r1,r2,r3;
        r1 = Cohesion(manada);//Bien
        r2 = Separacion(manada);//Bien
        r3 = Alineacion(manada);//?¿?¿
        
        Vector3 col = new Vector3(0,0,0);
        if(Colision()){
            col =  ObstacleRays () * settings.pesoEvitarChoque;
        }
        //print("r1: " + r1 + " r2: " + r2 + " r3: " + r3 + " col: " + col);
        if(col != new Vector3(0,0,0)){
            //print("COL DIFERENTE DE 0: " + col);
        }
        var aceleracion = (r1+r2+r3+col);
        velocidad += aceleracion;
        //velocidad += r1+r2+r3+col;
        
        //Actualizamos las nuevas direccion y velocidad
        var auxSpeed = velocidad.magnitude;
        Vector3 auxDir = velocidad/auxSpeed;
        //Nueva velocidad
        speed = Mathf.Clamp (auxSpeed, settings.minSpeed, settings.maxSpeed);
        velocidad = auxDir*speed;
        direccion = auxDir;

        transform.position += velocidad * Time.deltaTime;
    }

    //NOTA: VA MAL LO DE LOS OBSTACULOS Y RAYOS, REPASAR
    ///<summary>Comprobamos con un rayo si vamos a chocarnos o no con algun obstaculo</summary>
    public bool Colision(){
        RaycastHit hit;
        float speed = velocidad.magnitude;
        Vector3 dir = velocidad / speed;
        //if (Physics.SphereCast (transform.position, settings.radioPercepcion, dir, out hit, settings.dstSeparacion, settings.mascaraObstaculos)) {
        if (Physics.SphereCast (transform.position, 0.27f, dir, out hit, settings.dstSeparacion, settings.mascaraObstaculos)) {
            return true;
        } else { }
        return false;
    }
    public Ray rayo;
    ///<summary>Decidimos a donde nos movemos en funcion de los rayos casteados</summary>
    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions;
        //print("Long rayDirections" + rayDirections.Length);
        //Si un rayo NO golpea, nos movemos en esa direccion
        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = this.transform.TransformDirection (rayDirections[i]);
            //print("indice: " + i + " dir: " + dir);
            Ray ray = new Ray (transform.position, dir);
            //Devolvemos la direccion del primer rayo que no golpea un obstaculo
            //if (!Physics.SphereCast (ray, settings.radioPercepcion, settings.dstSeparacion, settings.mascaraObstaculos)) {
            if (!Physics.SphereCast (ray, 0.27f, settings.dstSeparacion, settings.mascaraObstaculos)) {
                rayo = ray;
                return dir;//*settings.pesoEvitarChoque;
            }
        }
        //Si todos los rayos han golpeado, seguimos adelante (Si todos golpean, no tenemos escapatoria asi que da igual que hacer)
        return direccion;
    }

    /*void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Vector3[] rayDirections = BoidHelper.directions;
        //Si un rayo NO golpea, nos movemos en esa direccion
        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = transform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (transform.position, dir);
            Gizmos.DrawRay(transform.position, dir*10);
            //Primer rayo que no golpea el obstaculo
            //Origen rayo, radio esfera, direccion
            if (!Physics.SphereCast (ray, settings.radioPercepcion/2, settings.radioSeparacion, settings.mascaraObstaculos)) {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, dir*10);
            }
        }
    }*/

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(rayo);

        var colorAux = Color.blue;colorAux.a = 0.1f;
        var colorAux2 = Color.yellow;colorAux2.a = 0.1f;

        Gizmos.color = colorAux;
        Gizmos.DrawSphere(transform.position, settings.radioPercepcion);

        Gizmos.color = colorAux2;
        Gizmos.DrawSphere(transform.position, settings.dstSeparacion);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, (direccion + transform.position)*1.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, r1+transform.position);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, r2+transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (r3+transform.position)*1.1f);
    }
}
