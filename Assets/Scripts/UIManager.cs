using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    float speed;
    public CarMovement carMovement;
    void Start()
    {
      
    }
    void Update()
    {
        carMovement = FindObjectOfType<CarMovement>();
        if (carMovement == null)
        {
            Debug.LogWarning("CarMovement script is not attached");
        }
        else
        {
            SpeedOmeter();
        }
    }
    void SpeedOmeter()
    {
        if (carMovement != null)
        {
            speed = carMovement.GetCarSpeed();
            Debug.Log("working");
        }
        else
        {
            Debug.LogWarning("carmovement script is not attached");
        }
        speedText.text = speed.ToString();
    }
}
