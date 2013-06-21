using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using MagicWarlock.Classes.Other;

namespace tools_spritesheet
{
    class CollisonsManager
    {

        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private ExplosionManager explosionManager;

        //ljudeffekt för träff av skott
        public SoundEffect hit;

        //skapar en vector2 med en position som är utanför skärmen och som får ett skott att bli borttaget om det har denna position
        private Vector2 offScreen = new Vector2(-500, -500);

        //konstruktorn till collisionmanagern
        public CollisonsManager(PlayerManager playerSprite, EnemyManager enemyManager, ExplosionManager explosionManager)
        {
            this.playerManager = playerSprite;
            this.enemyManager = enemyManager;
            this.explosionManager = explosionManager;
        }

        private void checkShotForEnemyCollisons()
        {
            foreach (Sprite shot in playerManager.playerShotManager.Shots)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(enemy.EnemySprite.Center, enemy.EnemySprite.CollisonRadius))
                    {
                        //om något skott som spelaren har skjutit träffar en fiende så ska skottet flyttas utanför skärmen så att det blir borttaget, fiended förlorar ett liv, explosionmanagern visar en explosion, och ljudeffekten spelas.
                        shot.Position = offScreen;
                        enemy.life--;
                        explosionManager.AddExplosion(enemy.EnemySprite.Center, enemy.EnemySprite.Velocity / 10);
                        hit.Play();
                    }
                }
            }
        }

        private void checkShotToPlayerCollisions()
        {
            foreach (Sprite shot in enemyManager.EnemyShotManager.Shots)
            { 
                if(shot.IsCircleColliding(playerManager.Center, playerManager.CollisionRadius))
                {
                    //om något skott som en fiende har skjutit träffar spelaren så ska skottet flyttas utanför skärmen så att det blir borttaget, spelaren förlorar ett liv, explosionmanagern visar en explosion, och ljudeffekten spelas.
                    shot.Position = offScreen;
                    playerManager.life--;
                    explosionManager.AddExplosion(playerManager.Center, Vector2.Zero);
                    hit.Play();
                }
            }
        }


        private void checkEnemyToPlayerCollisions()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                if (enemy.EnemySprite.IsCircleColliding(playerManager.Position, playerManager.CollisionRadius))
                {
                    //detta kommer inte att synas då så fort spelaren förstörs så slutförs spelrundan.

                    //om en fiende och spelaren kolliderar så ska båda två förstöras, och explosionmanagern visar en explosion på de.
                    enemy.Destroyed = true;
                    playerManager.Destroyed = true;
                    explosionManager.AddExplosion(playerManager.Center, Vector2.Zero);
                    explosionManager.AddExplosion(enemy.EnemySprite.Center, enemy.EnemySprite.Velocity / 10);
                }
            }
        
        
        }

        public void initiateSFX(SoundEffect traff)
        {
            //tar den ljud effekt som matas in och gör ljudeffekten som används i collisionManager en kopia av den ljudeffekten.
            hit = traff;
        }


        //den funktion som används i Game1.update, och som uppdaterar sammtliga kollisionsfunktioner
        public void CheckCollisions()
        {
            checkShotForEnemyCollisons();

            if (!playerManager.Destroyed)
            {
                checkShotToPlayerCollisions();
                checkEnemyToPlayerCollisions();
            }
        }

    }
}
