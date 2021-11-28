using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MyTools;

namespace Physics
{
    public class Water : Particle
    {
        public Water(Tile tile) : base(tile)
        {
            Color = Color.Blue;
            Weight = 1;
            Fluidity = 3;
            Roughness = 0.1;
            Conductivity = 0.5;
        }
        public override void DoAction()
        {
            if (Temperature >= 3)
            {
                Particle newPart = new Steam(Tile);
                Tile.Particle = newPart;
                newPart.Temperature = Temperature;
            }
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
            Conductivity = 0.2;
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
            Conductivity = 0.5;
        }
    }
    public class TNT : Particle
    {
        public TNT(Tile tile) : base(tile)
        {
            Color = Color.Red;
            Weight = 2;
            Fluidity = 0.0;
            Roughness = 1;
            Conductivity = 0.5;
            Temperature = 2;
        }
        override public void DoAction()
        {
            if (Temperature > 3)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!Tile.Neigs[j].IsWall)
                    {
                        foreach (Tile tile in Tile.Neigs[j].Neigs)
                        {
                            if (tile.Particle != null)
                            {
                                tile.Particle.SpeedX += MyRandom.GetNumRange(-25, 25);
                                tile.Particle.SpeedY += MyRandom.GetNumRange(-25, 25);
                            }
                        }
                        if (Tile.Neigs[j].Particle != null)
                        {
                            Tile.Neigs[j].Particle.SpeedX += MyRandom.GetNumRange(-25, 25);
                            Tile.Neigs[j].Particle.SpeedY += MyRandom.GetNumRange(-25, 25);
                        }
                    }
                }
                Tile.Particle = null;
                Tile = null;
                return;
            }
        }
    }
    public class Lava : Particle
    {
        public Lava(Tile tile) : base(tile)
        {
            Color = Color.Orange;
            Weight = 2;
            Fluidity = 3;
            Roughness = 0.5;
            Conductivity = 0.3;

            Temperature = 5;
        }
        public override void DoAction()
        {
            if (Temperature < 4)
            {
                Particle newPart = new Stone(Tile);
                Tile.Particle = newPart;
                newPart.Temperature = Temperature;
            }
        }
    }
    public class Stone : Particle
    {
        public Stone(Tile tile) : base(tile)
        {
            Color = Color.Gray;
            Weight = 2;
            Fluidity = 0.001;
            Roughness = 0.5;
            Conductivity = 0.2;
        }
        public override void DoAction()
        {
            if (Temperature > 4)
            {
                Particle newPart = new Lava(Tile);
                Tile.Particle = newPart;
                newPart.Temperature = Temperature;
            }
        }
    }
    public class Steam : Particle
    {
        public Steam(Tile tile) : base(tile)
        {
            Color = Color.LightGray;
            Weight = -0.5;
            Fluidity = 2;
            Roughness = 0.1;
            Conductivity = 0.3;

            Temperature = 3;
        }
        public override void DoAction()
        {
            InertiaX = MyRandom.GetNumRange(-1, 1);
            SpeedY *= 0.5;
            if (MyRandom.Check(1000))
                Temperature--;
            if (Temperature < 2)
            {
                Particle newPart = new Water(Tile);
                Tile.Particle = newPart;
                newPart.Temperature = Temperature;
            }
        }
    }

    public class Bedrock : Particle
    {
        public Bedrock(Tile tile) : base(tile)
        {
            IsStatic = true;

            Weight = 0;
            Fluidity = 0;
            Roughness = 0.5;
            Conductivity = 0;
            Elasticity = 0.5;
        }
    }

    public class Cloner : Particle
    {
        public Cloner(Tile tile) : base(tile)
        {
            IsStatic = true;

            Weight = 0;
            Fluidity = 0;
            Roughness = 0.5;
            Conductivity = 0;
            Elasticity = 0.5;
        }

        public override void DoAction()
        {
            if (Tile.Neigs[1].Particle != null && !Tile.Neigs[1].IsWall)
            {
                foreach(Tile neig in Tile.Neigs)
                {
                    if (neig.Particle == null)
                    {
                        neig.Particle = Particle.Create(Tile.Neigs[1].Particle.GetType(), neig);
                    }
                }
            }
        }
    }
}
