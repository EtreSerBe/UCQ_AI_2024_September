using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictableMovement : SimpleMovement
{

    [SerializeField] private GameObject[] PatrolPoints;

    private int CurrentPatrolPoint = 0;

    [SerializeField] private float PatrolPointToleranceRadius;


    protected SimpleMovement ObjectToEvade = null;

    // 2X + 3X
    // X*(2+3)
    // X*5

    // Start is called before the first frame update
    new void Start()
    {
        ObjectToEvade = FindObjectOfType<SimpleMovement>();
        if (ObjectToEvade == null)
        {
            Debug.LogError("ERROR: no object to evade.");
        }
        else if (ObjectToEvade == this)
        {
            Debug.LogError("ERROR: evading itself.");
        }
        else
        {
            Debug.Log("Found object to evade.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PredictedPosition = Vector3.zero;
        Vector3 PosToTarget = gameObject.transform.forward;

        if (currentEnemyState == EnemyState.Idle)
        {
            // Tenemos que definir un área de aceptación/tolerancia de "ya llegué, ¿cuál sigue?" 
            // Queremos checar si la distancia entre el punto de patrullaje actual y nuestro agente es menor o igual que 
            // un radio de tolerancia.
            if (Utilities.Utility.IsInsideRadius(PatrolPoints[CurrentPatrolPoint].transform.position, transform.position,
                    PatrolPointToleranceRadius))
            {
                // Si estamos dentro, entonces ya llegamos y ya nos podemos ir hacia el siguiente punto de patrullaje.
                CurrentPatrolPoint++;
                if (CurrentPatrolPoint >= PatrolPoints.Length)
                {
                    currentEnemyState = EnemyState.Alert;
                    Debug.Log("Pasé al estado de alerta.");
                }

                CurrentPatrolPoint %= PatrolPoints.Length;

                // 0 % 4 = 0
                // 1 % 4 = 1
                // 2 % 4 = 2
                // 3 % 4 = 3
                // 4 % 4 = 0
                // 5 % 4 = 1

                // La otra forma sería usando un if
                // Si nuestro contador "CurrentPatrolPoints" es mayor o igual que el número de elementos en el arreglo "PatrolPoints"
                // if (CurrentPatrolPoint >= PatrolPoints.Length)
                // {
                //     // Entonces lo reseteamos a 0.
                //     CurrentPatrolPoint = 0;
                // }
            }

            // Hacemos que nuestro agente haga Seek a los puntos de patrullaje
            // Será al punto de patrullaje al cual estemos yendo actualmente.
            PosToTarget = PuntaMenosCola(PatrolPoints[CurrentPatrolPoint].transform.position, transform.position); // SEEK

        }
        else if (currentEnemyState == EnemyState.Alert)
        {
            // si estamos en el estado de alerta vamos a hacer otra cosa, que es evadir al ObjectToEvade.
            PosToTarget = Evade(ObjectToEvade.transform.position, ObjectToEvade.Velocity);
        }


        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo más rápido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qué tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la dirección de un vector se pueden separar, únicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;
    } 


}
