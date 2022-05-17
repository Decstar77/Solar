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
        CONCRETE,
        GRASS,
        BUILDING01,
        ROAD,
    }
    internal class LevelCell
    {
        public LevelPlacementType type;
        public int xIndex;
        public int yIndex;
        public int zIndex;

        public Vector3 direction = Vector3.Zero;
        public LevelCell link;

        public LevelCell(int x, int y, int z, LevelPlacementType type)
        {
            xIndex = x;
            yIndex = y;
            zIndex = z;
            this.type = type;
        }

        public void Set(LevelPlacementType type, LevelCell link)
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
        public Guid RoadForwardModel;
        public Guid RoadFlat;
        public Guid Building01Model;
    }

    internal class LevelGenerator
    {
        private int cellXCount;
        private int cellYCount;
        private int cellZCount;

        private const int TileWidth = 4;
        private const int TileDepth = 4;
        private const int TileHeight = 2;

        private Vector3 TileDims = new Vector3(TileWidth, TileHeight, TileDepth);
        private Vector3 centerOffset;

        private int cX;
        private int cY;
        private int cZ;

        private LevelCell[,,] levelCells;

        private Random random;

        public LevelGenerator(int width, int height)
        {
            this.cellXCount = width;
            this.cellYCount = 2;
            this.cellZCount = height;

            random = new Random();

            levelCells = new LevelCell[cellXCount, cellYCount, cellZCount];

            cX = cellXCount / 2;
            cY = cellYCount / 2;
            cZ = cellZCount / 2;
            centerOffset = new Vector3(cX, 0, cZ) * TileDims;

            for (int x = 0; x < cellXCount; x++)
            {
                for (int y = 0; y < cellYCount; y++)
                {
                    for (int z = 0; z < cellZCount; z++)
                    {
                        levelCells[x, y, z] = new LevelCell(x, y, z, LevelPlacementType.NONE);
                    }
                }
            }
        }

        public void _DebugDraw()
        {
            ForEachCell(c =>
            {
                if (c.type != LevelPlacementType.NONE)
                {
                    Vector3 min = GetTileWorldPosition(c.xIndex, c.yIndex, c.zIndex);
                    Vector3 max = GetTileWorldPosition(c.xIndex, c.yIndex, c.zIndex) + TileDims;
                    DebugDraw.AlignedBox(new AlignedBox(min, max));
                }
            });
        }

        public void Generate(GameScene scene, LevelCreateInfo createInfo)
        {            
            for (int x = 0; x < cellXCount; x++)
            {
                for (int z = 0; z < cellZCount; z++)
                {
                    levelCells[x, 0, z].Set(LevelPlacementType.GRASS, null);
                }
            }

            while (true) {
                Vector3 indices = GetRandomIndex();
                indices.y = 1;
                if (PLaceBuilding01(indices))
                {
                    createInfo.DesiredHouseCount--;
                }
                if (createInfo.DesiredHouseCount == 0)
                    break;
            }
            

            for (int x = 0; x < cellXCount; x++)
            {
                for (int y = 0; y < cellYCount; y++)
                {
                    for (int z = 0; z < cellZCount; z++)
                    {
                        if (levelCells[x, y, z] != null)
                        {
                            switch (levelCells[x, y, z].type)
                            {
                                case LevelPlacementType.NONE:
                                    break;
                                case LevelPlacementType.CONCRETE:
                                    {
                                        Entity entity = scene.CreateEntity();
                                        entity.Position = GetTileWorldPosition(x, y, z);
                                        entity.RenderingState.ModelId = createInfo.TileContreteModel;
                                    }
                                    break;
                                case LevelPlacementType.GRASS:
                                    {
                                        Entity entity = scene.CreateEntity();
                                        entity.Position = GetTileWorldPosition(x, y, z);
                                        entity.RenderingState.ModelId = createInfo.TileGrassModel;
                                    }
                                    break;
                                case LevelPlacementType.BUILDING01:
                                    {
                                        Entity entity = scene.CreateEntity();
                                        entity.Position = GetTileWorldPosition(x, y, z);
                                        entity.RenderingState.ModelId = createInfo.Building01Model;
                                    }
                                    break;
                                case LevelPlacementType.ROAD:
                                    {
                                        Entity entity = scene.CreateEntity();
                                        entity.Position = GetTileWorldPosition(x, y, z);
                                        entity.RenderingState.ModelId = createInfo.RoadFlat;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private bool PLaceBuilding01(Vector3 indices)
        {
            return PLaceBuilding01((int)indices.x, (int)indices.y, (int)indices.z);
        }

        private bool PLaceBuilding01(int x, int y, int z)
        {           
            Vector3 dir = GetRandomDirection();

            dir.x = dir.x > 0 ? dir.x + 1 : dir.x;
            dir.z = dir.z > 0 ? dir.z + 1 : dir.z;

            if (CanPlace2x2(x, y, z) && CanPlaceCell(x + (int)dir.x, y, z + (int)dir.z))
            {
                Place2x2(x, y, z, LevelPlacementType.BUILDING01);

                levelCells[x, y, z].direction = dir;
                levelCells[x + (int)dir.x, y, z + (int)dir.z].Set(LevelPlacementType.ROAD, null);
                return true;
            }

            return false;
        }

        private void Place2x2(int x, int y, int z, LevelPlacementType type)
        {
            levelCells[x, y, z].Set(type, null);
            levelCells[x + 1, y, z + 1].Set(LevelPlacementType.OCCUPIED, levelCells[x, y, z]);
            levelCells[x + 1, y, z].Set(LevelPlacementType.OCCUPIED, levelCells[x, y, z]);
            levelCells[x, y, z + 1].Set(LevelPlacementType.OCCUPIED, levelCells[x, y, z]);
        }

        private bool CanPlace2x2(int x, int y, int z)
        {
            if (CanPlaceCell(x + 1, y, z) &&
                CanPlaceCell(x, y, z + 1) && 
                CanPlaceCell(x + 1, y, z + 1) && 
                CanPlaceCell(x, y, z))
            {
                return true;
            }

            return false;
        }

        private bool CanPlaceCell(int x, int y, int z)
        {
            if (x >= cellXCount)
                return false;
            if (x < 0)
                return false;
            if (z >= cellZCount)
                return false;
            if (z < 0)
                return false;

            if (levelCells[x, y, z].type != LevelPlacementType.NONE)
                return false;

            return true;
        }

        private Vector3 GetTileWorldPosition(int x, int y, int z)
        {
            return new Vector3(x, y, z) * TileDims - centerOffset;
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

        private Vector3 GetRandomIndex()
        {
            return new Vector3(random.Next(cellXCount), random.Next(cellYCount), random.Next(cellZCount));
        }

        private void ForEachCell(Action<LevelCell> predicate)
        {
            for (int x = 0; x < cellXCount; x++)
            {
                for (int y = 0; y < cellYCount; y++)
                {
                    for (int z = 0; z < cellZCount; z++)
                    {
                        predicate.Invoke(levelCells[x, y, z]);
                    }
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
                    createInfo.DesiredHouseCount = TileHorizontalCount / 3;
                    Guid.TryParse("c430f407-cf03-4a28-9876-98d7bcab5962", out createInfo.TileContreteModel);
                    Guid.TryParse("dfd0d315-0e1a-441b-8492-a1079d037810", out createInfo.TileGrassModel);
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
