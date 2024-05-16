using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using VehiclePhysics;
using UnityEngine.Events;
public class CarMovement : MonoBehaviour
{
    public List<WheelCollider> FrontWheels;
    public List<WheelCollider> BackWheels;
    [Space(10)]
    public List<Transform> FrontWheelTransform;
    public List<Transform> BackWheelTransform;
    [Space(10)]
    public Rigidbody CarRigidbody;
    public Transform CenterOfMass;
    private WheelFrictionCurve Wheel_forwardFriction, Wheel_sidewaysFriction;
    public float frictionMultiplier = 3f;
    [Space(10)]
    public float MotorTroque = 400f;
    public float MaximumSpeed = 440f;
    public float MaxSteerAngle = 25f;
    public float steerAngle = 0;
    public float BrakeForce = 150f;
    private float Brakes = 0f;
    [Space(10)]
    public float handBrakeFrictionMultiplier = 2;
    private float handBrakeFriction = 0.05f;
    public float tempo;
    [Space(10)]
    public KeyCode CarStartKey = KeyCode.T;
    public KeyCode CarEndKey = KeyCode.Escape;
    public KeyCode CarHornKey = KeyCode.H;
    [Space(10)]
    [Header("Drift Settings")]
    public bool Set_Drift_Settings_Automatically = true;
    public float Forward_Extremium_Value_When_Drifting;
    public float Sideways_Extremium_Value_When_Drifting;
    float currSpeed;
    [Space(15)]
    public float BoostAmount = 10f;
    public KeyCode BoostKeyCode = KeyCode.B;
    private float BoostCoolDown = 10f;
    private bool isBoosting = false;
    private float currentBoostTime = 0f;
    public float Car_SpeedKPH;
    bool CarStarted;
    [Space(10)]
    [Header("Events")]
    public UnityEvent<bool> OnBrakeStateChange = new UnityEvent<bool>();
    public AudioController audioController;

    void Start()
    {
        CarRigidbody = GetComponent<Rigidbody>();
        CarRigidbody.centerOfMass = CenterOfMass.transform.localPosition;
        audioController = GetComponent<AudioController>();


    }
    void Update()
    {
        WheelTransform();
        HandBrake();
    }
    void FixedUpdate()
    {
        MoveCar();
    }
    public void MoveCar()
    {
        if (Input.GetKey(CarStartKey))
        {
            foreach (WheelCollider w in FrontWheels)
            {
                w.motorTorque = MotorTroque;
            }
            foreach (WheelCollider w in BackWheels)
            {
                w.motorTorque = MotorTroque;
            }
            CarStarted = true;
            audioController.StartEngineSound.Invoke();
            audioController.StartIdelSound.Invoke();
        }
        else
        {
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.motorTorque = 0;
            }
            foreach (WheelCollider wheel in BackWheels)
            {
                wheel.motorTorque = 0;
            }           
        }
        if (Car_SpeedKPH < MaximumSpeed && CarStarted)
        {
            
            foreach (WheelCollider wheel in BackWheels)
            {
                wheel.motorTorque = Input.GetAxis("Vertical") * ((MotorTroque * 5) / (BackWheels.Count() + FrontWheels.Count()));
            }
        }
        Debug.Log("speed"+Car_SpeedKPH);
        audioController.StartRunningSound.Invoke(); 
        steerAngle = Input.GetAxis("Horizontal") * MaxSteerAngle;
        steerAngle = Mathf.Clamp(steerAngle,-MaxSteerAngle,MaxSteerAngle);
        foreach (WheelCollider frontWheel in FrontWheels)
        {
            frontWheel.steerAngle = steerAngle;
        }
        

        Car_SpeedKPH = CarRigidbody.velocity.magnitude * 3.6f;
        Car_SpeedKPH = (int)Car_SpeedKPH;


