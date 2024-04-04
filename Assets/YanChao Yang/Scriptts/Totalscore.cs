using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Totalscore : MonoBehaviour
{
    private Text text;
    float integer;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
        float integer = Eventmanagement.Instance.number;
        text.text = string.Format("time :{0}", integer);//赋值给计时ui
        Debug.Log(integer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
