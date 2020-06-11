using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Encargado de dividir el mundo en subregiones (X.X.X) y administrar los Boids que se encuentran en cada region</summary>
public class RegionManager : MonoBehaviour
{
    //Suponemos que el suelo que nos dan tiene dimensiones AxA. Vamos a dividir en cubos a partir de sus valores X y Z de Scale
    public GameObject sueloMundo;
    //TIENE QUE SER UN NUMERO CON RAIZ CUBICA ENTERA
    //REPASAR, NO TIRA BIEN, POR EJEMPLO, CON 216
    ///<summary>Numero de regiones en las que queremos dividir el mapa. De momento lo dejamos en 64</summary>
    public static int numeroRegiones = 64;
    public static List<Region> mapaRegiones = new List<Region>();
    [Header ("Dimension regiones")]
    //Dimensiones de las regiones
    public static int xR;
    public static int yR;
    public static int zR;
    
    [Header ("Dimension suelo")]
    //Dimensiones de suelo
    public int X;
    public int Y;
    public int Z;
    
    //void Start(){
    void Awake(){
        X = (int) sueloMundo.transform.localScale.x;
        //De momento suponemos que la altura es igual a X. NOTA: CAMBIAR EN EL FUTURO
        Y = X;
        Z = (int) sueloMundo.transform.localScale.z;
        //Dimensiones de las regiones
        xR = (int) Mathf.Pow(((X*Y*Z)/numeroRegiones), (1f/3f) ); yR = xR; zR = xR;
        InicializarMapaRegiones();
    }

    //NOTA: CREAR LAS REGIONES DE MANERA QUE LOS INDICES ESTEN RELATIVAMENTE ORDENADOS Y TENGA SENTIDO, HARA QUE LOS ADYACENTES SE PUEDAN ENCONTRAR DIRECTAMENTE
    //Y NO HAGA FALTA RECORRER NADA PARA ENCONTRAR ESAS REGIONES ADYACENTES
    void InicializarMapaRegiones(){
        int raizCubo = (int) Mathf.Pow(numeroRegiones, (1f/3f));
        Vector3 centroObjecto = sueloMundo.transform.position;
        //Centro de los cubos y sus dimensiones
        Vector3 pos, dimensiones;
        dimensiones = new Vector3(xR, yR, zR);
        //Centro de la primera region. Nota: la y es diferente porque es el centro del suelo que esta en y=0 del cubo que queremos subdividir
        pos = new Vector3(centroObjecto.x - (-X/2) - xR/2, yR/2, centroObjecto.z - (-Z/2) - zR/2);
        Vector3 posX, posY, posZ;
        posY = pos;
        Region regionAux;
        //Eje Y
        for (int a = 0; a < raizCubo; a++){
            posX = posY;
            posY.y += yR;
            //Eje X
            for (int b = 0; b < raizCubo; b++) {
                posZ = posX;
                posX.x -= xR;
                //Eje Z
                for (int c = 0; c < raizCubo; c++) {
                    regionAux = new Region(posZ, dimensiones);
                    mapaRegiones.Add(regionAux);
                    posZ.z -= zR;
                }
            }
        }
        /*
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
        }*/
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
    }

    ///<summary>Dado el indice de una region del mapa y el boid, lo elimina de la lista de boids de esa region.</summary>
    public static void EliminarBoidDeRegion(int indiceRegion, Boid boid){
        mapaRegiones[indiceRegion].boids.Remove(boid);
    }

    ///<summary>Dado el indice de una region del mapa y el boid, lo añade en la lista de boids de esa region.</summary>
    public static void AñadirBoidDeRegion(int indiceRegion, Boid boid){
        mapaRegiones[indiceRegion].boids.Add(boid);
    }

    ///<summary>Dada una posicion y el centro y dimensiones de un cubo. Devuelve si la posicion esta dentro de un cubo con las dimensiones dadas</summary>
    public static bool DentroDeCubo(Vector3 pos, Vector3 centroCubo,  Vector3 dimensionesCubo){
        float xMin = centroCubo.x - dimensionesCubo.x/2;float xMax = centroCubo.x + dimensionesCubo.x/2;
        float yMin = centroCubo.y - dimensionesCubo.y/2;float yMax = centroCubo.y + dimensionesCubo.y/2;
        float zMin = centroCubo.z - dimensionesCubo.z/2;float zMax = centroCubo.z + dimensionesCubo.z/2;

        bool Xin = pos.x >= xMin && pos.x <= xMax; bool Yin = pos.y >= yMin && pos.y <= yMax; bool Zin = pos.z >= zMin && pos.z <= zMax;
        return Xin && Yin && Zin; 
    }

    ///<summary>Dada una posicion, devuelve la region del mapa a la que pertenece</summary>
    public static int EncontrarRegion(Vector3 pos){
        int indice = 0;
        foreach (var r in mapaRegiones){
            if(DentroDeCubo(pos, r.posicion, r.dimensiones)){
                //return r;
                return indice;
            }
            indice+=1;
        }
        //Si llega aqui es que no funciona o se ha escapado del mapa en cuyo caso nos da igual
        //return new Region(new Vector3(0,0,0), new Vector3(0,0,0));
        //ERROR
        return -1;
    }

