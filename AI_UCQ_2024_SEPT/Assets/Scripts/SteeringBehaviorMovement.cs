using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviorMovement : SimpleMovement
{

    // Para acceder a un Component, hay que tener una referencia a él dentro de esta clase.
    // y para tener esa referencia, usamos una variable de dicho tipo.
    protected Rigidbody rb = null;


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
        Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Force o Acceleration nos dan lo mismo ahorita porque no vamos a modificar la masa.
        rb.AddForce(PosToTarget.normalized * MaxAcceleration, ForceMode.Force);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, MaxSpeed);


        // El rigidbody ya se va a encargar de cambiarnos nuestra velocity y nuestra transform.position de manera "física" (físicamente simulada).
    }
}
