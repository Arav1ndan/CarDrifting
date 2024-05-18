using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int world;
    public int stage;
    public CarMovement carMovement;
    void Start()
    {
        
    }
    void Update()
    {   
        carMovement = GetComponent<CarMovement>();
        if (carMovement == null){
            //Debug.Log("in game manager also the car movemet can't access it");
        }
    }
    
    public void NewGame()
    {
        LoadLevel(1,1);
    }
    public void NextLevel()
    {
        if(world == 1 && stage == 10)
        {
            LoadLevel(world + 1, 1);
        }
        LoadLevel(world,stage + 1);
    }
    public void LoadLevel(int world,int stage)
    {
        this.world = world;
        this.stage = stage;
        SceneManager.LoadScene($"{world}{stage}");
    }
    public void ResetLevel()
    {
        Debug.Log("currently not working");
    }
    public void GameOver()
    {
        NewGame();
    }
}
