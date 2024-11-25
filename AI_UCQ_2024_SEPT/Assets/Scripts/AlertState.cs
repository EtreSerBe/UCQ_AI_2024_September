using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BaseState
{

    public AlertState()
    {
        Name = "Alert State";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if (enemyFSM != null)
        {
            FSMRef.ChangeState(enemyFSM.PatrolState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta l�nea es lo mismo pero sin el chequeo de seguridad.
            return; // siempre que hagan un llamado a cambio de estado, debe de ir seguido por un return, para no ejecutar
            // nada m�s del OnUpdate del estado que va de salida.
        }

        // Cuando sales de un estado X, ya no tienes por qu� hacer nada m�s correspondiente a dicho estado X.

        Debug.Log("Jiji, sigo en el update del estado de Alerta");


        // Gameplay Ability System (GAS)

        // ActivateAbility()

        // NotifyFinish // hay otras varias

        // EndAbility()

        // Si t� activas tu habilidad, y ya estaba activada no entra la nueva.
        // si est�s activando tu habilidad, y la habilidad no se puede activar porque algo fall�, no hay recursos, etc.
        // entonces t� mandas el EndAbility de manera anticipada Y mandas a llamar un return, porque pues lo dem�s ya no aplicar�a.

    }


}
