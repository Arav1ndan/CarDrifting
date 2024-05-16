using UnityEngine;
using VehiclePhysics;

public class carControler : MonoBehaviour
{
    public enum CarType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;
    public Transform WheelFLtrans;
    public Transform WheelFRtrans;
    public Transform WheelRLtrans;
    public Transform WheelRRtrans;
    //Audio sourece and speed section
    private float speed;
    //public Audio_Manager audio_Manager;
    private float previousSpeed;
    //-----------------------------------
    public CarType carType = CarType.AllWheelDrive;
    public Vector3 eulertest;
    public float maxFwdSpeed = 3000;
    public float maxBwdSpeed = 1000f;
    float gravity = 9.8f;
    private bool braked = false;
    private float maxBrakeTorque = 1500;
    private Rigidbody rb;
    public Transform centreofmass;
    public float maxTorque = 3000;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreofmass.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (!braked)
        {
            WheelFL.brakeTorque = 0;
            WheelFR.brakeTorque = 0;
            WheelRL.brakeTorque = 0;
            WheelRR.brakeTorque = 0;
        }
        //float Vertical = Input.GetAxis("Vertical");

        if (carType == CarType.FrontWheelDrive)
        {
            WheelFL.motorTorque = maxTorque * Input.GetAxis("Vertical");
            WheelFR.motorTorque = maxTorque * Input.GetAxis("Vertical");

        }
        else if (carType == CarType.RearWheelDrive)
        {
            WheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
            WheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
        }
        else if (carType == CarType.AllWheelDrive)
        {
            WheelFL.motorTorque = maxTorque * Input.GetAxis("Vertical");
            WheelFR.motorTorque = maxTorque * Input.GetAxis("Vertical");
            WheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
            WheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
            //Debug.Log("speed "+ WheelFL.motorTorque);
        }
        WheelFL.steerAngle = 30 * (Input.GetAxis("Horizontal"));
        WheelFR.steerAngle = 30 * Input.GetAxis("Horizontal");
    }
    
    void Update()
    {
        HandBrake();

        //for tyre rotate
        WheelFLtrans.Rotate(WheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelFRtrans.Rotate(WheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelRLtrans.Rotate(WheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        WheelRRtrans.Rotate(WheelRL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        //changing tyre direction
        Vector3 temp = WheelFLtrans.localEulerAngles;
        Vector3 temp1 = WheelFRtrans.localEulerAngles;
        temp.y = WheelFL.steerAngle - (WheelFLtrans.localEulerAngles.z);
        WheelFLtrans.localEulerAngles = temp;
        temp1.y = WheelFR.steerAngle - WheelFRtrans.localEulerAngles.z;
        WheelFRtrans.localEulerAngles = temp1;
        eulertest = WheelFLtrans.localEulerAngles;

        speed = (rb.velocity.magnitude * 2.23693629f);
        //Debug.Log("speed is" + speed);
        float currentSpeed = speed;
        //Debug.Log("speed is" + currentSpeed);
        /*if (currentSpeed > 0.001234 && previousSpeed <= 0.02346)
        {
            audio_Manager.OnCarStart.Invoke();
            audio_Manager.OnCarIdel.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Idel));
            audio_Manager.OnCarAccelerate.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Running));
            audio_Manager.OnCarMax.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.MaxRev));
        }
        else if (currentSpeed > previousSpeed)
        {
            audio_Manager.OnCarAccelerate.Invoke();
            audio_Manager.OnCarStart.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.starting));
            audio_Manager.OnCarIdel.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Idel));
            audio_Manager.OnCarMax.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.MaxRev));
        }
        else if (currentSpeed == maxTorque)
        {
            audio_Manager.OnCarMax.Invoke();
            audio_Manager.OnCarStart.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.starting));
            audio_Manager.OnCarIdel.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Idel));
            audio_Manager.OnCarAccelerate.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Running));
        }
        else
        {
            audio_Manager.OnCarIdel.Invoke();
            audio_Manager.OnCarStart.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.starting));
            audio_Manager.OnCarAccelerate.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.Running));
            audio_Manager.OnCarMax.RemoveListener(() => audio_Manager.PlaySound(audio_Manager.MaxRev));
        }*/
        previousSpeed = currentSpeed;
    }
    void HandBrake()
    {
        //Debug.Log("brakes " + braked);
        if (Input.GetButton("Jump"))
        {
            braked = true;
        }
        else
        {
            braked = false;
        }
        if (braked)
        {
            WheelRL.brakeTorque = maxBrakeTorque * 30;//0000;
            WheelRR.brakeTorque = maxBrakeTorque * 30;//0000;
            WheelRL.motorTorque = 0;
            WheelRR.motorTorque = 0;
        }
    }
}
