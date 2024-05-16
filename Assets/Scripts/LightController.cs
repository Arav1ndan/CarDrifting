using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LightController : MonoBehaviour
{
    public List<Light> BrakeLight;
    private UnityAction<bool> ToggleBrackAction;
    void Start()
    {
        CarMovement carMovement = FindObjectOfType<CarMovement>();
        if (carMovement != null)
        {
            ToggleBrackAction = new UnityAction<bool>(TurnONBrakeLight);
            carMovement.OnBrakeStateChange.AddListener(TurnONBrakeLight);
        }
    }

    void TurnONBrakeLight(bool isBracking)
    {
        for(int i=0;i<BrakeLight.Count;i++){
            BrakeLight[i].enabled = isBracking;
        }      
        if (isBracking)
        {
            Debug.Log("light is on");
        }
        else
        {
            Debug.Log("light is off");
        }
    }
}
