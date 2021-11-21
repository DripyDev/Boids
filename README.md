# Boids
Implementacion aparte de los conocidos Boids de Craig Reynolds. El objetivo es simular un comportamiento de manada con una serie de reglas básicas a 
partir de las cuales se obtiene un comportamiento en grupo complejo.

El funcionamiento es sencillo y el proyecto está dividido en tres secciones: **Reglas básicas de comportamiento**, **Evitado de obstáculos** y **Optimización**.


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
1. **Detección de choque:** Para esto vamos a utilizar el paquete de *Physics* de Unity, en concreto la función ![SphereCast](https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html), con la que podremos detectar un objeto con una máscara concreta en una dirección dada, que en nuestro caso sería la dirección del boid. Cada boid hará esta comprobación de manera constante y en caso de detectar una posible colisión, adoptará una nueva dirección.
2. **Evitar obstáculo:** Una vez detectado el obstáculo en rumbo de colisión tenemos que esquivarlo, debemos elegir una nueva dirección, ¿pero como? Para ello lo que vamos a hacer es generar N puntos de manera ordenada en una esfera alrededor del boid, mas detalles de como hacerlo en el repositorio repo ya que tiene relación con un problema relativamente complejo e interesante. Una vez generados estos puntos, comprobamos si chocaríamos tomando estas nuevas direcciones, aquella primera dirección donde no hay colisión, será la nueva.

Boid a punto de colisionar |  Nueva dirección (magenta)
:-------------------------:|:-------------------------:
![](https://...Dark.png)  |  ![](https://...Ocean.png)
