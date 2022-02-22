# Boids
Implementacion independiente de los conocidos Boids de Craig Reynolds. El objetivo es simular un comportamiento de manada con una serie de reglas básicas a 
partir de las cuales se obtiene un comportamiento en grupo complejo.

El proyecto está dividido en tres secciones: **Reglas básicas de comportamiento**, **Evitado de obstáculos** y **Optimización**.


## Reglas básicas de comportamiento

Al igual que en la propuesta de Craig Reynolds original titulada [*Flocks, Herds, and Schools: A Distributed Behavioral Model*](https://team.inria.fr/imagine/files/2014/10/flocks-hers-and-schools.pdf),
vamos a tener tres reglas básicas de comportamiento, siendo estas: **Cohesión**, **Alineación** y **Separación**. Adicionalmente, habrá reglas extras como el evitado de obstáculos o la huida de los depredadores.
El funcionamiento de estas reglas es sencillo, un boid tiene un radio de percepción y todos los otros boids **sin incluirse a si mismo** dentro del mismo forman parte de su manada.

![Radio de percepcion y reglas básicas](https://user-images.githubusercontent.com/61519721/142773824-dfa1bb4e-5189-433f-9230-00e8e23b6d09.PNG)

1. **Cohesión:** El boid tratará de moverse al centro de la manada, ya que es el lugar mas seguro. Esto es representado en la imagen por la línea roja.
2. **Alineación:** El boid intentará seguir la misma dirección que la de la manada, que es la media de las direcciones de todos. La linea verde.
3. **Separación:** Se mantendrá una distancia mínima de seguridad entre boids con la finalidad de evitar colisiones. La línea blanca.

Cada una de estas reglas nos dará una dirección, la dirección final que acabará tomando el boid es la combinación de estas, la línea negra. A esto habría que incluir el evitado de obstáculos y la huida de los depredadores que es similar a la de separación. Cabe destacar que cada regla tendrá una importancia y peso diferentes que pueden ser modificados a gusto.

## Evistado de obstáculos

Para evitar obstáculos debemos enfrentarnos a dos cuestiones básicas: **¿Como detectamos un choque?** y **¿Como podemos elegir una nueva dirección para evitarlo?**
1. **Detección de choque:** Para esto vamos a utilizar el paquete de *Physics* de Unity, en concreto la función [SphereCast](https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html), con la que podremos detectar un objeto con una máscara concreta en una dirección dada, que en nuestro caso sería la dirección del boid. Cada boid hará esta comprobación de manera constante y en caso de detectar una posible colisión, adoptará una nueva dirección.
2. **Evitar obstáculo:** Una vez detectado el obstáculo en rumbo de colisión tenemos que esquivarlo, debemos elegir una nueva dirección, ¿pero como? Para ello lo que vamos a hacer es generar N puntos de manera ordenada en una esfera alrededor del boid, mas detalles de como hacerlo en el repositorio [EspiralesFibonacci](https://github.com/DripyDev/EspiralesFibonacci) ya que tiene relación con un problema relativamente complejo e interesante. Una vez generados estos puntos, comprobamos si chocaríamos tomando estas nuevas direcciones, aquella primera dirección donde no haya colisión, será la nueva.

Boid a punto de colisionar |  Nueva dirección (magenta)
:-------------------------:|:-------------------------:
![](https://user-images.githubusercontent.com/61519721/142774274-1f809923-dc5a-41b3-8756-1c2e9a007290.PNG)  |  ![](https://user-images.githubusercontent.com/61519721/142774273-da72f828-330d-4344-87ef-3eedf6649c92.PNG)

## Optimización

Para detectar a su manada, cada boid debe recorrer todos los que existen y comprobar cuales están a una distancia menor a su radio de percepción, lo que tiene un coste computacional de O(n<sup>2</sup>), lo cual puede ser mejorado. Para optimizar y mejorar el rendimiento general lo hacemos de dos formas diferentes: **Subdividiendo el mundo en regiones** y **Usando la GPU**.

1. **Subdivisión del mundo:** En lugar de buscar entre todos los boids, podemos: dividir el mundo en regiones, registrar los boids presentes en cada región y ver quienes forman parte de la manada de entre las regiones adyacentes (partiendo del hecho que el tamaño de cada región es menor al radio de percepción). En el peor de los casos, cuando todos los boids están en la misma región o en adyacentes, el coste computacional no varía, pero en el resto de los casos lo mejora. Los cálculos de cuales son las regiones adyacentes son sencillos. Cuanto mayor sea el número de regiones en las que subdividimos el mundo, mayor puede ser la mejora de rendimiento.

Vista frontal |  Vista lateral  |  Vista superior
:-------------------------:|:-------------------------:|:-------------------------:
![](https://user-images.githubusercontent.com/61519721/142774909-a61ac8cb-744b-481e-bf03-1723373f27ae.PNG)  |  ![](https://user-images.githubusercontent.com/61519721/142774906-919cd31a-ef2c-4234-b921-f0dd75e9bd02.PNG) |  ![](https://user-images.githubusercontent.com/61519721/142774908-f634bfac-8436-4ede-9a58-1600bc3ba4f6.PNG)


3. **GPU:** La GPU es capaz de realizar cálculos de manera paralela, al contrario que la CPU (aunque esto depende del número de nucleos que tenga). Esto signifíca que podríamos realizar los cálculos de pertenencia de manada y de las reglas de todos los boids de manera paralela, aumentando asi de manera significativa la velocidad general. Estos cálculos se realizan en el shadder [BoidCompute](https://github.com/DripyDev/Boids/blob/master/Assets/Scripts/BoidCompute.compute) y nos permiten aumentar el número de agentes en una cantidad significativa.
Cabe destacar que idealmente podrían unirse ambos métodos de optimización para aumentar todavía mas el rendimiento, pero ya que esto no erá una prioridad, no se hizo y quedó como tarea pendiente. La optimización a través de la GPU es increible y la diferencia de rendimiento es muy sustancial, por lo que recomiendo utilizar este método si se quieren simular muchos agentes.

![1500 Agentes simulados con GPU](https://user-images.githubusercontent.com/61519721/142774883-fae0b948-06bb-4628-9da7-f87fd9ea5a1c.PNG)


Este proyecto funciona de manera completamente independiente y no necesita ninguna clase de requisito especial para ser ejecutar, excepto Unity y una targeta gráfica en caso de utilizar los cálculo de la GPU. Sientete libre de utilizarlo a tu gusto sin restricción alguna y no dudes en preguntar cualquier duda.

Si se desea una explicación mas elaborada del funcionamiento, recomiendo leer la sección de Boids de mi [TFG](https://addi.ehu.es/handle/10810/48821).
