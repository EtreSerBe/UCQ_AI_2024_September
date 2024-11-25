using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public PatrolState()
    {
        Name = "Patrol State";
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // Oye, máquina de estados que es dueña de este script, cambia hacia el estado de Alerta.
        // ESTO NO ME DEJA PORQUE LA FSMRef NO ES DE TIPO EnemyFSM que es la que tiene el AlertState
        // FSMRef.ChangeState( FSMRef.AlertState ); 
        
        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if( enemyFSM != null )
        {
            FSMRef.ChangeState(enemyFSM.AlertState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta línea es lo mismo pero sin el chequeo de seguridad.
        }

    }

}
