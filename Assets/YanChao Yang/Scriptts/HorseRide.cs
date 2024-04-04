using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HorseRide : MonoBehaviour
{
    public float movespeed = 18;
    public float turnspeed = 30;
    
    Rigidbody rigid;
    Animator horse;
    public Vector3 jumpForce;
    public Transform ridingAnimal;
    AudioSource audiosource;

    public GameObject goldcoinObj;

    public float totalTime1;//累加计时分数
    public bool accelerate;//加速判断
    private Text text;//计时分数
    public GameObject time;//计时分数物体
    float integer;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        horse = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        text = time.GetComponent<Text>();//找到text
        StartCoroutine(Fasttiming());//调用协程
    }

    
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            movespeed = 40;//按下加速数值的同时改变布尔值为true
            accelerate = true;
            Debug.Log(accelerate + "减速！！！！！！！！！！！！！！！！！！！！！！！！");
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            movespeed = 18;//按下减速数值的同时改变布尔值为false
            accelerate = false;
            Debug.Log(accelerate + "减速！！！！！！！！！！！！！！！！！！！！！！！！");
        }
       Move();
       Turn();
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    void Jump()
    {
        float speed = 5;
        rigid.isKinematic = false;
        rigid.velocity = new Vector3(0, 0, speed*Time.deltaTime);
        rigid.AddForce(jumpForce);

        //音乐
        audiosource.Play();
    }
    void Move()
    {
        transform.Translate(new Vector3(0, 0, movespeed * Time.deltaTime));
    }

    void Turn()
    {
        float x = Input.GetAxis("Horizontal");

        if(Mathf.Abs(x) > 0.5f) 
        {
            transform.Rotate(0, x* Time.deltaTime * turnspeed, 0);
        }
        else
        {
            Quaternion mid = Quaternion.identity;
            transform.rotation = Quaternion.Slerp(transform.rotation, mid, 0.2f); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("碰到了"+LayerMask.LayerToName(other.gameObject.layer));
        if(other.gameObject.layer == LayerMask.NameToLayer("Tree")
        ||other.gameObject.layer == LayerMask.NameToLayer("Horse") )
        {
            Debug.Log("GameOver");
        }
        if (other.tag == "tree")//碰到tag叫tree的物体就跳到结束场景
        {
            Eventmanagement.Instance.number = integer;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Start 1");
        }
        if (other.tag== "Goldcoin")
        {
            totalTime1++;
            text.text = string.Format("time :{0}", totalTime1);
            Destroy(other.gameObject);
        }
    }
   
    private IEnumerator Fasttiming()//协程
    {
        while (true)
        {
            if (accelerate == true)
            {
                Debug.Log(totalTime1 + "进入加速!1111111111111111111111111111111111111111111111");
                totalTime1 += 10* Time.deltaTime;//累加当前时间
                integer = Mathf.Round(totalTime1);////四舍五入，用于去点当前时间的小数点
                text.text = string.Format("time :{0}", integer);//赋值给计时ui
            }
            if (accelerate == false)
            {
                totalTime1 += Time.deltaTime;
                integer = Mathf.Round(totalTime1);
                text.text = string.Format("time :{0}", integer);
            }
            yield return null;
        }

    }
}
