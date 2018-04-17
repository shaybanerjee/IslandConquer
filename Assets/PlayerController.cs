using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float sensitivity = 3f; 

    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // 3D Velocity Vector
        // between -1 and 1 
        float xMov = Input.GetAxisRaw("Horizontal");
        // between -1 amd 1
        float zMov = Input.GetAxisRaw("Vertical");

        // transforming our vectors to determine which
        // direction the character is going 
        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVirtical = transform.forward * zMov;

        // only use the normalized value as a direction 
        // this ensures uniform movement
        Vector3 velocity = (movHorizontal + movVirtical).normalized * speed;

        // Apply movement
        motor.Move(velocity);

        // Calculate Rotation as 3D vector
        // rotation around the y-axis so we look at movement of mouse in x-axis
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * sensitivity;

        motor.Rotate(rotation);

        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 cameraRotation = new Vector3(xRot, 0f, 0f) * sensitivity;

        motor.RotateCamera(cameraRotation);
    }
}
