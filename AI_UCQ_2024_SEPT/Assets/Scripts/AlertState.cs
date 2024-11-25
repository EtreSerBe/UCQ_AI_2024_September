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
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta línea es lo mismo pero sin el chequeo de seguridad.
            return; // siempre que hagan un llamado a cambio de estado, debe de ir seguido por un return, para no ejecutar
            // nada más del OnUpdate del estado que va de salida.
        }

        // Cuando sales de un estado X, ya no tienes por qué hacer nada más correspondiente a dicho estado X.

        Debug.Log("Jiji, sigo en el update del estado de Alerta");


        // Gameplay Ability System (GAS)

        // ActivateAbility()

        // NotifyFinish // hay otras varias

        // EndAbility()

        // Si tú activas tu habilidad, y ya estaba activada no entra la nueva.
        // si estás activando tu habilidad, y la habilidad no se puede activar porque algo falló, no hay recursos, etc.
        // entonces tú mandas el EndAbility de manera anticipada Y mandas a llamar un return, porque pues lo demás ya no aplicaría.

    }


}
