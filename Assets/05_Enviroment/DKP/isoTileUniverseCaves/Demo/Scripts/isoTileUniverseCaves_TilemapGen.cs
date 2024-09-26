using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace isoTile_UniverseCaves
{
    public enum CaveTypes
    {
        BrownCave,
        BrownCaveOcean
    }

    public enum Caves_Direction4_TilesetTypes
    {
        Cave_Walls_Brown
    }

    public enum CavesDirectionTypes
    {
        Top,
        Bottom,
        Left,
        Right,
        North,
        South,
        East,
        West
    }

    public enum TilesetCollectionTypes
    {
        DirtCave
    }

    public class isoTileUniverseCaves_TilemapGen : MonoBehaviour
    {
        public static isoTileUniverseCaves_TilemapGen GlobalAccess;
        void Awake()
        {
            GlobalAccess = this;

            // Set Active Tileset Collection
            if (Active_Tileset_Collection_Type == TilesetCollectionTypes.DirtCave)
            {
                Active_TilesetCollection = Cave_Dirt_TilesetCollection;
            }
        }

        [Header("Tilemap Camera:")]
        public Transform MainCamera_Transform;

        public bool Show_DemoInstructionsGUI = true;

        // Tile Markers
        public Tile TileMarker_1;
        public Tile TileMarker_2;

        public Tilemap Tilemap_ToGenerateOn;
        public Tile[] Tiles_ToUseWhenGenerating;
        public float RandomChanceToSpawn_BelowGenArray = 25f;
        public Tile[] Tiles_ToUseWhenGenerating_Random;

        public bool Regenerate_IsoMap = false;

        [Header("Generated Cave Stats")]
        public bool RandomlySpawn_TilesIn = false;
        public int RandomSeed = 1000;
        public int IsoMapSize_Width = 20;
        public int IsoMapSize_Height = 20;
        public float xOrg_Layer1;
        public float yOrg_Layer1;
        public float WorldScale = 20.0F;
        public int Snow_GenerationDepth_MIN = 76;
        public int Snow_GenerationDepth_MAX = 84;

        // Single Biome Generation -
        [Header("Single Cave Generation:")]
        public bool GenerateSingleBiome = false;
        public int Single_BiomeInterations = 4;
        public CaveTypes Single_BiomeType = CaveTypes.BrownCave;
        public CaveTypes Single_BiomeOceanType = CaveTypes.BrownCaveOcean;

        // Active Tileset
        [Header("Active Tileset Collection:")]
        public TilesetCollectionTypes Active_Tileset_Collection_Type = TilesetCollectionTypes.DirtCave;
        public isoTileUniverseCaves_TilesetCollection Active_TilesetCollection;

        // Tileset Collection:
        [Header("Available Tileset Collections:")]
        public isoTileUniverseCaves_TilesetCollection Cave_Dirt_TilesetCollection;

        // Generation Settings
        [Header("Generation Settings:")]
        public isoTileUniverseCaves_BasicCaveGenerator DirtCave_Settings;
        public isoTileUniverseCaves_BasicCaveGenerator Brown_CaveOceanSettings;

        // Grid And Marker Tiles
        public Tile BlankGrid_MapTile;
        public Tile[] Tiles_BiomeMarkers;

        [Header("Cave Resources In Rocks:")]
        public bool Spawn_Copper_OnMap = false;
        public float CopperSpawnChance = 10f;
        public bool Spawn_Iron_OnMap = false;
        public float IronSpawnChance = 10f;
        public bool Spawn_Silver_OnMap = false;
        public float SilverSpawnChance = 10f;
        public bool Spawn_Gold_OnMap = false;
        public float GoldSpawnChance = 10f;

        // Tileset Displays
        [Header("Tileset Displays:")]
        public bool Display_Tileset = false;
        public Color Camera_BackgroundColor = Color.white;
        public List<TileBase> TilesToDisplay_AsSet_List;

        // Start is called before the first frame update
        void Start()
        {
            // Setup Camera - Restraining It To The Generated Map
            MainCamera_Transform.SetParent(null);
            isoTileUniverseCaves_CameraController.GlobalAccess.DisconnectMapBoundGOs();
            Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
            isoTileUniverseCaves_CameraController.GlobalAccess.SetupCameraBounds(Tilemap_ToGenerateOn.GetCellCenterWorld(mapStartingPos));
            Setup_isoTileUniverse_Camera();
            isoTileUniverseCaves_CameraController.GlobalAccess.Set_CameraZoom_Level2();

            RiverAndRoad_Bots = new List<isoTileUniverseCaves_RiverAndRoadBot>();

            Generate_TileSet_Sample();

            WorldGenComplete = true;
        }

        private void Setup_isoTileUniverse_Camera()
        {
            // Generate Tiles
            Vector3Int currentCell = new Vector3Int(0, 0, 0);
            Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
            for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
            {
                for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
                {
                    // Set Map Tile
                    currentCell = new Vector3Int(w, h, 0);
                    isoTileUniverseCaves_CameraController.GlobalAccess.Update_CameraBounds(Tilemap_ToGenerateOn.GetCellCenterLocal(currentCell));
                }
            }
        }

        private void Generate_TileSet_Sample()
        {
            // Generate Tiles On Tilemap
            if (Tilemap_ToGenerateOn != null)
            {
                // Destroy All River And Road Bots
                if (RiverAndRoad_Bots.Count > 0)
                {
                    for (int i = 0; i < RiverAndRoad_Bots.Count; i++)
                    {
                        Destroy(RiverAndRoad_Bots[i].gameObject);
                    }
                    RiverAndRoad_Bots.Clear();
                }
                Process_RiverAndRoadBot_Activities = true;

                if (Tiles_ToUseWhenGenerating.Length > 0)
                {
                    // Set Randomize Seed
                    RandomSeed = Random.Range(0, 10000);
                    Random.InitState(RandomSeed);

                    // Clear Any Tiles On Tilemap
                    Tilemap_ToGenerateOn.ClearAllTiles();

                    if (Display_Tileset)
                    {

                        // Set Camera Background
                        isoTileUniverseCaves_CameraController.GlobalAccess.CameraScript.backgroundColor = isoTileUniverseCaves_CameraController.GlobalAccess.Normal_Camera_TilesetDisplayColor;

                        // Scripting Override Of Tiles
                        //TilesToDisplay_AsSet_List = new List<TileBase>();
                        //for (int i = 0; i < Active_TilesetCollection.Ocean_01_Tile_Array.Length; i++)
                        //{
                        //    TilesToDisplay_AsSet_List.Add(Active_TilesetCollection.Ocean_01_Tile_Array[i]);
                        //}
                        //for (int i = 0; i < Tiles_BlightedLands_InfectedMountains.Length; i++)
                        //{
                        //    TilesToDisplay_AsSet_List.Add(Tiles_BlightedLands_InfectedMountains[i]);
                        //}

                        // Setup Camera
                        Vector3 cameraPosition = new Vector3(0.5f, 1.1f, -10f);
                        if (TilesToDisplay_AsSet_List.Count == 8)
                            cameraPosition = new Vector3(0.5f, 0.9f, -10f);
                        else if (TilesToDisplay_AsSet_List.Count == 16)
                            cameraPosition = new Vector3(1.5f, 1.35f, -10f);
                        Camera.main.transform.position = cameraPosition;
                        Camera cameraScript = Camera.main.gameObject.GetComponent<Camera>();
                        if (TilesToDisplay_AsSet_List.Count == 8)
                            cameraScript.orthographicSize = 1.75f;
                        else if (TilesToDisplay_AsSet_List.Count == 16)
                            cameraScript.orthographicSize = 2.25f;

                        Tilemap_ToGenerateOn.ClearAllTiles();
                        Vector3Int blankmapStartingCell = new Vector3Int(-20, -20, 0);
                        for (int w = -20; w < 20; w++)
                        {
                            for (int h = -20; h < 20; h++)
                            {
                                Vector3Int currentCell = new Vector3Int(w, h, 0);
                                Tilemap_ToGenerateOn.SetTile(currentCell, BlankGrid_MapTile);
                            }
                        }


                        // Display A Certain Tileset
                        if (TilesToDisplay_AsSet_List.Count > 0)
                        {
                            Vector3Int startingTileCell = new Vector3Int(0, 0, 0);
                            int tileSetHalfwayMark = TilesToDisplay_AsSet_List.Count / 2;

                            //Tilemap_ToGenerateOn.ClearAllTiles();
                            int iPos = 0;
                            int iPos2 = 0;
                            for (int i = 0; i < TilesToDisplay_AsSet_List.Count; i++)
                            {
                                if (i < tileSetHalfwayMark)
                                {
                                    Vector3Int currentCell = startingTileCell;
                                    currentCell.x = iPos;
                                    Tilemap_ToGenerateOn.SetTile(currentCell, TilesToDisplay_AsSet_List[i]);
                                    iPos++;
                                }
                                else
                                {
                                    Vector3Int currentCell = startingTileCell;
                                    currentCell.x = iPos2;
                                    currentCell.y = 1;
                                    Tilemap_ToGenerateOn.SetTile(currentCell, TilesToDisplay_AsSet_List[i]);
                                    iPos2++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Single_BiomeType == CaveTypes.BrownCave)
                        {
                            // Generate Brown Cave
                            if (DirtCave_Settings != null)
                            {
                                List<Vector3Int> blAndtrCellsList = DirtCave_Settings.GenerateCave(Tilemap_ToGenerateOn, Active_TilesetCollection, IsoMapSize_Width, IsoMapSize_Height, xOrg_Layer1, yOrg_Layer1, WorldScale);
                                BLCell = blAndtrCellsList[0];
                                TRCell = blAndtrCellsList[1];
                            }
                        }

                        bool TRXLessThanBLXCell = false;
                        if (TRCell.x < BLCell.x)
                        {
                            TRXLessThanBLXCell = true;
                        }
                        // Calculate Map Width And Height
                        int mapWidth = 0;
                        if (!TRXLessThanBLXCell)
                        {
                            for (int w = BLCell.x - 1; w < TRCell.x + 1; w++)
                            {
                                mapWidth++;
                            }
                        }
                        else
                        {
                            for (int w = TRCell.x - 1; w < BLCell.x + 1; w++)
                            {
                                mapWidth++;
                            }
                        }
                        IsoMapSize_Width = mapWidth;


                        bool TRYLessThanBLYCell = false;
                        if (TRCell.y < BLCell.y)
                        {
                            TRYLessThanBLYCell = true;
                        }
                        // Calculate Map Width And Height
                        int mapHeight = 0;
                        if (!TRYLessThanBLYCell)
                        {
                            for (int w = BLCell.y - 1; w < TRCell.y + 1; w++)
                            {
                                mapHeight++;
                            }
                        }
                        else
                        {
                            for (int w = TRCell.y - 1; w < BLCell.y + 1; w++)
                            {
                                mapHeight++;
                            }
                        }
                        IsoMapSize_Height = mapHeight;

                        // Place Blank Tiles Around Generated Cave
                        Vector3Int currentCell = new Vector3Int(0, 0, 0);
                        for (int w = BLCell.x - 2; w < TRCell.x + 3; w++)
                        {
                            for (int h = BLCell.y - 2; h < TRCell.y + 3; h++)
                            {
                                currentCell = new Vector3Int(w, h, 0);
                                if (Tilemap_ToGenerateOn.GetTile(currentCell) == null)
                                {
                                    Tilemap_ToGenerateOn.SetTile(currentCell, BlankGrid_MapTile);
                                    //Vector3Int cell_Top = new Vector3Int(w, h + 1, 0);
                                    //Vector3Int cell_Bottom = new Vector3Int(w, h - 1, 0);
                                    //Vector3Int cell_Left = new Vector3Int(w - 1, h, 0);
                                    //Vector3Int cell_Right = new Vector3Int(w + 1, h, 0);

                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Top) == null)
                                    //    Tilemap_ToGenerateOn.SetTile(cell_Top, BlankGrid_MapTile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Bottom) == null)
                                    //    Tilemap_ToGenerateOn.SetTile(cell_Bottom, BlankGrid_MapTile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Left) == null)
                                    //    Tilemap_ToGenerateOn.SetTile(cell_Left, BlankGrid_MapTile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Right) == null)
                                    //    Tilemap_ToGenerateOn.SetTile(cell_Right, BlankGrid_MapTile);
                                }
                            }
                        }

                        // Place Cave Walls Around Map
                        currentCell = new Vector3Int(0, 0, 0);
                        for (int w = BLCell.x - 2; w < TRCell.x + 3; w++)
                        {
                            for (int h = BLCell.y - 2; h < TRCell.y + 3; h++)
                            {
                                currentCell = new Vector3Int(w, h, 0);
                                if (Tilemap_ToGenerateOn.GetTile(currentCell).name == BlankGrid_MapTile.name)
                                {
                                    Vector3Int cell_Top = new Vector3Int(w, h + 1, 0);
                                    Vector3Int cell_Bottom = new Vector3Int(w, h - 1, 0);
                                    Vector3Int cell_Left = new Vector3Int(w - 1, h, 0);
                                    Vector3Int cell_Right = new Vector3Int(w + 1, h, 0);

                                    if (Tilemap_ToGenerateOn.GetTile(cell_Top) != null)
                                        if (Tilemap_ToGenerateOn.GetTile(cell_Top).name != BlankGrid_MapTile.name)
                                            Tilemap_ToGenerateOn.SetTile(cell_Top, Active_TilesetCollection.Base_Walls_01_Tile);
                                    if (Tilemap_ToGenerateOn.GetTile(cell_Bottom) != null)
                                        if (Tilemap_ToGenerateOn.GetTile(cell_Bottom).name != BlankGrid_MapTile.name)
                                            Tilemap_ToGenerateOn.SetTile(cell_Bottom, Active_TilesetCollection.Base_Walls_01_Tile);
                                    if (Tilemap_ToGenerateOn.GetTile(cell_Left) != null)
                                        if (Tilemap_ToGenerateOn.GetTile(cell_Left).name != BlankGrid_MapTile.name)
                                            Tilemap_ToGenerateOn.SetTile(cell_Left, Active_TilesetCollection.Base_Walls_01_Tile);
                                    if (Tilemap_ToGenerateOn.GetTile(cell_Right) != null)
                                        if (Tilemap_ToGenerateOn.GetTile(cell_Right).name != BlankGrid_MapTile.name)
                                            Tilemap_ToGenerateOn.SetTile(cell_Right, Active_TilesetCollection.Base_Walls_01_Tile);

                                    //Vector3Int cell_Top2 = new Vector3Int(w, h + 2, 0);
                                    //Vector3Int cell_Bottom2 = new Vector3Int(w, h - 2, 0);
                                    //Vector3Int cell_Left2 = new Vector3Int(w - 2, h, 0);
                                    //Vector3Int cell_Right2 = new Vector3Int(w + 2, h, 0);

                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Top2) != null)
                                    //    if (Tilemap_ToGenerateOn.GetTile(cell_Top2).name != BlankGrid_MapTile.name)
                                    //        Tilemap_ToGenerateOn.SetTile(cell_Top2, Active_TilesetCollection.Base_Walls_01_Tile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Bottom2) != null)
                                    //    if (Tilemap_ToGenerateOn.GetTile(cell_Bottom2).name != BlankGrid_MapTile.name)
                                    //        Tilemap_ToGenerateOn.SetTile(cell_Bottom2, Active_TilesetCollection.Base_Walls_01_Tile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Left2) != null)
                                    //    if (Tilemap_ToGenerateOn.GetTile(cell_Left2).name != BlankGrid_MapTile.name)
                                    //        Tilemap_ToGenerateOn.SetTile(cell_Left2, Active_TilesetCollection.Base_Walls_01_Tile);
                                    //if (Tilemap_ToGenerateOn.GetTile(cell_Right2) != null)
                                    //    if (Tilemap_ToGenerateOn.GetTile(cell_Right2).name != BlankGrid_MapTile.name)
                                    //        Tilemap_ToGenerateOn.SetTile(cell_Right2, Active_TilesetCollection.Base_Walls_01_Tile);
                                }
                            }
                        }

                        // Remove Blank Tiles
                        currentCell = new Vector3Int(0, 0, 0);
                        for (int w = BLCell.x - 2; w < TRCell.x + 3; w++)
                        {
                            for (int h = BLCell.y - 2; h < TRCell.y + 3; h++)
                            {
                                currentCell = new Vector3Int(w, h, 0);
                                if (Tilemap_ToGenerateOn.GetTile(currentCell).name == BlankGrid_MapTile.name)
                                {
                                    Tilemap_ToGenerateOn.SetTile(currentCell, null);
                                }
                            }
                        }

                        // Check For Adjacent Tiles

                        // Process Adj Tiles
                        for (int w = BLCell.x - 2; w < TRCell.x + 3; w++)
                        {
                            for (int h = BLCell.y - 2; h < TRCell.y + 3; h++)
                            {
                                // Set Map Tile
                                currentCell = new Vector3Int(w, h, 0);
                                ProcessAdjacentTiles(Tilemap_ToGenerateOn, currentCell);
                            }
                        }

                        // Randomize Cave Interior
                        DirtCave_Settings.Randomize_CaveInterior();

                        //Tilemap_ToGenerateOn.SetTile(BLCell, TileMarker_1);
                        //Tilemap_ToGenerateOn.SetTile(TRCell, TileMarker_2);
                        isoTileUniverseCaves_CameraController.GlobalAccess.Update_CameraBounds(Tilemap_ToGenerateOn.CellToWorld(BLCell));
                        isoTileUniverseCaves_CameraController.GlobalAccess.Update_CameraBounds(Tilemap_ToGenerateOn.CellToWorld(TRCell));

                        // Place Resources In Rocks
                        PlaceResourcesOnMap();
                    }
                }
            }
        }


        public Vector3Int BLCell = new Vector3Int(0, 0, 0);
        public Vector3Int TRCell = new Vector3Int(0, 0, 0);

        private void PlaceResourcesOnMap()
        {
            // Generate Tiles
            Vector3Int currentCell = new Vector3Int(0, 0, 0);
            Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
            for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
            {
                for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
                {
                    // Set Map Tile
                    currentCell = new Vector3Int(w, h, 0);
                    if (Tilemap_ToGenerateOn.GetTile(currentCell) != null)
                    {
                        TileBase tileOnCell = Tilemap_ToGenerateOn.GetTile(currentCell);
                        if (Active_TilesetCollection.NameIs_CaveRock_01(tileOnCell.name))
                        {
                            bool hasSpawnedResources = false;

                            if (!hasSpawnedResources)
                            {
                                if (Spawn_Copper_OnMap)
                                {
                                    // Spawn Copper
                                    if (Random.Range(0, 100) < CopperSpawnChance)
                                    {
                                        Tilemap_ToGenerateOn.SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Resources_Copper_Array));
                                        hasSpawnedResources = true;
                                    }
                                }
                            }

                            if (!hasSpawnedResources)
                            {
                                if (Spawn_Iron_OnMap)
                                {
                                    // Spawn Copper
                                    if (Random.Range(0, 100) < IronSpawnChance)
                                    {
                                        Tilemap_ToGenerateOn.SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Resources_Iron_Array));
                                        hasSpawnedResources = true;
                                    }
                                }
                            }

                            if (!hasSpawnedResources)
                            {
                                if (Spawn_Silver_OnMap)
                                {
                                    // Spawn Copper
                                    if (Random.Range(0, 100) < SilverSpawnChance)
                                    {
                                        Tilemap_ToGenerateOn.SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Resources_Silver_Array));
                                        hasSpawnedResources = true;
                                    }
                                }
                            }

                            if (!hasSpawnedResources)
                            {
                                if (Spawn_Gold_OnMap)
                                {
                                    // Spawn Copper
                                    if (Random.Range(0, 100) < GoldSpawnChance)
                                    {
                                        Tilemap_ToGenerateOn.SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Resources_Gold_Array));
                                        hasSpawnedResources = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //private void Generate_Biome_PerlinWorld_FromTileArrays()
        //{
        //    // Generate Tiles
        //    Vector3Int currentCell = new Vector3Int(0, 0, 0);
        //    int x = 0;
        //    int y = 0;

        //    xOrg_Layer1 = Random.Range(0, 5000);
        //    yOrg_Layer1 = Random.Range(0, 5000);
        //    WorldScale = Random.Range(0.5f, 1.5f);

        //    List<Vector3Int> Possible_TileCells = new List<Vector3Int>();

        //    // Generate Plain Map
        //    Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            Possible_TileCells.Add(currentCell);
        //            if (Tilemap_ToGenerateOn.GetTile(currentCell) == null)
        //            {
        //                Tilemap_ToGenerateOn.SetTile(currentCell, BlankGrid_MapTile);
        //            }
        //        }
        //    }

        //    // Generate Single Biome
        //    for (int i = 0; i < Single_BiomeInterations; i++)
        //    {
        //        // Place Biome 1
        //        Tile biomeTileToPlace = Tiles_BiomeMarkers[0];
        //        int randomBiomeCenterCell = Random.Range(0, Possible_TileCells.Count);
        //        if (Possible_TileCells.Count > 0)
        //        {
        //            Generate_BiomeAroundCell(Possible_TileCells[randomBiomeCenterCell], biomeTileToPlace, Random.Range(2, 4), Random.Range(6, 8), Random.Range(3, 6));
        //        }
        //    }

        //    // Fill Remaining Empty Map Tiles With Biome 7
        //    Possible_TileCells.Clear();
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            if (Tilemap_ToGenerateOn.GetTile(currentCell) == BlankGrid_MapTile)
        //            {
        //                Tilemap_ToGenerateOn.SetTile(currentCell, Tiles_BiomeMarkers[6]);
        //            }
        //        }
        //    }

        //    // Place Biome Tiles
        //    // Single Biome Selection - Index 0
        //    Possible_TileCells.Clear();
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            if (Tilemap_ToGenerateOn.GetTile(currentCell) == Tiles_BiomeMarkers[0])
        //            {
        //                Possible_TileCells.Add(currentCell);
        //            }
        //        }
        //    }
        //    Generate_PerlinBiome_FromCellList(Possible_TileCells, Single_BiomeType);

        //    // Ocean - Single Ocean Biome Selection - Index 6
        //    Possible_TileCells.Clear();
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            if (Tilemap_ToGenerateOn.GetTile(currentCell) == Tiles_BiomeMarkers[6])
        //            {
        //                Possible_TileCells.Add(currentCell);
        //            }
        //        }
        //    }
        //    Generate_PerlinBiome_FromCellList(Possible_TileCells, Single_BiomeOceanType);

        //    // Process Adj Tiles
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            ProcessAdjacentTiles(Tilemap_ToGenerateOn, currentCell);
        //        }
        //    }
        //}

        private void Generate_BiomeAroundCell(Vector3Int biomeCenterCell, Tile biomeTileToPlace, int minVarDistance, int maxVarDistance, int iterations)
        {
            Tilemap_ToGenerateOn.SetTile(biomeCenterCell, biomeTileToPlace);
            Vector3Int newPointToGenerateFrom = biomeCenterCell;
            for (int i = 0; i < iterations; i++)
            {
                newPointToGenerateFrom.x += Random.Range(-minVarDistance, maxVarDistance);
                newPointToGenerateFrom.y += Random.Range(-minVarDistance, maxVarDistance);
                FillTilesRandomlyAroundPointForRange(newPointToGenerateFrom, Random.Range(2, 6), Random.Range(25, 75), biomeTileToPlace);
                newPointToGenerateFrom = biomeCenterCell;
            }
        }

        // Expand World Map Based On Tile Placement
        private void FillTilesRandomlyAroundPointForRange(Vector3Int centerPointCell, float distanceToFill, float chanceToPlaceTiles, Tile tileToPlace)
        {
            int startCellX = centerPointCell.x - (int)distanceToFill;
            int startCellY = centerPointCell.y - (int)distanceToFill;
            int width = (int)(distanceToFill * 2) + 1;
            int height = (int)(distanceToFill * 2) + 1;
            int endCellX = startCellX + width;
            int endCellY = startCellY + height;

            // Random Tile Placement On Map
            for (int w = startCellX; w < endCellX; w++)
            {
                for (int h = startCellY; h < endCellY; h++)
                {
                    //if (w > (IsoMapSize_Width / 2) && w < (IsoMapSize_Width / 2) &&
                    //    h > (IsoMapSize_Height / 2) && h < (IsoMapSize_Height / 2))
                    //{
                    // Do Terraforming
                    if (w < endCellX - 2 && h < endCellY - 2)
                    {
                        // Solid Center
                        Vector3Int cell = new Vector3Int(w, h, 0);
                        TileBase currentTile = Tilemap_ToGenerateOn.GetTile(cell);
                        if (currentTile == BlankGrid_MapTile)
                        {
                            // Check Tile Is Within Range
                            if (TileIsInRange(cell, distanceToFill, centerPointCell))
                            {
                                TileBase newMapTile = tileToPlace;
                                Tilemap_ToGenerateOn.SetTile(cell, newMapTile);
                            }
                        }
                    }
                    else
                    {
                        // Random Outside
                        if (Random.Range(0, 100) < chanceToPlaceTiles)
                        {
                            Vector3Int cell = new Vector3Int(w, h, 0);
                            TileBase currentTile = Tilemap_ToGenerateOn.GetTile(cell);
                            if (currentTile == BlankGrid_MapTile)
                            {
                                // Check Tile Is Within Range
                                if (TileIsInRange(cell, distanceToFill, centerPointCell))
                                {
                                    TileBase newMapTile = tileToPlace;
                                    Tilemap_ToGenerateOn.SetTile(cell, newMapTile);
                                }
                            }
                        }
                    }
                    //}
                }
            }
        }

        public bool TileIsInRange(Vector3Int originCellWorldCenter, float range, Vector3Int centerCellWorldCenter)
        {
            float distance = (new Vector2(originCellWorldCenter.x, originCellWorldCenter.y) - new Vector2(centerCellWorldCenter.x, centerCellWorldCenter.y)).magnitude;
            float rangeSqr = range + 0.5f;
            if (distance < rangeSqr)
            {
                // ok! within range, do stuff
                //Debug.Log(" distance: " + distance.ToString() + " < rangeSqr = " + rangeSqr.ToString());
                return true;
            }
            else
                return false;
        }

        //private void Generate_PerlinBiome_FromCellList(List<Vector3Int> cellsOfBiome, CaveTypes biomeToGenerate)
        //{
        //    // Generate Tiles
        //    Vector3Int currentCell = new Vector3Int(0, 0, 0);
        //    int x = 0;
        //    int y = 0;

        //    xOrg_Layer1 = Random.Range(0, 5000);
        //    yOrg_Layer1 = Random.Range(0, 5000);
        //    WorldScale = Random.Range(0.5f, 1.5f);

        //    if (cellsOfBiome.Count > 0)
        //    {
        //        for (int i = 0; i < cellsOfBiome.Count; i++)
        //        {

        //            // Set Map Tile
        //            currentCell = cellsOfBiome[i];
        //            x = currentCell.x;
        //            y = currentCell.y;

        //            if (biomeToGenerate == CaveTypes.BrownCave)
        //            {
        //                Tilemap_ToGenerateOn.SetTile(currentCell, DirtCave_Settings.GetRandom_Biome_MapTile_Perlin(x, y));
        //            }
        //            else if (biomeToGenerate == CaveTypes.BrownCaveOcean)
        //            {
        //                Tilemap_ToGenerateOn.SetTile(currentCell, Brown_CaveOceanSettings.GetRandom_Biome_MapTile_Perlin(x, y));
        //            }
        //        }
        //    }
        //}

        //private void Generate_PerlinWorldFromTileArrays()
        //{
        //    // Generate Tiles
        //    Vector3Int currentCell = new Vector3Int(0, 0, 0);
        //    int x = 0;
        //    int y = 0;

        //    xOrg_Layer1 = Random.Range(0, 5000);
        //    yOrg_Layer1 = Random.Range(0, 5000);
        //    WorldScale = Random.Range(0.5f, 1.5f);

        //    Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            if (Tilemap_ToGenerateOn.GetTile(currentCell) == null)
        //            {
        //                Tilemap_ToGenerateOn.SetTile(currentCell, GetRandom_Buildable_MapTile_Perlin(x, y));
        //            }
        //        }
        //    }

        //    // Process Adj Tiles
        //    for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
        //    {
        //        for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
        //        {
        //            x = w;
        //            y = h;
        //            // Set Map Tile
        //            currentCell = new Vector3Int(w, h, 0);
        //            ProcessAdjacentTiles(Tilemap_ToGenerateOn, currentCell);
        //        }
        //    }
        //}

        private TileBase GetRandomTileFromArray(TileBase[] tileArrayToSelectFrom)
        {
            int randomTileIndex = Random.Range(0, tileArrayToSelectFrom.Length);
            return tileArrayToSelectFrom[randomTileIndex];
        }

        //private TileBase GetRandom_Buildable_MapTile_Perlin(int x, int y)
        //{

        //    // Generate A World With: Grass, Forest, Rocks, And Mountains


        //    //int randomBuildableGroundTileIndex = Random.Range(0, Buildable_GroundTiles.Length);
        //    // Choose Map Tiles Based On Perlin Noise
        //    // Perlin Noise

        //    float xCoord = xOrg_Layer1 + x / (float)10 * WorldScale;
        //    float yCoord = yOrg_Layer1 + y / (float)10 * WorldScale;
        //    float sample = Mathf.PerlinNoise(xCoord, yCoord);

        //    TileBase tileToUse = GetRandomTileFromArray(Tiles_Grass);

        //    if (sample < 0.1f)
        //    {
        //        tileToUse = GetRandomTileFromArray(Tiles_Mountains);
        //    }
        //    else if (sample >= 0.1f && sample < 0.15f)
        //    {
        //        tileToUse = GetRandomTileFromArray(Tiles_Mountains);
        //    }
        //    else if (sample >= 0.15f && sample < 0.23f)
        //    {
        //        tileToUse = GetRandomTileFromArray(Tiles_Rocks);
        //    }
        //    else if (sample >= 0.25f && sample < 0.3f)
        //    {
        //        if (x < IsoMapSize_Height - Random.Range(Snow_GenerationDepth_MIN, Snow_GenerationDepth_MAX))
        //            tileToUse = GetRandomTileFromArray(Tiles_Forest);
        //        else
        //            tileToUse = GetRandomTileFromArray(Tiles_SnowyForest);
        //    }
        //    else if (sample >= 0.3f && sample < 0.35f)
        //    {
        //        if (x < IsoMapSize_Height - Random.Range(Snow_GenerationDepth_MIN, Snow_GenerationDepth_MAX))
        //            tileToUse = GetRandomTileFromArray(Tiles_Dirt);
        //        else
        //            tileToUse = GetRandomTileFromArray(Tiles_SnowyGrass);
        //    }
        //    else if (sample >= 0.40f && sample < 0.55f)
        //    {
        //        //tileToUse = GetRandomTileFromArray(Tiles_Dirt);
        //        tileToUse = Base_SandLake_01_Tile; //Get_ActiveTileSet().Ocean_01;
        //    }
        //    else
        //    {
        //        if (x < IsoMapSize_Height - Random.Range(Snow_GenerationDepth_MIN, Snow_GenerationDepth_MAX))
        //            tileToUse = GetRandomTileFromArray(Tiles_Dirt);
        //        else
        //            tileToUse = GetRandomTileFromArray(Tiles_SnowyGrass);
        //        //tileToUse = Get_ActiveTileSet().GetRandom_BaseGround_01();
        //    }

        //    return tileToUse;
        //}

        //
        //  Process Adj Tiles
        //
        public void ProcessAdjacentTiles(Tilemap tilemapParent, Vector3Int cellToSet)
        {
            // Clear Building Adjacent Bonuses
            //RCS_BuildingsManager.GlobalAccess.Clear_BuildingsAdjacentBonuses();
            // Process Adjacent Tiles and Update
            int w = cellToSet.x;
            int h = cellToSet.y;

            // Do Processing
            Vector3Int cell = new Vector3Int(w, h, 0);

            TileBase currentTile = tilemapParent.GetTile(cell);

            if (currentTile != null)
            {
                if (NameIs_CaveWallsDirt_01(currentTile.name))
                {
                    // Update Adj - Brown Cave Walls 01
                    Process_Adjacent_4Directions_01(tilemapParent, cellToSet,
                        Caves_Direction4_TilesetTypes.Cave_Walls_Brown, Active_TilesetCollection.Walls_01_Tile_Array);
                }
            }
        }
        // Cave Rocks 01 - Tiles
        public bool NameIs_CaveRock_01(string tileNameToCheck)
        {
            for (int i = 0; i < Active_TilesetCollection.Tiles_Rocks_Array.Length; i++)
            {
                if (tileNameToCheck == Active_TilesetCollection.Tiles_Rocks_Array[i].name)
                {
                    return true;
                }
            }
            return false;
        }

        // Dirt Cave Walls 01 - Tiles
        public bool NameIs_CaveWallsDirt_01(string tileNameToCheck)
        {
            for (int i = 0; i < Active_TilesetCollection.Walls_01_Tile_Array.Length; i++)
            {
                if (tileNameToCheck == Active_TilesetCollection.Walls_01_Tile_Array[i].name)
                {
                    return true;
                }
            }
            return false;
        }

        private void Process_Adjacent_4Directions_01(Tilemap tilemapParent,
            Vector3Int cellToSet,
            Caves_Direction4_TilesetTypes direction4TilesetType,
            TileBase[] tileArrayToUse)
        {
            // Process Adjacent Tiles and Update
            int w = cellToSet.x;
            int h = cellToSet.y;

            // Do Processing
            Vector3Int cell = new Vector3Int(w, h, 0);

            Vector3Int cell_Top = new Vector3Int(w, h + 1, 0);
            bool hasTop = false;
            Vector3Int cell_Bottom = new Vector3Int(w, h - 1, 0);
            bool hasBottom = false;
            Vector3Int cell_Left = new Vector3Int(w - 1, h, 0);
            bool hasLeft = false;
            Vector3Int cell_Right = new Vector3Int(w + 1, h, 0);
            bool hasRight = false;

            TileBase currentTile = tilemapParent.GetTile(cell);
            TileBase tile_Top = tilemapParent.GetTile(cell_Top);
            TileBase tile_Bottom = tilemapParent.GetTile(cell_Bottom);
            TileBase tile_Left = tilemapParent.GetTile(cell_Left);
            TileBase tile_Right = tilemapParent.GetTile(cell_Right);

            if (currentTile != null)
            {
                if (direction4TilesetType == Caves_Direction4_TilesetTypes.Cave_Walls_Brown)
                {
                    // Inverted
                    if (tile_Top != null)
                    {
                        hasTop = true;
                        //if (NameIs_CaveWallsDirt_01(tile_Top.name))
                        //    hasTop = false;
                        //else
                        //    hasTop = true;
                    }
                    else
                    {
                        hasTop = false;
                    }
                    if (tile_Bottom != null)
                    {
                        hasBottom = true;
                        //if (NameIs_CaveWallsDirt_01(tile_Bottom.name))
                        //    hasBottom = false;
                        //else
                        //    hasBottom = true;
                    }
                    else
                    {
                        hasBottom = false;
                    }
                    if (tile_Left != null)
                    {
                        hasLeft = true;
                        //if (NameIs_CaveWallsDirt_01(tile_Left.name))
                        //    hasLeft = false;
                        //else
                        //    hasLeft = true;
                    }
                    else
                    {
                        hasLeft = false;
                    }
                    if (tile_Right != null)
                    {
                        hasRight = true;
                        //if (NameIs_CaveWallsDirt_01(tile_Right.name))
                        //    hasRight = false;
                        //else
                        //    hasRight = true;
                    }
                    else
                    {
                        hasRight = false;
                    }
                }

                if (!hasTop && !hasLeft && !hasRight && !hasBottom)
                {
                    // None
                    tilemapParent.SetTile(cell, tileArrayToUse[0]);
                }
                else if (hasTop && hasLeft && hasRight && hasBottom)
                {
                    // TLRB
                    tilemapParent.SetTile(cell, tileArrayToUse[8]);
                }
                else if (hasTop && hasLeft && hasRight && !hasBottom)
                {
                    // TLR
                    tilemapParent.SetTile(cell, tileArrayToUse[12]);
                }
                else if (!hasTop && hasLeft && hasRight && hasBottom)
                {
                    // LRB
                    tilemapParent.SetTile(cell, tileArrayToUse[10]);
                }
                else if (hasTop && hasLeft && !hasRight && hasBottom)
                {
                    // TLB
                    tilemapParent.SetTile(cell, tileArrayToUse[9]);
                }
                else if (hasTop && !hasLeft && hasRight && hasBottom)
                {
                    // TRB
                    tilemapParent.SetTile(cell, tileArrayToUse[11]);
                }
                else if (!hasTop && hasLeft && !hasRight && hasBottom)
                {
                    // LB
                    tilemapParent.SetTile(cell, tileArrayToUse[13]);
                }
                else if (!hasTop && hasLeft && hasRight && !hasBottom)
                {
                    // LR
                    tilemapParent.SetTile(cell, tileArrayToUse[5]);
                }
                else if (!hasTop && !hasLeft && hasRight && hasBottom)
                {
                    // RB
                    tilemapParent.SetTile(cell, tileArrayToUse[14]);
                }
                else if (hasTop && !hasLeft && !hasRight && hasBottom)
                {
                    // TB
                    tilemapParent.SetTile(cell, tileArrayToUse[6]);
                }
                else if (hasTop && hasLeft && !hasRight && !hasBottom)
                {
                    // TL
                    tilemapParent.SetTile(cell, tileArrayToUse[15]);
                }
                else if (hasTop && !hasLeft && hasRight && !hasBottom)
                {
                    // TR
                    tilemapParent.SetTile(cell, tileArrayToUse[7]);
                }
                else if (!hasTop && !hasLeft && !hasRight && hasBottom)
                {
                    // B
                    tilemapParent.SetTile(cell, tileArrayToUse[2]);
                }
                else if (!hasTop && hasLeft && !hasRight && !hasBottom)
                {
                    // L
                    tilemapParent.SetTile(cell, tileArrayToUse[1]);
                }
                else if (!hasTop && !hasLeft && hasRight && !hasBottom)
                {
                    // R
                    tilemapParent.SetTile(cell, tileArrayToUse[4]);
                }
                else if (hasTop && !hasLeft && !hasRight && !hasBottom)
                {
                    // T
                    tilemapParent.SetTile(cell, tileArrayToUse[3]);
                }
            }
        }

        public bool WorldGenComplete = false;
        public List<isoTileUniverseCaves_RiverAndRoadBot> RiverAndRoad_Bots;
        public bool Process_RiverAndRoadBot_Activities = false;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
                Regenerate_IsoMap = true;
            if (Regenerate_IsoMap)
            {
                Generate_TileSet_Sample();
                Regenerate_IsoMap = false;
            }

            // Process Biome Rivers After All Complete
            if (!Display_Tileset)
            {
                if (WorldGenComplete)
                {
                    if (Process_RiverAndRoadBot_Activities)
                    {
                        bool hasAliveBot = false;
                        if (RiverAndRoad_Bots.Count > 0)
                        {
                            for (int i = 0; i < RiverAndRoad_Bots.Count; i++)
                            {
                                if (!RiverAndRoad_Bots[i].BotDead)
                                    hasAliveBot = true;
                            }
                        }
                        if (!hasAliveBot)
                        {
                            Vector3Int currentCell = new Vector3Int(0, 0, 0);
                            Vector3Int mapStartingPos = new Vector3Int(-(IsoMapSize_Width / 2), -(IsoMapSize_Height / 2), 0);
                            // Process Adj Tiles
                            for (int w = mapStartingPos.x; w < mapStartingPos.x + IsoMapSize_Width; w++)
                            {
                                for (int h = mapStartingPos.x; h < mapStartingPos.y + IsoMapSize_Height; h++)
                                {
                                    // Set Map Tile
                                    currentCell = new Vector3Int(w, h, 0);
                                    ProcessAdjacentTiles(Tilemap_ToGenerateOn, currentCell);
                                }
                            }
                            Process_RiverAndRoadBot_Activities = false;
                        }
                    }
                }
            }
        }
    }

}