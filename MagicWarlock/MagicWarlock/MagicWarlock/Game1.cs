using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TestGame.TileEngine;
using tools_spritesheet;
using MagicWarlock.Classes.Other;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace MagicWarlock
{
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Tile> tileList = new List<Tile>();
        Color TardisBlue;

        //gamestates
        enum GameStates { 
           playScreen,
           menuScreen,
           endScreen

        }

        
    //Keyboardstate
        KeyboardState keyboard, prevKeyboard;

        //initierar en gamestate variabel
        GameStates GameState;

        //skapar en playermanager
        PlayerManager Player, Enemy;

        //skapar en shootmanager till spelarens playermanager
        ShootManager playerShootManager, testManager;

        //initerar en enemymanager
        EnemyManager enemyManager;

        //skapar en shootmanager till enemymanagern
        ShootManager enemyShootManager;

        //skapar en mousestate som tar hand om inmatning av data från musen
        MouseState mouse;

        //skapr en instatns av kollisionmanager
        CollisonsManager collisionManager;

        //skapar en explosion manager
        ExplosionManager explosionManager;

        //skapar två starfield en för spelaren och en för fienderna, dessa kommer att agera livmätare.
        Starfield playerLife, enemyLife;

        //skapar en tillfällig variabel som räknas den totala hälsonivåån bland alla aktiva fiender
        int tempEnemyLife;

        //skapar en rektangel som kommer att representera spelplansområdet
        Rectangle screenRect;

        //skapar ett typsnitt som kommer att användas vid utskrivning av test i spelet.
        SpriteFont Arial;

        //skapar en "kodregion" för koden för spelplanen så att denna lätt kan gömmas.
        #region Map

        //skapar en multidimensionel integer-array som fylls med siffror som hänvisar till olika slags bildrutor som rutas ut och som utgör spelplanen.
        int[,] map = new int[,]
            {                                                                                                                                                                                                /*introduce enemies here*/                                                                                                                                                                                                                  /*introduce enemies here*/                                   
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };
        #endregion


        


        #region Network

        TcpClient client;

        String IP = "78.82.166.121";
        int PORT = 1490;
        int BUFFER_SIZE = 2048;
        byte[] readBuffer;

        MemoryStream readStream, writeStream;

        BinaryReader reader;
        BinaryWriter writer;

        #endregion


        public Game1()
        {
            //initierar en ny GraphicsDeviceManger som heter graphics
            graphics = new GraphicsDeviceManager(this);

            //sätter Content till rootmapp till ContentManagern Content
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            #region Network
           

            readStream = new MemoryStream();
            writeStream = new MemoryStream();

            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);
            #endregion
            //kallar Microsoft.Xna.Framework.Game.Initialize
            base.Initialize();
        }



        //skapar en region kallad Tile
        #region Tile 

            //skapar en funktion kallad Createflor
                private void createMap()
        {
                    //Sätter integern TileMapWidth till längden på den andra dimensionen i arrayen map.
            int tileMapWidth = map.GetLength(1);

            //Sätter integern TileMapHeight till längden på den första dimensionen i arrayen map.
            int tileMapHeight = map.GetLength(0);

                    //en for-loop som loopar igenom banans höjd
            for (int y = 0; y < tileMapHeight; y++)
            {
                //en for-loop som loopar igenom banans bredd
                for (int x = 0; x < tileMapWidth; x++)
                {
                    //en integer som anger värdet i en specifik kordinat i banans array
                    int textureIndex = map[y, x];


                    //ett switch-statment som kollar vilket värde den aktualla koordinaten i arrayen har
                    switch (textureIndex)
                    {
                            //om värdet är 1, lägg ut en ruta med trätextur
                        case 1:
                            Tile tile;

                            
                            //skapa en ruta på den aktualla kooridanten gånger storleken på alla rutor som är 48px
                            tile = new Tile(Content.Load<Texture2D>("Assets/img/wood"), 48, 48, new Vector2(48 * x, 48 * y), true);

                            //lägg till rutan i en lista för rendering
                            tileList.Add(tile);

                            //skriv ut den aktuella koordinaten och dess värde
                            Console.WriteLine("x=" + x + " y=" + y + " is " + textureIndex);

                            break;

                            //om det är 2, lägg ut en ruta med kartonglådetextur
                        case 2:


                            //skapa en ruta på den aktualla kooridanten gånger storleken på alla rutor som är 48px
                            tile = new Tile(Content.Load<Texture2D>("Assets/img/Crate"), 48, 48, new Vector2(48 * x, 48 * y), true);
                           
                            //lägg till rutan i en lista för rendering
                            tileList.Add(tile);

                            //skriv ut den aktuella koordinaten och dess värde
                            Console.WriteLine("x=" + x + " y=" + y + " is " + textureIndex);
                            break;

                        default:
                            //om värdet i koordinaten är något annat än vad som spelet använder, skriv ut koordinaten och dess värde.
                            Console.WriteLine("x=" + x + " y=" + y + " is " + textureIndex);
                            break;
                    }



                }
            }

        }

        #endregion





        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //skapar en ny spriteBatch, som används för att rita ut objekt
            spriteBatch = new SpriteBatch(GraphicsDevice);

            client = new TcpClient();
            client.NoDelay = true;
            
                client.Connect(IP, PORT);
           
            readBuffer = new byte[BUFFER_SIZE];
            client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);

            //Skapar en ny färgvariabel, kallad TardisBlue, med ett rgb värde av 0, 59, 111
            TardisBlue = new Color(0, 59, 111);

            //ändrar de föredragna fönsterdimensionerna till 720x720, och värkställer sedan ändringarna.
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 720;
            graphics.ApplyChanges();

            //sätter variabeln GameState till GameStates.menuScreen
            GameState = GameStates.menuScreen;

            //laddar det importerade typsnittet i Content kallat Arial till typsnittsvariabeln Arial
            Arial = Content.Load<SpriteFont>(@"Assets/Arial");
           
            //kallar funktionen createMap som skapar en spelplanen med rutor(Tiles)
            createMap();

            //gör så att man ser muspekaren
            IsMouseVisible = true;


          


           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //fyller variabeln keyboard med tangentbordets aktuella status 
            keyboard = Keyboard.GetState();

            //fyller variabeln mouse med musens aktuella status 
            mouse = Mouse.GetState();


            //stänger ner spelet om Back knappen trycks på en spelkontroll eller Escape på ett tangentbord 
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)||keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                this.Exit();

            //om enter trycks ned och gameState inte är på playScreen, sätt gameState till playScreen och kör setUp-funktionen
            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)&&(GameState !=GameStates.playScreen))
            {
                GameState = GameStates.playScreen;
                setUp();
                
            }

            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C) && !prevKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C)&&!client.Connected)
            {
                client.Connect(IP, PORT);
                client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
            }

            if (keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && !prevKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && client.Connected)
            {
                
                
             //   client.Close();
                
            }

            //om spelet är på spelskärmen playScreen gör dessa saker
            if (GameState == GameStates.playScreen)
            {
                Vector2 iPos = Player.Position;

                if (screenRect.Contains(mouse.X, mouse.Y))
                {
                    //om höger musknapp trycks ner, säg åt spelaren att gå till musens koordinater genom PlayerManager.goTo funktionen
                    if (mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        Player.goTo(new Vector2(mouse.X, mouse.Y));
                }
                if (screenRect.Contains(mouse.X, mouse.Y))
                {
                    //om vänster musknapp trycks ner, säg åt spelaren att skjuta mot musens koordinater genom PlayerManager.FireShot funktionen
                    if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        Player.FireShot(new Vector2(mouse.X, mouse.Y));
                    }
                }
                //sätt höjden på spelarens livmätare till antalet liv gånger 30px
                playerLife.screenHeight = Player.life * 30;

                //plussa på varje fiendes liv på integeren tempEnemyLife så att fiendens livmätare kan representera alla fienders liv
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    tempEnemyLife += enemy.life;
                }

                //sätt höjden på fiendernas livmätare till fiendernas totala liv gågner 30px
                enemyLife.screenHeight = tempEnemyLife * 30;

                

                //om spelaren dör, gå till slutsidan
                if (Player.Destroyed)
                {
                    GameState = GameStates.endScreen;
                }

                //uppdatera spelarens rörelser
                Player.handleSpriteMovement(gameTime);

                //uppdatera resterande saker knuta till spelaren
                Player.Update(gameTime);

                //uppdatera explosionManagern, som tar hand om alla explosioner, som i mitt spel agerar blodmoln
                explosionManager.Update(gameTime);

                if (Enemy != null)
                {
                    Enemy.Update(gameTime);
                    Enemy.handleSpriteMovement(gameTime);
                }
           /*     //uppdaterar enemyManagern, som tar hand om alla fiender
                enemyManager.Update(gameTime);

                //uppdaterar collisionManager, som tar hand om alla collisioner mellan objekt
                collisionManager.CheckCollisions();

                //uppdaterar det starField som representerar spelarens liv
                playerLife.Update(gameTime);

                //uppdaterar det starField som representerar fiendernas liv
                enemyLife.Update(gameTime);

                //rensar tempEnemyLife för att få antalet liv alla fiender just nu
                tempEnemyLife = 0;*/

                Vector2 nPos = Player.Position;

                Vector2 deltaPos = Vector2.Subtract(nPos, iPos);

                if (deltaPos != Vector2.Zero)
                {
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.PlayerMoved);
                    writer.Write(nPos.X);
                    writer.Write(nPos.Y);
                    writer.Write(Player.rotation);
                    SendData(GetDataFromMemoryStream(writeStream));
                }

            }
            
            prevKeyboard = keyboard;
            //kallar Microsoft.Xna.Framework.Game.Update
            base.Update(gameTime);
        }



