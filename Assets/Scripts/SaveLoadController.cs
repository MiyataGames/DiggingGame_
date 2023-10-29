using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Tilemaps;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;
    [SerializeField] TileScriptableObject tileSB;

    const string SAVE_FILE = "tilemap.json";
    const string DATA_DIR = "Assets/StreamingAssets/data";
    static string saveDataPath = Path.Combine(DATA_DIR + SAVE_FILE);

    // Start is called before the first frame update
    void Start()
    {
        //Save();
        //Load();
    }

    public void Save()
    {
        var data = new SaveTileMapData();
        // タイルマップの原点とサイズをタイルが存在する境界まで圧縮する　タイルマップの存在する範囲だけに境界を小さくする
        tileMap.CompressBounds();
        // タイルが存在する範囲をしれる
        var b = tileMap.cellBounds;

        string str = "";
        Debug.Log("xの最小値" + b.min.x + "yの最小値" + b.min.y);
        data.origin.Add(b.min.x.ToString());
        data.origin.Add(b.min.y.ToString());
        for (int y = b.min.y; y < b.max.y; y++)
        {
            for (int x = b.min.x; x < b.max.x; x++)
            {
                // どのセルにタイルが存在するかを取得
                if (tileMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    /*
                    Debug.Log(tileSB.tileDataList[0].tile);
                    Debug.Log(tileMap.GetTile(new Vector3Int(x, y, 0)));
                    Debug.Log(tileSB.tileDataList[0].tile);
                    */
                    // tileSBのtileDataListにアクセスする　tileMap.GetTile(new Vector3Int(x,y,0))が等しいもの　
                    str += tileSB.tileDataList.Single(t => t.tile == tileMap.GetTile(new Vector3Int(x, y, 0))).head + ",";
                }
                else
                {
                    str += " ,";
                }
            }
            str = str.TrimEnd(',');
            data.mapData.Add(str);
            str = "";
        }
        string json = JsonUtility.ToJson(data, true);
        if (!Directory.Exists(DATA_DIR))
        {
            Directory.CreateDirectory(DATA_DIR);
        }

        StreamWriter writer = new StreamWriter(saveDataPath, false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    public void Load()
    {
        tileMap.ClearAllTiles();

        FileStream stream = File.Open(saveDataPath, FileMode.Open);
        StreamReader reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        SaveTileMapData data = JsonUtility.FromJson<SaveTileMapData>(json);
        int xDeff = int.Parse(data.origin[0]);
        int yDeff = int.Parse(data.origin[1]);
        for (int y = 0; y < data.mapData.Count; y++)
        {
            string[] xlist = data.mapData[y].Split(',');
            for (int x = 0; x < xlist.Length; x++)
            {
                if (xlist[x] == " ") continue;
                tileMap.SetTile(new Vector3Int(xDeff + x, yDeff + y, 0), tileSB.tileDataList.Single(t => t.head == xlist[x]).tile);
            }
        }
    }

    [Serializable]
    public class SaveTileMapData
    {
        public List<string> origin = new List<string>();
        public List<string> mapData = new List<string>();
    }
}
