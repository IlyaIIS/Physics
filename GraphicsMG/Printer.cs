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

        static public Dictionary<System.Drawing.Color, Microsoft.Xna.Framework.Color> GetConvertedColor = new Dictionary<System.Drawing.Color, Color>
        {
            {System.Drawing.Color.Blue, Color.Blue },
            {System.Drawing.Color.SandyBrown, Color.SandyBrown },
            {System.Drawing.Color.Tan, Color.Tan },
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
                if (tile.IsWall)
                    SpriteBatch.Draw(TileSp, new Vector2((float)tile.X, (float)tile.Y), Color.Gray);
                else if (tile.Particle != null)
                    SpriteBatch.Draw(TileSp, new Vector2((float)tile.X, (float)tile.Y), GetConvertedColor[tile.Particle.Color]);

                /*if (!tile.IsLand)
                    SpriteBatch.Draw(TileSp, new Vector2((float)tile.pX, (float)tile.pY), tile.Color[0].GetMGColor());
                else
                    SpriteBatch.Draw(TileSp, new Vector2((float)tile.pX, (float)tile.pY), tile.Color[Settings.MapMode].GetMGColor());
                */
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
