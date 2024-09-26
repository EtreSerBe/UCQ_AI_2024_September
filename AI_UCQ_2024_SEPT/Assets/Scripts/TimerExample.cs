using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerExample : MonoBehaviour
{
    // ¿Cómo hacemos una función que nos diga que ya pasó X tiempo cada X tiempo?
    // por ejemplo, que cada segundo nos imprima un log que diga: pasó un segundo.

    // Las maneras rudimentarias, son dos
    // 1) es guardar y acumular el tiempo transcurrido en una variable.
    // ¿cómo que el tiempo transcurrido?
    private float AccumulatedTime = 0.0f;

    [SerializeField]
    private float TimeInterval = 1.0f;

    // 2) La segunda manera rudimentaria consiste en usar un reloj de referencia, por ejemplo el reloj de la máquina.
    // usando ese reloj, tomamos la fecha de este instante y la comparamos contra una fecha anterior que guardamos.
    // y después checamos si la diferencia entre ellas es mayor o igual que el intervalo de tiempo que queremos.
    // Necesitamos una variable dónde guardar la fecha de un instante.
    private float Date = 0.0f;


    // 3) el tercer método, consiste en usar Coroutines de Unity.
    // busquen información sobre las corrutinas, son vida, son amor, les van a hacer la vida más fácil.
    // pero si les dan problemas, pues tienen las dos maneras rudimentarias que vimos arriba para resolver lo del examen.
    // en cualquier caso, ya saben que pueden preguntar(me).


    // Start is called before the first frame update
    void Start()
    {
        // le decimos que guarde la fecha al inicio del juego.
        Date = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime nos dice cuánto tiempo transcurrió entre este cuadro/update y el anterior.
        // vamos acumulando el tiempo transcurrido que nos da time.deltatime en la variable AccumulatedTime.
        AccumulatedTime += Time.deltaTime;

        // checamos si el tiempo acumulado (AccumulatedTime) ya tiene un valor mayor o igual al de TimeInterval.
        if (AccumulatedTime >= TimeInterval)
        {
            // si sí, imprimimos un log que nos diga: pasó TimeInterval de tiempo.
            // Debug.Log("Pasó " + TimeInterval + " cantidad de tiempo. Porque el valor de AccumulatedTime es: " + AccumulatedTime);

            // ¿Qué nos falta hacer aquí para que esta condición solo se cumpla una vez cada TimeInterval tiempo?
            // nos falta reiniciar el tiempo acumulado para poder iniciar otra vez a medir ese intervalo de tiempo que queremos medir.
            AccumulatedTime = 0.0f;

            // otra alternativa es únicamente restarle la cantidad contra la que se comparó.
            // AccumulatedTime -= TimeInterval;

            // cada una de estas opciones tiene sus pros y sus contras. Hay que saber evaluarlos según nuestras necesidades.
        }

        // Método 2)
        // usando ese reloj, tomamos la fecha de este instante y la comparamos contra una fecha anterior que guardamos.
        // y después checamos si la diferencia entre ellas es mayor o igual que el intervalo de tiempo que queremos.
        float CurrentDate = Time.time;
        if (CurrentDate - Date >= TimeInterval)
        {
            // Debug.Log("Pasó " + TimeInterval + " cantidad de tiempo medido por el reloj de la máquina.");
            Date = CurrentDate; // hay que remplazar la fecha guardada por la nueva fecha en que ya se cumplió este timer.
        }


    }
}
