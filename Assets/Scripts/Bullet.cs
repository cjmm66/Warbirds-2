using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {// Destroy the bullet when it collides with any object
        Destroy(gameObject);
        //Destroy the object it collides with if it has the tag "Enemy

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }}
}
