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
    public List<Boid> todosBoids;
    public List<Boid> manada;
    public List<Boid> boidsRegion;

    ///<summary>Indice de la region en la que esta el Boid actualmente</summary>
    public int region;
    public List<int> regionesAdyacentes;
    ///<summary>Indice de la region anterior donde estaba el Boid. Lo usamos para quitar el Boid de la lista de la region si hemos cambiado de region</summary>
    public int regionAnterior;
    //Temporal para gizmos
    public Vector3 r1,r2,r3;
    
    //Inicializamos el Boid
    public void Inicializar(BoidSettings set, Vector3 pos){
        settings = set;
        this.transform.position = pos;
        //Direccion inicial: adelante
        direccion = transform.forward;
        //Velocidad de movimiento inicial
        speed = Random.Range(settings.maxSpeed, settings.minSpeed);
        //Velocidad inicial
        velocidad = direccion*speed;
        
        region = RegionManager.EncontrarRegion(transform.position);
        regionAnterior = region;
    }

    float Distancia(Vector3 destino, Vector3 origen){
        Vector3 offset = destino - origen;
        return Mathf.Sqrt(offset.x*offset.x + offset.y*offset.y +offset.z*offset.z);
    }

    ///<summary>Devuelve una lista con los boids en el rango settings.radioPercepcion de nosotros. Recorre todos los boids existentes</summary>
    List<Boid> PercibirBoidsGlobal(){
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

    ///<summary>Devuelve una lista con los boids en el rango settings.radioPercepcion de nosotros. Recorre todos los boids de la Region a la que pertenecemos y las adyacentes.</summary>
    List<Boid> PercibirBoidsRegion(){
        List<Boid> manadaAux = new List<Boid>();
        //Percibimos los boids de las regiones nuestra y adyacentes
        foreach (var indice in regionesAdyacentes){
            foreach (var b in RegionManager.mapaRegiones[indice].boids){
                if(b!=this){
                    float distancia = Distancia(b.transform.position, this.transform.position);
                    //La segunda condicion es para no contarnos a nosotros mismos
                    if(distancia < settings.radioPercepcion && distancia > 0){
                        manadaAux.Add(b);
                    }
                    //TEMPORAL PARA COMPROBAR LOS BOIDS QUE DETECTA EN LAS REGIONES SUYA Y ADYACENTES
                    if(!boidsRegion.Contains(b))
                        boidsRegion.Add(b);
                }
            }
        }
        return manadaAux;

        /*List<Boid> manadaAux = new List<Boid>();
        foreach (var b in RegionManager.mapaRegiones[region].boids){
            float distancia = Distancia(b.transform.position, this.transform.position);
            //La segunda condicion es para no contarnos a nosotros mismos
            if(distancia < settings.radioPercepcion && distancia > 0){
                manadaAux.Add(b);
            }
        }
        return manadaAux;*/
    }

    ///<summary>R1. Devuelve el vector entre nosotros y el centro de la manada</summary>
    Vector3 Cohesion(List<Boid> manada){
        Vector3 pC = new Vector3(0,0,0);
        if(manada.Count == 0)
            return pC;
        foreach (var b in manada) {
            pC += b.transform.position;
        }
        //NOTA: en la manada NO estamos nosotros nunca
        pC = (manada.Count) > 1? pC/manada.Count : pC;
        return (pC-this.transform.position)*settings.pesoCohesion;
    }
    ///<summary>R2. Devuelve el vector sumatorio entre nosotros y los boids demasiado cercanos a nosotros. Lo develve en negativo</summary>
    Vector3 Separacion(List<Boid> manada){
        Vector3 c = new Vector3(0,0,0);
        if(manada.Count == 0)
            return c;
        foreach (var b in manada) {
            if(Distancia(b.transform.position, this.transform.position) < settings.dstSeparacion){
                c-= b.transform.position - this.transform.position;
            }
        }
        return c*settings.pesoSeparacion;
    }
    ///<summary>R3. Devuelve el vector entre nosotros la direccion general de la manada.
    ///NOTA: De momento iguala la velocidad, pero creo que deberia de igualar la direccion</summary>
    Vector3 Alineacion(List<Boid> manada){
        Vector3 pV = new Vector3(0,0,0);
        if(manada.Count == 0)
            return pV;
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
        //Cambiamos de region
        if( !RegionManager.DentroDeCubo(this.transform.position, RegionManager.mapaRegiones[region].posicion, RegionManager.mapaRegiones[region].dimensiones) ){
            //Hemos cambiado de region, nos eliminamos de la lista de boids de esa region
            RegionManager.EliminarBoidDeRegion(regionAnterior, this);

            //Aqui se deberia de encontrar la region a la que pertenece pero con indice de la region inicial o algo asi.
            //ahora mismo busca la region de entre las 64 existentes, no es muy eficiente
            region = RegionManager.EncontrarRegion(transform.position);
            regionesAdyacentes = RegionManager.RegionesAdyacentes(region);

            RegionManager.AñadirBoidDeRegion(region, this);
            regionAnterior = region;

            boidsRegion.Clear();
        }
        boidsRegion.Clear();
        //manada = PercibirBoidsGlobal();
        //De momento solo percibe boids en su misma region, deberia de percibir tambien en regiones adyacentes porque sino al seguir una manada cuando estos cambian
        //de region, dejamos de seguirlo y funciona mal
        manada = PercibirBoidsRegion();
        
        //Las 3 reglas basicas
        //Vector3 r1,r2,r3;
        r1 = Cohesion(manada);//Bien
        r2 = Separacion(manada);//Bien
        r3 = Alineacion(manada);//Bien
        
        Vector3 col = new Vector3(0,0,0);
        if(Colision()){
            col =  ObstacleRays () * settings.pesoEvitarChoque;
        }
        //print("r1: " + r1 + " r2: " + r2 + " r3: " + r3 + " col: " + col);
        var aceleracion = (r1+r2+r3+col);
        velocidad += aceleracion;
        
        //Actualizamos las nuevas direccion y velocidad
        speed = velocidad.magnitude;
        Vector3 auxDir = velocidad/speed;
        //Nueva velocidad
        speed = Mathf.Clamp (speed, settings.minSpeed, settings.maxSpeed);
        velocidad = auxDir*speed;
        direccion = auxDir;

        //IMPORTANTE: ESTO ACTUALIZA LOS EJES DEL BOID, SI NO LO ACTUALIZAMOS, EL RAYCASTING NO VA A FUNCIONAR BIEN
        transform.forward = direccion;
        transform.position += velocidad * Time.deltaTime;
    }

    ///<summary>Comprobamos con un rayo si vamos a chocarnos o no con algun obstaculo</summary>
    public bool Colision(){
        RaycastHit hit;
        float speed = velocidad.magnitude;
        Vector3 dir = velocidad / speed;
        //origen, radio esfera, direccion, informacion hit, maxima distancia casteo, mascara de obstaculos
        if (Physics.SphereCast (transform.position, 0.2f, direccion, out hit, settings.distanciaEvitarColision, settings.mascaraObstaculos))
            return true;
        else
            return false;
    }
    public Ray rayo;
    ///<summary>Decidimos a donde nos movemos en funcion de los rayos casteados</summary>
    Vector3 ObstacleRays () {
        //Vector con las direcciones de los rayos en funcion del numero aureo
        Vector3[] rayDirections = BoidHelper.directions;
        //Si un rayo NO golpea, nos movemos en esa direccion
        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = this.transform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (transform.position, dir);
            //Devolvemos la direccion del primer rayo que no golpea un obstaculo
            if (!Physics.SphereCast (ray, 0.2f, settings.distanciaEvitarColision, settings.mascaraObstaculos)) {
                rayo = ray;
                return dir;
            }
        }
        //Si todos los rayos han golpeado, seguimos adelante (Si todos golpean, no tenemos escapatoria asi que da igual que hacer)
        return direccion;
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(rayo);
        /*if(Colision()){
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
                if (!Physics.SphereCast (ray, 0.27f, settings.dstSeparacion, settings.mascaraObstaculos)) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, dir*10);
                }
            }
        }*/
    }

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
        Gizmos.DrawLine(transform.position, (direccion + transform.position)*1f);

        Gizmos.color = Color.red;
        Vector3 cero = new Vector3(0,0,0);
        if(r1!=cero)
            Gizmos.DrawLine(transform.position, r1+transform.position);

        Gizmos.color = Color.white;
        if(r2!=cero)
            Gizmos.DrawLine(transform.position, r2+transform.position);

        Gizmos.color = Color.green;
        if(r3!=cero)
            Gizmos.DrawLine(transform.position, (r3+transform.position)*1.1f);
        
        Color auxRojo = Color.red;Color auxVerde = Color.green;
        auxRojo.a = 0.1f;auxVerde.a = 0.1f;
        Gizmos.color = auxVerde;
        Gizmos.DrawCube(RegionManager.mapaRegiones[region].posicion, RegionManager.mapaRegiones[region].dimensiones);

        Gizmos.color = auxRojo;
        foreach (var i in regionesAdyacentes) {
            Gizmos.DrawCube(RegionManager.mapaRegiones[i].posicion, RegionManager.mapaRegiones[i].dimensiones);
        }
    }
}
//int regionesPorPlanta = numeroRegiones/4;//16
//int regionesPorFila = regionesPorPlanta/4;//4