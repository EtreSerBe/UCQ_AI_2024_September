using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private int ContadorDeCuadros = 0;

    [SerializeField]
    protected float TiempoTranscurrido = 5;

    [SerializeField]
    protected float MaxSpeed = 5;


    // Necesitamos saber la posición de la "cosa de interés" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    //void Awake()
    //{
    //}

    public Vector3 PuntaMenosCola(Vector3 Punta, Vector3 Cola)
    {
        float X = Punta.x - Cola.x;
        float Y = Punta.y - Cola.y;
        float Z = Punta.z - Cola.z;

        return new Vector3(X, Y, Z);

        // return Punta - Cola; // es lo mismo pero ya con las bibliotecas de Unity.
    }


    // Start is called before the first frame update
    // El orden de cuál Start se ejecuta primero puede variar de ejecución a ejecución.
    void Start()
    {
        Debug.Log("Se está ejecutando Start. " + gameObject.name);
        return;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update número: " + ContadorDeCuadros);
        ContadorDeCuadros++;
        // este movimiento basado en cuántos cuadros han transcurrido no es justo para la gente con menos poder de cómputo
        // transform.position = new Vector3(ContadorDeCuadros, 0, -1);
        // Ahorita tenemos una velocidad de 1 unidad en el eje X por cada cuadro de ejecución.
        // Qué tal si hacemos que avance una unidad en X por cada segundo que transcurra?

        
        // modificando la posición (acumulando los cambios)
        // transform.position += new Vector3(1 * Time.deltaTime, 0, 0);


        // Cada cuadro hay que actualizar el vector que nos dice a dónde perseguir a nuestro objetivo.
        Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Vector3 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);  // FLEE

        // Queremos que lo más rápido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qué tan grande se la
        // flecha de PosToTarget.
        // Como la magnitud y la dirección de un vector se pueden separar, únicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Vector3 Velocity = PosToTarget.normalized * MaxSpeed;

        transform.position += Velocity * Time.deltaTime;


        // transform.position += 
    }

    void FixedUpdate()
    {

    }


    int RetornarInt()
    {
        return 0;
    }
}
