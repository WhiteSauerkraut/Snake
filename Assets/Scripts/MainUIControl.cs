using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主界面UI控制
/// </summary>
public class MainUIControl : MonoBehaviour
{
    private static MainUIControl _instance;
    public static MainUIControl Instance
    {
        get
        {
            return _instance;
        }
    }

    // 暂停标志
    public bool isPause = false;
    // 分数
    public int score = 0;
    // 长度
    public int length = 0;

    // 边界标志
    public bool hasBorder = true;

    public Sprite[] pauseSprites;

    private Text msgText;
    private Text scoreText;
    private Text lengthText;

    private Button btn_pause;
    private Button btn_home;

    private Image pauseImage;
    private Image bgImage;
    private Color tempColor;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        msgText = transform.Find("ControlPanel/Msg").GetComponent<Text>();
        scoreText = transform.Find("ControlPanel/Score").GetComponent<Text>();
        lengthText = transform.Find("ControlPanel/Length").GetComponent<Text>();

        pauseImage = transform.Find("ControlPanel/Pause/Image").GetComponent<Image>();
        bgImage = transform.Find("Bg").GetComponent<Image>();

        btn_pause = transform.Find("ControlPanel/Pause").GetComponent<Button>();
        btn_home = transform.Find("ControlPanel/Home").GetComponent<Button>();

        btn_pause.onClick.AddListener(Pause);
        btn_home.onClick.AddListener(Home);

        // 隐藏边界
        if (PlayerPrefs.GetInt("border", 1) == 0)
        {
            hasBorder = false;
            foreach (Transform t in bgImage.gameObject.transform)
            {
                t.gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }

    void Update()
    {
        if(score > 200 && score <= 400)
        {
            ColorUtility.TryParseHtmlString("#CCEEFFFF", out tempColor);
            bgImage.color = tempColor;
            msgText.text = "阶段" + 2;
        }
        else if(score > 400 && score <= 600)
        {
            ColorUtility.TryParseHtmlString("#CCFFDBFF", out tempColor);
            bgImage.color = tempColor;
            msgText.text = "阶段" + 3;
        }
        else if (score > 600 && score <= 800)
        {
            ColorUtility.TryParseHtmlString("#EBFFCCFF", out tempColor);
            bgImage.color = tempColor;
            msgText.text = "阶段" + 4;
        }
        else if (score > 800 && score <= 1000)
        {
            ColorUtility.TryParseHtmlString("#FFF3CCFF", out tempColor);
            bgImage.color = tempColor;
            msgText.text = "阶段" + 5;
        }
        else if(score > 1000)
        {
            ColorUtility.TryParseHtmlString("#FFDACCFF", out tempColor);
            bgImage.color = tempColor;
            msgText.text = "无尽阶段";
        }
    }

    // 更新UI显示
    public void UpdateUI(int s = 5, int l = 1)
    {
        score += s;
        length += l;
        scoreText.text = "得分\n" + score;
        lengthText.text = "长度\n" + length;
    }

    // 暂停键
    public void Pause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0;
            pauseImage.sprite = pauseSprites[1];
        }
        else
        {
            Time.timeScale = 1;
            pauseImage.sprite = pauseSprites[0];
        }
    }

    // 菜单键
    public void Home()
    {
        isPause = false;
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
