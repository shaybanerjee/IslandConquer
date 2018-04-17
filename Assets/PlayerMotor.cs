using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam; 

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 camera_rotation = Vector3.zero;



    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 vel)
    {
        velocity = vel;
        
    }

    public void Rotate(Vector3 rot)
    {
        rotation = rot;
    }

    public void RotateCamera(Vector3 rot)
    {
        camera_rotation = rot;
    }

    // Unity Method that runs every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity *  Time.fixedDeltaTime);
        }
    }

    private void PerformRotation()
    {
        // converts our rotation vector to a Quaternion
        // rb.rotation stores a Quaternion 
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            cam.transform.Rotate(-camera_rotation);
        }
    }
}

