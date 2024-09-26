using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SpriteHoveredDelegate(int index);
public delegate void SpriteClickedDelegate();

public class SpriteButton : MonoBehaviour
{
    public SpriteHoveredDelegate spriteHoveredDelegate;
    public SpriteClickedDelegate spriteClickedDelegate;
    public int selectedId;

    private void OnMouseEnter()
    {
        // Debug.Log("ホバー中");
        // Debug.Log(selectedId + "ばんめ");
        if (spriteHoveredDelegate != null) spriteHoveredDelegate(selectedId);

    }

    private void OnMouseDown()
    {
        Debug.Log("クリック");
        if (spriteClickedDelegate != null) spriteClickedDelegate();
    }
}
