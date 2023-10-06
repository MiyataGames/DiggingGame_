using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string playerName;

    // Start is called before the first frame update
    public List<Item> Items { get; set; }

    public Player(string name)
    {
        playerName = name;
        Items = new List<Item>();
    }
}