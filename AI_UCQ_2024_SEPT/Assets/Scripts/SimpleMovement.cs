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

    // Queremos que nuestro agente tenga una posici�n en el espacio, y que tenga una velocidad actual a la que se est� moviendo.
    // La variable que nos dice en qu� posici�n en el espacio est� el due�o de este script es: transform.position

    // Si ustedes quieren la posici�n de Otro gameObject que no sea el due�o de este script, tambi�n la acceder�an a trav�s de 
    // transform.position, pero de ese gameObject en espec�fico.
    // Por ejemplo, la posici�n del gameObject PiernaDerecha, tendr�an que tener una referencia (una variable) a ese gameObject
    // y de ah�, acceder a la variable de posici�n as�: PiernaDerecha.transform.position.
    

    // La velocidad actual a la que se est� moviendo debe estar guardada en una variable. Es lo mismo que CurrentSpeed.
    protected Vector3 Velocity = Vector3.zero;

    // Para manejar la aceleraci�n, necesitamos otra variable, una que nos diga cu�l es su m�xima aceleraci�n
    [SerializeField]
    protected float MaxAcceleration = 1.0f;

    // Necesitamos saber la posici�n de la "cosa de inter�s" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    // Queremos poder preguntarle al DebugConfigManager si ciertas banderas de debug est�n activadas.
    // para ello, pues necesitamos tener una referencia al DebugConfigManager.
    protected DebugConfigManager debugConfigManagerRef = null;

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
    // El orden de cu�l Start se ejecuta primero puede variar de ejecuci�n a ejecuci�n.
    protected void Start()
    {
        Debug.Log("Se est� ejecutando Start. " + gameObject.name);

        debugConfigManagerRef = GameObject.FindAnyObjectByType<DebugConfigManager>();
        return;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update n�mero: " + ContadorDeCuadros);
        ContadorDeCuadros++;
        // este movimiento basado en cu�ntos cuadros han transcurrido no es justo para la gente con menos poder de c�mputo
        // transform.position = new Vector3(ContadorDeCuadros, 0, -1);
        // Ahorita tenemos una velocidad de 1 unidad en el eje X por cada cuadro de ejecuci�n.
        // Qu� tal si hacemos que avance una unidad en X por cada segundo que transcurra?

        
        // modificando la posici�n (acumulando los cambios)
        // transform.position += new Vector3(1 * Time.deltaTime, 0, 0);


        // Cada cuadro hay que actualizar el vector que nos dice a d�nde perseguir a nuestro objetivo.
        Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Vector3 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);  // FLEE

        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo m�s r�pido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qu� tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la direcci�n de un vector se pueden separar, �nicamente necesitamos limitar la magnitud para
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
        if(debugConfigManagerRef != null)
        {
            if(debugConfigManagerRef.VelocityLines)
            {
                Gizmos.color = Color.yellow;
                // Velocity S� tiene direcci�n y magnitud (es un vector de 1 o m�s dimensiones),
                // mientras que Speed no, �nicamente es una magnitud (o sea, un solo valor flotante)
                // Primero, dibujamos la "flecha naranja" que es nuestra velocidad (Velocity) actual, partiendo desde nuestra posici�n actual.
                Gizmos.DrawLine(transform.position, transform.position + Velocity);
            }
            // Ahora vamos con la "flecha azul" que es la direcci�n y magnitud hacia nuestro objetivo (la posici�n de nuestro objetivo).
            if (debugConfigManagerRef.DesiredVectors)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + (targetGameObject.transform.position - transform.position));
            }

        }

    }


    int RetornarInt()
    {
        return 0;
    }
}
