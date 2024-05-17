using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class AudioController : MonoBehaviour
{
   public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    public AudioSource reverseSound;
    public float reverseMaxVolume;
    public float reverseMaxPitch;
    public AudioSource idleSound;
    public float idleMaxVolume;
    public float speedRatio;
    private float revLimiter;
    public float LimiterSound = 1f;
    public float LimiterFrequency = 3f;
    public float LimiterEngage = 0.8f;
    public bool isEngineRunning = false;

    public AudioSource startingSound;
    private CarMovement carMove;
    void Start()
    {
        carMove = GetComponent<CarMovement>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }
    void Update()
    {
        float SpeedSign = 0;
        if(carMove)
        {
            SpeedSign = Mathf.Sign(carMove.Car_SpeedKPH);
            speedRatio = Mathf.Abs(carMove.Car_SpeedKPH);
        }
        if(speedRatio > LimiterEngage)
        {
            revLimiter = (Mathf.Sin(Time.time * LimiterFrequency) + 1f) * LimiterSound * (speedRatio - LimiterEngage);
        }
         if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            if (SpeedSign > 0)
            {
                reverseSound.volume = 0;
                runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
                runningSound.pitch = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
            else
            {
                runningSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                reverseSound.pitch = Mathf.Lerp(reverseSound.pitch, Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
            }
        }
        else {
            idleSound.volume = 0;
            runningSound.volume = 0;
        }
    }
    public IEnumerator StartEngine()
    {
        startingSound.Play();
        carMove.Car_SpeedKPH = 1;
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
        carMove.Car_SpeedKPH = 2;
    }
}

