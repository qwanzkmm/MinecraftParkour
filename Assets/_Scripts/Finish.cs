using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private AudioSource winSound;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            winSound.Play();
            Cursor.lockState = CursorLockMode.Confined;
            Invoke(nameof(SetNextScene), 0.5f);
            Debug.Log("Player finished!");
            //Debug.Log($"isLevel{SceneManager.GetActiveScene().buildIndex}Passed");
            YandexPlayerPrefs.SetBool($"isLevel{SceneManager.GetActiveScene().buildIndex}Passed", true);
        }
    }

    private void SetNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
    }
}
