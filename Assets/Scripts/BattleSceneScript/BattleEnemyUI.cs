using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleEnemyUI : MonoBehaviour
{
    // 敵
    [SerializeField] private TextMeshProUGUI enemyNameText;

    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private HPBar enemyHpBar;

    private Enemy enemy;

    // 敵の詳細
    [SerializeField] private GameObject selectedArrow;

    [SerializeField] private GameObject EnemyDiscriptionPanel;

    public GameObject SelectedArrow { get => selectedArrow; set => selectedArrow = value; }

    public void SetEnemyData(Enemy enemy)
    {
        this.enemy = enemy;
        enemyNameText.text = enemy.EnemyBase.EnemyName;
        enemyLevelText.text = "Lv." + enemy.Level.ToString();
        enemyHpBar.SetHP(enemy.Hp, enemy.MaxHp);
        //weakImage.SetActive(false);
    }

    public void UpdateHp()
    {
        Debug.Log("enemyName" + enemy.EnemyBase.EnemyName + "hp" + enemy.Hp + "maxhp" + enemy.MaxHp);
        // コルーチンの中でStartCoroutineは省略可能
        enemyHpBar.SetHP((float)enemy.Hp, enemy.MaxHp);
        //enemyHpBar.SetHP((float)enemy.Hp,enemy.MaxHp);
    }

    public void SetActivenessDiscriptionPanel(bool activeness)
    {
        EnemyDiscriptionPanel.SetActive(activeness);
    }

    public void SetActiveSelectedArrow(bool activeness)
    {
        SelectedArrow.SetActive(activeness);
    }

    public void UnActiveUIPanel()
    {
        this.GetComponent<Canvas>().enabled = false;
    }

    public void ActivateUIPanel()
    {
        this.GetComponent<Canvas>().enabled = true;
    }
}