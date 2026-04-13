using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletLogic : MonoBehaviour
{
    [SerializeField] GameObject explosive;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy Plane"))
        {
            //GameObject explosion = Instantiate(explosive, transform.position, Quaternion.identity);
            //ExplosionDestroyer destroyer = explosion.AddComponent<ExplosionDestroyer>();
          //  destroyer.lifeSecondsLeft = 4;
            Destroy(gameObject);

        }
    }
}
