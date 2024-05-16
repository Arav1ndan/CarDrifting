using UnityEngine;
using UnityEngine.Events;
public class Audio_Manager : MonoBehaviour
{
    public AudioClip starting;
    public AudioClip Idel;
    public AudioClip Running;
    public AudioClip MaxRev;
    

    public UnityEvent OnCarStart;
    public UnityEvent OnCarIdel;
    public UnityEvent OnCarAccelerate;
    public UnityEvent OnCarMax;
    private AudioSource audioSource;
    void Awake()
    {
        Debug.Log("Awake method called");

        // Check if UnityEvents are not null
        Debug.Log("OnCarStart event: " + (OnCarStart != null));
        Debug.Log("OnCarIdel event: " + (OnCarIdel != null));
        Debug.Log("OnCarAccelerate event: " + (OnCarAccelerate != null));
        Debug.Log("OnCarMax event: " + (OnCarMax != null));

        audioSource = GetComponent<AudioSource>();
        OnCarStart.AddListener(() => PlaySound(starting));
        OnCarIdel.AddListener(() => PlaySound(Idel));
        OnCarAccelerate.AddListener(() => PlaySound(Running));
        OnCarMax.AddListener(() => PlaySound(MaxRev));

    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        
    }
}
