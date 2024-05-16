using UnityEngine;

/*
    CarControllerLite class is the main class for controlling the car
*/
public class CarControllerLite : MonoBehaviour {
    public enum CarType
    {
        FrontWheelDrive,   // Motor torque just applies to the front wheels
        RearWheelDrive,    // Motor torque just applies to the rear wheels
        FourWheelDrive     // Motor torque applies to the all wheels
    }

    //************** Drag each wheel collider to the corresponding variable *********************
    public WheelCollider Wheel_Collider_Front_Left;
    public WheelCollider Wheel_Collider_Front_Right;
    public WheelCollider Wheel_Collider_Rear_Left;
    public WheelCollider Wheel_Collider_Rear_Right;
    //*******************************************************************************************
    //************** Drag each wheel mesh to the corresponding variable *************************
    public GameObject Wheel_Mesh_Front_Left;
    public GameObject Wheel_Mesh_Front_Right;
    public GameObject Wheel_Mesh_Rear_Left;
    public GameObject Wheel_Mesh_Rear_Right;
    //*******************************************************************************************

    public float maxMotorTorque;                        // Maximum torque the motor can apply to wheels
    public float maxSteeringAngle = 20;                 // Maximum steering angle the wheels can have    
    public float maxSpeed;                              // Car maximum speed
    public float brakePower;                            // Maximum brake power
    public Transform CenterOfMass;                      // Center of mass of the car
    public CarType carType = CarType.FourWheelDrive;    // Set car type here
    float carSpeed;                                     // The car speed in meter per second 
    float carSpeedConverted;                            // The car speed in kilometer per hour
    float motorTorque;                                  // Current Motor torque
    float tireAngle;                                    // Current steer angle
    float vertical = 0;                                 // The vertical input
    float horizontal = 0;                               // The horizontal input   
    bool hBrake = false;                                // If handbrake button(Spacebar) pressed it becomes true
    Rigidbody carRigidbody;                             // Rigidbody of the car
    AudioSource engineAudioSource;                      // The engine audio source
    float engineAudioPitch = 1.4f;                      // The engine audio pitch
    public Transform brakeLightLeft;                    // Drag the left brake light mesh here
    public Transform brakeLightRight;                   // Drag the right brake light mesh here
    Material brakeLightLeftMat;                         // Reference to the left brake light material
    Material brakeLightRightMat;                        // Reference to the right brake light material
    Color brakeColor = new Color32(180, 0, 10, 0);      // The emission color of the brake lights
                                 


    void Start() {

        brakeLightLeftMat = brakeLightLeft.GetComponent<Renderer>().material;
        brakeLightRightMat = brakeLightRight.GetComponent<Renderer>().material;

        brakeLightLeftMat.EnableKeyword("_EMISSION");
        brakeLightRightMat.EnableKeyword("_EMISSION");

        carRigidbody = GetComponent<Rigidbody>();

        carRigidbody.centerOfMass = CenterOfMass.localPosition;

        engineAudioSource = GetComponent<AudioSource>();         
        engineAudioSource.Play();
    }
    void Update() 
    {
        horizontal = Input.GetAxis("Horizontal");                  
        vertical = Input.GetAxisRaw("Vertical");                    

        tireAngle = maxSteeringAngle * horizontal;                  // Calculate the front tires angles

        carSpeed = carRigidbody.velocity.magnitude;                 // Calculate the car speed in meter/second                 

        carSpeedConverted = Mathf.Round(carSpeed*3.6f);             // Convert the car speed from meter/second to kilometer/hour
       // carSpeedRounded = Mathf.Round(carSpeed * 2.237f);         // Use this formula for mile/hour

        Wheel_Collider_Front_Left.steerAngle = tireAngle;           // Set front wheel colliders steer angles
        Wheel_Collider_Front_Right.steerAngle = tireAngle;

        if (Input.GetKey(KeyCode.Space))
            hBrake = true;
        else
            hBrake = false;

        if (hBrake)
        {
            // If handbrake button pressed run this part

            motorTorque = 0;
            Wheel_Collider_Rear_Left.brakeTorque = brakePower;
            Wheel_Collider_Rear_Right.brakeTorque = brakePower;

            brakeLightLeftMat.SetColor("_EmissionColor", brakeColor);
            brakeLightRightMat.SetColor("_EmissionColor", brakeColor);
        }
        else
        {
            // Else if vertical is pressed change brake light color and set brakeTorques to 0
            if (vertical >= 0 )
            {
                brakeLightLeftMat.SetColor("_EmissionColor", Color.black);
                brakeLightRightMat.SetColor("_EmissionColor", Color.black);
            }
            else
            {
                brakeLightLeftMat.SetColor("_EmissionColor", brakeColor);
                brakeLightRightMat.SetColor("_EmissionColor", brakeColor);
            }

            Wheel_Collider_Front_Left.brakeTorque = 0;
            Wheel_Collider_Front_Right.brakeTorque = 0;
            Wheel_Collider_Rear_Left.brakeTorque = 0;
            Wheel_Collider_Rear_Right.brakeTorque = 0;

            // Check if car speed has exceeded from maxSpeed
            if (carSpeedConverted < maxSpeed)
                motorTorque = maxMotorTorque * vertical;
            else
                motorTorque = 0;
        }

        // Set the motorTorques based on the carType
        if (carType == CarType.FrontWheelDrive)
        {
            Wheel_Collider_Front_Left.motorTorque = motorTorque;
            Wheel_Collider_Front_Right.motorTorque = motorTorque;
        }
        else if (carType == CarType.RearWheelDrive)
        {
            Wheel_Collider_Rear_Left.motorTorque = motorTorque;
            Wheel_Collider_Rear_Right.motorTorque = motorTorque;
        }
        else if (carType == CarType.FourWheelDrive)
        {
            Wheel_Collider_Front_Left.motorTorque = motorTorque;
            Wheel_Collider_Front_Right.motorTorque = motorTorque;
            Wheel_Collider_Rear_Left.motorTorque = motorTorque;
            Wheel_Collider_Rear_Right.motorTorque = motorTorque;
        }

        // Calculate the engine sound
        engineSound();

        // Set the wheel meshes to the correct position and rotation based on their wheel collider position and rotation
        ApplyTransformToWheels();

    }

