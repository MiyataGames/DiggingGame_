using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField]
    private EnemyBase[] enemyBasies;
    [SerializeField]
    GameObject enemyCanvas;

    [SerializeField]
    private GameObject[] enemyPos9;

    private Enemy[] enemies;

    public Enemy[] Enemies { get => enemies; }

    public void SetUp(int lowLevel, int highLevel)
    {
        int enemyNum = 3; /*= Random.Range(1,3);*/
        // レベル帯1-3のダンジョン
        int enemyKindNum;

        enemies = new Enemy[enemyNum];
        // 順番にソート
        Dictionary<Enemy, int> agiEnemyDic = new Dictionary<Enemy, int>();
        //enemyModels = new GameObject[enemyNum];
        for (int i = 0; i < enemyNum; i++)
        {
            enemyKindNum = Random.Range(0, enemyBasies.Length);
            Enemy enemy = new Enemy(enemyBasies[enemyKindNum], Random.Range(lowLevel, highLevel));
            Debug.Log(enemy.EnemyBase.EnemyName);
            agiEnemyDic.Add(enemy, enemy.EnemyBase.Agi);
        }

        int j = 0;
        // 敵の数が3体だったら
        if (enemyNum == 3)
        {
            List<int> positions = new List<int>();
            int positionIndex;
            foreach (var enemy in agiEnemyDic.OrderByDescending(c => c.Value))
            {
                positionIndex = Random.Range(0, enemyPos9.Length);
                // 敵を生成する位置をきめる
                // 前に生成した敵と同じ位置だったら
                while (positions.Any(value => value == positionIndex))
                {
                    // ふりなおす
                    positionIndex = Random.Range(0, enemyPos9.Length - 1);

                }
                positions.Add(positionIndex);
                enemies[j] = enemy.Key;
                // 敵のモデルを生成
                enemies[j].EnemyPrefab = Instantiate(enemies[j].EnemyBase.EnemyPrefab, enemyPos9[positionIndex].transform.position, Quaternion.identity, enemyPos9[positionIndex].transform);
                // アニメーターをいれる
                // enemies[j].EnemyAnimator = enemies[j].EnemyModel.GetComponent<Animator>();
                // EnemyUIのオブジェクトをみつける
                //enemies[j].EnemyUIObject = enemyPos3[j].transform.Find("Child/EnemyCanvas").gameObject;
                // EnemyUIをEnemyPosの子オブジェクトに生成
                Vector3 uiPosition = new Vector3(enemyPos9[positionIndex].transform.position.x, enemyPos9[positionIndex].transform.position.y + 2.5f, enemyPos9[positionIndex].transform.position.z + 1);
                enemies[j].EnemyUI = Instantiate(enemyCanvas, uiPosition, Quaternion.identity, enemyPos9[positionIndex].transform).gameObject.GetComponent<BattleEnemyUI>();
                // EnemyUIのセットアップ
                enemies[j].EnemyUI.SetEnemyData(enemies[j]);
                // EnemyUIのパネルを表示する
                enemies[j].EnemyUI.ActivateUIPanel();
                j++;
            }
        }
    }
}