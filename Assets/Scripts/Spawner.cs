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
    
    //GPU
    ///<summary>Indica si vamos a usar la gpu o no</summary>
    public static bool activarGPU = false;
    public ComputeShader shader;
    private List<Boid> todosBoids = new List<Boid>();
    private List<Depredador> todosDepredadores = new List<Depredador>();

    //void Awake() {
    void Start() {
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
        //Inicializamos los depredadores
        Vector3 diferenciaD = new Vector3(3f,3f,3f);
        for (int i = 0; i < numeroDepredadores; i++) {
            Vector3 posRandom = transform.position + diferenciaD + Random.insideUnitSphere * radioSpawner;
            var dep = Instantiate(prefabDepredador);
            dep.Inicializar(settingsDepredador, posRandom);
            todosDepredadores.Add(dep);

            dep.transform.position = posRandom;
            dep.transform.forward = Random.insideUnitSphere;
        }
    }

    //Solo se llamara si activamos la gpu
    void Update() {
        if(activarGPU){
            if (todosBoids.Count > 0) {
                var datosBoid = new DatosBoid[todosBoids.Count];
                for(int i=0; i<todosBoids.Count; i++) {
                    datosBoid[i].position = todosBoids[i].transform.position;
                    datosBoid[i].direccion = todosBoids[i].direccion;
                    datosBoid[i].velocidad = todosBoids[i].velocidad;
                }
                //Especificamos el numero de buffers y su tamaño para poder usarlo en el shader
                var boidBuffer = new ComputeBuffer(todosBoids.Count, DatosBoid.Size);
                boidBuffer.SetData(datosBoid);

                shader.SetBuffer(0, "boids", boidBuffer);
                shader.SetInt("numeroBoids", todosBoids.Count);
                shader.SetFloat("radioVision", settings.radioPercepcion);
                shader.SetFloat("distanciaSeparacion", settings.dstSeparacion);

                int threadGroups = Mathf.CeilToInt (todosBoids.Count / (float) 1024);
                //Numero de threads que va a usar el shader
                shader.Dispatch (0, threadGroups, 1, 1);
                //Procesamos los datos en la gpu que nos va a calcular: Numero de boids cercanos,
                //direccion de manada, centro manada y si deberiamos de separarnos de alguien
                boidBuffer.GetData (datosBoid);

                for (int i = 0; i < todosBoids.Count; i++) {
                    //Cohesion
                    todosBoids[i].r1 = datosBoid[i].numeroBoidsManada <= 0? new Vector3(0,0,0) : datosBoid[i].centroManada / datosBoid[i].numeroBoidsManada;
                    //Separacion
                    todosBoids[i].r2 = datosBoid[i].separacion;
                    //Alineacion
                    todosBoids[i].r3 = datosBoid[i].numeroBoidsManada <= 0? new Vector3(0,0,0) :  datosBoid[i].direccionManada / datosBoid[i].numeroBoidsManada;

                    //Boids percibidos. La manada actual
                    todosBoids[i].numeroManada = datosBoid[i].numeroBoidsManada;
                    //Movemos el boid y actualizamos sus datos
                    todosBoids[i].ActualizarBoid();
                }
                boidBuffer.Release ();
            }
        }
    }

    //NOTA: Esta estructura es la que va a adoptar la estructura boid del shader. Los parametros tienen que estar en el mismo orden
    //Prueba con parametros basicos, sin regiones
    public struct DatosBoid {
        public Vector3 position;
        public Vector3 direccion;
        public Vector3 velocidad;

        //Reglas basicas
        public Vector3 centroManada;
        public Vector3 separacion;
        public Vector3 direccionManada;
        
        //public Depredador dep;
        public int numeroBoidsManada;
        //public int region;
        //public int[] regionesAdyacentes;
        //NOTA: El tamaño va en funcion del numero de floats y ints de la estructura. 1Vector3 = sizeof (float)*3, por ejemplo
        public static int Size {
            get {
                //6 vector3 son 3x6 floats y 1 int
                return sizeof (float) * 3 * 6 + sizeof (int);
            }
        }
    }
}
