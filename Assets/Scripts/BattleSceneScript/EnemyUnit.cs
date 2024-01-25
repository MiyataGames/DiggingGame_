using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class EnemyBases
{
    public List<EnemyBase> enemyBases = new List<EnemyBase>();
}

public class EnemyUnit : MonoBehaviour
{
    [SerializeField]
    private List<EnemyBases> enemyBasiesList;
    [SerializeField] int enemyBaseNumber;
    [SerializeField]
    GameObject enemyCanvas;
    [SerializeField]
    private GameObject[] enemyPos9;
    private List<Enemy> enemies;

    public List<Enemy> Enemies { get => enemies; }
    int[] debugPositionIndexs = { 7, 8, 4 };
    Dictionary<Enemy, int> enemyKindNums;// エネミーごとの数
    public void SetUp(int lowLevel, int highLevel)
    {
        int enemyNum = 3; /*= Random.Range(1,3);*/
        // レベル帯1-3のダンジョン
        int enemyKindNum;
        enemies = new List<Enemy>();
        // 順番にソート
        Dictionary<Enemy, int> agiEnemyDic = new Dictionary<Enemy, int>();
        //enemyModels = new GameObject[enemyNum];
        for (int i = 0; i < enemyNum; i++)
        {
            enemyKindNum = UnityEngine.Random.Range(0, enemyBasiesList[enemyBaseNumber].enemyBases.Count);
            Enemy enemy = new Enemy(enemyBasiesList[enemyBaseNumber].enemyBases[enemyKindNum], UnityEngine.Random.Range(lowLevel, highLevel));
            Debug.Log(enemy.EnemyBase.EnemyName);
            agiEnemyDic.Add(enemy, enemy.EnemyBase.Agi);
        }

        int j = 0;
        // 敵の数が3体だったら

        List<int> positions = new List<int>();
        int positionIndex;
        foreach (var enemy in agiEnemyDic.OrderByDescending(c => c.Value))
        {
            /*
            positionIndex = Random.Range(0, enemyPos9.Length);
            // 敵を生成する位置をきめる
            // 前に生成した敵と同じ位置だったら
            while (positions.Any(value => value == positionIndex))
            {
                // ふりなおす
                positionIndex = Random.Range(0, enemyPos9.Length - 1);

            }*/
            //positions.Add(positionIndex);
            enemies.Add(enemy.Key);
            //enemies[j].positionIndex = positionIndex;
            enemies[j].positionIndex = debugPositionIndexs[j];
            // 敵のモデルを生成
            enemies[j].EnemyPrefab = Instantiate(enemies[j].EnemyBase.EnemyPrefab, enemyPos9[enemies[j].positionIndex].transform.position, Quaternion.identity, enemyPos9[enemies[j].positionIndex].transform);
            // アニメーターをいれる
            enemies[j].EnemyAnimator = enemies[j].EnemyPrefab.GetComponent<Animator>();
            // EnemyUIのオブジェクトをみつける
            //enemies[j].EnemyUIObject = enemyPos3[j].transform.Find("Child/EnemyCanvas").gameObject;
            // EnemyUIをEnemyPosの子オブジェクトに生成
            Vector3 uiPosition = new Vector3(enemyPos9[enemies[j].positionIndex].transform.position.x, enemyPos9[enemies[j].positionIndex].transform.position.y + 2.5f, enemyPos9[enemies[j].positionIndex].transform.position.z + 1);
            enemies[j].EnemyUI = Instantiate(enemyCanvas, uiPosition, Quaternion.identity, enemies[j].EnemyPrefab.transform).gameObject.GetComponent<BattleEnemyUI>();
            Debug.Log(enemies[j].EnemyUI);
            j++;
        }

        for(int i = 0;i < enemyNum; i++)
        {
            if (enemies[i].Counted == false &&  enemies.Count(enemy => enemy.EnemyBase.EnemyID == enemies[i].EnemyBase.EnemyID) > 0)
            {
                List<Enemy> countEnemies = enemies.FindAll(enemy => enemy.EnemyBase.EnemyID == enemies[i].EnemyBase.EnemyID);
                for(int k = 0; k < countEnemies.Count; k++)
                {
                    countEnemies[k].Counted = true;
                    countEnemies[k].EnemyBattleName = countEnemies[k].EnemyBase.EnemyName + (k + 1).ToString();
                }
            }
        }

        for(int i = 0;i < enemies.Count; i++)
        {
            // EnemyUIのセットアップ
            enemies[i].EnemyUI.SetEnemyData(enemies[i]);
            // EnemyUIのパネルを表示する
            enemies[i].EnemyUI.ActivateUIPanel();
        }
    }
}