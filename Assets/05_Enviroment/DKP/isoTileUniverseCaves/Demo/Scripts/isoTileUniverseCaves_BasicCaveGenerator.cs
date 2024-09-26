using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace isoTile_UniverseCaves
{

    public enum CavesLayerTypes
    {
        Basic,
        Secondary,
        First,
        Second,
        Third,
        Fourth
    }

    public class isoTileUniverseCaves_BasicCaveGenerator : MonoBehaviour
    {

        void Start()
        {
            RiverBot_GOs_List = new List<GameObject>();
        }

        private Tilemap Tilemap_ToGenerateOn;
        private int IsoMapSize_Width = 0;
        private int IsoMapSize_Height = 0;

        private List<Vector3Int> allCellsUsedList;

        public isoTileUniverseCaves_TilesetCollection Active_TilesetCollection;

        public List<Vector3Int> GenerateCave(Tilemap Tilemap_ToGenerateOnIn, isoTileUniverseCaves_TilesetCollection tilesetCollectionToUse, int mapWidth, int mapHeight, float xOrg_Layer1, float yOrg_Layer1, float WorldScale)
        {
            Tilemap_ToGenerateOn = Tilemap_ToGenerateOnIn;
            Active_TilesetCollection = tilesetCollectionToUse;

            List<Vector3Int> BLAndBRCells = new List<Vector3Int>();
            IsoMapSize_Width = mapWidth;
            IsoMapSize_Height = mapHeight;

            // All Tiles
            allCellsUsedList = new List<Vector3Int>();

            xOrg_Layer1 = Random.Range(0, 5000);
            yOrg_Layer1 = Random.Range(0, 5000);
            WorldScale = Random.Range(0.5f, 1.5f);

            float caveRoomWidthMIN = 4;
            float caveRoomWidthMAX = 14;

            // Generate First Room
            Vector3Int roomCenterCell = new Vector3Int(Random.Range(0, 10), Random.Range(0, 10), 0);
            int NumberOfCaveRoomsToGenerate = Random.Range(4, 10);
            for (int i = 0; i < NumberOfCaveRoomsToGenerate; i++)
            {
                float randomCaveRoomDistanceToFill = Random.Range(caveRoomWidthMIN, caveRoomWidthMAX);
                GenerateCaveRoom(roomCenterCell, randomCaveRoomDistanceToFill, Active_TilesetCollection.Tiles_Floor_Array);

                // Randomly Generate Center Of Room Area
                if (Random.Range(0, 100) < 20)
                {
                    float randomCenterDistanceToFill = randomCaveRoomDistanceToFill - 3;
                    GenerateCaveRoom(roomCenterCell, randomCenterDistanceToFill, null);
                }

                bool NextCaveRoomHorz = true;
                bool NextCaveRoomUp = true;
                if (Random.Range(0, 100) < 50)
                {
                    // Go Horz
                    NextCaveRoomHorz = true;
                }
                else
                {
                    // Go Horz
                    NextCaveRoomHorz = false;
                }
                if (Random.Range(0, 100) < 50)
                {
                    // Go Horz
                    NextCaveRoomUp = true;
                }
                else
                {
                    // Go Horz
                    NextCaveRoomUp = false;
                }
                if (NextCaveRoomHorz)
                {
                    // Move To Next Room Cell - Horz
                    int hallwayDistance = (int)Random.Range(randomCaveRoomDistanceToFill, randomCaveRoomDistanceToFill + Random.Range(6, 8));
                    int randomHallwayDistance = (int)randomCaveRoomDistanceToFill + hallwayDistance;
                    for (int j = 0; j < randomHallwayDistance; j++)
                    {
                        roomCenterCell.x++;
                        SetTile(roomCenterCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));

                        Vector3Int cellUp = roomCenterCell;
                        cellUp.y += 1;
                        SetTile(cellUp, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));

                        Vector3Int cellDown = roomCenterCell;
                        cellDown.y -= 1;
                        SetTile(cellDown, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));

                        if (Random.Range(0, 100) < 50)
                        {
                            Vector3Int cellUp2 = roomCenterCell;
                            cellUp2.y += 2;
                            SetTile(cellUp2, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));
                        }
                        if (Random.Range(0, 100) < 50)
                        {
                            Vector3Int cellDown2 = roomCenterCell;
                            cellDown2.y -= 2;
                            SetTile(cellDown2, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));
                        }
                    }
                }
                else
                {
                    // Move To Next Room Cell - Horz
                    int hallwayDistance = (int)Random.Range(randomCaveRoomDistanceToFill, randomCaveRoomDistanceToFill + Random.Range(3, 6));
                    int randomHallwayDistance = (int)randomCaveRoomDistanceToFill + hallwayDistance;
                    for (int j = 0; j < randomHallwayDistance; j++)
                    {
                        if (NextCaveRoomUp)
                        {
                            roomCenterCell.y++;
                        }
                        else
                        {
                            roomCenterCell.y--;
                        }
                        SetTile(roomCenterCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));

                        Vector3Int cellLeft = roomCenterCell;
                        cellLeft.x -= 1;
                        SetTile(cellLeft, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));
                        Vector3Int cellRight = roomCenterCell;
                        cellRight.x += 1;
                        SetTile(cellRight, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));

                        if (Random.Range(0, 100) < 50)
                        {
                            Vector3Int cellLeft2 = roomCenterCell;
                            cellLeft2.x -= 2;
                            SetTile(cellLeft2, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));
                        }
                        if (Random.Range(0, 100) < 50)
                        {
                            Vector3Int cellRight2 = roomCenterCell;
                            cellRight2.x += 2;
                            SetTile(cellRight2, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Floor_Array));
                        }
                    }
                }
            }

            // Generate Exit Room
            float randomDistanceToFill = Random.Range(caveRoomWidthMIN, caveRoomWidthMAX);
            GenerateCaveRoom(roomCenterCell, randomDistanceToFill, Active_TilesetCollection.Tiles_Floor_Array);

            // Get Bottom Left And Top Right Cells
            Vector3Int BLCell = new Vector3Int(0, 0, 0);
            Vector3Int TRCell = new Vector3Int(0, 0, 0);
            for (int i = 0; i < allCellsUsedList.Count; i++)
            {
                Vector3Int currentCell = allCellsUsedList[i];
                if (currentCell.x < BLCell.x)
                    BLCell.x = currentCell.x;
            }
            for (int i = 0; i < allCellsUsedList.Count; i++)
            {
                Vector3Int currentCell = allCellsUsedList[i];
                if (currentCell.y < BLCell.y)
                    BLCell.y = currentCell.y;
            }
            for (int i = 0; i < allCellsUsedList.Count; i++)
            {
                Vector3Int currentCell = allCellsUsedList[i];
                if (currentCell.x > TRCell.x)
                    TRCell.x = currentCell.x;
            }
            for (int i = 0; i < allCellsUsedList.Count; i++)
            {
                Vector3Int currentCell = allCellsUsedList[i];
                if (currentCell.y > TRCell.y)
                    TRCell.y = currentCell.y;
            }

            // Generate Cave Walls
            //Vector3Int mapStartingPos = new Vector3Int(-(mapWidth / 2), -(mapHeight / 2), 0);
            //for (int w = mapStartingPos.x; w < mapStartingPos.x + mapWidth; w++)
            //{
            //    for (int h = mapStartingPos.x; h < mapStartingPos.y + mapHeight; h++)
            //    {
            //        x = w;
            //        y = h;
            //        // Set Map Tile
            //        currentCell = new Vector3Int(w, h, 0);
            //        // Edges
            //        //if (currentCell.x > mapStartingPos.x && currentCell.y > mapStartingPos.y && currentCell.x < mapStartingPos.x + mapWidth - 1 && currentCell.y < mapStartingPos.y + mapHeight - 1)
            //        //{
            //        if (Tilemap_ToGenerateOn.GetTile(currentCell) == null)
            //        {
            //            Possible_TileCells.Add(currentCell);
            //            if (Random.Range(0, 100) < 90)
            //                Tilemap_ToGenerateOn.SetTile(currentCell, Basic_Cave_Walls_Tile);
            //        }
            //        //}
            //    }
            //}

            // Generate First Cave Room
            //Generate_CaveRoom(Possible_TileCells);

            // Return
            BLAndBRCells.Add(BLCell);
            BLAndBRCells.Add(TRCell);
            return BLAndBRCells;
        }

        public void Randomize_CaveInterior()
        {
            // Remove Blank Tiles
            Vector3Int currentCell = new Vector3Int(0, 0, 0);
            for (int w = isoTileUniverseCaves_TilemapGen.GlobalAccess.BLCell.x - 2; w < isoTileUniverseCaves_TilemapGen.GlobalAccess.TRCell.x + 3; w++)
            {
                for (int h = isoTileUniverseCaves_TilemapGen.GlobalAccess.BLCell.y - 2; h < isoTileUniverseCaves_TilemapGen.GlobalAccess.TRCell.y + 3; h++)
                {
                    currentCell = new Vector3Int(w, h, 0);

                    // Place Random Rocks
                    if (Tilemap_ToGenerateOn.GetTile(currentCell) != null)
                    {
                        if (TileNameIsFlooring(Tilemap_ToGenerateOn.GetTile(currentCell).name))
                        {
                            // Randomly Spawn Rocks
                            if (Random.Range(0, 100) < Active_TilesetCollection.RandomChance_Rocks)
                                SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Rocks_Array));
                            // Randomly Spawn Pillars
                            if (Random.Range(0, 100) < Active_TilesetCollection.RandomChance_Pillars)
                                SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Pillars_Array));
                            // Randomly Spawn Stalagmites
                            if (Random.Range(0, 100) < Active_TilesetCollection.RandomChance_Stalagmites)
                                SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Stalagmites_Array));
                            // Randomly Spawn Stalactites
                            if (Random.Range(0, 100) < Active_TilesetCollection.RandomChance_Stalactites)
                                SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_Stalactites_Array));
                            // Randomly Spawn Mushroom Forests
                            if (Random.Range(0, 100) < Active_TilesetCollection.RandomChance_MushroomForest)
                                SetTile(currentCell, GetRandomTileFromArray(Active_TilesetCollection.Tiles_MushroomForest_Array));
                        }
                        else
                        {
                            bool entrancePlaced = false;
                            if (Active_TilesetCollection.Tiles_EntrancesAndExits_Array.Length == Active_TilesetCollection.Tiles_EntrancesAndExits_OverrideTiles_Array.Length)
                            {
                                for (int i = 0; i < Active_TilesetCollection.Tiles_EntrancesAndExits_Array.Length; i++)
                                {
                                    if (!entrancePlaced)
                                    {
                                        if (Tilemap_ToGenerateOn.GetTile(currentCell).name == Active_TilesetCollection.Tiles_EntrancesAndExits_OverrideTiles_Array[i].name)
                                        {
                                            if (Random.Range(0, 100) < 2)
                                            {
                                                SetTile(currentCell, Active_TilesetCollection.Tiles_EntrancesAndExits_Array[i]);
                                                entrancePlaced = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool TileNameIsFlooring(string tileNameToCheck)
        {
            for (int i = 0; i < Active_TilesetCollection.Tiles_Floor_Array.Length; i++)
            {
                if (tileNameToCheck == Active_TilesetCollection.Tiles_Floor_Array[i].name)
                {
                    return true;
                }
            }
            return false;
        }

        public int TotalTilesPlaced = 0;

        private void SetTile(Vector3Int cell, Tile tileToSet)
        {
            if (tileToSet != null)
            {
                Tilemap_ToGenerateOn.SetTile(cell, tileToSet);
                isoTileUniverseCaves_CameraController.GlobalAccess.Update_CameraBounds(Tilemap_ToGenerateOn.CellToWorld(cell));
                allCellsUsedList.Add(cell);
                TotalTilesPlaced++;
            }
        }

        private void GenerateCaveRoom(Vector3Int roomCenterCell, float randomDistanceToFill, Tile[] tileArrayToUse)
        {
            List<Vector3Int> caveRoom_TileCells = FillTilesRandomlyAroundPointForRange(Tilemap_ToGenerateOn, roomCenterCell, randomDistanceToFill + 2f, 5f, tileArrayToUse);
            List<Vector3Int> caveRoom2_TileCells = FillTilesRandomlyAroundPointForRange(Tilemap_ToGenerateOn, roomCenterCell, randomDistanceToFill, 95f, tileArrayToUse);
        }

        private void Generate_CaveRoom(List<Vector3Int> possibleCells)
        {
            Debug.Log("Generating Cave Room..." + possibleCells.Count.ToString());
            int randomCellCenter = Random.Range(0, possibleCells.Count);
            float randomDistanceToFill = Random.Range(12f, 18f);
            FillTilesRandomlyAroundPointForRange(Tilemap_ToGenerateOn, possibleCells[randomCellCenter], randomDistanceToFill, 15f, null);
        }

        private Tile GetRandomTileFromArray(Tile[] tileArrayToSelectFrom)
        {
            int randomTileIndex = Random.Range(0, tileArrayToSelectFrom.Length);
            return tileArrayToSelectFrom[randomTileIndex];
        }

        // Expand World Map Based On Tile Placement
        private List<Vector3Int> FillTilesRandomlyAroundPointForRange(Tilemap Tilemap_ToGenerateOn, Vector3Int centerPointCell, float distanceToFill, float chanceToPlaceTiles, Tile[] tilesToPlaceArray)
        {
            List<Vector3Int> cellListToReturn = new List<Vector3Int>();

            Debug.Log("Filling Cave Room..." + centerPointCell.ToString());
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
                    if (w < endCellX - 4 && h < endCellY - 4)
                    {
                        // Solid Center
                        Vector3Int cell = new Vector3Int(w, h, 0);
                        TileBase currentTile = Tilemap_ToGenerateOn.GetTile(cell);
                        //if (currentTile == Basic_Cave_Walls_Tile)
                        //{
                        // Check Tile Is Within Range
                        if (TileIsInRange(cell, distanceToFill, centerPointCell))
                        {
                            if (tilesToPlaceArray != null)
                            {
                                Tile newMapTile = GetRandomTileFromArray(tilesToPlaceArray);
                                SetTile(cell, newMapTile);
                            }
                            else
                            {
                                Tile newMapTile = null;
                                SetTile(cell, newMapTile);
                            }
                            cellListToReturn.Add(cell);
                        }
                        //}
                    }
                    else
                    {
                        // Random Outside
                        if (Random.Range(0, 100) < chanceToPlaceTiles)
                        {
                            Vector3Int cell = new Vector3Int(w, h, 0);
                            TileBase currentTile = Tilemap_ToGenerateOn.GetTile(cell);
                            //if (currentTile == Basic_Cave_Walls_Tile)
                            //{
                            // Check Tile Is Within Range
                            if (TileIsInRange(cell, distanceToFill, centerPointCell))
                            {
                                if (tilesToPlaceArray != null)
                                {
                                    Tile newMapTile = GetRandomTileFromArray(tilesToPlaceArray);
                                    SetTile(cell, newMapTile);
                                }
                                else
                                {
                                    Tile newMapTile = null;
                                    SetTile(cell, newMapTile);
                                }
                                cellListToReturn.Add(cell);
                            }
                            //}
                        }
                    }
                    //}
                }
            }

            return cellListToReturn;
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

        // River Spawning
        public bool Spawn_BiomeRivers = false;
        public int RiverBotsSpawned_Count = 0;
        public float ChanceToSpawn_RiverBot = 2f;
        public float ChanceToSpawn_02_RiverBot = 2f;
        public Tile Biome_River_BaseTile;
        public Tile[] Biome_River_PossibleTravelTiles;
        public List<GameObject> RiverBot_GOs_List;
        public void Spawn_RiverBot(Vector3Int riverBotStartingCell)
        {
            if (Tilemap_ToGenerateOn != null)
            {
                if (Random.Range(0, 100) < ChanceToSpawn_02_RiverBot)
                {
                    //Debug.Log("Spawning River Bot...");
                    RiverBotsSpawned_Count++;
                    GameObject newRiverBotGO = new GameObject("RiverBot_" + RiverBotsSpawned_Count.ToString("##0"));
                    newRiverBotGO.transform.SetParent(gameObject.transform, false);
                    RiverBot_GOs_List.Add(newRiverBotGO);
                    isoTileUniverseCaves_RiverAndRoadBot riverBotScript = newRiverBotGO.AddComponent<isoTileUniverseCaves_RiverAndRoadBot>();
                    riverBotScript.Initialize_Bot(Tilemap_ToGenerateOn, Biome_River_PossibleTravelTiles, Biome_River_BaseTile, riverBotStartingCell);
                    isoTileUniverseCaves_TilemapGen.GlobalAccess.RiverAndRoad_Bots.Add(riverBotScript);
                }
            }
        }


    }
}