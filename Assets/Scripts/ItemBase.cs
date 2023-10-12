using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���̃}�X�^�[�f�[�^
/// </summary>
[CreateAssetMenu(menuName = "Base/ItemBase")]
public class ItemBase : ScriptableObject
{
    [SerializeField] private string itemName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private int id;
    [SerializeField] private ItemType itemType;

    [SerializeField] private bool isAll;// �S�̂�
    [SerializeField] private bool isHeal;// �񕜂�
    [SerializeField] private int healHp;// �ǂꂭ�炢�񕜂��邩

    public string ItemName { get => itemName; }
    public string Description { get => description; }
    public bool IsAll { get => isAll; }
    public bool IsHeal { get => isHeal; }
    public int HealHp { get => healHp; }
    public int Id { get => id; set => id = value; }
    public ItemType ItemType { get => itemType; set => itemType = value; }
}