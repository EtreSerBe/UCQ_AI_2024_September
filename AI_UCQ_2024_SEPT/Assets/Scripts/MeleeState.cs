using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MeleeState : BaseState
{

    public enum MeleeSubstate
    {
        SubstateSelection,
        BasicAttack,
        Dash,
        AreaAttack,
        Ultimate
    }

    private MeleeSubstate _meleeSubstate = MeleeSubstate.BasicAttack;

    // Si el player est� lejos, me acerco hasta estar en X rango.

    // tenemos que tener una referencia al player (u objetivo) al que se va a acercar.
    // Posibilidad 1, que la referencia la tenga la EnemyFSM
    // 
    // este enemigo debe tener una forma de moverse hacia su objetivo.
    // componente NavMeshAgent 
    // tiene que poder detectar si una posici�n X est� dentro de un rango Y.


    // Si el player est� dentro de X rango, hago UN ataque b�sico, 
    // de los 3 disponibles, cada uno tiene 33% de probabilidad // vamos empezando solo con 1.

    private EnemyFSM enemyFSMRef;

    private BaseEnemy owner;

    private bool SubstateEntered = false;

    private int BasicAttackCounter = 0;

    public MeleeState()
    {
        Name = "Melee State";
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Setup();
    }

    public void Setup()
    {
        if(enemyFSMRef == null)
            enemyFSMRef = (EnemyFSM)FSMRef;

        if(owner == null)
            owner = enemyFSMRef.Owner;

        if (enemyFSMRef != null && owner != null)
        {
            // StartCoroutine(Buildup());
        }
    }

    void OnExitBasicAttack()
    {
        Debug.Log("On exit del basic attack, aqu� liberar�a los recursos que solamente necesita este subestado.");
        SubstateEntered = false;
    }

    IEnumerator Buildup(float BuildupTime, IEnumerator CoroutineName)
    {
        if(((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Build-Up " + CoroutineName.ToString());
        yield return new WaitForSeconds(BuildupTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� Build-Up " + CoroutineName.ToString());
        StartCoroutine(CoroutineName);
    }

    IEnumerator AreaAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Area attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de �rea n�mero: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks/2.0f, TimeBetweenAttacks/2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� area attack");
        StartCoroutine(Cooldown(bossEnemy.AreaAttackCooldownTime, "Melee Area attack"));
    }

    IEnumerator DashAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Dash attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de dash n�mero: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� Dash attack");
        StartCoroutine(Cooldown(bossEnemy.DashCooldownTime, "Melee Dash"));
    }

    IEnumerator Cooldown(float CooldownTime, string ActionOnCooldown)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empec� Cooldown de " + ActionOnCooldown);
        yield return new WaitForSeconds(CooldownTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Termin� cooldown de " + ActionOnCooldown);

        GoToSelectionState();
    }

    // En �rboles de decisiones esto se conoce como GoToParent o GoToRoot.
    void GoToSelectionState()
    {
        _meleeSubstate = MeleeSubstate.SubstateSelection;
        SubstateEntered = false;
    }

    // En el OnStart de este estado MeleeState, no necesitamos checar en qu� subestado vamos a estar, 
    // porque siempre va a ser en BasicAttack, porque eso nos especifica el diagrama del dise�o.

    public override void OnUpdate()
    {
        base.OnUpdate();


        // En el OnUpdate es donde s� vamos a diferenciar en qu� subestado (_meleeSubstate) estamos.
        // este switch tal cual va a ser nuestra "FSM" para los subestados.
        switch (_meleeSubstate)
        {
            case MeleeSubstate.BasicAttack:
                /*
                 Si el player est� lejos, me acerco hasta estar en X rango.
                    Si el player est� dentro de X rango, hago UN ataque b�sico,
                de los 3 disponibles, cada uno tiene 33% de probabilidad.

                 */

                if(!SubstateEntered)
                {
                    SubstateEntered = true;
                    // Le dices que persiga al player que quiere atacar.
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    // Cuando entramos a este subestado, reseteamos el contador de ataques b�sicos que ha realizado este enemigo.
                    BasicAttackCounter = 0;
                }

                if(owner.IsPlayerInRange(5))
                {
                    Debug.Log("Puedo atacar b�sico");
                    BasicAttackCounter++;
                }

                // Checamos posible transici�n:
                // Haber hecho ataque b�sico 3 veces Y
                // Si el player est� dentro del rango X donde s� le alcanzar�a(o casi) a pegar este ataque de �rea (10 por ahora)
                if(BasicAttackCounter >= 3 && owner.IsPlayerInRange(10))
                {
                    // si se cumplen estas dos condiciones, entonces cambiamos al subestado de ataque de �rea (AreaAttack).
                    OnExitBasicAttack();
                    _meleeSubstate = MeleeSubstate.AreaAttack;
                    return;
                }

                // Cuando vayamos a salir del Subestado de BasicAttack, tenemos que cambiar "SubstateEntered" a false
                // para que el nuevo subestado al que cambiemos entre adecuadamente.

                break;
            case MeleeSubstate.Dash:
                StartCoroutine(Buildup(((BossEnemy)owner).DashBuildupTime, DashAttack()));
                break;
            case MeleeSubstate.AreaAttack:
                // Cuando entra a este subestado NO se mueve hacia el jugador (al menos la del juego de Hades)
                if (!SubstateEntered)  // este if(!SubstateEntered) es b�sicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podr�a llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);

                    // Iniciar el build-up del ataque.
                    StartCoroutine(Buildup(((BossEnemy)owner).AreaAttackBuildupTime, AreaAttack()));
                    // return; pero no es necesario por c�mo est� especificado mi subestado de AreaAttack.
                }

                // Hace build-up
                // Hace ataque
                // Hace cooldown
                // Y al terminar cooldown puede cambiar de estado/ subestado

                break;
            case MeleeSubstate.Ultimate: 
                break;
            default:
                break;
        }

        // Oye, m�quina de estados que es due�a de este script, cambia hacia el estado de Alerta.
        // ESTO NO ME DEJA PORQUE LA FSMRef NO ES DE TIPO EnemyFSM que es la que tiene el AlertState
        // FSMRef.ChangeState( FSMRef.AlertState ); 

        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if (enemyFSM != null)
        {
            // FSMRef.ChangeState(enemyFSM.AlertState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta l�nea es lo mismo pero sin el chequeo de seguridad.
        }

    }

}
