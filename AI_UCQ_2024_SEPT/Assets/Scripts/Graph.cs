using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.UI.Image;



public enum NodePriority
{
    BestFirst,
    Djikstra,
    AStar
}

public class Node
{
    public Node(string inName, Vector3 inCoords)
    {
        Name = inName;
        Parent = null;
        Coords = inCoords;
    }

    public string Name = "";  // es string por pura claridad, idealmente se usan ints para diferenciar objetos.

    public Node Parent;  // referencia al nodo padre de este nodo en el árbol que se genera durante un Pathfinding.

    public float DistancePriority = Single.PositiveInfinity;

    public float Weight = 1;  // este uno porque queremos que cada paso al menos nos cueste 1, para que el algoritmo funcione.

    public float AccumulatedWeight = Single.PositiveInfinity;

    public float AccumulatedPlusDistance = Single.PositiveInfinity;

    public Vector3 Coords;

    public float GetPriority(NodePriority inPriority)
    {
        switch (inPriority)
        {
            case NodePriority.BestFirst:
                return DistancePriority;
            case NodePriority.Djikstra:
                return AccumulatedWeight;
            case NodePriority.AStar:
                return AccumulatedPlusDistance;
            default:
                return 0.0f;
        }
    }

    // Nos da la prioridad (heurística) respecto a la pura distancia entre este nodo y otro nodo
    public float GetDistance(Node otherNode)
    {
        // Ahorita dijimos que va a hacer un teorema de Pitágoras entre las coordenadas de ambos nodos.
        DistancePriority = Vector3.Distance(Coords, otherNode.Coords);
        return DistancePriority;
    }

}

public class Edge
{
    public Edge(string inName, Node inA, Node inB, float inWeight = 1.0f)
    {
        Name = inName;
        A = inA;
        B = inB;
        Weight = inWeight;
    }

    public string Name = ""; // es string por pura claridad, las aristas normalmente no necesitan un nombre.
    public Node A;
    public Node B;
    public float Weight = 1.0f;

    // EdgeA == EdgeB
    // Si son punteros/referencias pues nomás comparan la dirección de memoria y ya.
    // PERO SI NO, ustedes tendrían que comparar una o más cosas.
    // Por ejemplo podríamos checar EdgeA.A == EdgeB.A && EdgeA.B == EdgeB.B && EdgeA.Weight == EdgeB.Weight

    // Un hash te da un solo número que representa a ese objeto.

    // Vector3 A == Vector3 B?
    // A.x == B.x && A.y == B.y && A.z == B.z

}


public class Graph : MonoBehaviour
{

    // Podríamos guardarlos en un array.
    // Podríamos guardarlos en un List, Set
    // Dictionary, Queue, Stack, DynamicArray, Heap

    // Array:
    // Ventajas: super rápido de acceder de manera secuencial. Te da el espacio de memoria completo.
    // int [10]Array
    // Desventajas: Te da el espacio de memoria completo (lo vayas a usar o no, lo que puede llevar a desperdicios).
    // desventajas: su tamaño (capacidad de almacenamiento) es totalmente estático.
    // desventajas: poner y quitar elementos que hagan que cambie el tamaño del array es MUY lento.


    // ¿Qué es un "Set" en estructuras de datos / programación?
    // Un set es una estructura de datos que no permite repetidos
    // específicamente en nuestros grafos, no vamos a querer ni nodos ni aristas repetidas.


    protected HashSet<Node> NodeSet = new HashSet<Node>();
    protected HashSet<Edge> EdgeSet = new HashSet<Edge>();


    [SerializeField]
    private Transform[] NodePositions = new Transform[8];


