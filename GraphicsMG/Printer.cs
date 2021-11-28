using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsMG
{
    static public class Printer
    {
        static public SpriteBatch SpriteBatch { get; set; }
        static public Texture2D TileSp { get; set; }

        static public SpriteFont Font { get; set; }

        static public Dictionary<Type, Microsoft.Xna.Framework.Color> GetConvertedColor = new Dictionary<Type, Color>
        {
            {typeof(Water), Color.Blue },
            {typeof(Dirt), Color.SandyBrown },
            {typeof(Sand), Color.Tan },
            {typeof(TNT), Color.Red },
            {typeof(Lava), Color.DarkOrange },
            {typeof(Stone), Color.Gray },
            {typeof(Steam), Color.LightGray },
            {typeof(Bedrock), Color.Gray },
            {typeof(Cloner), Color.Pink },
        };

        static public void DrawLevel(SpriteBatch spriteBatch, Field field)
        {
            SpriteBatch = spriteBatch;
            DrawTiles(field);
        }

        static void DrawTiles(Field field)
        {
            foreach(Tile tile in field.Tiles)
            {
                if (tile.Particle != null)
                    SpriteBatch.Draw(TileSp, new Vector2((float)tile.X, (float)tile.Y), GetConvertedColor[tile.Particle.GetType()]);
            }

            //инфа от тайла под курсором
            {
                MouseState mouse = Mouse.GetState();
                //Tile tile = field.GetTileFromCoord(mouse.X + Controller.Cam.Pos.X - Controller.Cam.ViewportWidth/2, mouse.Y + Controller.Cam.Pos.Y - Controller.Cam.ViewportHeight / 2);

                //SpriteBatch.DrawString(Font2, Math.Round(tile.Altitude).ToString(), new Vector2((float)tile.X, (float)tile.Y), Color.Black);
                //SpriteBatch.DrawString(Font2, tile.Resources[0].Type.ToString().Substring(0, 3), new Vector2((float)tile.X, (float)tile.Y), Color.Black);
                //SpriteBatch.DrawString(Font2, tile.alt.ToString(), new Vector2((float)tile.X, (float)tile.Y), Color.Black);
            }
        }

        static public void DrawTechInf(Field field)
        {
            MouseState mouse = Mouse.GetState();
            //Tile tile = Map.GetTileFromCoord(mouse.X, mouse.Y );

            //SpriteBatch.DrawString(Font2, tile.Altitude.ToString(), new Vector2((float)tile.X, (float)(tile.Y)), Color.Black);
            //SpriteBatch.DrawString(Font, mouse.X + " " + mouse.Y, new Vector2(mouse.X, mouse.Y - 20), Color.Black);
            //SpriteBatch.DrawString(Font2, Controller.Cam._pos.X + " " + Controller.Cam._pos.Y, new Vector2(mouse.X, mouse.Y - 20), Color.Black);
        }
    }
}
