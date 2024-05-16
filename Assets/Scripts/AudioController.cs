using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class AudioController : MonoBehaviour
{
   public AudioSource EngineIdel;
    public AudioSource RunningSource;
    public AudioSource StartSound;
    public float MinimumPitchValue = 0.5f;
    public float MaximumPitchValue = 3f;
    public float speedToPitchFactor = 0.01f;
    public CarMovement CarMovement;
    [Space(15)]
    public UnityAction StartEngineSound;
    public UnityAction StartIdelSound;
    public UnityAction StartRunningSound;

    bool isPlaying = false;
    void Start()
    {
        CarMovement CarMovement = GetComponent<CarMovement>();
        if(CarMovement != null)
        {
            StartEngineSound += StartEngine;
            
        }
        StartIdelSound += PlayEngineIdelSound;
        StartRunningSound += RunningEngineSound;
    }
    public void StartEngine()
    {
        
        StartSound.enabled = true;
    }
   public void PlayEngineIdelSound()
    {
        if(!isPlaying){
       StartCoroutine(PlayIdelSound());}
    }
    IEnumerator PlayIdelSound()
    {   
        isPlaying = true;
        Debug.Log("wait after 0.3f");
        yield return new WaitForSeconds(1f);
        EngineIdel.enabled = true;
        isPlaying = false;
    }
    public void RunningEngineSound()
    {
        float speedRatio = CarMovement.Car_SpeedKPH / CarMovement.MaximumSpeed;
        float targetPitch = Mathf.Lerp(MinimumPitchValue,MaximumPitchValue,speedRatio);
        RunningSource.pitch = targetPitch;
    }
    public void PlayHorn()
    {
       
    }
    public void StopHorn()
    {
        
    }
    private float CalculatePitch()
    {
        float pitch = CarMovement.Car_SpeedKPH / MaximumPitchValue + 1f;
        return Mathf.Clamp(pitch, MinimumPitchValue, MaximumPitchValue);
    }
}
