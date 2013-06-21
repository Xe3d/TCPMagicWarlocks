using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace tools_spritesheet
{
    class EnemyManager
    {

    //Samma som vanligt fast med lite ändrade värden i variablerna, mindre "skepp", och högre skottchans osv.

        private Texture2D texture;
        private Rectangle initialFrame;
        private int frameCount;

        public List<Enemy> Enemies = new List<Enemy>();
        public ShootManager EnemyShotManager;
        private PlayerManager playerManager;

        public int MinShipsPerWave = 1;
        public int MaxShipsPerWave = 1;
        private float nextWaveTimer = 0.0f;
        private float nextWaveMinTimer = 2.0f;
        private float shipSpawnTimer = 0.0f;
        private float shipSpawnWaitTime = 0.0f;
        private int numberInWave = 1;
        
        private float shipShotChanse = 0.5f;
        private List<List<Vector2>> pathWaypoints = new List<List<Vector2>>();
        private Dictionary<int, int> waveSpawns = new Dictionary<int, int>();
        public bool Active = true;
        private Random rand = new Random();


    //ny variabel är spawnQueue, som skapar nya fiender som inte blir staplade på varandra, hjälper nu till med spawning av fiendevågor
        private int spawnQueue = 0;

        //ny variabel shootFX, är ljudeffekt för när du skjuter iväg ett skott
        private SoundEffect shootFX;

        //levelvariabel som börjar på 1
        public int level = 1;


        //finns endast en väg de kan ta, och är gjord så att den loopar snyggt
        public void setUpWaypoints()
        {
            List<Vector2> path0 = new List<Vector2>();
            path0.Add(new Vector2(95, 95));
            path0.Add(new Vector2(670, 95));
            path0.Add(new Vector2(670, 670));
            path0.Add(new Vector2(95, 670));
            pathWaypoints.Add(path0);
            waveSpawns[0] = 0;
        }

        public EnemyManager(Texture2D texture, Rectangle initialFrame, int frameCount, PlayerManager playerSprite, Rectangle screenBounds, ShootManager shootManager, SoundEffect soundFX)
        {

        //samma som vanliga plus importering av ljudeffekt
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.frameCount = frameCount;
            this.playerManager = playerSprite;
            this.EnemyShotManager = shootManager;

            shootFX = soundFX;
            setUpWaypoints();

        }

        //samma som vanliga
        public void SpawnEnemy(int path)
        {
            Enemy thisEnemy = new Enemy(texture, pathWaypoints[path][0], initialFrame, frameCount);

            for (int x = 0; x < pathWaypoints[path].Count(); x++)
            {
                thisEnemy.AddWaypoint(pathWaypoints[path][x]);
            }
            Enemies.Add(thisEnemy);
        }

        //samma som vanliga
        public void SpawnWave(int waveType)
        {
            waveSpawns[waveType] += rand.Next(MinShipsPerWave, MaxShipsPerWave + 1);
        }

        //samma som vanliga fast utan if-sats med timern mellan spawns, och den används bara för nextWaveTimer, men hela funktionen är kvar pga eventuell funktionalitet i framtiden 
        private void updateWaveSpawns(GameTime gameTime)
        {
            shipSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (shipSpawnTimer > shipSpawnWaitTime)
            {
                for (int x = waveSpawns.Count - 1; x >= 0; x--)
                {
                    if (waveSpawns[x] > 0)
                    {
                        waveSpawns[x]--;
                        SpawnEnemy(x);
                    }
                }
                shipSpawnTimer = 0f;
            }

            nextWaveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            

        }

        public void Update(GameTime gameTime)
        {
            EnemyShotManager.Update(gameTime);

            //om enemie.count är lika med 0, ta spawnQueue till dubbla antalet fiender i förra runden, dubbla sedan numberInWave så att den ökar mellan alla rundor, och öka sedan levelvariablen.
            if (Enemies.Count == 0)
            {
                spawnQueue = numberInWave * 2;
                numberInWave *= 2;
                level += 1;
            }

            //om spawnQueue är över 0, nextWaveTimer är över 1, så spawna en fiende på path 0, minska spawnQueue med en och sätt nextWaveTimer till 0
            if ((spawnQueue > 0))
            { 

                if(nextWaveTimer > 1f)
                {
                    SpawnEnemy(0);
                    spawnQueue--;
                    nextWaveTimer = 0f;
                }
                

                
            }

            //det mesta är normalt, förutom skjutdelen, och restart-delen
            for (int x = Enemies.Count - 1; x >= 0; x--)
            {
                Enemies[x].Update(gameTime);

                if (Enemies[x].isActive() == false)
                {
                    Enemies.RemoveAt(x);
                }
                else
                {
                    if ((float)rand.Next(0, 1000) / 10 <= shipShotChanse)
                    {
                        //när fienden skjuter, så skjuter den nu mot spelaren, och den roterar eldklotet(skottet) till den lämpliga vinkeln så att det blir snyggt, och den spelar en ljudeffekt

                        Vector2 fireLoc = Enemies[x].EnemySprite.Position;
                        fireLoc += Enemies[x].gunOffset;

                        Vector2 shotDirection = playerManager.position - fireLoc;

                        float rot = (float)Math.Atan2(shotDirection.Y, shotDirection.X);

                        shotDirection.Normalize();
                        shootFX.Play(0.7f, 0f, 0.0f);
                        EnemyShotManager.FireShot(fireLoc, shotDirection, false, rot-((float)Math.PI));
                    }

                    if (Enemies[x].restart)
                    {
                         
                        for (int i = 0; i < pathWaypoints[0].Count(); i++)
                        {
                            Enemies[x].AddWaypoint(pathWaypoints[0][i]);
                        }
                        

                        Enemies[x].restart = false;

                    }

                }

                

            }

            //uppdaterar den näst intil onödiga funktionen updateWaveSpawns
            if (Active)
            {
                updateWaveSpawns(gameTime);

            }

        }

        //ritar ut det som vanligt
        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShotManager.Draw(spriteBatch);

            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
