using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace tools_spritesheet
{
    class ShootManager
    {
        //allt som vanligt
        public List<Sprite> Shots = new List<Sprite>();
        private Rectangle screenBounds;

        private static Texture2D Texture;
        private Rectangle InitialFrame;
        private static int FrameCount;
        private float shotSpeed;
        private static int CollisionRadius;


        public ShootManager(Texture2D texture, Rectangle initialFrame, int frameCount, int collisionRadius, float shotSpeed, Rectangle screenBounds)
        {
            Texture = texture;
            InitialFrame = initialFrame;
            FrameCount = frameCount;
            CollisionRadius = collisionRadius;
            this.shotSpeed = shotSpeed;
            this.screenBounds = screenBounds;
        }

        //samma som innan, den skjuter rätt genom att velociten skickas med från de olika managererna
        public void FireShot(Vector2 position, Vector2 velocity, bool playerFired, float rotation)
        {
            Sprite thisShot = new Sprite(position, Texture, InitialFrame, velocity, rotation);
            thisShot.Velocity *= shotSpeed;

            for (int x = 1; x < FrameCount; x++)
            {
                thisShot.AddFrame(new Rectangle(InitialFrame.X + (InitialFrame.Width * x), InitialFrame.Y, InitialFrame.Width, InitialFrame.Height));
            }

            thisShot.CollisonRadius = CollisionRadius;
            Shots.Add(thisShot);
        }

        //samma som vanligt
        public void Update(GameTime gameTime)
        {
            for (int x = Shots.Count - 1; x >= 0; x--)
            {
                Shots[x].Update(gameTime);
                if (!screenBounds.Intersects(Shots[x].Destination))
                    Shots.RemoveAt(x);
            }
        }

        //samma som vanligt
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite shot in Shots)
            {
                shot.Draw(spriteBatch);
            }
        }

    }



}
