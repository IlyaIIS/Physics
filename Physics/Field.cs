using MyTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public class Field
    {
        readonly public int Width;
        readonly public int Height;
        readonly public Tile[,] Tiles;
        public double G { get; set; } = 1;

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
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
            }
        }

        public void Update()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.Particle != null)
                {
                    tile.Particle.Move(this);
                }
            }
            foreach (Tile tile in MyRandom.GetMixedArray(Tiles))
            {
                if (tile.Particle != null)
                {
                    tile.Particle.Shift(this);
                }
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
