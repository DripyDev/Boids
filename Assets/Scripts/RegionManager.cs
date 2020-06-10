using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Encargado de dividir el mundo en subregiones (X.X.X) y administrar los Boids que se encuentran en cada region</summary>
public class RegionManager : MonoBehaviour
{
    //Suponemos que el suelo que nos dan tiene dimensiones AxA. Vamos a dividir en cubos a partir de sus valores X y Z de Scale
    public GameObject sueloMundo;
    ///<summary>Numero de regiones en las que queremos dividir el mapa. De momento lo dejamos en 64</summary>
    public int numeroRegiones = 64;
    public static List<Region> mapaRegiones = new List<Region>();
    [Header ("Dimension regiones")]
    //Dimensiones de las regiones
    public static float xR;
    public static float yR;
    public static float zR;
    [Header ("Dimension suelo")]
    //Dimensiones de suelo
    public float X;
    public float Y;
    public float Z;
    void Start(){
        X = sueloMundo.transform.localScale.x;
        //De momento suponemos que la altura es igual a X. NOTA: CAMBIAR EN EL FUTURO
        Y = X;
        Z = sueloMundo.transform.localScale.z;
        //Dimensiones de las regiones
        xR = Mathf.Pow(((X*Y*Z)/numeroRegiones), (1f/3f) ); yR = xR; zR = xR;
        InicializarMapaRegiones();
    }

    void InicializarMapaRegiones(){
        float raizCubo = Mathf.Pow(numeroRegiones, (1f/3f));
        Vector3 centroObjecto = sueloMundo.transform.position;
        //Centro de los cubos y sus dimensiones
        Vector3 pos, dimensiones;
        dimensiones = new Vector3(xR, yR, zR);
        //Centro de la primera region. Nota: la y es diferente porque es el centro del suelo que esta en y=0 del cubo que queremos subdividir
        pos = new Vector3(centroObjecto.x - (-X/2) - xR/2, yR/2, centroObjecto.z - (-Z/2) - zR/2);
        Vector3 posX = pos;
        Region regionAux;
        //Eje X
        for (int a = 0; a < raizCubo; a++){
            if (a!=0)
                posX.x -= xR;
            regionAux = new Region(posX, dimensiones);
            mapaRegiones.Add(regionAux);
            Vector3 posY = posX;
            //Eje Y
            for (int b = 0; b < raizCubo-1; b++) {
                posY.y += yR;
                regionAux = new Region(posY, dimensiones);
                mapaRegiones.Add(regionAux);
                Vector3 posZ = posY;
                //Eje Z
                for (int c = 0; c < raizCubo-1; c++) {
                    posZ.z -= zR;
                    //Eje Z con altura inicial
                    if (b==0){
                        Vector3 primeraLinea = posZ;
                        //Lo hacemos para tener las regiones con la altura del primer cubo
                        primeraLinea.y-=yR;
                        regionAux = new Region(primeraLinea, dimensiones);
                        mapaRegiones.Add(regionAux);
                    }
                    regionAux = new Region(posZ, dimensiones);
                    mapaRegiones.Add(regionAux);
                }
            }
        }
    }

    void OnDrawGizmosSelected() {
        float raizCubo = Mathf.Pow(numeroRegiones, (1f/3f));
        Color color = Color.red; color.a = 0.1f;
        Gizmos.color = color;
        foreach (var r in mapaRegiones) {
            Gizmos.DrawCube(r.posicion, r.dimensiones);
            color += new Color(1f/64f,1f/64f,1f/64f);
            color.a = 0.1f;
            Gizmos.color = color;
        }
        /*Vector3 centroObjecto = sueloMundo.transform.position;
        //Centro de los cubos y sus dimensiones
        Vector3 pos, dimensiones;
        dimensiones = new Vector3(xR, yR, zR);
        //Centro de la primera region. Nota: la y es diferente porque es el centro del suelo que esta en y=0 del cubo que queremos subdividir
        pos = new Vector3(centroObjecto.x - (-X/2) - xR/2, yR/2, centroObjecto.z - (-Z/2) - zR/2);
        Vector3 posX = pos;
        //Dibujamos las regiones
        for (int a = 0; a < raizCubo; a++){
            if (a!=0)
                posX.x -= xR;
            Gizmos.DrawCube(posX,dimensiones);
            Vector3 posY = posX;
            for (int b = 0; b < raizCubo-1; b++) {
                //if (b!=0)
                posY.y += yR;
                Gizmos.DrawCube(posY,dimensiones);
                Vector3 posZ = posY;
                for (int c = 0; c < raizCubo-1; c++) {
                    posZ.z -= zR;
                    //Para dibujar el primer fondo
                    if (b==0){
                        Vector3 primeraLinea = posZ; primeraLinea.y-=yR;
                        Gizmos.DrawCube(primeraLinea,dimensiones);
                    }
                    Gizmos.DrawCube(posZ,dimensiones);
                    //Debug.Log(prueba);
                    //prueba+=1;
                }
            }
        }*/
    }
    ///<summary>Dada una posicion, devuelve la region del mapa a la que pertenece</summary>
    public static Region EncontrarRegion(Vector3 pos){
        foreach (var r in mapaRegiones){
            float xMin = r.posicion.x - r.dimensiones.x/2;float xMax = r.posicion.x + r.dimensiones.x/2;
            float yMin = r.posicion.y - r.dimensiones.y/2;float yMax = r.posicion.y + r.dimensiones.y/2;
            float zMin = r.posicion.z - r.dimensiones.z/2;float zMax = r.posicion.z + r.dimensiones.z/2;

            bool Xin = pos.x >= xMin && pos.x <= xMax; bool Yin = pos.y >= yMin && pos.y <= yMax; bool Zin = pos.z >= zMin && pos.z <= zMax; 
            if(Xin && Yin && Zin){
                return r;
            }
        }
        //Si llega aqui es que no funciona o se ha escapado del mapa en cuyo caso nos da igual
        return new Region(new Vector3(0,0,0), new Vector3(0,0,0));
    }

    public struct Region{
        public Vector3 posicion;
        public Vector3 dimensiones;
        public Region(Vector3 pos, Vector3 dim){
            this.posicion = pos;
            this.dimensiones = dim;
        }
    }

}
