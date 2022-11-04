using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sr;
    public Sprite[] runSprite;
    public Sprite climbSprite;
    private int spriteIndex;

    private Rigidbody2D rb;
    private Vector2 direction;
    public float moveSpeed = 3f;
    public float jumpStrength = 5f;
    public Collider2D[] result;
    public Collider2D collider;
    private bool grounded;
    private bool climbing;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<Collider2D>();
        result = new Collider2D[4];
        sr = gameObject.GetComponent<SpriteRenderer>();

    }

    private void Update()
    {
        CheckCollision();

        if (climbing)
        {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
        }
        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jumpStrength;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (direction.x > 0) { transform.eulerAngles = Vector3.zero; }
        else if (direction.x < 0) { transform.eulerAngles = new Vector3(0f, 180f, 0f); }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * Time.fixedDeltaTime);
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.x /= 2f;
        size.y += 0.1f;
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, result);
        for (int i = 0; i < amount; i++)
        {
            GameObject hit = result[i].gameObject;
            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < transform.position.y - 0.2f;

                Physics2D.IgnoreCollision(collider, result[i], !grounded);
            }

            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && enabled)
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelFailed();
        }
        else if (collision.gameObject.CompareTag("Objective") && enabled)
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();
        }
    }

    private void AnimateSprite()
    {
        if (climbing)
        {
            sr.sprite = climbSprite;
        }
        else if (direction.x != 0)
        {
            spriteIndex++;
            if (spriteIndex >= runSprite.Length)
            {
                spriteIndex = 0;
            }
            sr.sprite = runSprite[spriteIndex];
        }
    }
}