    ///<summary>Dada una posicion, devuelve el indice de la region cuya posicion sea la misma</summary>
    public static int EncontrarRegionPorPosicion(Vector3 pos){
        int indice = 0;
        foreach (var r in mapaRegiones) {
            if(r.posicion == pos)
                return indice;
            indice +=1;
        }
        //Error
        return -1;
    }

    //Como solo tomamos como adyacentes AA, DA, ID. Cuando las manadas recorren regiones en diagonal, pierde de vista a la manada. Eso no deberia de pasar
    //NOTA: CREAR EL MAPA DE REGIONES DE MANERA ORDENADA Y ASI SE PODRA SACAR LOS ADYACENTES POR INDICES SIN TENER QUE RECORRER NADA
    ///<summary>Dado el indice de una region, devuelve los indices de las regiones adyacentes y la original.
    ///NOTA: DE MOMENTO DEVUELVE SOLO LAS DE ARRIBA, ABAJO, DELANTE ATRAS Y LATERALES. EN PRINCIPIO DEBERIA DE DEVOLVER 3X3X3=27 REGIONES</summary>
    public static List<int> RegionesAdyacentes2(int indiceRegion){
        List<int> indices = new List<int>();

        int arriba = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion + new Vector3(0,yR,0));
        int abajo = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion - new Vector3(0,yR,0)); 

        int izquierda = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion + new Vector3(xR, 0, 0)); 
        int derecha = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion - new Vector3(xR, 0, 0)); 

        int delante = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion + new Vector3(0, 0, zR)); 
        int atras = EncontrarRegionPorPosicion(mapaRegiones[indiceRegion].posicion - new Vector3(0, 0, zR));

        int[] aux = {indiceRegion, arriba, abajo, izquierda, derecha, delante, atras};
        for (int i = 0; i < 7; i++){
            if(aux[i] != -1)
                indices.Add(aux[i]);
        }
        return indices;
    }
    ///<summary>Regiones adyacentes pero con las regiones creadas en un orden concreto. Entonces los adyacentes se pueden calcular con operaciones simples</summary>
    public static List<int> RegionesAdyacentes(int indiceRegion){
        List<int> indices = new List<int>();
        int regionesPorPlanta = (int) Mathf.Pow(numeroRegiones, (2f/3f) );//64:16 / 216:36
        int regionesPorFila = (int) Mathf.Pow(numeroRegiones, (1f/3f) );//64:4 / 216:6
        //print("Planta: " + regionesPorPlanta + " fila: " + regionesPorFila);

        //int arriba = indiceRegion+regionesPorPlanta<numeroRegiones? indiceRegion+regionesPorPlanta : -1;
        //int abajo = indiceRegion-regionesPorPlanta>=0? indiceRegion-regionesPorPlanta : -1;
        int arriba = indiceRegion+16<numeroRegiones? indiceRegion+16 : -1;
        int abajo = indiceRegion-16>=0? indiceRegion-16 : -1;

        //indice - planta
        int izquierda = ( (indiceRegion - (((int)(indiceRegion/16))*16) ) < 4 )? -1 : indiceRegion-4;
        int derecha = ( (indiceRegion - (((int)(indiceRegion/16))*16) ) > 11 )? -1 : indiceRegion+4;
        //int izquierda = ( (indiceRegion - (((int)(indiceRegion/regionesPorPlanta))*regionesPorPlanta) ) < regionesPorFila )? -1 : indiceRegion-regionesPorFila;
        //int derecha = ( (indiceRegion - (((int)(indiceRegion/regionesPorPlanta))*regionesPorPlanta) ) > (regionesPorPlanta-regionesPorFila-1) )? -1 : indiceRegion+regionesPorFila;

        int delante = ((indiceRegion)%4==0)? -1 : indiceRegion-1;
        int atras = ((indiceRegion-3)%4==0)? -1 : indiceRegion+1;
        //int delante = ( ((indiceRegion)%regionesPorFila) ==0)? -1 : indiceRegion-1;
        //int atras = ( ((indiceRegion- (regionesPorFila+1) )%regionesPorFila) ==0)? -1 : indiceRegion+1;

        int[] aux = {indiceRegion, arriba, abajo, izquierda, derecha, delante, atras};
        for (int i = 0; i < 7; i++){
            if(aux[i] >= 0)
                indices.Add(aux[i]);
        }
        return indices;
    }

    public struct Region{
        public Vector3 posicion;
        public Vector3 dimensiones;
        public List<Boid> boids;
        public Region(Vector3 pos, Vector3 dim){
            this.posicion = pos;
            this.dimensiones = dim;
            this.boids = new List<Boid>();
        }
    }

}
