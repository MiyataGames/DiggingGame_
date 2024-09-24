using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCoin : MonoBehaviour
{
    [SerializeField] private CoinBase CoinBase;
    public int Price
    {
        get { return CoinBase.Price; }
    }
}
