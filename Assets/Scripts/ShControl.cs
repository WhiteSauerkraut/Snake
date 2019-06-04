using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Snake Head控制代码
/// </summary>
public class ShControl : MonoBehaviour
{
    // 移动速度
    public float velocity = 0.35f;
    // 加速大小
    public float delta_vel = 0.2f;
    // 移动步长
    public int step = 30;
    // x,y方向移动距离
    private int x;
    private int y;
    // 蛇身列表
    public List<Transform> bodyList = new List<Transform>();
    // 蛇身预制体
    public GameObject bodyPrefab;
    // 蛇身图片
    public Sprite[] bodySprites = new Sprite[2];
    // 画布对象
    private Transform canvas;

    void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
    }

    void Start()
    {
        InvokeRepeating("Move", 0, velocity);
        x = step;
        y = 0;
    }

    void Update()
    {
        // 空格按下加速
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity - delta_vel);
        }
        // 空格弹起恢复正常速度
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
        // 按下W键，向上移动
        if (Input.GetKey(KeyCode.W) && y != -step)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            x = 0;
            y = step;
        }
        // 按下S键，向下移动
        if (Input.GetKey(KeyCode.S) && y != step)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0;
            y = -step;
        }
        // 按下A键，向左移动
        if (Input.GetKey(KeyCode.A) && x != step)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
            x = -step;
            y = 0;
        }
        // 按下D键，向右移动
        if (Input.GetKey(KeyCode.D) && x != -step)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step;
            y = 0;
        }
    }

    // 碰撞进入
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            Destroy(collision.gameObject);
            Grow();
            FoodMaker.Instance.MakeFood();
        }
    }

    // 移动函数
    void Move()
    {
        Vector3 head_pos = transform.localPosition;
        transform.localPosition = new Vector3(head_pos.x + x, head_pos.y + y, head_pos.z);

        if (bodyList.Count > 0)
        {
            // 从后往前开始移动蛇身
            for (int i = bodyList.Count - 2; i >= 0; i--)                                          
            {
                bodyList[i + 1].localPosition = bodyList[i].localPosition;                         
            }
            // 第一个蛇身移动到蛇头移动前的位置
            bodyList[0].localPosition = head_pos;                                                    
        }
    }

    // 生成蛇身(在屏幕外生成)
    void Grow()
    {
        int index = bodyList.Count % 2;
        GameObject body = Instantiate(bodyPrefab, new Vector3(2000, 2000, 0), Quaternion.identity);
        body.GetComponent<Image>().sprite = bodySprites[index];
        body.transform.SetParent(canvas, false);
        bodyList.Add(body.transform);
    }
}
