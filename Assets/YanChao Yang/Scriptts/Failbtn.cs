using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Failbtn : MonoBehaviour
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");//按下跳到开始场景
    }
}
