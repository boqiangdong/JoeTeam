using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestProgressBar : MonoBehaviour
{

    public Text TimeLabel;//时间显示UI
    public float sumTime;//总时间 单位 秒

    private void Start()
    {
        StartCoroutine("startCountDown");
    }

    public IEnumerator startCountDown()
    {
        while (sumTime >= 0)
        {
            sumTime--;//总时间 单位 秒，倒计时
            TimeLabel.text = "Time:" + sumTime;//时间显示UI
            if (sumTime == 0)
            {
                Debug.Log("gameOver");
                yield break;//停止 协程
            }
            else if (sumTime > 0)
            {
                yield return new WaitForSeconds(1);// 每次 自减1，等待 1 秒
            }
        }
    }
}

