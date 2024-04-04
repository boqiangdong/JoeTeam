using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Startbtn : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(this.btn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void btn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game");//跳到游戏场景
    }
}
