using MyTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Physics
{
    public class Field
    {
        readonly public int Width;
        readonly public int Height;
        readonly public Tile[,] Tiles;
        readonly public Tile[] TilesArray;
        public double G { get; set; } = 1;

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            TilesArray = new Tile[width * height];
            GenerateStartField();
        }

        void GenerateStartField()
        {
            SetNewTiles();

            /*
            foreach(Tile tile in Tiles)
            {
                if (!tile.IsWall)
                    if (MyRandom.CheckChance(20))
                        tile.Particle = new Water(tile);
                    else if (MyRandom.CheckChance(20))
                        tile.Particle = new Dirt(tile);
            }
            //*/

            void SetNewTiles()
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                            Tiles[x, y] = new Tile(x, y, true);
                        else
                            Tiles[x, y] = new Tile(x, y, false);
                    }
                }

                foreach(Tile tile in Tiles)
                {
                    if (!tile.IsEdge)
                        tile.DefineNeigs(Tiles);
                }

                foreach (Tile tile in Tiles)
                {
                    TilesArray[tile.X + tile.Y * Width] = tile;
                }
            }
        }

        public void Update()
        {
            Stack<Particle> activeParticles = new Stack<Particle>();
            Stack<Particle> activeParticlesNew = new Stack<Particle>();
            foreach (Tile tile in MyRandom.GetMixedArray(Tiles))
            {
                if (tile.Particle != null)
                {
                    tile.Particle.DefineMoving(ref activeParticles);
                }
            }
            while (activeParticles.Count > 0)
            {
                while (activeParticles.Count > 0)
                {
                    activeParticles.Pop().Shift(activeParticlesNew, this);
                }

                activeParticles = new Stack<Particle>(activeParticlesNew);
                activeParticlesNew.Clear();
            }
            /*foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                {
                    tile.Particle.TrySpread();
                }
            }*/
            foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                {
                    tile.Particle.Move(this);
                }
            }
            foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                {
                    tile.Particle.DoAction();
                }
            }
            int totalTemp = 0;
            foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                    totalTemp += tile.Particle.Temperature;
            }
            foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                    tile.Particle.SpreadTemperature();
            }
           
        }
    }

    public class Tile
    {
        readonly public int X;
        readonly public int Y;
        public Tile[] Neigs { get; private set; } = new Tile[4];
        public bool IsWall { get; set; }
        public bool IsEdge { get; }
        public Particle Particle { get; set; }

        public Tile(int x, int y, bool isEdge)
        {
            X = x;
            Y = y;
            IsEdge = isEdge;
            IsWall = isEdge;
        }

        public void DefineNeigs(Tile[,] tiles)
        {
            if (!IsWall)
            {
                Neigs[0] = tiles[X + 1, Y];
                Neigs[1] = tiles[X, Y - 1];
                Neigs[2] = tiles[X - 1, Y];
                Neigs[3] = tiles[X, Y + 1];
            }
        }
    }
}
