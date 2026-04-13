using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterLogic : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float Speed = 2.5f;
    [SerializeField] GameObject Helicopter;

    [Header("Firing")]
    [SerializeField] GameObject Bullets;
    [SerializeField] GameObject firingpoint;
    [SerializeField] float BulletSpeed = 2f;
    [SerializeField] float fireRate = 0.15f;
    [SerializeField] float bulletsPerBurst = 7;
    [SerializeField] float BulletSpread = 5f;

    // Fixed horizontal borders
    private const float maxBorderX = 10f;

    // State machine
    private enum State { Entering, Moving, AtBorder }
    private State currentState = State.Entering;

    private bool movingRight = true;
    private bool isInsideBorders = false;

    // Border-triggered burst firing
    private float bulletsFiredInBurst = 0f;
    private float nextFireTime = 0f;
    private float spreadangle = 0f;

    private Transform truck;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            truck = player.transform;

        // Determine direction based on which side of the border we spawned on
        // Always move toward center
        if (transform.position.x < -maxBorderX)
            movingRight = true;   // spawned left of border → move right
        else if (transform.position.x > maxBorderX)
            movingRight = false;  // spawned right of border → move left
        // else already inside — keep default (movingRight = true)
        UpdateVisualRotation();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Entering:
                HandleEntering();
                break;

            case State.Moving:
                HandleMoving();
                break;

            case State.AtBorder:
                HandleAtBorder();
                break;
        }
    }

    // ── ENTERING: move inward until inside the borders ─────────────────────
    private void HandleEntering()
    {
        MoveInCurrentDirection();

        bool insideRight = transform.position.x <= maxBorderX;
        bool insideLeft  = transform.position.x >= -maxBorderX;

        if (insideRight && insideLeft)
        {
            isInsideBorders = true;
            currentState = State.Moving;
        }
    }

    // ── MOVING: patrol until a border is reached ────────────────────────────
    private void HandleMoving()
    {
        MoveInCurrentDirection();

        if (movingRight && transform.position.x >= maxBorderX)
        {
            // Clamp to border, flip, and begin border pause
            Vector3 pos = transform.position;
            pos.x = maxBorderX;
            transform.position = pos;

            movingRight = false;
            UpdateVisualRotation();
            BeginBorderBurst();
        }
        else if (!movingRight && transform.position.x <= -maxBorderX)
        {
            Vector3 pos = transform.position;
            pos.x = -maxBorderX;
            transform.position = pos;

            movingRight = true;
            UpdateVisualRotation();
            BeginBorderBurst();
        }
    }

    // ── AT BORDER: fire burst then resume moving ────────────────────────────
    private void HandleAtBorder()
    {
        if (bulletsFiredInBurst < bulletsPerBurst)
        {
            if (Time.time >= nextFireTime)
            {
                FiringMechanism();
                nextFireTime = Time.time + fireRate;
                bulletsFiredInBurst++;
            }
        }
        else
        {
            // Burst complete — resume movement
            bulletsFiredInBurst = 0f;
            currentState = State.Moving;
        }
    }

    // ── HELPERS ─────────────────────────────────────────────────────────────
    private void MoveInCurrentDirection()
    {
        Vector3 dir = movingRight ? Vector3.right : Vector3.left;
        transform.Translate(dir * Speed * Time.deltaTime, Space.World);
    }

    private void UpdateVisualRotation()
    {
        if (Helicopter == null) return;
        Helicopter.transform.rotation = movingRight
            ? Quaternion.Euler(0f, 0f, 0f)
            : Quaternion.Euler(0f, 180f, 0f);
    }

    private void BeginBorderBurst()
    {
        bulletsFiredInBurst = 0f;
        nextFireTime = Time.time;          // fire first bullet immediately
        currentState = State.AtBorder;
    }

    public void FiringMechanism()
    {
        if (truck == null || Bullets == null || firingpoint == null) return;

        Vector2 direction       = (truck.position - firingpoint.transform.position);
        Vector2 firingdirection = direction.normalized;

        float angle        = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spreadangle        = Random.Range(-BulletSpread, BulletSpread);
        Quaternion spread  = Quaternion.Euler(0f, 0f, spreadangle);
        Vector2 spreadDir  = spread * firingdirection;

        GameObject projectile = Instantiate(Bullets, firingpoint.transform.position, Quaternion.Euler(180f, 0f, -angle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = spreadDir * BulletSpeed;
    }
}
