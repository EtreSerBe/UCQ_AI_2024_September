using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;



public class Node
{
    public Node(string inName)
    {
        Name = inName;
        Parent = null;
    }

    public string Name = "";  // es string por pura claridad, idealmente se usan ints para diferenciar objetos.

    public Node Parent;  // referencia al nodo padre de este nodo en el árbol que se genera durante un Pathfinding.
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


    // ¿Qué es un "Set" en estructuras de datos / programación?
    // Un set es una estructura de datos que no permite repetidos
    // específicamente en nuestros grafos, no vamos a querer ni nodos ni aristas repetidas.


    protected HashSet<Node> NodeSet = new HashSet<Node>();
    protected HashSet<Edge> EdgeSet = new HashSet<Edge>();


    // Start is called before the first frame update
    void Start()
    {
        // Vamos a llenar nuestros sets de nodos y aristas.
        // Comenzamos creando todos los nodos, porque las aristas necesitan que ya existan los nodos.
        Node NodeA = new Node("A");
        Node NodeB = new Node("B");
        Node NodeC = new Node("C");
        Node NodeD = new Node("D");
        Node NodeE = new Node("E");
        Node NodeF = new Node("F");
        Node NodeG = new Node("G");
        Node NodeH = new Node("H");

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

        // FuncionRecursiva(0);  // comentada para que no truene ahorita.
    }

    // Funciones recursivas VS funciones iterativas.

    // las funciones recursivas son funciones que se mandan a llamar a sí mismas.
    void FuncionRecursiva(int Counter)
    {
        Debug.Log("Hola número: " + Counter);
        FuncionRecursiva(Counter+1);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