    // Start is called before the first frame update
    void Start()
    {
        // Vamos a llenar nuestros sets de nodos y aristas.
        // Comenzamos creando todos los nodos, porque las aristas necesitan que ya existan los nodos.
        Node NodeA = new Node("A", NodePositions[0].position);
        Node NodeB = new Node("B", NodePositions[1].position);
        Node NodeC = new Node("C", NodePositions[2].position);
        Node NodeD = new Node("D", NodePositions[3].position);
        Node NodeE = new Node("E", NodePositions[4].position);
        Node NodeF = new Node("F", NodePositions[5].position);
        Node NodeG = new Node("G", NodePositions[6].position);
        Node NodeH = new Node("H", NodePositions[7].position);

        NodeSet.Add(NodeA);
        NodeSet.Add(NodeB);
        NodeSet.Add(NodeC);
        NodeSet.Add(NodeD);
        NodeSet.Add(NodeE);
        NodeSet.Add(NodeF);
        NodeSet.Add(NodeG);
        NodeSet.Add(NodeH);

        // Ahora queremos declarar las aristas.
        Edge EdgeAB = new Edge("AB", NodeA, NodeB);
        Edge EdgeAE = new Edge("AE", NodeA, NodeE);
        Edge EdgeBC = new Edge("BC", NodeB, NodeC);
        Edge EdgeBD = new Edge("BD", NodeB, NodeD);
        Edge EdgeEF = new Edge("EF", NodeE, NodeF);
        Edge EdgeEG = new Edge("EG", NodeE, NodeG);
        Edge EdgeEH = new Edge("EH", NodeE, NodeH);

        EdgeSet.Add(EdgeAB);
        EdgeSet.Add(EdgeAE);
        EdgeSet.Add(EdgeBC);
        EdgeSet.Add(EdgeBD);
        EdgeSet.Add(EdgeEF);
        EdgeSet.Add(EdgeEG);
        EdgeSet.Add(EdgeEH);

        List<Node> PathToGoalDFS = new List<Node>();

        if (Djikstra(NodeA, NodeH, out PathToGoalDFS))
        {
            Debug.Log("Djikstra: Sí hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        }
        else
        {
            Debug.Log("Djikstra: No hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        }

        foreach (Node node in NodeSet)
        {
            if (node.Parent != null)
            {
                Debug.Log(" Nodo: " + node.Name + " su total weight = " + node.AccumulatedWeight + ", hijo de nodo: " + node.Parent.Name);
            }
            else
            {
                Debug.Log(" Nodo: " + node.Name + " su total weight = " + node.AccumulatedWeight + ", y es el origen.");
            }
        }

        // OJO: no olviden poner el out antes de los parámetros que son de salida.
        //if (BestFirstSearch(NodeA, NodeH, out PathToGoalDFS))
        //{
        //    Debug.Log("Best First Search: Sí hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        //}
        //else
        //{
        //    Debug.Log("Best First Search: No hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        //}

        ResetNodes(NodeSet);

        //if (RecursiveDFS(NodeA, NodeH))
        //{
        //    Debug.Log("Sí hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        //}
        //else
        //{
        //    Debug.Log("No hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        //}


        // FuncionRecursiva(0);  // comentada para que no truene ahorita.
    }

    // No recibe nada porque ya los tenemos guardados en el NodeSet
    void ResetNodes(HashSet<Node> inNodeSet)
    {
        foreach (Node node in inNodeSet)
        {
            node.Parent = null;
        }
    }

    // Cosas costosas de Djikstra:
    // 1) Si tenemos que cambiarle el valor a un nodo que ya está en la lista bierta, tenemos que buscar en nuestra lista ligada, quitar el nodo
    // y luego volverlo a insertar.
    // digamos que tenemos 1 millón de elementos
    // Penúltimo=999999, último 1000000
    // cambiando el último de 1,000,000 a 999,998.5
    // en el peor de los casos nos costó 2*n

    bool AStar(Node Origin, Node Goal, out List<Node> PathToGoal)
    {
        PathToGoal = new List<Node>(); // Lo inicializamos en 0 por defecto por si no encontramos ningún camino.

        // Nodos cerrados es: ya no se tocan, solo sirven para checar si un nodo ya está cerrado o no.
        HashSet<Node> closedNodesSet = new HashSet<Node>();

        // Nodos Abiertos es: si un nodo está abierto, es porque todavía no está cerrado, entonces hay que cerrarlo, y para hacerlo
        // hay que hacerle todo lo necesario, que en este caso es meter a todos sus vecinos posibles a la lista de nodos abiertos.
        PriorityQueue openNodesPriorityQueue = new PriorityQueue();

        // Es importante que el origin tenga 
        // Origin.Weight = 0; // por ahora podemos omitir este pasito. Pero en cada caso puede cambiar.
        Origin.AccumulatedWeight = Origin.Weight;
        openNodesPriorityQueue.Insert(Origin, Origin.AccumulatedWeight, NodePriority.Djikstra);

        Node CurrentNode = null;

        // El ciclo sigue hasta que A) lleguemos a la meta o B) no haya más nodos abiertos.
        while (openNodesPriorityQueue.Count() > 0)
        {
            CurrentNode = openNodesPriorityQueue.Dequeue();
            closedNodesSet.Add(CurrentNode); // lo metemos a la lista cerrada en cuanto lo sacamos de la lista abierta.

            // Ya sabemos que en cuanto lleguemos a la meta nos podemos salir de TODA la función, no solo del while, 
            // entonces podemos hacer eso.
            if (CurrentNode == Goal)
            {
                return true;
            }

            // Si no se ha cumplido ninguna de esas condiciones, 
            List<Node> Neighbors = GetNeighbors(CurrentNode);

            // Ahora revisamos cuáles ya están cerrados, abiertos, o desconocidos, y hacer lo que corresponda con cada uno de ellos.
            foreach (Node neighbor in Neighbors)
            {
                if (closedNodesSet.Contains(neighbor))
                    continue; // si ya está en lo cerrados, no hay nada que hacer con él. Nos pasamos al siguiente vecino.

                // Ahora checamos si el vecino ya tiene padre; si sí, checamos si el peso total de neighbor 
                // es mejor que si currentNode fuera su padre (es decir, con el currentNode.AccumulatedWeight).
                if (neighbor.Parent != null)
                {
                    if (neighbor.AccumulatedWeight > neighbor.Weight + CurrentNode.AccumulatedWeight)
                    {
                        // Si sí es un mejor camino, entonces le actualizamos al neighbor su parent y su total weight.
                        // Primero lo sacamos de la lista.
                        openNodesPriorityQueue.Remove(neighbor);

                        // Y ahora hay que cambiarle su lugar en la lista Abierta.
                        // Este proceso se realiza abajo, como si no hubiera tenido el parent.
                    }
                    else
                    {
                        continue;
                    }
                }

                neighbor.Parent = CurrentNode;
                neighbor.AccumulatedWeight = neighbor.Weight + CurrentNode.AccumulatedWeight;

                // Le decimos: este es tu valor de heurística (distance)
                neighbor.GetDistance(Goal);  // esta función ya le setea su Distance internamente.

                // Y este es tu valor "f", que es la heurística más el peso acumulado.
                neighbor.AccumulatedPlusDistance = neighbor.AccumulatedWeight + neighbor.DistancePriority;

                // Finalmente, lo metemos en la fila de prioridad (lista abierta) usando el valor F como prioridad.

                // A los que no estén en ninguna de las tres condiciones anteriores, a esos sí los podemos meter a la lista 
                // calculando su distancia (heurística) respecto al nodo Goal, y los hacemos hijos de este CurrentNode.
                openNodesPriorityQueue.Insert(neighbor, neighbor.AccumulatedPlusDistance, NodePriority.AStar);
            }
        }

        return false;
    }


    bool Djikstra(Node Origin, Node Goal, out List<Node> PathToGoal)
    {
        PathToGoal = new List<Node>(); // Lo inicializamos en 0 por defecto por si no encontramos ningún camino.

        // Nodos cerrados es: ya no se tocan, solo sirven para checar si un nodo ya está cerrado o no.
        HashSet<Node> closedNodesSet = new HashSet<Node>();

        // Nodos Abiertos es: si un nodo está abierto, es porque todavía no está cerrado, entonces hay que cerrarlo, y para hacerlo
        // hay que hacerle todo lo necesario, que en este caso es meter a todos sus vecinos posibles a la lista de nodos abiertos.
        PriorityQueue openNodesPriorityQueue = new PriorityQueue();

        // Es importante que el origin tenga 
        // Origin.Weight = 0; // por ahora podemos omitir este pasito. Pero en cada caso puede cambiar.
        Origin.AccumulatedWeight = Origin.Weight;
        openNodesPriorityQueue.Insert(Origin, Origin.AccumulatedWeight, NodePriority.Djikstra);

        Node CurrentNode = null;

        // El ciclo sigue hasta que A) lleguemos a la meta o B) no haya más nodos abiertos.
        while (openNodesPriorityQueue.Count() > 0)
        {
            CurrentNode = openNodesPriorityQueue.Dequeue();
            closedNodesSet.Add(CurrentNode); // lo metemos a la lista cerrada en cuanto lo sacamos de la lista abierta.

            // Ya sabemos que en cuanto lleguemos a la meta nos podemos salir de TODA la función, no solo del while, 
            // entonces podemos hacer eso.
            if (CurrentNode == Goal)
            {
                return true;
            }

            // Si no se ha cumplido ninguna de esas condiciones, 
            List<Node> Neighbors = GetNeighbors(CurrentNode);

            // Ahora revisamos cuáles ya están cerrados, abiertos, o desconocidos, y hacer lo que corresponda con cada uno de ellos.
            foreach (Node neighbor in Neighbors)
            {
                if (closedNodesSet.Contains(neighbor))
                    continue; // si ya está en lo cerrados, no hay nada que hacer con él. Nos pasamos al siguiente vecino.

                // Ahora checamos si el vecino ya tiene padre; si sí, checamos si el peso total de neighbor 
                // es mejor que si currentNode fuera su padre (es decir, con el currentNode.AccumulatedWeight).
                if (neighbor.Parent != null)
                {
                    if (neighbor.AccumulatedWeight > neighbor.Weight + CurrentNode.AccumulatedWeight)
                    {
                        // Si sí es un mejor camino, entonces le actualizamos al neighbor su parent y su total weight.
                        // Primero lo sacamos de la lista.
                        openNodesPriorityQueue.Remove(neighbor);

                        // Y ahora hay que cambiarle su lugar en la lista Abierta.
                        // Este proceso se realiza abajo, como si no hubiera tenido el parent.
                    }
                    else
                    {
                        continue;
                    }
                }

                // Si el nodo ya tiene Parent quiere decir que está en la lista abierta.
                //if (openNodesPriorityQueue.Contains(neighbor))
                //    continue;

                neighbor.Parent = CurrentNode;
                neighbor.AccumulatedWeight = neighbor.Weight + CurrentNode.AccumulatedWeight;

                // A los que no estén en ninguna de las tres condiciones anteriores, a esos sí los podemos meter a la lista 
                // calculando su distancia (heurística) respecto al nodo Goal, y los hacemos hijos de este CurrentNode.
                openNodesPriorityQueue.Insert(neighbor, neighbor.AccumulatedWeight, NodePriority.Djikstra);
            }
        }

        return false;
    }


    bool BestFirstSearch(Node Origin, Node Goal, out List<Node> PathToGoal)
    {
        PathToGoal = new List<Node>(); // Lo inicializamos en 0 por defecto por si no encontramos ningún camino.

        // Nodos cerrados es: ya no se tocan, solo sirven para checar si un nodo ya está cerrado o no.
        HashSet<Node> closedNodesSet = new HashSet<Node>();

        // Nodos Abiertos es: si un nodo está abierto, es porque todavía no está cerrado, entonces hay que cerrarlo, y para hacerlo
        // hay que hacerle todo lo necesario, que en este caso es meter a todos sus vecinos posibles a la lista de nodos abiertos.
        PriorityQueue openNodesPriorityQueue = new PriorityQueue();

        openNodesPriorityQueue.Insert(Origin, Origin.GetDistance(Goal), NodePriority.BestFirst);

        Node CurrentNode = null;


        // El ciclo sigue hasta que A) lleguemos a la meta o B) no haya más nodos abiertos.
        while (openNodesPriorityQueue.Count() > 0)
        {
            CurrentNode = openNodesPriorityQueue.Dequeue();
            closedNodesSet.Add(CurrentNode); // lo metemos a la lista cerrada en cuanto lo sacamos de la lista abierta.

            // Ya sabemos que en cuanto lleguemos a la meta nos podemos salir de TODA la función, no solo del while, 
            // entonces podemos hacer eso.
            if (CurrentNode == Goal)
            {
                return true;
            }

            // Si no se ha cumplido ninguna de esas condiciones, 
            List<Node> Neighbors = GetNeighbors(CurrentNode);

            // Ahora revisamos cuáles ya están cerrados, abiertos, o desconocidos, y hacer lo que corresponda con cada uno de ellos.
            foreach (Node neighbor in Neighbors)
            {
                if (closedNodesSet.Contains(neighbor))
                    continue; // si ya está en lo cerrados, no hay nada que hacer con él. Nos pasamos al siguiente vecino.

                // Ahora checamos si el vecino ya tiene padre; si sí, nosotros no debemos de hacer nada con él. (en este algoritmo)
                if (neighbor.Parent != null)
                    continue;

                if (openNodesPriorityQueue.Contains(neighbor))
                    continue;

                // A los que no estén en ninguna de las tres condiciones anteriores, a esos sí los podemos meter a la lista 
                // calculando su distancia (heurística) respecto al nodo Goal, y los hacemos hijos de este CurrentNode.
                openNodesPriorityQueue.Insert(neighbor, neighbor.GetDistance(Goal), NodePriority.BestFirst);
                neighbor.Parent = CurrentNode;
            }


        }



        return false;
    }

    // Nuestro DFS iterativo debe dar exactamente los mismos resultados que el recursivo.
    // Nos dice si hay un camino desde un Nodo Origen hasta un nodo Destino (de un grafo)
    // y si sí hay un camino, nos dice cuál fue. Esto del camino tiene un truco interesante: El Backtracking.
    // El camino nos lo pasará a través del parámetro de salida: PathToGoal (nótese el término "out" que lo marca como de salida).
    bool DFS(Node Origin, Node Goal, out List<Node> PathToGoal)
    {
        PathToGoal = new List<Node>(); // Lo inicializamos en 0 por defecto por si no encontramos ningún camino.

        // Para saber cuántos nodos hay todavía por visitar,
        // necesitamos llevar registro de cuáles nodos ya hemos visitado.
        // Necesitamos dos contenedores de nodos, uno para los ya visitados y otro para los conocidos.

        // Un Set es un contenedor perfecto para los visitados, 
        // ya que solo necesitamos saber si ya está dentro de visitados o no.
        HashSet<Node> VisitedNodes = new HashSet<Node>();

        // Podemos usar la estructura de datos Pila (stack) para reemplazar la Pila de llamadas que usaba la versión recursiva
        // del algoritmo para mantener su orden.
        // ¿Cuándo se meten nodos en la pila? En cuanto tu nodo actual lo puede alcanzar (tiene una arista con él), Y no 
        // tiene ya un padre asignado (el que no tenga parent quiere decir que ningún otro nodo ha llegado ya a este nuevo nodo).
        // Los nodos que todavía estén en esta pila son los nodos que todavía hay por visitar.
        Stack<Node> KnownStack = new Stack<Node>();

        // Con esto evitamos que algún otro nodo trate de meter al origin en los nodos por visitar.
        Origin.Parent = Origin;

        // Para que no se termine el While inmediatamente (porque la KnownStack está vacía)
        // nosotros tenemos que meter al menos un nodo a dicha Stack. Metemos el único no que tenemos certeza de que podemos alcanzar ahorita.
        KnownStack.Push(Origin);

        Node CurrentNode = null;

        // Para "simular" la recursividad, necesitamos hacer un ciclo, ya sea un for o un while, etc.
        // Nuestro ciclo va a tener como condición de finalización las mismas condiciones que la versión recursiva:
        // es decir: 1) Ya llegué a la meta (goal); 2) No hay camino en absoluto,
        // esta condición 2, se cumple cuando ya visitaste TODOS los nodos que pudiste alcanzar y ninguno de ellos fue la meta (goal).
        while ( CurrentNode != Goal && KnownStack.Count != 0 ) /* todavía haya nodos por visitar */
        {
            // Las pilas (Stack) se trabajan sobre el elemento que está en el tope de la pila.
            CurrentNode = KnownStack.Peek(); // lee el elemento del tope de la pila PERO no lo saques.
            Debug.Log("Nodo: " + CurrentNode.Name);

            // Ahora queremos meter a la Pila a los vecinos de current que no tengan parent y que no estén en los visitados.
            // paso 1) Obtener sus vecinos
            List<Node> currentNeighbors = GetNeighbors(CurrentNode);

            // paso 2) filtrar a los que ya estén en visitados.
            List<Node> nonVisitedNodes = RemoveVisitedNodes(currentNeighbors, VisitedNodes);

            // paso 3) filtrar a los que tengan parent.
            List<Node> nonParentNeighbors = RemoveNodesWithParent(nonVisitedNodes);

            // Ahora sí, ya podemos meter a la pila al primero de esa lista de los que quedaron después de filtrar (nonParentNeighbors)
            if (nonParentNeighbors.Count > 0)
            {
                // Como este nodo currentNode está metiendo a la stack al nodo "nonParentNeighbors[0]", entonces currentNode se vuelve su padre.
                nonParentNeighbors[0].Parent = CurrentNode;

                // entonces sí hay alguien a quien meter en la pila, y metemos al primer elemento de dicha lista.
                KnownStack.Push(nonParentNeighbors[0]);
                continue;
            }

            // Un nodo no se saca de la pila hasta que ya no tiene otro nodo a quien meter a la pila.
            Node PoppedNode = KnownStack.Pop();

            // Después de hacerle Pop, lo tenemos que meter a los visitados.
            VisitedNodes.Add(PoppedNode);
        }

        // Nos falta comprobar por qué se rompió el ciclo while de arriba.
        // Si esto se cumple, es porque sí llegamos a la meta.
        if (Goal == CurrentNode)
        {
            // Ahorita no hacemos nada más con ella, pero si lo quisiéramos hacer, pues de aquí la tomaríamos.
            PathToGoal = Backtrack(CurrentNode);

            return true;
        }

        // Si no, ¡pues no!
        return false;
    }

    // únicamente necesitamos que nos pasen el nodo desde el cual se quiere realizar el Backtracking.
    List<Node> Backtrack(Node inNode)
    {
        Node TempNode = inNode;

        // Aquí ya llegamos a la meta. Estamos parados en el nodo Goal.
        List<Node> InvertedPathToGoal = new List<Node>();
        InvertedPathToGoal.Add(TempNode);

        while (TempNode != TempNode.Parent)  // esta condición solo se cumple en el nodo Origin.
        {
            TempNode = TempNode.Parent;
            InvertedPathToGoal.Add(TempNode);
        }

        // Necesitamos invertir la lista porque el back nos la da en el orden inverso.
        List<Node> PathToGoal = new List<Node>();
        for (int i = InvertedPathToGoal.Count - 1; i >= 0; i--)
        {
            PathToGoal.Add(InvertedPathToGoal[i]);
            Debug.Log("El nodo: " + InvertedPathToGoal[i].Name + " fue parte del camino a la meta.");
        }

        return PathToGoal;
    }

    // Hacemos una función que nos dé los vecinos para poder reutilizarla y que nuestras funciones no tengan tantas líneas de código.
    // Nos dice cuáles nodos comparten una arista con inNode
    List<Node> GetNeighbors(Node inNode)
    {
        List<Node> Neighbors = new List<Node>();

        foreach (Edge currentEdge in EdgeSet)
        {
            // Vamos a checar si la arista en cuestión hace referencia a este nodo "CurrentNode". Checamos su A y su B. 
            if (currentEdge.A == inNode)
            {
                Neighbors.Add(currentEdge.B);
            }
            else if (currentEdge.B == inNode)
            {
                Neighbors.Add(currentEdge.A);
            }
        }

        return Neighbors;
    }

    List<Node> RemoveNodesWithParent(List<Node> NodesToBeFiltered)
    {
        List<Node> FilteredNeighbors = new List<Node>();
        foreach (Node neighbor in NodesToBeFiltered)
        {
            // ¿Este nodo tiene Parent? Si no, lo añadimos a los que vamos a regresar.
            if (neighbor.Parent == null)
            {
                FilteredNeighbors.Add(neighbor);
            }
        }

        return FilteredNeighbors;
    }

    List<Node> RemoveVisitedNodes(List<Node> NodesToBeFiltered, HashSet<Node> VisitedNodesSet)
    {
        List<Node> nonVisitedNodes = new List<Node>();
        foreach (Node neighbor in NodesToBeFiltered)
        {
            // Si los nodos visitados no contienen a este nodo, no lo quitamos.
            if (!VisitedNodesSet.Contains(neighbor))
                nonVisitedNodes.Add(neighbor);
        }

        return nonVisitedNodes;
    }


    // Vamos a implementar el algoritmo de depth-first search (DFS) usando la pila de llamadas,
    // de manera recursiva.
    // ¿Qué pregunta nos va a responder o qué resultado nos va a dar?
    // Pues nos dice si hay un camino desde un Nodo Origen hasta un nodo Destino (de un grafo)
    // y si sí hay un camino, nos dice cuál fue. Esto del camino tiene un truco interesante.
    bool RecursiveDFS(Node Origin, Node Goal)
    {
        // Para evitar que alguien se vuelva padre del nodo raíz de todo el árbol
        // hacemos que el nodo raíz del árbol sea su propio padre.
        if (Origin.Parent == null)
        {
            // si esto se cumple, entonces este nodo es la raíz del árbol.
            Origin.Parent = Origin;
        }

        // La condición de terminación de esta función recursiva es 
        // "el nodo en el que estoy actualmente (Origin) es la meta (Goal)"
        if (Origin == Goal)
            return true;

        // Desde el nodo donde estamos ahorita, checamos cuáles son nuestros vecinos.
        // Los vecinos de este nodo son los que comparten una arista con él.
        // lo que podemos hacer es revisar todas las aristas y obtener las que hagan referencia a este nodo.
        // 1) Checar todas las aristas.
        foreach (Edge currentEdge in EdgeSet)
        {
            bool Result = false;
            Node Neighbor = null;
            // Vamos a checar si la arista en cuestión hace referencia a este nodo "Origin"
            // Checamos su A y su B. Y tenemos que checar que el vecino NO tenga padre, para que Origin 
            // se convierta en su padre.
            if (currentEdge.A == Origin && currentEdge.B.Parent == null)
            {
                // entonces sí hace referencia a este nodo. Lo que hacemos es meter al Nodo vecino.
                // Si encontramos un vecino, primero le decimos que el nodo Origin actual es
                // su nodo padre, y después mandamos a llamar la función de nuevo, pero 
                // usando a este vecino como el nuevo origen.
                currentEdge.B.Parent = Origin;
                Neighbor = currentEdge.B;
            }
            else if (currentEdge.B == Origin && currentEdge.A.Parent == null)
            {
                // entonces sí hace referencia a este nodo. Lo que hacemos es meter al Nodo vecino.
                // Si encontramos un vecino, primero le decimos que el nodo Origin actual es
                // su nodo padre, y después mandamos a llamar la función de nuevo, pero 
                // usando a este vecino como el nuevo origen.
                currentEdge.A.Parent = Origin;
                Neighbor = currentEdge.A;
            }

            // Necesitamos esta comprobación por si no entra ni al if ni al if else de arriba.
            if(Neighbor != null)
                Result = RecursiveDFS(Neighbor, Goal);


            if (Result == true)
            {
                // Si este nodo fue parte del camino al Goal, le decimos que imprima su nombre.
                Debug.Log("El nodo: " + Origin.Name + " fue parte del camino a la meta.");

                // entonces el hijo de este nodo ya encontró el camino
                // y eso hace una reacción en cadena de "Papá, sí encontré el camino".
                return true;
            }
        }

        // si ya acabó el ciclo de intentar visitar a sus vecinos, se regresa a la función del nodo
        // que es su padre.
        return false;
    }


    // Funciones recursivas VS funciones iterativas.

    // las funciones recursivas son funciones que se mandan a llamar a sí mismas.
    void FuncionRecursiva(int Counter)
    {
        Debug.Log("Hola número: " + Counter);
        if (Counter == 10)
            return;
        FuncionRecursiva(Counter+1);
    }

    // MyArray [0, 1, 2, 3, 4...]

    // MyStack [0]
    // [1, 0]
    // 2, 1, 0
    // 3, 2, 1, 0
    // Ahora vamoas a sacar elementos
    // sacas el 3, que es el último que metiste, y te quedaría:
    // 2, 1, 0
    // 1, 0, 
    // 0
    // Last in, First out
    // solo puedes sacar el último elemento que metiste.

    // Update is called once per frame
    void Update()
    {
        
    }
}
