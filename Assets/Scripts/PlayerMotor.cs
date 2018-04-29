using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float CamRotLimit = 85f; 

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    // Rotation on the x-axis, looking up and down
    private float camera_rotationX = 0f;
    private float currest_cam_rotX = 0f;
    private Vector3 thrusterForce = Vector3.zero;
    



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

    public void RotateCamera(float rotX)
    {
        camera_rotationX = rotX;
    }

    public void ApplyThruster(Vector3 tForce)
    {
        thrusterForce = tForce;
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
        if (thrusterForce != Vector3.zero)
        {
            // ForceMode.Acceleration disregards mass of rigid body
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);        
        }
    }

    private void PerformRotation()
    {
        // converts our rotation vector to a Quaternion
        // rb.rotation stores a Quaternion 
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currest_cam_rotX -= camera_rotationX;
            currest_cam_rotX = Mathf.Clamp(currest_cam_rotX, -CamRotLimit, CamRotLimit);
            cam.transform.localEulerAngles = new Vector3(currest_cam_rotX, 0f, 0f);
        }
    }
}

