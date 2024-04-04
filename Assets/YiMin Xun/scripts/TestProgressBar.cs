using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestProgressBar : MonoBehaviour
{

    public Text TimeLabel;//ʱ����ʾUI
    public float sumTime;//��ʱ�� ��λ ��

    private void Start()
    {
        StartCoroutine("startCountDown");
    }

    public IEnumerator startCountDown()
    {
        while (sumTime >= 0)
        {
            sumTime--;//��ʱ�� ��λ �룬����ʱ
            TimeLabel.text = "Time:" + sumTime;//ʱ����ʾUI
            if (sumTime == 0)
            {
                Debug.Log("gameOver");
                yield break;//ֹͣ Э��
            }
            else if (sumTime > 0)
            {
                yield return new WaitForSeconds(1);// ÿ�� �Լ�1���ȴ� 1 ��
            }
        }
    }
}

