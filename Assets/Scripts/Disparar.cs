using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparar : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;

    public float shotForce = 2000;
    public float shotRate = 0.5f;
    public float Damage;

    public float shotRateTime = 0;

    public bool mouseIsDown = false;


    public void Start()
    {
        shotForce = 2000;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseIsDown = true;
            
        }
        if(mouseIsDown)
        {
            if(shotForce<3000)
            {
                shotForce += Time.deltaTime * 400;
            }
            Damage += Time.deltaTime;
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            
            mouseIsDown = false;
            
            GameObject newBullet;
            newBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
            newBullet.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * shotForce);
            shotRateTime = Time.time + shotRate;
            Destroy(newBullet, 2);

            Damage = 0;
            shotForce = 2000;
        }
    }
}
