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
        public double Roughness { get; protected set; }
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
                        double k = Weight > other.Weight ? 1 : 3;
                        if (other.GetType() != this.GetType() && shift.Y > 0 && Fluidity < other.Fluidity &&
                            40*k < (Math.Max(Math.Abs(SpeedX - other.SpeedX), Math.Abs(SpeedY - other.SpeedY)) * (Weight / other.Density) * (other.Fluidity / Fluidity)))
                        {
                            if (nextTile.Particle.TryDisplace())
                            {
                                Tile.Particle = null;
                            }
                            else
                            {
                                Tile.Particle = nextTile.Particle;
                                nextTile.Particle.Tile = Tile;
                            }
                            nextTile.Particle = this;
                            Tile = nextTile;

                            double impulseX = SpeedX * (Weight / other.Weight) * other.Roughness * 2 * Math.Abs(shift.X);
                            double impulseY = SpeedY * (Weight / other.Weight) * other.Roughness * 2 * Math.Abs(shift.Y);
                            other.SpeedX += impulseX;
                            other.SpeedY += impulseY;
                            SpeedX -= impulseX;
                            SpeedY -= impulseY;

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
        private bool TryDisplace()
        {
            int[] neigNums = MyRandom.GetMixedArray(new int[4] { 0, 1, 2, 3 });
            for (int i = 0; i < 4; i++)
            {
                Tile tile = Tile.Neigs[neigNums[i]];
                if (tile.Particle == null && !tile.IsWall)
                {
                    Tile.Particle = null;
                    tile.Particle = this;
                    Tile = tile;
                    return true;
                }
            }

            return false;
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
                for (int i = 0; i < 4; i++)
                {
                    Tile tile = Tile.Neigs[neigNums[i]];
                    if (!tile.IsWall && tile.Particle != null)
                    {
                        tile.Particle.SpeedX += 0.2 * ((-neigNums[i] + 1) % 2) * 0.25 * Weight;
                        tile.Particle.SpeedY += 0.2 * ((neigNums[i] - 2) % 2) * 0.25 * Weight;
                    }
                }
                /*for (int i = 0; i < 4; i++)
                {
                    Tile tile = Tile.Neigs[neigNums[i]];
                    if (!tile.IsWall && tile.Particle != null && tile.Particle.GetType() == GetType())
                    {
                        tile.Particle.Density++;
                        Density--;
                        break;
                    }
                }*/
            }
        }
        
        public void Move(Field field)
        {
            SpeedY += field.G;
            Fall(field);
            Rub();
            InertiaX += SpeedX;
            InertiaY += SpeedY;
        }
        private void Fall(Field field)
        {
            if (Math.Abs(SpeedX) < 2)
                if (Tile.Neigs[3].Particle != null)
                {
                    if (Tile.Neigs[0].Particle == null)
                        SpeedX += (Fluidity + MyRandom.GetNum0(Fluidity))*field.G;
                    if (Tile.Neigs[2].Particle == null)
                        SpeedX -= (Fluidity + MyRandom.GetNum0(Fluidity))*field.G;
                }
        }
        private void Rub()
        {
            for (int i = 0; i < 4; i++)
            {
                Tile tile = Tile.Neigs[i];
                Particle other = tile.Particle;
                if (other != null)
                {
                    double k = 0.25 * other.Roughness;
                    DoublePoint Impulse = new DoublePoint(SpeedX * k, SpeedY * k);
                    other.SpeedX += Impulse.X;
                    other.SpeedY += Impulse.Y;
                    SpeedX -= Impulse.X;
                    SpeedY -= Impulse.Y;
                }
            }
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
            Fluidity = 3;
            Roughness = 0.1;
        }
    }
    public class Sand : Particle
    {
        public Sand(Tile tile) : base(tile)
        {
            Color = Color.Tan;
            Weight = 2;
            Fluidity = 0.3;
            Roughness = 0.3;
        }
    }
    public class Dirt : Particle
    {
        public Dirt(Tile tile) : base(tile)
        {
            Color = Color.SandyBrown;
            Weight = 2;
            Fluidity = 0.05;
            Roughness = 0.5;
        }
    }
}
