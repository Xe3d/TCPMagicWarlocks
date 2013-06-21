using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace tools_spritesheet
{
    class Enemy
    {
        //samma som i orginalet plus variabel för liv, och en för att se om spelaren har slufört vägen, och isf matar in koordinaterna igen.
        public Sprite EnemySprite;
        public Vector2 gunOffset = new Vector2(25, 25);
        private Queue<Vector2> waypoints = new Queue<Vector2>();
        private Vector2 currentWaypoint = Vector2.Zero;
        private float speed = 120f;
        public bool Destroyed = false;
        private int enemyRadius = 15;
        private Vector2 previousPoition = Vector2.Zero;
        public bool restart;
        public int life = 3;
        

        //konstruktor för en fiende
        public Enemy (Texture2D texture, Vector2 position, Rectangle initialFrame, int frameCount)
        {

            EnemySprite = new Sprite(position, texture, initialFrame, Vector2.Zero, 0f);

            for (int i = 1; i < frameCount; i++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X = initialFrame.Width *i, initialFrame.Y, initialFrame.Width, initialFrame.Height));

            }

            previousPoition = position;
            currentWaypoint = position;
            EnemySprite.CollisonRadius = enemyRadius;

        }

        //sätt en ny waypoint i kö
        public void AddWaypoint(Vector2 waypoint)
        {
            waypoints.Enqueue(waypoint);
        }

        //om fienden har kommit fram till waypointen returna boolen som true, annars returna som false.
        public bool WaypointReached()
        {
            if (Vector2.Distance(EnemySprite.Position, currentWaypoint) < (float)EnemySprite.Source.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //kollar om fienden ska vara aktiv eller inte, om det inte finns några waypoints kvar, livet är under 1, eller den förstörs blir boolen false.
        public bool isActive()
        { 
            if (life < 1)
            {
                return false;
            }
            if (Destroyed)
            {
                return false;
            }

            if (waypoints.Count > 0)
            {
                return true;
            }

            if (WaypointReached())
            {
               return false;
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            //samma som vanlig förutom om waypoints är noll eller mindre, sätt då restart till true
            if (isActive())
            {
                Vector2 heading = currentWaypoint - EnemySprite.Position;
                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }
                heading *= speed;
                EnemySprite.Velocity = heading;
                previousPoition = EnemySprite.Position;
                EnemySprite.Update(gameTime);
                EnemySprite.Rotation = (float)Math.Atan2(EnemySprite.Position.Y - previousPoition.Y, EnemySprite.Position.X - previousPoition.X);

                if (WaypointReached())
                {
                    if (waypoints.Count > 0)
                    {
                        currentWaypoint = waypoints.Dequeue();
                    }
                    if (waypoints.Count <= 0)
                    restart = true;
                        
                }
            }
        }

        //samma som vanligt
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive())
            {
                EnemySprite.Draw(spriteBatch);
            }
        }

    }
}
