using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace TestGame.TileEngine
{
    class Tile
    {

        //deklarerar variabler

        public bool Landable { get; set; }
        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        //en hitbox för rutan/tilen
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)this.Position.X, (int)this.Position.Y, this.SpriteWidth, this.SpriteHeight); }
        }

   
        //Konstruktor som är ganska lättförståerlig, Landable-boolen använd endast i sidescolling med "fysik"
        public Tile(Texture2D texture, int spriteWidth, int spriteHeight, Vector2 startingPosition, bool isLandable)
        {
            this.Texture = texture;
            this.SpriteWidth = spriteWidth;
            this.SpriteHeight = spriteHeight;
            this.Position = startingPosition;
            this.Landable = isLandable;
        }

        //ritar ut tilen
        public void Draw(SpriteBatch sb)
        {
            //scale används här om din textur är för stor/liten för din spriteHeight/SpriteWidth
            Vector2 scale = new Vector2(SpriteWidth / (float)this.Texture.Width, SpriteHeight / (float)this.Texture.Height);
            sb.Draw(this.Texture, this.Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        }
    }
}
