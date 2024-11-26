using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BaseEnemy : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float HP;
    public float VisionRange;
    public GameObject PlayerRef;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        PlayerRef = GameObject.Find("Player");
        if (PlayerRef == null)
        {
            Debug.LogError("No Player was found.");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if(navMeshAgent == null )
        {
            Debug.LogError("No navMeshAgent assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsPlayerInRange(float range)
    {
        if (PlayerRef != null) 
        { 
            return Utilities.Utility.IsInsideRadius(PlayerRef.transform.position, transform.position, range);
        }
        return false;
    }
}
