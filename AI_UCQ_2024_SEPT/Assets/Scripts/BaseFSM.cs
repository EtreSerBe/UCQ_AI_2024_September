using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFSM : MonoBehaviour
{
    // la m�quina de estados tiene que tener estados, �no?

    // Debe de tener una variable que indique cu�l es el estado actual de la FSM.
    private BaseState currentState;
    // otra que indique cu�l es el estado inicial.
    // private BaseState initialState;

    // Start is called before the first frame update
    public virtual void Start()
    {
        // 0) es obtener el estado inicial independientemente de qu� clase de FSM estamos usando.
        // 1) al iniciar la m�quina de estados, debe entrar al estado Inicial y asignarlo como el estado actual.
        currentState = GetInitialState();
     
        if (currentState == null)
        {
            Debug.LogWarning("Peligro, el estado inicial no es v�lido.");
            return;
        }

        // 1.1) Y como se asign� como estado actual, entonces llamamos su funci�n Enter().
        currentState.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            // 2) En el update de la FSM �nicamente se ejecuta el Update del estado actual.
            currentState.OnUpdate();
        }
    }

    public void ChangeState(BaseState newState)
    {
        // 3) La FSM debe de poder cambiar el estado actual por otro estado, y asignar ese otro como el nuevo estado actual.
        // 3.1) Primero sale del estado actual, por lo tanto ejecuta su funci�n Exit().
        currentState.OnExit();
        // 3.2) Despu�s asigna al nuevo estado como el actual.
        currentState = newState;
        // 3.3) Finalmente, ejecuta la funci�n Enter del nuevo estado actual.
        currentState.OnEnter();
    }

    public virtual BaseState GetInitialState()
    {
        // Regresa null para que cause error porque la funci�n de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return null;
    }
}
