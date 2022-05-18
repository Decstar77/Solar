using SolarSharp;
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
    internal enum LevelPlacementType
    {
        NONE,
        OCCUPIED,
        
        TILE_CONCRETE,
        TILE_GRASS,
        TILE_GRAVEL,
        TILE_SAND,
        TILE_SNOW,
        TILE_WATER,

        BUILDING01,
        ROAD,
    }

    internal class LevelCell
    {
        public LevelPlacementType type;
        public int xIndex;
        public int zIndex;

        public Vector3 direction = Vector3.Zero;
        public LevelCell? link;

        public int Elevation = 0;

        public LevelCell(int x, int z, LevelPlacementType type)
        {
            xIndex = x;
            zIndex = z;
            this.type = type;
        }

        public void Set(LevelPlacementType type, LevelCell? link)
        {
            this.type = type;
            this.link = link;
        }

    }

    internal struct LevelCreateInfo
    {
        public int DesiredHouseCount;

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

        private LevelCell[,] levelCells;

        private Random random;

        public LevelGenerator(int width, int height)
        {
            this.cellXCount = width;
            this.cellZCount = height;

            random = new Random();

            levelCells = new LevelCell[cellXCount,  cellZCount];

            cX = cellXCount / 2;
            cZ = cellZCount / 2;
            centerOffset = new Vector3(cX, 0, cZ) * TileDims;

            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    levelCells[x, z] = new LevelCell(x, z, LevelPlacementType.NONE);
                }
            }
        }

        public void _DebugDraw()
        {
            ForEachCell(c =>
            {
                if (c.type != LevelPlacementType.NONE)
                {
                    Vector3 min = GetTileWorldPosition(c.xIndex, c.zIndex);
                    Vector3 max = GetTileWorldPosition(c.xIndex, c.zIndex) + TileDims;
                    DebugDraw.AlignedBox(new AlignedBox(min, max));
                }
            });
        }

        public void Generate(GameScene scene, LevelCreateInfo createInfo)
        {
            ForEachCell((c) => {
                float sX = c.xIndex / (float)cellXCount;
                float sZ = c.zIndex / (float)cellZCount;
                                
                c.Elevation = (int)MathF.Round(PerlinNoise.Noise(sX, sZ) * 6);
                //c.Elevation = 0;
            });


            while (true) {
                Vector2 indices = GetRandomIndex();                
                if (PLaceBuilding01(indices))
                {
                    createInfo.DesiredHouseCount--;
                }
                if (createInfo.DesiredHouseCount == 0)
                    break;
            }

            ConnectRoads();

            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    LevelCell cell = levelCells[x, z];
                    if (cell.type == LevelPlacementType.NONE)
                    {
                        if (cell.Elevation >= SnowElevation)
                        {
                            cell.Set(LevelPlacementType.TILE_SNOW, null);
                        }
                        else if (cell.Elevation >= GravelElevation)
                        {
                            cell.Set(LevelPlacementType.TILE_GRAVEL, null);
                        }
                        else if (cell.Elevation >= GrassElevation)
                        {
                            cell.Set(LevelPlacementType.TILE_GRASS, null);
                        }
                        else if (cell.Elevation >= SandElevation)
                        {
                            cell.Set(LevelPlacementType.TILE_SAND, null);
                        }
                        else if (cell.Elevation >= WaterElevation)
                        {
                            cell.Set(LevelPlacementType.TILE_WATER, null);
                        }
                        else
                        {
                            cell.Set(LevelPlacementType.TILE_WATER, null);
                        }
                    }                    
                }
            }

            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    LevelCell cell = levelCells[x, z];
                    if (cell != null)
                    {
                        Entity entity = scene.CreateEntity();
                        entity.Position = GetTileWorldPosition(x, z);
                        entity.Position += new Vector3(0, cell.Elevation * TileHeight, 0);
                        switch (cell.type)
                        {
                            case LevelPlacementType.NONE:
                                break;
                            case LevelPlacementType.TILE_CONCRETE: {                                    
                                    entity.RenderingState.ModelId = createInfo.TileContreteModel;
                                }
                                break;
                            case LevelPlacementType.TILE_GRASS: {
                                    entity.RenderingState.ModelId = createInfo.TileGrassModel;
                                }
                                break;
                            case LevelPlacementType.TILE_GRAVEL: {
                                    entity.RenderingState.ModelId = createInfo.TileGravelModel;
                                }
                                break;
                            case LevelPlacementType.TILE_SAND: {
                                    entity.RenderingState.ModelId = createInfo.TileSandModel;
                                }
                                break;
                            case LevelPlacementType.TILE_WATER: {
                                    entity.RenderingState.ModelId = createInfo.TileWaterModel;
                                }
                                break;
                            case LevelPlacementType.TILE_SNOW: {
                                    entity.RenderingState.ModelId = createInfo.TileSnowModel;
                                }
                                break;
                            case LevelPlacementType.BUILDING01: {
                                    entity.Position += new Vector3(0, TileHeight, 0);                                    
                                    entity.RenderingState.ModelId = createInfo.Building01Model;
                                }
                                break;
                            case LevelPlacementType.ROAD: {
                                    entity.Position += new Vector3(0, TileHeight, 0);
                                    entity.RenderingState.ModelId = createInfo.RoadFlat;

                                    Entity tile = scene.CreateEntity();
                                    tile.Position = entity.Position - new Vector3(0, TileHeight, 0);
                                    tile.RenderingState.ModelId = createInfo.TileContreteModel;

                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ConnectRoads()
        {
            List<LevelCell> roads = new List<LevelCell>();
            ForEachCell(x =>
            {
                if (x.type == LevelPlacementType.ROAD)
                {
                    roads.Add(x);
                }
            });

            if (roads.Count > 0) {
                LevelCell cell = roads[0];
                for (int i = 1; i < roads.Count; i++)
                {
                    ConnectRoad(cell, roads[i]);
                }
            }
        }

        private void ConnectRoad(LevelCell startPoint, LevelCell endPoint)
        {
            bool[,] visited = new bool[cellXCount, cellZCount];
            LevelCell[,] links = new LevelCell[cellXCount, cellZCount];
            Queue<LevelCell> cells = new Queue<LevelCell>();

            visited[startPoint.xIndex, startPoint.zIndex] = true;
            cells.Enqueue(startPoint);            

            LevelCell? goal = null;
            while (cells.Count > 0)
            {
                LevelCell cell = cells.Dequeue();

                if (cell == endPoint) {
                    goal = cell;
                    break;
                }

                if (cell.xIndex + 1 < cellXCount && !visited[cell.xIndex + 1, cell.zIndex]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelCells[cell.xIndex + 1, cell.zIndex]);
                        links[cell.xIndex + 1, cell.zIndex] = cell;
                        visited[cell.xIndex + 1, cell.zIndex] = true;
                    }
                }
                
                if (cell.xIndex - 1 >= 0 && !visited[cell.xIndex - 1, cell.zIndex]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelCells[cell.xIndex - 1, cell.zIndex]);
                        links[cell.xIndex - 1, cell.zIndex] = cell;
                        visited[cell.xIndex - 1, cell.zIndex] = true;
                    }
                }

                if (cell.zIndex + 1 < cellZCount && !visited[cell.xIndex, cell.zIndex + 1]) {
                    if (CanPlaceRoad(cell)) {
                        cells.Enqueue(levelCells[cell.xIndex, cell.zIndex + 1]);
                        links[cell.xIndex, cell.zIndex + 1] = cell;
                        visited[cell.xIndex, cell.zIndex + 1] = true;
                    }
                }

                if (cell.zIndex - 1 >= 0 && !visited[cell.xIndex, cell.zIndex - 1]) {
                    if (CanPlaceRoad(cell)) { 
                        cells.Enqueue(levelCells[cell.xIndex, cell.zIndex - 1]);
                        links[cell.xIndex, cell.zIndex - 1] = cell;
                        visited[cell.xIndex, cell.zIndex - 1] = true;
                    }
                }
            }

            List<LevelCell> path = new List<LevelCell>();
            if (goal != null)
            {
                LevelCell cur = links[goal.xIndex, goal.zIndex];
                while (cur != null)
                {
                    path.Add(cur);
                    cur = links[cur.xIndex, cur.zIndex];
                }
            }
         

            foreach(LevelCell cur in path)
            {
                cur.type = LevelPlacementType.ROAD;
            }
        }

        private bool CanPlaceRoad(LevelCell cell)
        {
            if (cell.type == LevelPlacementType.NONE || cell.type == LevelPlacementType.ROAD)
            {
                if (cell.Elevation > WaterElevation)
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

            if (CanPlaceBuilding2x2(x, z) && CanPlaceBuilding(x + (int)dir.x, z + (int)dir.z))
            {
                Place2x2(x, z, LevelPlacementType.BUILDING01);

                levelCells[x, z].direction = dir;
                levelCells[x + (int)dir.x, z + (int)dir.z].Set(LevelPlacementType.ROAD, null);
                return true;
            }

            return false;
        }

        private void Place2x2(int x, int z, LevelPlacementType type)
        {
            levelCells[x, z].Set(type, null);
            levelCells[x + 1, z + 1].Set(LevelPlacementType.OCCUPIED, levelCells[x, z]);
            levelCells[x + 1, z].Set(LevelPlacementType.OCCUPIED, levelCells[x, z]);
            levelCells[x, z + 1].Set(LevelPlacementType.OCCUPIED, levelCells[x, z]);
        }

        private bool CanPlaceBuilding2x2(int x, int z)
        {
            if (CanPlaceBuilding(x + 1, z) &&
                CanPlaceBuilding(x, z + 1) && 
                CanPlaceBuilding(x + 1, z + 1) && 
                CanPlaceBuilding(x, z))
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

            if (levelCells[x, z].type != LevelPlacementType.NONE)
                return false;

            if (levelCells[x, z].Elevation <= WaterElevation)
                return false;

            return true;
        }

        private Vector3 GetTileWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * TileDims - centerOffset;
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

        private void ForEachCell(Action<LevelCell> predicate)
        {
            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    predicate.Invoke(levelCells[x, z]);
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
