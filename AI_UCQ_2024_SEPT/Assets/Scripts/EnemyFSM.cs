using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : BaseFSM
{

    private PatrolState _patrolState;
    private AlertState _alertState;
    private MeleeState _meleeState;

    public MeleeState MeleeState
    { get { return _meleeState; } }

    public PatrolState PatrolState
    { get { return _patrolState; } }


    public AlertState AlertState
    {  get { return _alertState; } }


    public BaseEnemy Owner;


    // Start is called before the first frame update
    public override void Start()
    {
        _meleeState = gameObject.AddComponent<MeleeState>();
        _meleeState.FSMRef = this;

        _patrolState = gameObject.AddComponent<PatrolState>();
        _patrolState.FSMRef = this;

        _alertState = gameObject.AddComponent<AlertState>();
        _alertState.FSMRef = this;

        base.Start(); // Esto es mandar a llamar función del padre.
    }

    public override BaseState GetInitialState()
    {
        // AQUÍ NOS QUEDAMOS POR AHORA.
        return _meleeState; // ESTO NO DEBE SER NULL.
    }

}