        if (Input.GetKeyDown(BoostKeyCode) && CarStarted && !isBoosting && currentBoostTime <= 0)
        {
            StartCoroutine(ActivateBoost());
        }
        IEnumerator ActivateBoost()
        {
            isBoosting = true;
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.motorTorque += BoostAmount;
            }
            foreach (WheelCollider wheel in BackWheels)
            {
                wheel.motorTorque += BoostAmount;
            }
            yield return new WaitForSeconds(BoostCoolDown);
            isBoosting = false;
            currentBoostTime = BoostCoolDown;
        }
        WheelHit wheelHit;
        foreach (WheelCollider wheel in BackWheels)
        {
            wheel.GetGroundHit(out wheelHit);
            if (wheelHit.sidewaysSlip < 0)
                tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakeFrictionMultiplier);

            if (tempo < 0.5) tempo = 0.5f;

            if (wheelHit.sidewaysSlip > 0)
                tempo = (1 + Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakeFrictionMultiplier);

            if (tempo < 0.5) tempo = 0.5f;

            if (wheelHit.sidewaysSlip > .99f || wheelHit.sidewaysSlip < -.99f)
            {
                float velocity = 0;
                handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction, tempo * 3, ref velocity, 0.1f * Time.deltaTime);
            }
            else
            {
                handBrakeFriction = tempo;
            }
        }
         foreach(WheelCollider Wheel in FrontWheels){
            Wheel.GetGroundHit(out wheelHit);

            if(wheelHit.sidewaysSlip < 0 )	
                tempo = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > 0 )	
                tempo = (1 + Input.GetAxis("Horizontal") )* Mathf.Abs(wheelHit.sidewaysSlip *handBrakeFrictionMultiplier);

                if(tempo < 0.5) tempo = 0.5f;

            if(wheelHit.sidewaysSlip > .99f || wheelHit.sidewaysSlip < -.99f){
                //handBrakeFriction = tempo * 3;
                float velocity = 0;
                handBrakeFriction = Mathf.SmoothDamp(handBrakeFriction,tempo* 3,ref velocity ,0.1f * Time.deltaTime);
                }

            else{
                handBrakeFriction = tempo;
            }
        }
    }
    void WheelTransform()
    {
        Vector3 temp;
        for (int i = 0; i < FrontWheelTransform.Count; i++)
        {
            float rotationAmount = FrontWheels[i].rpm / 60 * 360 * Time.deltaTime;
            FrontWheelTransform[i].Rotate(Vector3.right, rotationAmount);

            temp = FrontWheelTransform[i].localEulerAngles;
            float desiredAngle = FrontWheels[i].steerAngle;
            desiredAngle = Mathf.Clamp(desiredAngle, -45f,45);
            //float currentAngle = Mathf.Lerp(temp.y,desiredAngle,Time.deltaTime * steerAngle);
            temp.y = desiredAngle;
            FrontWheelTransform[i].localEulerAngles = temp;
        }
        for (int i = 0; i < BackWheelTransform.Count; i++)
        {
            float rotationAmount = BackWheels[i].rpm / 60 * 360 * Time.deltaTime;
            BackWheelTransform[i].Rotate(Vector3.right, rotationAmount);
        }
       
    }
    void HandBrake()
    {
        if (Input.GetKey(KeyCode.Space) && CarStarted)
        {
            Brakes = BrakeForce;
            OnBrakeStateChange.Invoke(true);
            foreach (WheelCollider wheel in BackWheels)
            {
                wheel.brakeTorque = Brakes;
            }
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.brakeTorque = Brakes;
            }
            if (Set_Drift_Settings_Automatically)
            {
                foreach (WheelCollider wheel in BackWheels)
                {
                    Wheel_forwardFriction = wheel.forwardFriction;
                    Wheel_sidewaysFriction = wheel.sidewaysFriction;

                    Wheel_forwardFriction.extremumValue = Wheel_forwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                    Wheel_sidewaysFriction.extremumValue = Wheel_sidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                }
                foreach (WheelCollider wheel in FrontWheels)
                {
                    Wheel_forwardFriction = wheel.forwardFriction;
                    Wheel_sidewaysFriction = wheel.sidewaysFriction;

                    Wheel_forwardFriction.extremumValue = Wheel_forwardFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                    Wheel_sidewaysFriction.extremumValue = Wheel_sidewaysFriction.asymptoteValue = ((currSpeed * frictionMultiplier) / 300) + 1;
                }
            }
        }
        else
        {
            Brakes = 0f;
            OnBrakeStateChange.Invoke(false);
            foreach (WheelCollider wheel in BackWheels)
            {
                wheel.brakeTorque = 0f;
            }
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.brakeTorque = 0f;
            }
        }
    }
}