#region Network

        private void StreamReceived(IAsyncResult ar)
        {
            if (client.Connected)
            {
                int bytesRead = 0;

                try
                {
                    lock (client.GetStream())
                    {
                        bytesRead = client.GetStream().EndRead(ar);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (bytesRead == 0)
                {
                    client.Close();
                    return;
                }

                byte[] data = new byte[bytesRead];

                for (int i = 0; i < bytesRead; i++)
                {
                    data[i] = readBuffer[i];

                }

                ProcessData(data);

                client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
            }
        }
        
        
        private void ProcessData(byte[] data)
        {
            readStream.SetLength(0);
            readStream.Position = 0;

            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;

            Protocol p;

            try
            {
                p = (Protocol)reader.ReadByte();

                if (p == Protocol.Connected)
                {
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    
                    if (Enemy == null)
                    {
                        testManager = new ShootManager(Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(256, 48, 32, 48), 1, 16, 250f, screenRect);

                        Enemy = new PlayerManager(Content.Load<Texture2D>("Assets/img/runer"), 1, 32, 48, screenRect, testManager, Content.Load<SoundEffect>(@"Assets/sfx/skjut"));
                        Enemy.Position = new Vector2(180, 300);

                        if (Player==null)
                        {
                            Enemy.position = new Vector2(540, 300);
                        }

                        writeStream.Position = 0;

                        writer.Write((byte)Protocol.Connected);
                        SendData(GetDataFromMemoryStream(writeStream));
                    }
                
                }
                else if (p == Protocol.PlayerMoved)
                {
                    float pX = reader.ReadSingle();
                    float pY = reader.ReadSingle();
                    float pRotation = reader.ReadSingle();
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    Enemy.position = new Vector2(pX, pY);
                    Enemy.rotation = pRotation;

                }
                else if (p == Protocol.Disconnected)
                {
                    byte id = reader.ReadByte();
                    string ip = reader.ReadString();
                    Enemy = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Converts a MemoryStream to a byte array
        /// </summary>
        /// <param name="ms">MemoryStream to convert</param>
        /// <returns>Byte array representation of the data</returns>
        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;

            //Async method called this, so lets lock the object to make sure other threads/async calls need to wait to use it.
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }


        /// <summary>
        /// Code to actually send the data to the client
        /// </summary>
        /// <param name="b">Data to send</param>
        public void SendData(byte[] b)
        {
            //Try to send the data.  If an exception is thrown, disconnect the client
            try
            {
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch (Exception e)
            {
                
            }
        }


        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //sätter bakgrundsfärgen till tardisBlue
            GraphicsDevice.Clear(TardisBlue);


            spriteBatch.Begin();

            //om spelet är på spelskärmen
            if (GameState == GameStates.playScreen)
            {

                //rita ut alla rutor
                foreach (Tile tile in tileList)
                {
                    tile.Draw(spriteBatch);

                }
                
                //Rita ut spelaren, spelarens liv, fienderna, fiendernas liv, och explosionerna
                if(Enemy != null)
                Enemy.Draw(spriteBatch);

                
                Player.Draw(spriteBatch);
                playerLife.Draw(spriteBatch);
                enemyLife.Draw(spriteBatch);
                enemyManager.Draw(spriteBatch);
                explosionManager.Draw(spriteBatch);

                
            }
                //om spelet är på menyskärmen
            else if (GameState == GameStates.menuScreen)
            {
                //skriv ut "Press Enter to start!" i mitten av skärmen i vitt.
                spriteBatch.DrawString(Arial, "Press Enter to start!", new Vector2((GraphicsDevice.Viewport.Width / 2)-(0.5f*Arial.MeasureString("Press Enter to start!").X), GraphicsDevice.Viewport.Height / 2), Color.White);
            }
            //om spelet är på slutskärmen
            else if (GameState == GameStates.endScreen)
            {
                //skriv ut "Press Enter to start!" i mitten av skärmen i vitt.
                spriteBatch.DrawString(Arial, "Press Enter to start!", new Vector2((GraphicsDevice.Viewport.Width / 2) - (0.5f * Arial.MeasureString("Press Enter to start!").X), GraphicsDevice.Viewport.Height / 2), Color.White);

                //skriv ut "viken nivå du nådde" i botten av skärmen.
                spriteBatch.DrawString(Arial, "You reached Level " + enemyManager.level, new Vector2((GraphicsDevice.Viewport.Width / 2) - (0.5f * Arial.MeasureString("You reached Level " + enemyManager.level).X), GraphicsDevice.Viewport.Height *0.75f), Color.White);

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void setUp()
        {
            //sätt rectanglen som representerar fönstrets dimensioner till fönstrets dimensioner
            screenRect = new Rectangle(0, 0, 720, 720);

            //initiera playerShootManager
            playerShootManager = new ShootManager(Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(256, 48, 32, 48), 1, 16, 250f, screenRect);

            //initiera player och sätt dess position till 300, 300
            Player = new PlayerManager(Content.Load<Texture2D>("Assets/img/runer"), 1, 32, 48, screenRect, playerShootManager, Content.Load<SoundEffect>(@"Assets/sfx/skjut"));
            if (Enemy == null)
            {
                Player.Position = new Vector2(540, 300);
            }
            else if (Enemy != null)
            {
                Player.Position = new Vector2(180, 300);
            }

            //initiera enemyShootManager
           enemyShootManager = new ShootManager(Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(256, 48, 32, 48), 1, 16, 250f, screenRect);

            //initiera enemyManager
            enemyManager = new EnemyManager(Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(64, 48, 32, 16), 5, Player, screenRect, enemyShootManager, Content.Load<SoundEffect>(@"Assets/sfx/skjut"));
            
            //skapa och inititera en random vid namn rand
            Random rand = new Random();

            //initierar playerLife
            playerLife = new Starfield(50, 100, 2000, Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(0, 48, 2, 2), new Vector2(0, 0));

            //initierar enemyLife
            enemyLife = new Starfield(50, 100, 2000, Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(0, 48, 2, 2), new Vector2(screenRect.Width - 50, 0));

            //skapa den första fienden
            enemyManager.SpawnEnemy(0);

            //initerar explosionManager
            explosionManager = new ExplosionManager(Content.Load<Texture2D>("Assets/img/Shots"), new Rectangle(320, 48, 38, 38), 4, new Rectangle(0, 450, 2, 2));

            //initierar collisionManager
            collisionManager = new CollisonsManager(Player, enemyManager, explosionManager);

            //matar in en ljudeffekt i collisionManager
            collisionManager.initiateSFX(Content.Load<SoundEffect>(@"Assets/sfx/traff"));
        }

    }
}
