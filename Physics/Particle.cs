using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MyTools;

namespace Physics
{
    public class Particle
    {
        public Color Color { get; protected set; }
        public double Weight { get; protected set; }
        public double Fluidity { get; protected set; }
        public Tile Tile { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double InertiaX { get; protected set; }
        public double InertiaY { get; protected set; }

        public Particle(Tile tile)
        {
            Tile = tile;
        }

        public void Shift(Field field)
        {
            while (Math.Abs(InertiaX) > 1 || Math.Abs(InertiaY) > 1)
            {
                Point shift = new Point(Math.Sign((int)InertiaX), Math.Sign((int)InertiaY));
                InertiaX -= shift.X;
                InertiaY -= shift.Y;
                TryShift(shift);
            }

            void TryShift(Point shift)
            {
                Tile newTile = field.Tiles[Tile.X + shift.X, Tile.Y + shift.Y];
                if (!newTile.IsWall)
                {
                    if (newTile.Particle == null)
                    {
                        Tile.Particle = null;
                        Tile = newTile;
                        newTile.Particle = this;
                    }
                    else
                    {
                        Particle other = newTile.Particle;
                        if (newTile.Particle.Weight >= Weight)
                        {
                            other.SpeedX += SpeedX * (Weight/other.Weight) * 0.5 * Math.Abs(shift.X);
                            other.SpeedY += SpeedY * (Weight / other.Weight) * 0.5 * Math.Abs(shift.Y);
                            SpeedX *= shift.X == 0 ? 1 : 0.5;
                            SpeedY *= shift.Y == 0 ? 1 : 0.5;

                            SpeedX += shift.Y != 0 && Math.Abs(SpeedX) < 1 ? MyRandom.GetNumRange(Fluidity / 2, Fluidity) * MyRandom.GetNumRange(-1, 2) : 0;
                            return;
                        }
                        else
                        {
                            Tile.Particle = newTile.Particle;
                            newTile.Particle.Tile = Tile;
                            Tile = newTile;
                            newTile.Particle = this;
                            SpeedX *= shift.X == 0 ? 1 : (1 - other.Weight / Weight);
                            SpeedY *= shift.Y == 0 ? 1 : (1 - other.Weight / Weight);
                            other.SpeedX += shift.X == 0 ? 0 : 1 * (other.Weight / Weight);
                            other.SpeedY += shift.Y == 0 ? 0 : 1 * (other.Weight / Weight);
                        }
                    }
                }
                else
                {
                    double elasticity = 0;
                    InertiaX *= shift.X == 0 ? 1 : elasticity;
                    InertiaY *= shift.Y == 0 ? 1 : elasticity;
                    SpeedX *= shift.X == 0 ? 1 : elasticity;
                    SpeedY *= shift.Y == 0 ? 1 : elasticity;
                    return;
                }
            }
        }

        public void Move(Field field)
        {
            SpeedY += field.G;
            //SpeedX += MyRandom.GetNumRange(-3, 3d);
            InertiaX += SpeedX;
            InertiaY += SpeedY;
        }
    }

    public class Water : Particle
    {
        public Water(Tile tile) : base(tile)
        {
            Color = Color.Blue;
            Weight = 1;
            Fluidity = 10;
        }
    }
    public class Sand : Particle
    {
        public Sand(Tile tile) : base(tile)
        {
            Color = Color.Tan;
            Weight = 2;
            Fluidity = 4;
        }
    }
    public class Dirt : Particle
    {
        public Dirt(Tile tile) : base(tile)
        {
            Color = Color.SandyBrown;
            Weight = 2;
            Fluidity = 1;
        }
    }
}
