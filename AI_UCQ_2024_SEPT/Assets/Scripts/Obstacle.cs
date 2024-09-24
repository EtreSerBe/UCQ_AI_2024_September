using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private float ObstacleForceToApply = 1.0f;

    // Como tip, podemos tomar en cuenta las dimensiones del collider/trigger para multiplicar esa ObstacleForceToApply

    // Queremos que cuando detecte un agente en su trigger, que lo empuje en otra dirección para que lo
    // desvíe ligeramente hacia otra dirección.

    void OnTriggerStay(Collider other)
    {

        // Si detectamos que un agente está dentro de nuestro radio/área de activación (en este caso es nuestro trigger),
        // calculamos un vector con origen en la posición de este objeto, y cuyo fin es la posición de ese agente
        // NOTA: Esta resta es hacia el CENTRO del agente, por lo que sí puede llegar a ser más grande que el radius del collider.
        Vector3 OriginToAgent = other.transform.position - transform.position;
        // y después se lo aplicamos al agente como una fuerza que afecta su steering behavior.
        SimpleMovement otherSimpleMovement = other.gameObject.GetComponent<SimpleMovement>();

        if (otherSimpleMovement == null)
        {
            // entonces no le podemos asignar fuerzas ni nada, porque es null.
            return;
        }
        else
        {
            // Queremos que entre más cerca esté el agente de este obstáculo, más grande sea la fuerza que se aplica.
            // entre más chica sea la distancia entre estos dos objetos, con relación al radio del trigger, mayor 
            // será la fuerza aplicada.

            float distance = OriginToAgent.magnitude;

            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider == null)
            {
                return;
            }

            // collider.radius nos da el radio en espacio local, sin embargo, nosotros lo necesitamos en espacio de mundo
            // es decir, escalado por las escalas de sus padres en la Jerarquía de la escena. 
            float obstacleColliderRadius = collider.radius; // * transform.lossyScale.y;

            float calculatedForce = ObstacleForceToApply * (1.0f - distance / obstacleColliderRadius);

            otherSimpleMovement.AddExternalForce(OriginToAgent.normalized * calculatedForce);
        }


        // (le podemos poner una variable para ajustar la cantidad de fuerza que se le puede aplicar al agente).

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
