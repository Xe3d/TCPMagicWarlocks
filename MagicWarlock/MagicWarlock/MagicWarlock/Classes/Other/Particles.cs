using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tools_spritesheet;

namespace MagicWarlock.Classes.Other
{
    class Particles:Sprite
    {
        private Vector2 acceleration;
        private float maxSpeed;
        private int initialDuration;
        private int remainingDuration;
        private Color initialColor;
        private Color finalColor;


        public int ElapsedDuration
        {
            get { return initialDuration - remainingDuration; }
        }

        public float DurationProgress
        {
            get { return (float)ElapsedDuration / (float)initialDuration; }
        }

        public bool isActive
        {
            get { return remainingDuration > 0; }
        }

        //är inte säker på att detta är korrekt men, konstruktor till particles, som är en förlängning på Sprite, så äen sprite skapas samtidigt
        public Particles(Vector2 location, Texture2D texture, Rectangle initialFrame, Vector2 velocity, Vector2 acceleration, float maxSpeed, int duration, Color initialColor, Color finalColor)
            : base(location, texture, initialFrame, velocity, 0f)
        {
            initialDuration = duration;
            remainingDuration = duration;
            this.acceleration = acceleration;
            this.initialColor = initialColor;
            this.maxSpeed = maxSpeed;
            this.finalColor = finalColor;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive) {
                velocity += acceleration;
                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }
                TintColor = Color.Lerp(initialColor, finalColor, DurationProgress);
                remainingDuration--;

                //Uppdaterar basen, som är Sprite
            base.Update(gameTime);
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                //Ritar ut basen, som är Sprite
                base.Draw(spriteBatch);
            }
        }


    }
}
