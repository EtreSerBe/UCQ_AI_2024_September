using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConfigManager;


public enum WeaponType
{
    Weak,
    Normal,
    Hielo,
    Fuego,
    Trueno
}

public enum EnemyState
{
    Idle,
    Alert, // sospechoso/investigando/etc.
    Attack // ya te detectó y te ataca.
}


// On/Off son dos posibilidades únicamente
// true/false son dos posibilidades únicamente
// cliente/servidor
// para todas esas situaciones donde solo hay dos posibilidades, nos basta con un solo bit.

// cuántos bits tiene un booleano? 8 bits, que son un byte.
// 8 bits almacenan hasta 256 posibilidades (2^8)
// qué pasa con los otros 7 bits que no usamos para true/false?
// una posibilidad es meter otros true/false en los bits restantes.

// por ejemplo, el primer bit podría ser, OrigenDelMensaje (o cliente o servidor)
// otro podría ser: JugadorPremium (1 si sí es, 0 si no es)
// 

public enum ExampleMessageBits
{
    ClientOrServer = 1, // [0 0 0 0 0 0 0 X ]
    PremiumUser = 2, // [0 0 0 0 0 0 X 0 ]
    EstáVolando = 4, // [0 0 0 0 0 X 0 0 ]
    OtraCosa = 8, // [0 0 0 0 X 0 0 0 ]
    OtraMas = 16,
}

public enum Layers
{
    Default = 1,
    TransparentFX = 2,

}


// Armas[5] = {Weak, Normal, Hielo, Fuego, Trueno}

// Armas[0]

// Armas[WeaponType.Normal]


// Desventajas: es más largo de escribir, puede parecer tedioso al inicio.

