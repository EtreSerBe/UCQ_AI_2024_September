using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConfigManager;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private int ContadorDeCuadros = 0;

    [SerializeField]
    protected float TiempoTranscurrido = 5;

    [SerializeField]
    protected float MaxSpeed = 5;

    // Queremos que nuestro agente tenga una posición en el espacio, y que tenga una velocidad actual a la que se está moviendo.
    // La variable que nos dice en qué posición en el espacio está el dueño de este script es: transform.position

    // Si ustedes quieren la posición de Otro gameObject que no sea el dueño de este script, también la accederían a través de 
    // transform.position, pero de ese gameObject en específico.
    // Por ejemplo, la posición del gameObject PiernaDerecha, tendrían que tener una referencia (una variable) a ese gameObject
    // y de ahí, acceder a la variable de posición así: PiernaDerecha.transform.position.
    

    // La velocidad actual a la que se está moviendo debe estar guardada en una variable. Es lo mismo que CurrentSpeed.
    protected Vector3 Velocity = Vector3.zero;

    // Para manejar la aceleración, necesitamos otra variable, una que nos diga cuál es su máxima aceleración
    [SerializeField]
    protected float MaxAcceleration = 1.0f;

    // Necesitamos saber la posición de la "cosa de interés" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    // Queremos poder preguntarle al DebugConfigManager si ciertas banderas de debug están activadas.
    // para ello, pues necesitamos tener una referencia al DebugConfigManager.
    // protected DebugConfigManager debugConfigManagerRef = null;

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
    protected void Start()
    {
        Debug.Log("Se está ejecutando Start. " + gameObject.name);

        // debugConfigManagerRef = GameObject.FindAnyObjectByType<DebugConfigManager>();
        return;
    }


    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Update número: " + ContadorDeCuadros);
        // ContadorDeCuadros++;
        // este movimiento basado en cuántos cuadros han transcurrido no es justo para la gente con menos poder de cómputo
        // transform.position = new Vector3(ContadorDeCuadros, 0, -1);
        // Ahorita tenemos una velocidad de 1 unidad en el eje X por cada cuadro de ejecución.
        // Qué tal si hacemos que avance una unidad en X por cada segundo que transcurra?

        
        // modificando la posición (acumulando los cambios)
        // transform.position += new Vector3(1 * Time.deltaTime, 0, 0);


        // Cada cuadro hay que actualizar el vector que nos dice a dónde perseguir a nuestro objetivo.
        Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Vector3 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);  // FLEE

        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo más rápido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qué tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la dirección de un vector se pueden separar, únicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;


        // transform.position += 
    }

    void FixedUpdate()
    {

    }


    void OnDrawGizmos()
    {

        if(DebugGizmoManager.VelocityLines)
        {
            Gizmos.color = Color.yellow;
            // Velocity SÍ tiene dirección y magnitud (es un vector de 1 o más dimensiones),
            // mientras que Speed no, únicamente es una magnitud (o sea, un solo valor flotante)
            // Primero, dibujamos la "flecha naranja" que es nuestra velocidad (Velocity) actual, partiendo desde nuestra posición actual.
            Gizmos.DrawLine(transform.position, transform.position + Velocity);
        }
        // Ahora vamos con la "flecha azul" que es la dirección y magnitud hacia nuestro objetivo (la posición de nuestro objetivo).
        if (DebugGizmoManager.DesiredVectors)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (targetGameObject.transform.position - transform.position));
        }

    }


    int RetornarInt()
    {
        return 0;
    }
}
