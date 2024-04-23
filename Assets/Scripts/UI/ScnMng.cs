using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScnMng : NetworkBehaviour
{
   
    private void Awake()
    {
        DontDestroyOnLoad( target: this);       
    }

    public void loadScene()
    {

        SceneManager.LoadScene(1);
    }
       
    public void GoBack()
    {
        DontDestroyOnLoad( target: this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}