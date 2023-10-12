using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    MATERIAL,
    TOOL,
    HEAL_ITEM,
    WEAPON
}

public class Item : MonoBehaviour
{
    [SerializeField] private ItemBase itemBase;
    private int itemCount = 0;
    private int id;
    [SerializeField] private ItemType itemType;

    public Item(ItemBase iBase)
    {
        ItemBase = iBase;
        id = iBase.Id;
    }

    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public int Id { get => id; }
    public ItemType ItemType { get => itemType; }
}