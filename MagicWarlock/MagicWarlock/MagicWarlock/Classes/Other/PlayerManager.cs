using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace tools_spritesheet
{

    

    class PlayerManager
    {

    //Typ samma som vanligt men ökad hastighet och rotation och lite småändringar

        Texture2D texture;
        public Vector2 position;
        Vector2 velocity;
        Rectangle sourceRect;
        public int spriteWidth = 32;
        public int spriteHeight = 32;
        int spriteSpeed = 120;
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.5f;
        public ShootManager playerShotManager;
        Rectangle screenBounds;
        int numOfFrames;
        Rectangle Rect;
        public float rotation;
        public bool Destroyed = false;
        public int CollisionRadius = 20;
        
    
        //livvariabel
        public int life = 3;

        //som vanligt
        float timer = 0f;
        float interval = 80f;
        int currentFrame = 0;

        //ljudeffekt
        SoundEffect shootFX;

        //vanliga keyboard variabler, används inte så mycket/alls
        KeyboardState keyboard;
        KeyboardState prevKeyboard;

        //en målsPosition och en bool som säger om spelaren är på väg någonstans.
        Vector2 targetPos;
        bool onWay;

        //en get/set Positions variabel som sätter positionen till mitten av karaktären så att den funkar bra med en central "Source-punkt" i draw
        public Vector2 Position
        {
            get { return position; }
            set { position = new Vector2(value.X + spriteWidth / 2, value.Y + spriteHeight / 2); }
        }

        //Ganska vanliga Get/set variabler, men en modifierad Center, så source punkten är i mitten
        public Vector2 Velicity
        {	
            get{return velocity;}
            set{velocity = value;}	
        }
 
        public Texture2D Texture
        {	
            get{return texture;}
            set{texture = value;}	
        }

        public Rectangle SourceRect
        {
            get { return sourceRect;}
            set { sourceRect = value;}
        }

        public Vector2 Center 
        {
            get
            {
                return position;
            }
        }

        

        public PlayerManager(Texture2D texture, int currentFrame, int spriteWidth, int spriteHeight, Rectangle screenBounds, ShootManager shootManager, SoundEffect soundFX)
        {
            //Samma som vanligt, plus ljud effekt
            this.texture = texture;
            this.currentFrame = currentFrame;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.screenBounds = screenBounds;
            this.playerShotManager = shootManager;
            numOfFrames = texture.Width / spriteWidth;
            shootFX = soundFX;
            
        }


       

        public void handleSpriteMovement(GameTime gameTime)
        {
            if (!Destroyed)
            {
                //som vanligt
                prevKeyboard = keyboard;
                keyboard = Keyboard.GetState();

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timer > interval)
                {
                    currentFrame++;

                    if (currentFrame > numOfFrames - 1)
                        currentFrame = 1;

                    timer = 0f;
                }


                //om onWay är true, gå mot positionen tagen från ratationen mot målet

                if (onWay)
                {

                    position.X -= (float)Math.Cos(rotation) * spriteSpeed * elapsed;
                    position.Y -= (float)Math.Sin(rotation) * spriteSpeed * elapsed;
                }



            //om gubbern kommer fram, sätt onWay till false
                if (Rect.Contains((int)targetPos.X, (int)targetPos.Y))
                {
                    onWay = false;
                }

                //som vanligt
                sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);


                if (keyboard.IsKeyDown(Keys.Right) == true)
                {


                    position.X += spriteSpeed;
                }


                if (keyboard.IsKeyDown(Keys.Left) == true)
                {


                    position.X -= spriteSpeed;
                }

                if (keyboard.IsKeyDown(Keys.Down) == true)
                {


                    position.Y += spriteSpeed;
                }

                if (keyboard.IsKeyDown(Keys.Up) == true)
                {


                    position.Y -= spriteSpeed;
                }
                //bort kommenterad så att man inte kan skjuta med space
                if (keyboard.IsKeyDown(Keys.Space) == true)
                {
                    //   FireShot();
                }

                
                //velocity variabel som inte riktigt används
                velocity = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            }
        }

    //Får spelaren att gå mot positionen som matas in i funktionen, fixar även rotationen
        public void goTo(Vector2 goToPos)
        {

            targetPos = goToPos;
            onWay = true;

            Vector2 facing;
            facing = position - targetPos;

            rotation = (float)Math.Atan2(facing.Y, facing.X);


        }
        

        //skjuter ett skott mot målpositionen, roterar det så att det blir snyggt och spelar ljudeffekten
        public void FireShot(Vector2 Target)
        {

            if ((shotTimer >= minShotTimer)&&(!Destroyed))
            {

                Vector2 direction;
                float way;
                direction = position - Target;

                way = (float)Math.Atan2(direction.Y, direction.X);


                Vector2 gunOffset;
                float offset = 1;
                shootFX.Play(0.7f,0f, 0.0f);
                gunOffset = new Vector2((float)Math.Cos(way+offset), (float)Math.Sin(way+offset));

                playerShotManager.FireShot(position + gunOffset, new Vector2(-(float)Math.Cos(way), -(float)Math.Sin(way)), true, way);
                shotTimer = 0.0f;

            }
        }

        public void Update(GameTime gameTime)
        {

            playerShotManager.Update(gameTime);

            //gör en hitboxrektangel baserad på spelarens position, och storlek, men denna kan dock inte roteras, så den har en viss felmarginal i hitboxsynpunkt
            Rect = new Rectangle((int)position.X-spriteWidth/2, (int)position.Y-spriteHeight/2, spriteWidth, spriteHeight);

            
           
            shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //om livet går under 1, sätt Destroyed till true
            if (life < 1)
            {
                Destroyed = true;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //rita ut skotten
            playerShotManager.Draw(spriteBatch);

            if (!Destroyed)
            {
                //rita ut gubben, med rotation och allt annat.
                spriteBatch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight), SourceRect, Color.White, rotation - ((float)Math.PI * 0.5f), new Vector2(spriteWidth / 2, spriteHeight / 2), SpriteEffects.None, 1f);
            }
        }


    }
}
