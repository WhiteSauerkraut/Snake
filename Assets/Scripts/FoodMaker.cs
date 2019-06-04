using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 食物随机生成
/// </summary>
public class FoodMaker : MonoBehaviour
{
    // 单例
    private static FoodMaker _instance;
    public static FoodMaker Instance
    {
        get
        {
            return _instance;
        }
    }
    // x方向限制
    public int x_limit = 20;
    // 左右限制偏移量
    public int x_offset = 9;
    // y方向限制
    public int y_limit = 11;
    // 食物预制体
    public GameObject food_prefab;
    // 奖励预制体
    public GameObject reward_prefab;
    // 食物图片数组
    public Sprite[] food_sprites;
    // 食物存放容器
    public Transform foodHolder;


    void Start()
    {
        foodHolder = GameObject.Find("Canvas/FoodHolder").transform;
        _instance = this;
        MakeFood();
    }

    // 随机位置生成食物
    public void MakeFood()
    {
        int index = Random.Range(0, food_sprites.Length);
        GameObject food = Instantiate(food_prefab);
        food.GetComponent<Image>().sprite = food_sprites[index];
        food.transform.SetParent(foodHolder, false);
        int x = Random.Range(-x_limit + x_offset, x_limit);
        int y = Random.Range(-y_limit, y_limit);
        food.transform.localPosition = new Vector3(x * 30, y * 30, 0);

        // 20%概率生成奖励
        if(Random.Range(0,100) < 20)
        {
            GameObject reward = Instantiate(reward_prefab);
            reward.transform.SetParent(foodHolder, false);
            x = Random.Range(-x_limit + x_offset, x_limit);
            y = Random.Range(-y_limit, y_limit);
            reward.transform.localPosition = new Vector3(x * 30, y * 30, 0);
        }
    }
}
