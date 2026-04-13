using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Rigidbody2D rb;
    public float torqueAmount = 0.15f;  // Adjust this value to control the rotation speed
    public ContactFilter2D groundContactFilter;
    [SerializeField] GameObject Explosion1Prefab;

    public BomberController parentBomber; // As is
    
   
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.AddTorque(torqueAmount);
    }


    void Update()
    {

    }
    
    private bool raycastGround(out RaycastHit2D result)
    {

        RaycastHit2D[] results = new RaycastHit2D[20];
        Physics2D.Raycast(transform.position, Vector2.down, groundContactFilter, results);
        for (int resultIndex = 0; resultIndex < results.Length; resultIndex++)
        {
            if (results[resultIndex].transform.gameObject.isStatic)
            {
                result = results[resultIndex];
                return true;
            }
        }
        result = new RaycastHit2D();
        return false;
    }
 
    void FixedUpdate()
    {

        RaycastHit2D raycastResult;
        bool success = raycastGround(out raycastResult);
        if (success)
        {
            float distance = raycastResult.distance;

            //float fallTimeEstimated = distance * 1;
            float alpha = 0.015f / Mathf.Min(distance, 1);
            float targetRotation = -90f; // nose pointing straight down
            float newRotation = Mathf.LerpAngle(rb.rotation, targetRotation, alpha);
            rb.SetRotation(newRotation);
            
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        BombExplosion();
       
    }
    void BombExplosion()
    {
        GameObject explosion = Instantiate(Explosion1Prefab,transform.position, Quaternion.identity);
       

       ExplosionDestroyer destroyer = explosion.AddComponent<ExplosionDestroyer>();
       destroyer.lifeSecondsLeft = 4;
     Destroy(gameObject);
     
    }

}