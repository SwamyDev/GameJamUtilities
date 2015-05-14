using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class VelocityMotor : MonoBehaviour, IMoveable
{
    [SerializeField]
    private float speed;

    private Rigidbody rigidbody3D;
    private Rigidbody2D rigidbody2D;

    private Vector3 direction;
    private float inputMagnitude;

    void Awake()
    {
        rigidbody3D = GetComponent<Rigidbody>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rigidbody2D != null)
            Update2D();

        if (rigidbody3D != null)
            Update3D();
    }

    private void Update3D()
    {
        rigidbody3D.velocity = direction * speed * inputMagnitude;
    }

    private void Update2D()
    {
        rigidbody2D.velocity = direction * speed * inputMagnitude;
    }

    public void Move(Vector3 input)
    {
        inputMagnitude = input.magnitude;
        direction = input.normalized;
    }
}

}   // namespace JamUtilities