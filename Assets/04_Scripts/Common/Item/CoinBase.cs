using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �R�C���̃}�X�^�[�f�[�^

/// </summary>
[CreateAssetMenu(menuName = "Base/CoinBase")]
public class CoinBase : ScriptableObject
{
    [SerializeField]
    int price = 100;

    public int Price { get => price; set => price = value; }
}