    // Set the wheel meshes to the correct position and rotation based on their wheel collider position and rotation
    public void ApplyTransformToWheels()
    {
        Vector3 position;
        Quaternion rotation;

        Wheel_Collider_Front_Left.GetWorldPose(out position, out rotation);
        Wheel_Mesh_Front_Left.transform.position = position;
        Wheel_Mesh_Front_Left.transform.rotation = rotation;

        Wheel_Collider_Front_Right.GetWorldPose(out position, out rotation);
        Wheel_Mesh_Front_Right.transform.position = position;
        Wheel_Mesh_Front_Right.transform.rotation = rotation;

        Wheel_Collider_Rear_Left.GetWorldPose(out position, out rotation);
        Wheel_Mesh_Rear_Left.transform.position = position;
        Wheel_Mesh_Rear_Left.transform.rotation = rotation;

        Wheel_Collider_Rear_Right.GetWorldPose(out position, out rotation);
        Wheel_Mesh_Rear_Right.transform.position = position;
        Wheel_Mesh_Rear_Right.transform.rotation = rotation;
    }

    // Calculate the engine sound based on the car speed by changing the audio pitch
    private void engineSound()
    {
        float y = 0.4f;
        float z = 0.1f;

        engineAudioSource.volume = 0.8f;

        if (vertical == 0 && carSpeedConverted > 30)
        {
            engineAudioSource.volume = 0.5f;

            if (engineAudioPitch >= 0.35f)
            {
                engineAudioPitch -= Time.deltaTime * 0.1f;
            }
        }
        else if (carSpeedConverted <= 5)
        {
            engineAudioPitch = 0.15f;
        }
        else if (vertical != 0 && carSpeedConverted > 5 && carSpeedConverted <= 45)
        {

            float x = ((carSpeedConverted - 5) / 40) * y;
            engineAudioPitch = z + x;
        }
        else if (vertical != 0 && carSpeedConverted > 45 && carSpeedConverted <= 85)
        {
            float x = ((carSpeedConverted - 45) / 40) * y;
            engineAudioPitch = z + x + 0.2f;
        }
        else if (vertical != 0 && carSpeedConverted > 85 && carSpeedConverted <= 115)
        {
            float x = ((carSpeedConverted - 85) / 30) * y;
            engineAudioPitch = z + x + 0.3f;
        }
        else if (vertical != 0 && carSpeedConverted > 115 && carSpeedConverted <= 145)
        {
            float x = ((carSpeedConverted - 115) / 30) * y;
            engineAudioPitch = z + x + 0.4f;
        }
        else if (vertical != 0 && carSpeedConverted > 145 && carSpeedConverted <= 165)
        {
            float x = ((carSpeedConverted - 125) / 20) * y;
            engineAudioPitch = z + x + 0.6f;
        }
        else if (vertical != 0 && carSpeedConverted > 165)
        {
            engineAudioPitch = 1.5f ;
        }

        engineAudioSource.pitch = engineAudioPitch;
    }
   
}

