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
    // 死亡标志
    public bool isDie = false;
    // 死亡粒子特效
    public GameObject dieEffect;
    // 吃食物音效
    public AudioClip eatClip;
    // 死亡音效
    public AudioClip dieClip;
    // 画布对象
    private Transform canvas;

    void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("SnakePart/" + PlayerPrefs.GetString("sh", "sh02"));
        bodySprites[0] = Resources.Load<Sprite>("SnakePart/" + PlayerPrefs.GetString("sb01", "sb0201"));
        bodySprites[1] = Resources.Load<Sprite>("SnakePart/" + PlayerPrefs.GetString("sb02", "sb0202"));
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
        if (Input.GetKeyDown(KeyCode.Space) && !MainUIControl.Instance.isPause && !isDie)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity - delta_vel);
        }
        // 空格弹起恢复正常速度
        if (Input.GetKeyUp(KeyCode.Space) && !MainUIControl.Instance.isPause && !isDie)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
        // 按下W键，向上移动
        if (Input.GetKey(KeyCode.W) && y != -step && !MainUIControl.Instance.isPause && !isDie)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            x = 0;
            y = step;
        }
        // 按下S键，向下移动
        if (Input.GetKey(KeyCode.S) && y != step && !MainUIControl.Instance.isPause && !isDie)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0;
            y = -step;
        }
        // 按下A键，向左移动
        if (Input.GetKey(KeyCode.A) && x != step && !MainUIControl.Instance.isPause && !isDie)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            x = -step;
            y = 0;
        }
        // 按下D键，向右移动
        if (Input.GetKey(KeyCode.D) && x != -step && !MainUIControl.Instance.isPause && !isDie)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step;
            y = 0;
        }
    }

    // 碰撞进入
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 与食物碰撞
        if (collision.tag == "Food")
        {
            AudioSource.PlayClipAtPoint(eatClip, Vector3.zero);
            Destroy(collision.gameObject);
            MainUIControl.Instance.UpdateUI();
            Grow();
            FoodMaker.Instance.MakeFood();
        }
        // 与奖励物品碰撞
        else if(collision.tag == "Reward")
        {
            AudioSource.PlayClipAtPoint(eatClip, Vector3.zero);
            Destroy(collision.gameObject);
            MainUIControl.Instance.UpdateUI(Random.Range(5, 15) * 10, 1);
            Grow();
        }
        // 与身体碰撞
        else if(collision.tag == "Body")
        {
            Die();
        }
        // 与边界碰撞
        else
        {
            if (MainUIControl.Instance.hasBorder)
            {
                Die();
            }
            else
            {
                string name = collision.gameObject.name;
                switch (name)
                {
                    case "Up":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 30, transform.localPosition.z);
                        break;
                    case "Down":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 30, transform.localPosition.z);
                        break;
                    case "Left":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 270, transform.localPosition.y, transform.localPosition.z);
                        break;
                    case "Right":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 330, transform.localPosition.y, transform.localPosition.z);
                        break;
                }
            }
        }
    }

    // 死亡
    void Die()
    {
        AudioSource.PlayClipAtPoint(dieClip, Vector3.zero);
        CancelInvoke();
        isDie = true;
        Instantiate(dieEffect);

        // 存储游戏数据
        PlayerPrefs.SetInt("last_len", MainUIControl.Instance.length);
        PlayerPrefs.SetInt("last_score", MainUIControl.Instance.score);
        if (PlayerPrefs.GetInt("best_score", 0) < MainUIControl.Instance.score)
        {
            PlayerPrefs.SetInt("best_len", MainUIControl.Instance.length);
            PlayerPrefs.SetInt("best_score", MainUIControl.Instance.score);
        }

        StartCoroutine(GameOver(1.5f));
    }

    IEnumerator GameOver(float t)
    {
        yield return new WaitForSeconds(t);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
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
