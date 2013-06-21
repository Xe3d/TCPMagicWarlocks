using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tools_spritesheet
{
    class Starfield
    {
        //som vanligt
        private List<Sprite> stars = new List<Sprite>();
        public int screenWidth = 800;
        public int screenHeight = 600;
        private Random rand = new Random();
        private Vector2 pos;
        private Color[] colors = {
                                 Color.White, Color.Yellow, Color.Wheat, Color.WhiteSmoke, Color.SlateGray
                                 };

        //tagit bort bort velocity vector2n, då jag ville ha nytt randomvärde på varje enskild "stjärna"
        public Starfield(int screenWidth, int screenHeight, int starCount, Texture2D texture, Rectangle frameRectangle, Vector2 pos)
        {

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.pos = pos;
            for (int x = 0; x < starCount; x++)
            {
                //ändrat startpositionen för "stjärnorna" lite så att inte alla kommer från samma punkt, har även "hard-kodat" in en velocity med random här
               stars.Add(new Sprite(new Vector2(pos.X + screenWidth/2, pos.Y + screenHeight/2), texture, frameRectangle, new Vector2(rand.Next(-55, 55), rand.Next(-55, 55)), 0f));
                Color starColor = colors[rand.Next(0, colors.Count())];
                starColor *= (float)(rand.Next(30, 80) / 100f);
                stars[stars.Count() - 1].TintColor = starColor;
            }
            
        }


        public void Update(GameTime gameTime)
        {

            Rectangle rect = new Rectangle((int)pos.X,(int)pos.Y, screenWidth, screenHeight);

            foreach (Sprite star in stars)
            {
                star.Update(gameTime);


        //gjorde en egen if-sats för att kolla om "stjärnan" har kommit utanför "skärm-rutan"
                if (!rect.Contains((int)star.Position.X, (int)star.Position.Y))
                {
                    star.Position = new Vector2(pos.X + rand.Next(0, screenWidth+1), pos.Y + rand.Next(0, screenHeight+1));
                }

            }
        }

        //ritar ut dem som vanligt
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite star in stars)
            {
                star.Draw(spriteBatch);
            }
        }


    }
}
