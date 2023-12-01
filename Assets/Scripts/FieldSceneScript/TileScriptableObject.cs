using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileScriptableObject : ScriptableObject
{
    public List<TileStore> tileDataList = new List<TileStore>();
}

[Serializable]
public class TileStore
{
    public string head;
    public Tile tile;
}