// Ventajas: 
// 1) Claridad. 0 es mucho menos claro para alguien que "WeaponType.Normal"
// 2) Exclusividad. No te deja asignar valores numéricos directamente a una variable de tipo enum (a menos que hayas puesto la conversión
//  explícita, que casi nunca lo deberían de hacer ni necesitar).
// 3) Poder separar claramente distintas opciones de ejecución de un programa. (por ejemplo, lo del EnemyState mostrado arriba).

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
    public Vector3 Velocity = Vector3.zero;

    // Para manejar la aceleración, necesitamos otra variable, una que nos diga cuál es su máxima aceleración
    [SerializeField]
    protected float MaxAcceleration = 1.0f;

    // Qué tanto tiempo a futuro (o pasado, si es negativa) va a predecir el movimiento de su target.
    protected float PursuitTimePrediction = 1.0f;

    // Necesitamos saber la posición de la "cosa de interés" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    [SerializeField]
    protected float ObstacleForceToApply = 1.0f;

    [SerializeField]
    private WeaponType myWeaponType;

    protected EnemyState currentEnemyState = EnemyState.Idle;

    protected Vector3 ExternalForces = Vector3.zero;

    [SerializeField] protected String ObstacleLayerName = "Obstacle";

    public void AddExternalForce(Vector3 ExternalForce)
    {
        ExternalForces += ExternalForce;
    }

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

        // myWeaponType = 1;  // No te deja, para mantener esa exclusividad de los enum.

        // debugConfigManagerRef = GameObject.FindAnyObjectByType<DebugConfigManager>();
        return;
    }

    // Aquí, other va a ser el obstáculo, no el agente.
    void OnTriggerStay(Collider other)
    {

        // Si esta colisión es contra alguien que NO es un obstáculo (es decir, no está en la Layer de Obstacle),
        // entonces, no hagas nada.
        if (other.gameObject.layer != LayerMask.NameToLayer(ObstacleLayerName))
        {
            return;
        }

        // Si detectamos que un agente está dentro de nuestro radio/área de activación (en este caso es nuestro trigger),
        // calculamos un vector con origen en la posición de este objeto, y cuyo fin es la posición de ese agente
        // NOTA: Esta resta es hacia el CENTRO del agente, por lo que sí puede llegar a ser más grande que el radius del collider.
        Vector3 OriginToAgent = transform.position - other.transform.position;
        // y después se lo aplicamos al agente como una fuerza que afecta su steering behavior.
        //SimpleMovement otherSimpleMovement = GetComponent<SimpleMovement>();
        // aquí podemos usar "this" en vez de hacer el getComponent que hacíamos cuando estábamos en el obstáculo.

        // Debug.Log("Entré a OnTriggerStay de SimpleMovement con: " + other.gameObject.name);

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

        AddExternalForce(OriginToAgent.normalized * calculatedForce);
    }


    // Update is called once per frame
    void Update()
    {
        switch (myWeaponType)
        {
            case WeaponType.Weak:
                // Atacar con el arma weak
                break;
            case WeaponType.Normal: 
                // atacar con el arma Normal.
                break;
        }


        // Debug.Log("Update número: " + ContadorDeCuadros);
        // ContadorDeCuadros++;
        // este movimiento basado en cuántos cuadros han transcurrido no es justo para la gente con menos poder de cómputo
        // transform.position = new Vector3(ContadorDeCuadros, 0, -1);
        // Ahorita tenemos una velocidad de 1 unidad en el eje X por cada cuadro de ejecución.
        // Qué tal si hacemos que avance una unidad en X por cada segundo que transcurra?


        // modificando la posición (acumulando los cambios)
        // transform.position += new Vector3(1 * Time.deltaTime, 0, 0);


        // Cada cuadro hay que actualizar el vector que nos dice a dónde perseguir a nuestro objetivo.
        // Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Vector3 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);  // FLEE


        // Hay que pedirle al targetGameObject que nos dé acceso a su Velocity, la cual está en el script SimpleMovement
        // Cuando usen algo que le pertenezca a un objeto que pueda llegar a ser null, chéquenlo.
        if (targetGameObject == null)
        {
            // lo importante aquí es que hicimos que ya no truene.
            return; // OJO: ahorita, en este en específico, ya no va a hacer nada, ni siquiera moverse, cuando sea null.
        }

        Vector3 currentVelocity = targetGameObject.GetComponent<SimpleMovement>().Velocity;

        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, targetGameObject.transform.position);

        // Primero predigo dónde va a estar mi objetivo
        Vector3 PredictedPosition =
            PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

        // Hago seek hacia la posición predicha.
        Vector3 PosToTarget = PuntaMenosCola(PredictedPosition, transform.position); // SEEK

        PosToTarget += ExternalForces;


        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo más rápido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qué tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la dirección de un vector se pueden separar, únicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;


        // Hay que resetearlas cada frame, si no se van a seguir aplicando aunque ya no se las deban aplicar.
        // Hay que resetearla al final del cuadro, si no se le va a quitar antes de poder utilizarla.
        ExternalForces = Vector3.zero;

        // transform.position += 
    }

    // Esta función predice a dónde se moverá un objeto cuya posición actual es InitialPosition, su velocidad actual es Velocity,
    // tras una cantidad de tiempo TimePrediction.
    protected Vector3 PredictPosition(Vector3 InitialPosition, Vector3 Velocity, float TimePrediction)
    {
        // Con base en la Velocity dada vamos a calcular en qué posición estará nuestro objeto con posición InitialPosition,
        // tras una cantidad X de tiempo (TimePrediction).
        return InitialPosition + Velocity * TimePrediction;

        // nosotros empezamos
    }

    protected float CalculatePredictedTime(float MaxSpeed, Vector3 InitialPosition, Vector3 TargetPosition)
    {
        // Primero obtenemos la distancia entre InitialPosition y TargetPosition. Lo hacemos con un punta menos cola, 
        // y nos quedamos con la pura magnitud, porque solo queremos saber cuánto distancia hay entre ellos, no en qué dirección.
        float Distance = PuntaMenosCola(TargetPosition, InitialPosition).magnitude;

        // Luego, dividimos nuestra distancia obtenida entre nuestra velocidad máxima.
        return Distance / MaxSpeed;
    }

    protected Vector3 Pursuit(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, TargetCurrentPosition);

        // Primero predigo dónde va a estar mi objetivo
        Vector3 PredictedPosition =
            PredictPosition(TargetCurrentPosition, TargetCurrentVelocity, PursuitTimePrediction);

        // Hago seek hacia la posición predicha.
        return PuntaMenosCola(PredictedPosition, transform.position); // SEEK
    }

    protected Vector3 Evade(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        return - Pursuit(TargetCurrentPosition, TargetCurrentVelocity);
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
        if (DebugGizmoManager.DesiredVectors && targetGameObject != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (targetGameObject.transform.position - transform.position));
        }

        if(targetGameObject != null) 
        { 
            // Vamos a dibujar la posición a futuro que está prediciendo.
            Vector3 currentVelocity = targetGameObject.GetComponent<SimpleMovement>().Velocity;

            PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, targetGameObject.transform.position);

            // Primero predigo dónde va a estar mi objetivo
            Vector3 PredictedPosition =
                PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(PredictedPosition, Vector3.one);
        }
    }


    int RetornarInt()
    {
        return 0;
    }
}
