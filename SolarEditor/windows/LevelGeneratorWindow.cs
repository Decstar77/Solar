using SolarSharp;
using SolarSharp.Assets;
using SolarSharp.core;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    [Flags]
    internal enum TileDirectionType
    {
        N = 0,
        L = 1,
        R = 2,
        F = 4,
        B = 8,
        FULL = L | R | F | B,
        RB = R | B,
        RF = R | F,
        LF = L | F,
        LB = L | B,
        RFB = R | F | B,
        LRF = L | R | F,
        LFB = L | F| B,
        LRB = L | R| B,
    }


    internal enum LevelTileType
    {
        NONE,
        CONCRETE,
        GRASS,
        GRAVEL,
        SAND,
        SNOW,
        WATER,
    }

    internal class LevelTile
    {
        public LevelTileType type;
        public int xIndex;
        public int zIndex;
        public int Elevation = 0;

        public LevelTile(int x, int z, LevelTileType type)
        {
            xIndex = x;
            zIndex = z;
            this.type = type;
        }
    }

    internal enum LevelPlacementType
    {
        NONE,
        OCCUPIED,
        BUILDING01,
        ROAD
    }


    internal class LevelPlacement
    {
        public LevelPlacementType type;
        public int xIndex;
        public int zIndex;

        public Vector3 direction = Vector3.Zero;

        public LevelPlacement(int x, int z, LevelPlacementType type)
        {
            xIndex = x;
            zIndex = z;
            this.type = type;
        }
    }


    internal struct LevelCreateInfo
    {
        public int DesiredHouseCount;


        public Guid TileNone;
        public Guid TileFull;
        public Guid TileL;
        public Guid TileR;
        public Guid TileF;
        public Guid TileB;
        public Guid TileRB;
        public Guid TileRF;
        public Guid TileLF;
        public Guid TileLB;
        public Guid TileRFB;
        public Guid TileLRF;
        public Guid TileLFB;
        public Guid TileLRB;       


        public Guid TileContreteModel;
        public Guid TileGrassModel;
        public Guid TileGravelModel;
        public Guid TileSnowModel;
        public Guid TileWaterModel;
        public Guid TileSandModel;

        public Guid RoadForwardModel;
        public Guid RoadFlat;
        public Guid Building01Model;
    }

    internal class LevelGenerator
    {
        private int cellXCount;
        private int cellZCount;

        private const int TileWidth = 4;
        private const int TileDepth = 4;
        private const int TileHeight = 2;

        private const int SnowElevation = 3;
        private const int GravelElevation = 2;
        private const int GrassElevation = -1;
        private const int SandElevation = -2;
        private const int WaterElevation = -3;

        private Vector3 TileDims = new Vector3(TileWidth, TileHeight, TileDepth);
        private Vector3 centerOffset;

        private int cX;
        private int cZ;

        private LevelTile[,] levelTiles;
        private LevelPlacement[,] levelPlacements;

        private Random random;

        public LevelGenerator(int width, int height)
        {
            this.cellXCount = width;
            this.cellZCount = height;

            random = new Random();

            levelTiles = new LevelTile[cellXCount,  cellZCount];
            levelPlacements = new LevelPlacement[cellXCount, cellZCount];

            cX = cellXCount / 2;
            cZ = cellZCount / 2;
            centerOffset = new Vector3(cX, 0, cZ) * TileDims;

            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    levelTiles[x, z] = new LevelTile(x, z, LevelTileType.NONE);
                    levelPlacements[x, z] = new LevelPlacement(x, z, LevelPlacementType.NONE);
                }
            }
        }

        public void _DebugDraw()
        {
            ForEachPlacement(p =>
            {
                if (p.type != LevelPlacementType.NONE)
                {
                    Vector3 min = GetPlacementWorldPostion(p.xIndex, p.zIndex);
                    Vector3 max = GetPlacementWorldPostion(p.xIndex, p.zIndex) + TileDims;
                    DebugDraw.AlignedBox(new AlignedBox(min, max));
                }
            });
        }

        public void Generate(GameScene scene, LevelCreateInfo createInfo)
        {
            ForEachTile((c) => {
                float sX = c.xIndex / (float)cellXCount;
                float sZ = c.zIndex / (float)cellZCount;                                
                c.Elevation = (int)MathF.Round(PerlinNoise.Noise(sX, sZ) * 6);
                //c.Elevation = 0;
                
                if (c.Elevation >= SnowElevation)
                {
                    c.type = LevelTileType.SNOW;
                }
                else if (c.Elevation >= GravelElevation)
                {
                    c.type = LevelTileType.GRAVEL;
                }
                else if (c.Elevation >= GrassElevation)
                {
                    c.type = LevelTileType.GRASS;
                }
                else if (c.Elevation >= SandElevation)
                {
                    c.type = LevelTileType.SAND;
                }
                else if (c.Elevation >= WaterElevation)
                {
                    c.type = LevelTileType.WATER;
                }
                else
                {
                    c.type = LevelTileType.WATER;
                }
            });

            ForEachTile((cell) => {
                if (cell != null)
                {
                    Entity entity = scene.CreateEntity();
                    entity.Position = GetTileWorldPosition(cell.xIndex, cell.zIndex);
                    entity.Position += new Vector3(0, cell.Elevation * TileHeight, 0);

                    TileDirectionType direction = TileDirectionType.N;
                    if (cell.xIndex + 1 < cellXCount && levelTiles[cell.xIndex + 1, cell.zIndex].Elevation < cell.Elevation) {
                        direction |= TileDirectionType.R;
                    }
                    if (cell.xIndex - 1 >= 0 && levelTiles[cell.xIndex - 1, cell.zIndex].Elevation < cell.Elevation) {
                        direction |= TileDirectionType.L;
                    }
                    if (cell.zIndex + 1 < cellZCount && levelTiles[cell.xIndex, cell.zIndex + 1].Elevation < cell.Elevation) {
                        direction |= TileDirectionType.B;                
                    }
                    if (cell.zIndex - 1 >= 0 && levelTiles[cell.xIndex, cell.zIndex - 1].Elevation < cell.Elevation) {
                        direction |= TileDirectionType.F;
                    }

                    switch (direction)
                    {
                        case TileDirectionType.N: entity.RenderingState.ModelId = createInfo.TileNone; break;
                        case TileDirectionType.L: entity.RenderingState.ModelId = createInfo.TileL; break;
                        case TileDirectionType.R: entity.RenderingState.ModelId = createInfo.TileR; break;
                        case TileDirectionType.F: entity.RenderingState.ModelId = createInfo.TileF; break;
                        case TileDirectionType.B: entity.RenderingState.ModelId = createInfo.TileB; break;
                        case TileDirectionType.FULL: entity.RenderingState.ModelId = createInfo.TileFull; break;
                        case TileDirectionType.RB: entity.RenderingState.ModelId = createInfo.TileRB; break;
                        case TileDirectionType.RF: entity.RenderingState.ModelId = createInfo.TileRF; break;
                        case TileDirectionType.LF: entity.RenderingState.ModelId = createInfo.TileLF; break;
                        case TileDirectionType.LB: entity.RenderingState.ModelId = createInfo.TileLB; break;
                        case TileDirectionType.RFB: entity.RenderingState.ModelId = createInfo.TileRFB; break;
                        case TileDirectionType.LRF: entity.RenderingState.ModelId = createInfo.TileLRF; break;
                        case TileDirectionType.LFB: entity.RenderingState.ModelId = createInfo.TileLFB; break;
                        case TileDirectionType.LRB: entity.RenderingState.ModelId = createInfo.TileLRB; break;
                    }

                    switch (cell.type)
                    {
                        case LevelTileType.NONE:
                        case LevelTileType.CONCRETE:    entity.RenderingState.MaterialReference = "Grey_Asphalt"; break;
                        case LevelTileType.GRASS:       entity.RenderingState.MaterialReference = "Grass"; break;
                        case LevelTileType.GRAVEL:      entity.RenderingState.MaterialReference = "Sand"; break;
                        case LevelTileType.SAND:        entity.RenderingState.MaterialReference = "Sand"; break;
                        case LevelTileType.WATER:       entity.RenderingState.MaterialReference = "Water";break;
                        case LevelTileType.SNOW:        entity.RenderingState.MaterialReference = "Snow"; break;
                    }
                }
            });

            while (true)
            {
                Vector2 indices = GetRandomIndex();
                if (PLaceBuilding01(indices))
                {
                    createInfo.DesiredHouseCount--;
                }
                if (createInfo.DesiredHouseCount == 0)
                    break;
            }

            ConnectRoads();

            ForEachPlacement(placement =>
            {
                switch (placement.type)
                {
                    case LevelPlacementType.BUILDING01:
                        { 
                            Entity entity = scene.CreateEntity();
                            entity.Position = GetPlacementWorldPostion(placement.xIndex, placement.zIndex);
                            entity.RenderingState.ModelId = createInfo.Building01Model;
                        }
                        break;
                    case LevelPlacementType.ROAD:
                        {
                            Entity entity = scene.CreateEntity();
                            entity.Position = GetPlacementWorldPostion(placement.xIndex, placement.zIndex);
                            entity.RenderingState.ModelId = createInfo.RoadFlat;
                        }
                        break;
                }
            });

        }

        private void ConnectRoads()
        {
            List<LevelPlacement> roads = new List<LevelPlacement>();
            ForEachPlacement(x =>
            {
                if (x.type == LevelPlacementType.ROAD)
                {
                    roads.Add(x);
                }
            });
            
            if (roads.Count > 0) {
                LevelPlacement cell = roads[0];
                for (int i = 1; i < roads.Count; i++)
                {
                    ConnectRoad(cell, roads[i]);
                }
            }
        }

        private void ConnectRoad(LevelPlacement startPoint, LevelPlacement endPoint)
        {
            bool[,] visited = new bool[cellXCount, cellZCount];
            LevelPlacement [,] links = new LevelPlacement[cellXCount, cellZCount];
            Queue<LevelPlacement> cells = new Queue<LevelPlacement>();

            visited[startPoint.xIndex, startPoint.zIndex] = true;
            cells.Enqueue(startPoint);

            LevelPlacement? goal = null;
            while (cells.Count > 0)
            {
                LevelPlacement cell = cells.Dequeue();

                if (cell == endPoint) {
                    goal = cell;
                    break;
                }

                if (cell.xIndex + 1 < cellXCount && !visited[cell.xIndex + 1, cell.zIndex]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelPlacements[cell.xIndex + 1, cell.zIndex]);
                        links[cell.xIndex + 1, cell.zIndex] = cell;
                        visited[cell.xIndex + 1, cell.zIndex] = true;
                    }
                }
                
                if (cell.xIndex - 1 >= 0 && !visited[cell.xIndex - 1, cell.zIndex]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelPlacements[cell.xIndex - 1, cell.zIndex]);
                        links[cell.xIndex - 1, cell.zIndex] = cell;
                        visited[cell.xIndex - 1, cell.zIndex] = true;
                    }
                }

                if (cell.zIndex + 1 < cellZCount && !visited[cell.xIndex, cell.zIndex + 1]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelPlacements[cell.xIndex, cell.zIndex + 1]);
                        links[cell.xIndex, cell.zIndex + 1] = cell;
                        visited[cell.xIndex, cell.zIndex + 1] = true;
                    }
                }

                if (cell.zIndex - 1 >= 0 && !visited[cell.xIndex, cell.zIndex - 1]) {
                    if (CanPlaceRoad(cell)) { 
                        cells.Enqueue(levelPlacements[cell.xIndex, cell.zIndex - 1]);
                        links[cell.xIndex, cell.zIndex - 1] = cell;
                        visited[cell.xIndex, cell.zIndex - 1] = true;
                    }
                }
            }

            List<LevelPlacement> path = new List<LevelPlacement>();
            if (goal != null)
            {
                LevelPlacement cur = links[goal.xIndex, goal.zIndex];
                while (cur != null)
                {
                    path.Add(cur);
                    cur = links[cur.xIndex, cur.zIndex];
                }
            }
         

            foreach(LevelPlacement cur in path)
            {
                cur.type = LevelPlacementType.ROAD;
            }
        }

        private bool CanPlaceRoad(LevelPlacement cell)
        {
            if (cell.type == LevelPlacementType.NONE|| cell.type == LevelPlacementType.ROAD)
            {
                if (levelTiles[cell.xIndex, cell.zIndex].type != LevelTileType.WATER)
                {
                    return true;
                }
            }
            return false;
        }

        private bool PLaceBuilding01(Vector2 indices)
        {
            return PLaceBuilding01((int)indices.x, (int)indices.y);
        }

        private bool PLaceBuilding01(int x, int z)
        {           
            Vector3 dir = GetRandomDirection();

            dir.x = dir.x > 0 ? dir.x + 1 : dir.x;
            dir.z = dir.z > 0 ? dir.z + 1 : dir.z;

            if (CanPlaceBuilding2x2(x, z) && CanPlaceBuilding(x + (int)dir.x, z + (int)dir.z, levelTiles[x, z].Elevation))
            {
                Place2x2(x, z, LevelPlacementType.BUILDING01);

                levelPlacements[x, z].direction = dir;
                levelPlacements[x + (int)dir.x, z + (int)dir.z].type = LevelPlacementType.ROAD;
                return true;
            }

            return false;
        }

        private void Place2x2(int x, int z, LevelPlacementType type)
        {
            levelPlacements[x, z].type = type;
            levelPlacements[x + 1, z + 1].type = LevelPlacementType.OCCUPIED;
            levelPlacements[x + 1, z].type = LevelPlacementType.OCCUPIED;
            levelPlacements[x, z + 1].type = LevelPlacementType.OCCUPIED;
        }

        private bool CanPlaceBuilding2x2(int x, int z)
        {
            int el = levelTiles[x, z].Elevation;
            if (CanPlaceBuilding(x + 1, z, el) &&
                CanPlaceBuilding(x, z + 1, el) && 
                CanPlaceBuilding(x + 1, z + 1, el) && 
                CanPlaceBuilding(x, z, el))
            {
                return true;
            }

            return false;
        }

        private bool CanPlaceBuilding(int x, int z)
        {
            if (x >= cellXCount)
                return false;
            if (x < 0)
                return false;
            if (z >= cellZCount)
                return false;
            if (z < 0)
                return false;

            if (levelPlacements[x, z].type != LevelPlacementType.NONE)
                return false;

            if (levelTiles[x, z].Elevation <= WaterElevation)
                return false;

            return true;
        }

        private bool CanPlaceBuilding(int x, int z, int elevation)
        {
            if (CanPlaceBuilding(x, z) && levelTiles[x, z].Elevation == elevation)
                return true;

            return false;
        }

        private Vector3 GetTileWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * TileDims - centerOffset;
        }

        private Vector3 GetPlacementWorldPostion(int x, int z)
        {
            return new Vector3(x * TileDims.x, levelTiles[x, z].Elevation * TileDims.y + 2, z * TileDims.z) - centerOffset;
        }

        private Vector3 GetRandomDirection()
        {
            int dir =  random.Next(4);
            switch(dir)
            {
                case 0: return new Vector3(-1, 0, 0);
                case 1: return new Vector3(1, 0, 0);
                case 2: return new Vector3(0, 0, 1);
                case 3: return new Vector3(0, 0, -1);
            }

            return Vector3.UnitX;
        }

        private Vector2 GetRandomIndex()
        {
            return new Vector2(random.Next(cellXCount), random.Next(cellZCount));
        }

        private void ForEachPlacement(Action<LevelPlacement> predicate)
        {
            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    predicate.Invoke(levelPlacements[x, z]);
                }
            }
        }

        private void ForEachTile(Action<LevelTile> predicate)
        {
            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    predicate.Invoke(levelTiles[x, z]);
                }
            }
        }
    }

    internal class LevelGeneratorWindow : EditorWindow
    {
        public int TileHorizontalCount = 9 * 3;
        public int TileVerticalCount = 9 * 3;
        LevelGenerator levelGenerator;

        public override void Show(EditorState editorState)
        {
            if (ImGui.Begin("Level generator", ref show))
            {
                ImGui.InputInt("Tile Horizontal Count", ref TileHorizontalCount);
                ImGui.InputInt("Tile Vertical Count", ref TileVerticalCount);
                if (ImGui.Button("Generate"))
                {
                    
                    LevelCreateInfo createInfo = new LevelCreateInfo();
                    createInfo.DesiredHouseCount = (int)((float)TileHorizontalCount * 0.7f);

                    createInfo.TileNone = AssetSystem.GetModelAsset("Tile_W.obj").Guid;
                    createInfo.TileL = AssetSystem.GetModelAsset("Tile_L.obj").Guid;
                    createInfo.TileR = AssetSystem.GetModelAsset("Tile_R.obj").Guid;
                    createInfo.TileF = AssetSystem.GetModelAsset("Tile_F.obj").Guid;
                    createInfo.TileB = AssetSystem.GetModelAsset("Tile_B.obj").Guid;
                    createInfo.TileFull = AssetSystem.GetModelAsset("Tile_Full.obj").Guid;
                    createInfo.TileRB = AssetSystem.GetModelAsset("Tile_RB.obj").Guid;
                    createInfo.TileRF = AssetSystem.GetModelAsset("Tile_RF.obj").Guid;
                    createInfo.TileLF = AssetSystem.GetModelAsset("Tile_LF.obj").Guid;
                    createInfo.TileLB = AssetSystem.GetModelAsset("Tile_LB.obj").Guid;
                    createInfo.TileRFB = AssetSystem.GetModelAsset("Tile_RFB.obj").Guid;
                    createInfo.TileLRF = AssetSystem.GetModelAsset("Tile_LRF.obj").Guid;
                    createInfo.TileLFB = AssetSystem.GetModelAsset("Tile_LFB.obj").Guid;
                    createInfo.TileLRB = AssetSystem.GetModelAsset("Tile_LRB.obj").Guid; 

                    Guid.TryParse("c430f407-cf03-4a28-9876-98d7bcab5962", out createInfo.TileContreteModel);
                    Guid.TryParse("dfd0d315-0e1a-441b-8492-a1079d037810", out createInfo.TileGrassModel);
                    Guid.TryParse("b84a53b7-54ac-47cf-89b1-5a113f58e1a8", out createInfo.TileGravelModel);
                    Guid.TryParse("4edc2c7e-fa26-4cff-bd3f-b5273c84157e", out createInfo.TileSnowModel);                    
                    Guid.TryParse("55ca9ce2-eb3a-42a5-b096-cc2e9aca761b", out createInfo.TileWaterModel);
                    Guid.TryParse("72faa7d3-1b8d-4081-a350-0905f435f7dc", out createInfo.TileSandModel);                    

                    Guid.TryParse("a1fc79b4-8e82-4a2f-b551-ab077f26c823", out createInfo.Building01Model);  
                    Guid.TryParse("a0d1c221-ae20-4a63-b772-87c00e278ad8", out createInfo.RoadFlat);
                    Guid.TryParse("f94aa063-7bb2-427e-826a-55af086e6dff", out createInfo.RoadForwardModel);

                    levelGenerator = new LevelGenerator(TileHorizontalCount, TileVerticalCount);
                    editorState.currentContext.scene.DestroyAllEntities();
                    levelGenerator.Generate(editorState.currentContext.scene, createInfo);
                }

                if (levelGenerator != null)
                {
                    //levelGenerator._DebugDraw();
                }                
            }

            ImGui.End();
        }

        public override void Shutdown()
        {
        }

        public override void Start()
        {
        }
    }
}
