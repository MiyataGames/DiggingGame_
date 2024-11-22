using UnityEngine;
using UnityEngine.Tilemaps;

namespace isoTile_UniverseCaves
{
    public class isoTileUniverseCaves_TilesetCollection : MonoBehaviour
    {

        public string TilesetName = "Unnamed Cave Tileset";

        // Specific Tileset Collection
        [Header("Cave Tileset Arrays:")]
        public Tile[] Tiles_Floor_Array;
        public Tile[] Tiles_Rocks_Array;
        public bool NameIs_CaveRock_01(string tileNameToCheck)
        {
            for (int i = 0; i < Tiles_Rocks_Array.Length; i++)
            {
                if (tileNameToCheck == Tiles_Rocks_Array[i].name)
                {
                    return true;
                }
            }
            return false;
        }
        public Tile[] Tiles_Pillars_Array;
        public Tile[] Tiles_Stalagmites_Array;
        public Tile[] Tiles_Stalactites_Array;
        public Tile[] Tiles_MushroomForest_Array;
        public Tile[] Tiles_EntrancesAndExits_Array;
        public Tile[] Tiles_EntrancesAndExits_OverrideTiles_Array;

        public Tile[] Tiles_Resources_Copper_Array;
        public Tile[] Tiles_Resources_Iron_Array;
        public Tile[] Tiles_Resources_Silver_Array;
        public Tile[] Tiles_Resources_Gold_Array;

        // Walls
        public TileBase Base_Walls_01_Tile;
        public TileBase[] Walls_01_Tile_Array;

        [Header("Random Chance (0% - 100%) To Spawn:")]
        public float RandomChance_Rocks = 2f;
        public float RandomChance_Pillars = 2f;
        public float RandomChance_Stalagmites = 2f;
        public float RandomChance_Stalactites = 2f;
        public float RandomChance_MushroomForest = 1f;

    }
}
