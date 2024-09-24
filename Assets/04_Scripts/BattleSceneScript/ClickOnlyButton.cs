using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickOnlyButton : Button
{
    // OnPointerDownはマウスボタンが押された時に呼ばれる
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    // OnSubmitはキーボード入力によって呼ばれるが、何もしないようにオーバーライドする
    public override void OnSubmit(BaseEventData eventData)
    {
        // 何もしない
    }
}
