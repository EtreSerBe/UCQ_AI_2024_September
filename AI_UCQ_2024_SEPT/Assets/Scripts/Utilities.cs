using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class Utility
    {
        public static bool IsInsideRadius(Vector3 inTargetPos, Vector3 inSpherePos, float inSphereRadius, bool PrintResult = false)
        {
            // Para saber si un punto en el espacio (llamado TargetPos) está dentro o fuera de una esfera en el espacio, 
            // hacemos un vector que inicia en el origen de la esfera y que termine en TargetPos (punta menos cola)
            Vector3 AgentPositionToTarget = inTargetPos - inSpherePos;
            // Y luego obtenemos la magnitud de dicho vector
            float VectorMagnitude = AgentPositionToTarget.magnitude;
            // y finalmente comparamos esa magnitud contra el radio de la esfera.
            // (usamos operadores de comparación: == , !=  , > < <= >=...)
            // Si el radio es mayor o igual que la magnitud de ese vector, entonces TargetPos está dentro de la esfera,
            if (inSphereRadius >= VectorMagnitude)
            {
                if(PrintResult)
                    Debug.Log("Sí está en el radio");
                return true;
            }
            // de lo contrario, está fuera de la esfera
            else
            {
                return false;
            }
        }

        // Función de Cono de Visión.

    }
}
