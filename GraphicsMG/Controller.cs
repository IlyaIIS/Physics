using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsMG
{
    static public class Controller
    {
        static private KeyboardState keyState;
        static private MouseState mouseState;
        static public Camera Cam { get; set; }
        static private int preMouseScroll = 0;
        static Dictionary<Keys, bool> wasKeyPressed = new Dictionary<Keys, bool>()
        {
            { Keys.Q, false},
            { Keys.E, false},
            { Keys.R, false},
            { Keys.G, false},
        };
        static bool wasLeftPressed = false;

        static public void CheckKeyActions(Field field)
        {
            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            CheckCamMove();
            CheckCamZoom();
            CheckMouseActions(field);
            PerformKeyActions(field);
        }
        static void CheckMouseActions(Field field)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Tile tile = GetTileUnderMouse(field);
                if (tile != null && !tile.IsWall && tile.Particle == null)
                {
                    Particle particle;
                    particle = new Sand(tile);
                    particle.SpeedX = 0;
                    particle.SpeedY = 0;
                    tile.Particle = particle;
                }
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Tile tile = GetTileUnderMouse(field);
                if (tile != null && !tile.IsWall && tile.Particle == null)
                {
                    Particle particle;
                    particle = new Dirt(tile);
                    particle.SpeedX = 0;
                    particle.SpeedY = 0;
                    tile.Particle = particle;
                }
            }
        }
        static void PerformKeyActions(Field field)
        {
            DoActionIfKeyReleased(Keys.G, () => { field.G = field.G == 0 ? 1: 0 ; });
        }

        static void DoActionIfKeyReleased(Keys key, Action Action)
        {
            if (!keyState.IsKeyDown(key))
            {
                if (wasKeyPressed[key])
                {
                    wasKeyPressed[key] = false;
                    Action();
                }
            }
            else
            {
                wasKeyPressed[key] = true;
            }
        }
        static void CheckCamMove()
        {
            float speed = 10;

            if (keyState.IsKeyDown(Keys.Right))
                Cam.Move(new Vector2(speed, 0));
            if (keyState.IsKeyDown(Keys.Up))
                Cam.Move(new Vector2(0, -speed));
            if (keyState.IsKeyDown(Keys.Left))
                Cam.Move(new Vector2(-speed, 0));
            if (keyState.IsKeyDown(Keys.Down))
                Cam.Move(new Vector2(0, speed));
        }

        static void CheckCamZoom()
        {
            if (mouseState.ScrollWheelValue > preMouseScroll)
                Cam.Zoom += 0.1f * Cam.Zoom;
            if (mouseState.ScrollWheelValue < preMouseScroll)
                Cam.Zoom -= 0.1f * Cam.Zoom;

            preMouseScroll = mouseState.ScrollWheelValue;
        }


        static public Tile GetTileUnderMouse(Field field)
        {
            MouseState mouse = Mouse.GetState();
            int x = (int)((mouse.X + Cam.Pos.X - Cam.ViewportWidth / 2) / Cam.Zoom);
            int y = (int)((mouse.Y + Cam.Pos.Y - Cam.ViewportHeight / 2) / Cam.Zoom);
            if (x >= 0 && x < field.Width && y >= 0 && y < field.Height)
                return field.Tiles[x, y];
            else
                return null;
        }
    }
}
