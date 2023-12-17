using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

public class SkillCellView : EnhancedScrollerCellView
{
    private SkillCellData skillData;

    // 選択した時のアイコン
    public GameObject selectedIconImage;

    // セルデータを表示するためのUI
    public TextMeshProUGUI skillNameText;

    [SerializeField] private TextMeshProUGUI spText;

    // この関数は、Demoのデータを受け取って表示するだけ
    public void SetSkillData(SkillCellData data)
    {
        // 基礎となるデータソースを設定します。これは、セルビューをリフレッシュする必要があるときに使用します。
        skillData = data;
        skillNameText.text = skillData.skillText;
        //typeIcon.sprite = typeImageSprites[(int)skillData.type];
        spText.text = skillData.sp.ToString();
        // 選択範囲のハイライトを設定するだけのrefreshメソッドを呼び出します。
        RefreshCellView();
    }

    // RefreshCellViewメソッドをオーバーライドすると、
    // データを再読み込みすることなくUIを更新することができます。
    public override void RefreshCellView()
    {
        if (skillData.isSelected == true)
        {
            selectedIconImage.SetActive(true);
            //iconImage.GetComponent<Image>().color = selectedColor;
        }
        else
        {
            selectedIconImage.SetActive(false);
            //iconImage.GetComponent<Image>().color = unselectedColor;
        }
    }
}