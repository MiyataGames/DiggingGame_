using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    private Item item;
    public ItemBase itemBase; // �C���X�y�N�^����ݒ�

    public Item Item { get => item; set => item = value; }

    void Start()
    {
        item = new Item(itemBase);// ������
    }

    public void ApplyItemAffect(Item item)
    {
        Debug.Log("CCCCCCCCC");
        switch (item.ItemBase.ItemType)
        {
            case ItemType.WEAPON:
                Debug.Log("WEAPON");
                Destroy(gameObject);
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("�Փ�");
            ApplyItemAffect(item);
        }
    }

}
