# Boids
Implementacion aparte de los conocidos Boids de Craig Reynolds. El objetivo es simular un comportamiento de manada con una serie de reglas básicas a 
partir de las cuales se obtiene un comportamiento en grupo complejo.

El funcionamiento es sencillo y el proyecto está dividido en tres secciones: **Reglas básicas de comportamiento**, **Evitado de obstáculos** y **Optimización**.


## Reglas básicas de comportamiento

Al igual que en la propuesta de Craig Reynolds original titulada [*Flocks, Herds, and Schools: A Distributed Behavioral Model*](https://team.inria.fr/imagine/files/2014/10/flocks-hers-and-schools.pdf),
vamos a tener tres reglas básicas de comportamiento, siendo estas: **Cohesión**, **Alineación** y **Separación**. Adicionalmente, habrá reglas extras como el evitado de obstáculos o la huida de los depredadores.
El funcionamiento de estas reglas es sencillo, un boid tiene un radio de percepción y todos los otros boids **sin incluirse a si mismo** dentro del mismo forman parte de su manada.

![Radio de percepcion y reglas básicas](https://imgur.com/a/7cWHYED)
