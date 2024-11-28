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

    // Lo que hace esta lista es decirnos cuáles han sido los subestados que se han ejecutado y su orden.
    private List<MeleeSubstate> _substateHistory = new List<MeleeSubstate>();

    // Si el player está lejos, me acerco hasta estar en X rango.

    // tenemos que tener una referencia al player (u objetivo) al que se va a acercar.
    // Posibilidad 1, que la referencia la tenga la EnemyFSM
    // 
    // este enemigo debe tener una forma de moverse hacia su objetivo.
    // componente NavMeshAgent 
    // tiene que poder detectar si una posición X está dentro de un rango Y.


    // Si el player está dentro de X rango, hago UN ataque básico, 
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
        Debug.Log("On exit del basic attack, aquí liberaría los recursos que solamente necesita este subestado.");
    }

    IEnumerator Buildup(float BuildupTime, IEnumerator CoroutineName)
    {
        if(((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Build-Up " + CoroutineName.ToString());
        yield return new WaitForSeconds(BuildupTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé Build-Up " + CoroutineName.ToString());
        StartCoroutine(CoroutineName);
    }

    IEnumerator AreaAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Area attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.AreaAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de área número: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks/2.0f, TimeBetweenAttacks/2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé area attack");
        StartCoroutine(Cooldown(bossEnemy.AreaAttackCooldownTime, "Melee Area attack"));
    }

    IEnumerator DashAttack()
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Dash attack");

        BossEnemy bossEnemy = (BossEnemy)owner;
        int counter = 0;

        int NumAttacks = Random.Range(2, 5);
        float TimeBetweenAttacks = bossEnemy.DashAttackTime / NumAttacks;

        while (counter < NumAttacks)
        {
            if (((BossEnemy)owner).EnableDebug)
                Debug.Log("Atacando de dash número: " + counter);

            yield return new WaitForSeconds(TimeBetweenAttacks + Random.Range(-TimeBetweenAttacks / 2.0f, TimeBetweenAttacks / 2));
            counter++;
        }

        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé Dash attack");
        StartCoroutine(Cooldown(bossEnemy.DashCooldownTime, "Melee Dash"));
    }

    IEnumerator Cooldown(float CooldownTime, string ActionOnCooldown)
    {
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Empecé Cooldown de " + ActionOnCooldown);
        yield return new WaitForSeconds(CooldownTime);
        if (((BossEnemy)owner).EnableDebug)
            Debug.Log("Terminé cooldown de " + ActionOnCooldown);

        GoToSelectionState();
    }

    // En árboles de decisiones esto se conoce como GoToParent o GoToRoot.
    void GoToSelectionState()
    {
        // Le decimos que nos guarde este subestado como el más reciente que se tuvo.
        _substateHistory.Add(_meleeSubstate);

        switch (_meleeSubstate)
        {
            case MeleeSubstate.BasicAttack:
                // Hacer el OnExit() de Basic Attack.
                OnExitBasicAttack(); // nomás imprime, pero es para dar la idea de cómo se llamaría el OnExit de cada subestado.
                break;
            case MeleeSubstate.AreaAttack:
                // Hacer el OnExit()
                Debug.Log("On Exit de Area Attack.");
                break;
            case MeleeSubstate.Dash:
                // Hacer el OnExit()
                Debug.Log("On Exit de Dash.");
                break;
            case MeleeSubstate.Ultimate:
                // Hacer el OnExit()
                Debug.Log("On Exit de Ultimate.");
                break;
            default:
                // No hace nada.
                Debug.LogWarning("Entré a GoToSelectionState() sin un Subestado válido, fue el subestado: " + _meleeSubstate.ToString());
                break;
        }    

        // Ponemos que se vaya al subestado de selección, donde elegiremos el subestado que sigue, según lo que dicte el diseño.
        _meleeSubstate = MeleeSubstate.SubstateSelection;
        // Ponemos esta bandera a False, para que el nuevo subestado que entre haga su "OnEnter" según lo necesite.
        SubstateEntered = false;
    }

    // En el OnStart de este estado MeleeState, no necesitamos checar en qué subestado vamos a estar, 
    // porque siempre va a ser en BasicAttack, porque eso nos especifica el diagrama del diseño.

    public override void OnUpdate()
    {
        base.OnUpdate();

        BossEnemy bossOwner = (BossEnemy)owner;

        // En el OnUpdate es donde sí vamos a diferenciar en qué subestado (_meleeSubstate) estamos.
        // este switch tal cual va a ser nuestra "FSM" para los subestados.
        switch (_meleeSubstate)
        {
            case MeleeSubstate.SubstateSelection:
                Debug.Log("Entré al substate selection.");

                // NOTA: Hay que tener cuidado si quieren poner que el subestado seleccionador
                // sea el subestado inicial, por esta consideración de este IF.
                if(_substateHistory.Count == 0)
                {
                    Debug.LogWarning("No hay acciones previas al tratar de seleccionar subestado en Melee. Podría causar problemas, manéjese con cuidado.");
                }

                // ¿Cómo podemos saber cuál fue el último Subestado activo antes de SubstateSelection?
                // El _SubstateHistory nos da cuáles fueron las acciones pasadas realizadas y qué orden.

                // Si el último subestado fue el Ultimate. Cambia al Estado (no subestado) de AtaqueADistancia.
                if (_substateHistory[_substateHistory.Count - 1] == MeleeSubstate.Ultimate)
                {
                    enemyFSMRef.ChangeState(enemyFSMRef.AlertState); // NOTA, NO PONER ALERT, PONER EL OTRO ESTADO DE ATAQUE A DISTANCIA.
                    return;                     
                }
               
                // En mi diseño, el Ultimate se hace cuando ya se realizó al menos uno de cada uno de los demás subestados.
                if (_substateHistory.Contains(MeleeSubstate.BasicAttack) &&
                    _substateHistory.Contains(MeleeSubstate.AreaAttack) &&
                    _substateHistory.Contains(MeleeSubstate.Dash))
                {
                    // Entonces hacemos el Ultimate.
                    _meleeSubstate = MeleeSubstate.Ultimate;
                    return;
                }


                // Si la última acción fue un Dash o un AreaAttack, entonces nos vamos a Basic Attack, según el diagrama establece.
                if(_substateHistory[_substateHistory.Count-1] == MeleeSubstate.AreaAttack ||
                    _substateHistory[_substateHistory.Count - 1] == MeleeSubstate.Dash)
                {
                    _meleeSubstate = MeleeSubstate.BasicAttack;
                    return;
                }

                // Checamos posible transición:
                // Haber hecho ataque básico 3 veces Y
                // Si el player está dentro del rango X donde sí le alcanzaría(o casi) a pegar este ataque de área.
                if (owner.IsPlayerInRange(bossOwner.AreaAttackRange)  ||
                    _substateHistory.LastIndexOf(MeleeSubstate.Dash) > _substateHistory.LastIndexOf(MeleeSubstate.AreaAttack))  // le añadí que si l
                {
                    // si se cumplen estas dos condiciones, entonces cambiamos al subestado de ataque de área (AreaAttack).
                    _meleeSubstate = MeleeSubstate.AreaAttack;
                    return;
                }
                else // según el diagrama, si no está en el rango del ataque de área, entonces hace el Dash.
                {
                    _meleeSubstate = MeleeSubstate.Dash;
                    return;
                }

                break;

            case MeleeSubstate.BasicAttack:
                /*
                 Si el player está lejos, me acerco hasta estar en X rango.
                    Si el player está dentro de X rango, hago UN ataque básico,
                de los 3 disponibles, cada uno tiene 33% de probabilidad.

                 */

                if(!SubstateEntered)
                {
                    SubstateEntered = true;
                    // Le dices que persiga al player que quiere atacar.
                    owner.navMeshAgent.SetDestination(owner.PlayerRef.transform.position);
                    // Cuando entramos a este subestado, reseteamos el contador de ataques básicos que ha realizado este enemigo.
                    BasicAttackCounter = 0;
                }

                if(owner.IsPlayerInRange(bossOwner.BasicAttackRange))
                {
                    Debug.Log("Puedo atacar básico");
                    BasicAttackCounter++;
                }

                if (BasicAttackCounter >= bossOwner.NumberOfBasicAttacksBeforeExit)
                    GoToSelectionState();

                break;
            case MeleeSubstate.Dash:
                if (!SubstateEntered)  // este if(!SubstateEntered) es básicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podría llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);

                    // Iniciar el build-up del ataque.
                    StartCoroutine(Buildup(bossOwner.DashBuildupTime, DashAttack()));
                    // return; pero no es necesario por cómo está especificado mi subestado de dash.
                }
                break;
            case MeleeSubstate.AreaAttack:
                // Cuando entra a este subestado NO se mueve hacia el jugador (al menos la del juego de Hades)
                if (!SubstateEntered)  // este if(!SubstateEntered) es básicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podría llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);

                    // Iniciar el build-up del ataque.
                    StartCoroutine(Buildup(bossOwner.AreaAttackBuildupTime, AreaAttack()));
                    // return; pero no es necesario por cómo está especificado mi subestado de AreaAttack.
                }

                // Hace build-up
                // Hace ataque
                // Hace cooldown
                // Y al terminar cooldown puede cambiar de estado/ subestado

                break;
            case MeleeSubstate.Ultimate:
                // Cuando entra a este subestado NO se mueve hacia el jugador (al menos la del juego de Hades)
                if (!SubstateEntered)  // este if(!SubstateEntered) es básicamente el OnEnter de cada subestado.
                {
                    SubstateEntered = true;
                    // Le dices que ahorita no se mueva. (Boss de hades, en sus casos podría llegar a variar)
                    owner.navMeshAgent.SetDestination(owner.transform.position);


                    Debug.Log("HACIENDO ULTIMATE!");
                    // Iniciar el build-up del ataque.
                    // StartCoroutine(Buildup(bossOwner.AreaAttackBuildupTime, AreaAttack()));
                    // return; pero no es necesario por cómo está especificado mi subestado de AreaAttack.
                }
                break;
            default:
                break;
        }

        // Oye, máquina de estados que es dueña de este script, cambia hacia el estado de Alerta.
        // ESTO NO ME DEJA PORQUE LA FSMRef NO ES DE TIPO EnemyFSM que es la que tiene el AlertState
        // FSMRef.ChangeState( FSMRef.AlertState ); 

        EnemyFSM enemyFSM = (EnemyFSM)(FSMRef);
        if (enemyFSM != null)
        {
            // FSMRef.ChangeState(enemyFSM.AlertState);
            // FSMRef.ChangeState((EnemyFSM)(FSMRef).AlertState); // esta línea es lo mismo pero sin el chequeo de seguridad.
        }

    }

}
