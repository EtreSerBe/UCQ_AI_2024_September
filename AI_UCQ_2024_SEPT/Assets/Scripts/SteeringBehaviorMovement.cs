using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Utilities;
using DebugConfigManager;
using Unity.VisualScripting;

public class SteeringBehaviorMovement : SimpleMovement
{

    // Para acceder a un Component, hay que tener una referencia a él dentro de esta clase.
    // y para tener esa referencia, usamos una variable de dicho tipo.
    protected Rigidbody rb = null;


    public Vector3 TargetPos = Vector3.zero;

    public Vector3 SpherePos = Vector3.zero;
    public float SphereRadius = 1.0f;



// Para obtener la distancia entre dos puntos en el espacio, simplemente hacemos Punta menos Cola, pero nos
    // quedamos únicamente con la magnitud de dicho vector.

    // La figura más sencilla (al menos matemáticamente hablando) es la esfera (3D) y círculo (2D).
    // Una esfera está definida por un punto de origen y por un radio.




    bool IsInsideSphere()
    {
        // Para saber si un punto en el espacio (llamado TargetPos) está dentro o fuera de una esfera en el espacio, 
        // hacemos un vector que inicia en el origen de la esfera y que termine en TargetPos (punta menos cola)
        Vector3 SphereToTarget = TargetPos - SpherePos;
        // Y luego obtenemos la magnitud de dicho vector
        float VectorMagnitude = SphereToTarget.magnitude;
        // y finalmente comparamos esa magnitud contra el radio de la esfera.
        // (usamos operadores de comparación: == , !=  , > < <= >=...)
        // Si el radio es mayor o igual que la magnitud de ese vector, entonces TargetPos está dentro de la esfera,
        if (SphereRadius >= VectorMagnitude)
        {
            return true;
        }
        // de lo contrario, está fuera de la esfera
        else
        {
            return false; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si hay un evento de OnTriggerEnter, ¿qué debería pasar?
        // puede ser, con el dueño de este script, con el objeto "other" con quien chocamos, con otra cosa, 
        // por ejemplo, las monedas en mario bros, las tocas, ellas desaparecen y aumenta tu cantidad de monedas en la UI.

        // En este caso, cuando detectemos al otro personaje, queremos que nuestro agente conozca a ese gameObject que 
        // triggereó este evento.
        Debug.Log("Entré a OnTriggerEnter chocando con: " + other.gameObject.name);

        // Hacer una clase de un BouncePad como los de sonic, que te mande a otra dirección rápidamente.
    }

    void OnTriggerExit(Collider other)
    {

    }



    void Start()
    {
        // En vez de sobreescribir el método Start de la clase padre, lo vamos a extender.
        base.Start();

        // Aquí ya terminó de ejecutar el Start del padre, y podemos hacer lo demás que necesitemos que sea exclusivo para esta clase.
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Utility.IsInsideRadius(targetGameObject.transform.position, transform.position, SphereRadius))
        {
            // Debug.Log("Sí está dentro de la esfera");
        }
        else
        {
            // Debug.Log("Está fuera de la esfera.");
        }


        Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Force o Acceleration nos dan lo mismo ahorita porque no vamos a modificar la masa.
        rb.AddForce(PosToTarget.normalized * MaxAcceleration, ForceMode.Force);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);


        // El rigidbody ya se va a encargar de cambiarnos nuestra velocity y nuestra transform.position de manera "física" (físicamente simulada).
    }

    void OnDrawGizmos()
    {

        if (DebugGizmoManager.DetectionSphere)
        {
            if (Utility.IsInsideRadius(targetGameObject.transform.position, transform.position, SphereRadius))
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            // Vamos a dibujar nuestra esfera (con su radio)
            Gizmos.DrawWireSphere(transform.position, SphereRadius);
        }
        // La TargetPos. ESTA NO LA VOY A PONER EN EL CONFIG MANAGER PORQUE LA VAMOS A CAMBIAR PRÓXIMAMENTE.
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(targetGameObject.transform.position,  Vector3.one);

        if(DebugGizmoManager.DesiredVectors)
        {
            Gizmos.color = Color.blue;
            // Y la flecha del origen de nuestra esfera (transform.position) hasta la TargetPos
            Gizmos.DrawLine(transform.position, targetGameObject.transform.position);
        }
    }
}
