using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void SetScene(int index)
    {
        SceneManager.LoadScene(index);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
    
    public void SetNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
