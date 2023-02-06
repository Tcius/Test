using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoverAgente : MonoBehaviour
{
    public NavMeshAgent agent;
       public GameObject goal;
       
       public float x;
       public float y;
        
       void Start () 
       {
          NavMeshAgent agent = GetComponent<NavMeshAgent>();
          x = Random.Range(-25, 25);
          y = Random.Range(-25, 25);

          goal.transform.position = new Vector3(x, 1, y);

          agent.destination = goal.transform.position; 
       }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == ("goal"))
            {
                                x = Random.Range(-25, 25);
                y = Random.Range(-25, 25);

                goal.transform.position = new Vector3(x, 1, y);

                agent.destination = goal.transform.position; 
            }
        }
}
