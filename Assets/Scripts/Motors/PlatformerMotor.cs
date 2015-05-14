using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class PlatformerMotor : MonoBehaviour, IMoveable
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;

    private Rigidbody2D body;
    private float inputMagnitude;
    private bool isRequestingJump;
    private bool canJump;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 velocity = Vector2.zero;
        if (isRequestingJump && canJump)
        {
            canJump = false;
            velocity.y = jumpSpeed;
        }

        velocity.x = inputMagnitude * speed;
        body.velocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        canJump = true;
    }

    public void Move(Vector3 input)
    {
        isRequestingJump = !Mathf.Approximately(input.y, 0.0f);
        inputMagnitude = input.x;
    }
}

}   // namespace JamUtilities