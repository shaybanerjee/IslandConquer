using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float thrusterForce = 1000f;

    private Animator animator; 

    [Header("Joint Options (Spring Settings):")]
    [SerializeField]
    private JointDriveMode jMode = JointDriveMode.Position;
    [SerializeField]
    private float jSpring = 20f;
    [SerializeField]
    private float jMaxForce = 40f; 

    [SerializeField]
    private float sensitivity = 3f; 

    private PlayerMotor motor;
    private ConfigurableJoint cj;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        cj = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointConfig(jSpring);
    }

    void Update()
    {
        // 3D Velocity Vector
        // between -1 and 1 
        float xMov = Input.GetAxis("Horizontal");
        // between -1 amd 1
        float zMov = Input.GetAxis("Vertical");

        // transforming our vectors to determine which
        // direction the character is going 
        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVirtical = transform.forward * zMov;

        // only use the normalized value as a direction 
        // this ensures uniform movement
        Vector3 velocity = (movHorizontal + movVirtical).normalized * speed;

        animator.SetFloat("ForwardVelocity", zMov);

        // Apply movement
        motor.Move(velocity);

        // Calculate Rotation as 3D vector
        // rotation around the y-axis so we look at movement of mouse in x-axis
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * sensitivity;

        motor.Rotate(rotation);

        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * sensitivity;

        motor.RotateCamera(cameraRotationX);

        // Apply thrust to character
        Vector3 tForce = Vector3.zero;
        if (Input.GetButton("Jump"))
        {
            tForce = Vector3.up * thrusterForce;
            SetJointConfig(0f);
        }
        else
        {
            SetJointConfig(jSpring);
        }
        // if jump was not inputed by player it will just apply a zero vector for thrust
        motor.ApplyThruster(tForce);
    }

    private void SetJointConfig(float jointSpring)
    {
        // JointDrive contains information about spring, damper, max force and more
        cj.yDrive = new JointDrive { mode = jMode, positionSpring = jointSpring, maximumForce = jMaxForce};
    }
}
