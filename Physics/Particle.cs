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
        private double weight;
        public double Weight { get { return weight * Density; } protected set { weight = value; } }
        public double Fluidity { get; protected set; }
        public Tile Tile { get; set; }
        public int Density { get; protected set; } = 1;
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double InertiaX { get; protected set; }
        public double InertiaY { get; protected set; }

        public Particle(Tile tile)
        {
            Tile = tile;
        }

        public void DefineMoving(ref Stack<Particle> activeParticles)
        {
            if (Math.Abs(InertiaX) > 1 || Math.Abs(InertiaY) > 1)
            {
                activeParticles.Push(this);
            }
        }
        public void Shift(Stack<Particle> activeParticles, Field field)
        {
            Point shift = new Point(Math.Sign((int)InertiaX), Math.Sign((int)InertiaY));
            InertiaX -= shift.X;
            InertiaY -= shift.Y;
            TryShift(shift);

            void TryShift(Point shift)
            {
                Tile nextTile = field.Tiles[Tile.X + shift.X, Tile.Y + shift.Y];
                if (!nextTile.IsWall)
                {
                    if (nextTile.Particle != null)
                    {
                        Particle other = nextTile.Particle;
                        if (false && other.GetType() == this.GetType() && (Math.Max(Math.Abs(SpeedX - other.SpeedX), Math.Abs(SpeedY - other.SpeedY)) > 20 * other.Density))
                        {
                            other.Density++;
                            Tile.Particle = null;
                            Tile = null;

                            double impulseX = SpeedX * (Weight / other.Weight) * 0.5 * Math.Abs(shift.X);
                            double impulseY = SpeedY * (Weight / other.Weight) * 0.5 * Math.Abs(shift.Y);
                            other.SpeedX += impulseX;
                            other.SpeedY += impulseY;

                            return;
                        }
                        else
                        {
                            double impulseX = SpeedX * (Weight / other.Weight) * 0.5 * Math.Abs(shift.X);
                            double impulseY = SpeedY * (Weight / other.Weight) * 0.5 * Math.Abs(shift.Y);
                            other.SpeedX += impulseX;
                            other.SpeedY += impulseY;
                            SpeedX -= impulseX;
                            SpeedY -= impulseY;
                        }
                    }
                    else
                    {
                        Tile.Particle = null;
                        Tile = nextTile;
                        nextTile.Particle = this;
                    }
                }
                else
                {
                    double elasticity = 0.5 * (field.Tiles[Tile.X, Tile.Y + shift.Y].IsWall ? 1 : -1);
                    InertiaX *= shift.X == 0 ? 1 : elasticity;
                    InertiaY *= shift.Y == 0 ? 1 : -elasticity;
                    SpeedX *= shift.X == 0 ? 1 : elasticity;
                    SpeedY *= shift.Y == 0 ? 1 : -elasticity;
                }

                if (Math.Abs(InertiaX) > 1 || Math.Abs(InertiaY) > 1)
                    activeParticles.Push(this);
            }
        }

        public void TrySpread()
        {
            if (Density > 1)
            {
                int[] neigNums = MyRandom.GetMixedArray(new int[4] { 0, 1, 2, 3 });
                for (int i = 0; i < 4; i++)
                {
                    Tile tile = Tile.Neigs[neigNums[i]];
                    if (!tile.IsWall && tile.Particle == null)
                    {
                        Particle newPart = Create(this.GetType(), tile);
                        tile.Particle = newPart;
                        double k = ((Density - 1) / (double)Density);
                        DoublePoint Speed = new DoublePoint(SpeedX * k, SpeedY * k);
                        DoublePoint Inertia = new DoublePoint(InertiaX * k, InertiaY * k);
                        newPart.SpeedX += Speed.X;
                        newPart.SpeedY += Speed.Y;
                        newPart.InertiaX += Inertia.X;
                        newPart.InertiaY += Inertia.Y;
                        SpeedX -= Speed.X;
                        SpeedY -= Speed.Y;
                        InertiaX -= Inertia.X;
                        InertiaY -= Inertia.Y;
                        Density--;
                        return;
                    }
                }
                /*for (int i = 0; i < 4; i++)
                {
                    Tile tile = Tile.Neigs[neigNums[i]];
                    if (!tile.IsWall && tile.Particle != null)
                    {
                        tile.Particle.SpeedX += 0.2 * ((-neigNums[i] + 1) % 2) * 0.25 * Weight;
                        tile.Particle.SpeedY += 0.2 * ((neigNums[i] - 2) % 2) * 0.25 * Weight;
                    }
                }*/
                for (int i = 0; i < 4; i++)
                {
                    Tile tile = Tile.Neigs[neigNums[i]];
                    if (!tile.IsWall && tile.Particle != null && tile.Particle.GetType() == GetType())
                    {
                        tile.Particle.Density++;
                        Density--;
                        break;
                    }
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

        public static Particle Create(Type type, Tile tile)
        {
            Particle output;
            if (type == typeof(Water))
                output = new Water(tile);
            else if (type == typeof(Sand))
                output = new Sand(tile);
            else if (type == typeof(Dirt))
                output = new Dirt(tile);
            else
                throw new Exception();

            return output;
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
