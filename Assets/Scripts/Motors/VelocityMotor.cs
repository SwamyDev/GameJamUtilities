using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class VelocityMotor : MonoBehaviour, IMoveable
{
    [SerializeField]
    private float speed;

    private Rigidbody body3D;
    private Rigidbody2D body2D;

    private Vector3 direction;
    private float inputMagnitude;

    void Awake()
    {
        body3D = GetComponent<Rigidbody>();
        body2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (body2D != null)
            Update2D();

        if (body3D != null)
            Update3D();
    }

    private void Update3D()
    {
        body3D.velocity = direction * speed * inputMagnitude;
    }

    private void Update2D()
    {
        body2D.velocity = direction * speed * inputMagnitude;
    }

    public void Move(Vector3 input)
    {
        inputMagnitude = input.magnitude;
        direction = input.normalized;
    }
}

}   // namespace JamUtilities