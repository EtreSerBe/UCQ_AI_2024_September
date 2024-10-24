using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    private LinkedList<Node> NodeList;

    // Queremos insertar/quitar elementos según su prioridad, 
    // de manera que vayan quedando siempre ordenados.

    // Función de meter elementos se debe de llamar "insertar"
    public void Insert(Node inNode, float Priority)
    {
        // Primero hay que checar que la NodeList no esté vacía.
        if (NodeList.Count == 0)
        {
            // si sí está vacía metemos el elemento inNode y ya.
            NodeList.AddFirst(inNode);  // NOTA: Aquí se usa AddFirst, no AddAfter.
            // Ahora nos salimos de la función porque ya metimos el nodo.
            return;
        }

        // Debe de buscar la nodo actualmente dentro de la lista cuya prioridad sea más grande que la suya (Priority).
        // cuando encuentras un nodo con prioridad mayor, es porque este nodo inNode debe de ir antes de él.
        LinkedListNode<Node> currentLinkedNode = NodeList.First;
        while (currentLinkedNode.Value.Priority < Priority)
        {
            // Si se sigue cumpliendo la condición del while, pasa al siguiente nodo de la lista.
            currentLinkedNode = currentLinkedNode.Next;
            if (currentLinkedNode == null)
            {
                // Aquí ya se acabaron los elementos, entonces este nodo inNode debe ser el último de la NodeList.
                NodeList.AddLast(inNode);
                // Ahora nos salimos de la función porque ya metimos el nodo.
                return;
            }
        }

        // Si sí se sale del while, quiere decir que encontramos dónde debemos de insertar este nodo.
        // Si el currentLinkedNode tiene una prioridad mayor, entonces inNode debe de ir antes que él

        // Le pedimos a la NodeList que lo meta antes de currentLinkedNode y listo.
        NodeList.AddBefore(currentLinkedNode, inNode);
    }

    // Ahora hacemos la función de quitar del inicio de la Fila:
    public Node Dequeue()
    {
        // Debemos checar si está vacía antes de hacer cualquier cosa.
        if (NodeList.Count == 0)
        {
            Debug.LogWarning("Se llamó la función Dequeue de nuestra PriorityQueue estando vacía.");
            return null;
        }

        Node OutNode = NodeList.First.Value;

        // Ahora sí lo borramos de la NodeList para que ya no cuente en la fila.
        NodeList.RemoveFirst();

        // y lo regresamos para hacer lo que necesitemos con este nodo.
        return OutNode;
    }

    public bool Contains(Node inNode)
    {
        return NodeList.Contains(inNode);
    }

    public int Count()
    {
        return NodeList.Count;
    }
}
