using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroyer : MonoBehaviour
{

    public float lifeSecondsLeft;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    private void FixedUpdate()
    {
        lifeSecondsLeft -= Time.deltaTime;
        if (lifeSecondsLeft < 0 )
        {
            Destroy( gameObject );
        }
    }
}
