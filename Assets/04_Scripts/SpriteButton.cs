using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void SpriteHoveredDelegate(int index);
public delegate void SpriteClickedDelegate();

public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public SpriteHoveredDelegate spriteHoveredDelegate;
    public SpriteClickedDelegate spriteClickedDelegate;
    public int selectedId;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("ホバー中");
        // Debug.Log(selectedId + "ばんめ");
        if (spriteHoveredDelegate != null) spriteHoveredDelegate(selectedId);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("クリック");
        if (spriteClickedDelegate != null) spriteClickedDelegate();
    }
}
