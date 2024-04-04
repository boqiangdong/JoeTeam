using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class NaviScript : MonoBehaviour
{
    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
    public void exitGame(){
           /*将状态设置false才能退出游戏*/
        // UnityEditor.EditorApplication.isPlaying = false;
           Application.Quit();
    }

}
