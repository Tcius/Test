using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        
        if(other.gameObject.tag == ("Player"))
        {
            other.gameObject.GetComponent<FirstPersonController>().Hit(1);
            
        }
    }
}
