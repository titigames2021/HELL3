using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float life = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (life <= 0)
        {
            gameObject.SetActive(false);
        }


        
    }


    public void PunchHit()
    {

        life--;
        Debug.Log("golpeado");

    }



}
