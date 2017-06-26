using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PlayerArea;
using PlayerIOClient;
using System;
using System.Collections.Generic;
namespace Ship_Fighters
{



    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Player Loading;
        Texture2D NormalLoading;
        Texture2D IlluminatiLoading;
        Texture2D NathanLoading;
        Texture2D MtnDew;
        Texture2D NathanRedBullet;
        Texture2D NathanBlueBullet;
        Texture2D RedBullet;
        Texture2D BlueBullet;
        Texture2D BlueShipTexture;
        Texture2D RedShipTexture;
        Texture2D NathanBlueShip;
        Texture2D NathanRedShip;
        Player Sanic;
        bool Refresh = false;
        string status;
        int connectionnumber;
        string PlayerName = "";
        string EnemyName = "";
        bool connecting = false;
        bool Firing = false;
        Client client;
        Player RedIlluminati;
        Player BlueIlluminati;
        Connection connection;
        SoundEffect Shoot;
        SoundEffect PoorWeeCrowBoy;
        SoundEffect MLGShot;
        Song Background;
        bool songstart = false;
        Vector2 RedBulletPos;
        Vector2 BlueBulletPos;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player BlueShip;
        Player RedShip;
        Player Title;
        List<RedBullet> RedBullets;
        Texture2D RedBulletTexture;
        Rectangle RedBulletRectangle;
        Rectangle RedAICollisionRectangle;
        Rectangle BlueAICollisionRectangle;
        bool IsChangingGraphics = false;
        List<BlueBullet> BlueBullets;
        Texture2D BlueBulletTexture;
        Rectangle BlueBulletRectangle;
        private SpriteFont font;
        private SpriteFont titlefont;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        private const float delay = 0.5f; 
        private float remainingDelay = delay;
        // A movement speed for the player
        bool escaping;
        bool mute;
        bool FiringRedBullet = false;
        bool FiringBlueBullet = false;
        float playerMoveSpeed;
        int RedWins = 0;
        int BlueWins = 0;
        string Screen = "title";
        string gamemode = "singleplayer";
        bool HoldingF8 = false;
        bool HoldingF1 = false;
        bool HoldingF2 = false;
        
        
        Color Colour;
        string Mode = "Normal";
        Dictionary<string, int> AIInfo = new Dictionary<string, int>();
        Random Random = new Random();
        enum BState

        {
            HOVER,
            UP,
            JUST_RELEASED,
            DOWN
        }
        const int NUMBER_OF_BUTTONS = 3,
            Singleplayer = 0,
            Multiplayer = 1,
            Online = 2,
            BUTTON_HEIGHT = 128,
            BUTTON_WIDTH = 1024;
        
        Color[] button_color = new Color[NUMBER_OF_BUTTONS];
        Rectangle[] button_rectangle = new Rectangle[NUMBER_OF_BUTTONS];
        BState[] button_state = new BState[NUMBER_OF_BUTTONS];
        Texture2D[] button_texture = new Texture2D[NUMBER_OF_BUTTONS];
        double[] button_timer = new double[NUMBER_OF_BUTTONS];
        //mouse pressed and mouse just pressed
        bool mpressed, prev_mpressed = false;
        //mouse location in window
        int mx, my;
        double frame_time;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        private void UpdateAI()
        {

            if (BlueShip.Active == true)
            {
                //BulletCollisionRectangle = new Rectangle((int)BlueShip.Position.X, (int)BlueShip.Position.Y + BlueShip.Height, (int)BlueShip.Position.X + BlueShip.Width, (int)BlueShip.Position.Y + BlueShip.Height + 10);
                RedAICollisionRectangle = new Rectangle((int)RedShip.Position.X,  1, (int)RedShip.Position.X + RedShip.Width, 1);
                BlueAICollisionRectangle = new Rectangle((int)BlueShip.Position.X + 16,  1, (int)BlueShip.Position.X + BlueShip.Width - 16, 1);
                for (int i = 0; i < RedBullets.Count; i++)
                {
                    /*
                    if (BulletCollisionRectangle.Intersects(new Rectangle((int)RedBullets[i].Position.X, (int)RedBullets[i].Position.Y, (int)RedBullets[i].Position.X + RedBullets[i].Width, (int)RedBullets[i].Position.Y - RedBullets[i].Height)))
                    {
                        if (BlueShip.Position.X + BlueShip.Width/2 > RedBullets[i].Position.X + RedBullets[i].Width / 2)
                        {
                            AIInfo["Direction"] = 2;
                            
                            AIInfo["TravelDistance"] = 0;
                        }
                        else
                        {
                            AIInfo["Direction"] = 1;
                            
                            AIInfo["TravelDistance"] = 0;
                        }
                    }
                    */
                }
                if (RedAICollisionRectangle.Intersects(BlueAICollisionRectangle))
                {
                    AIInfo["RedRelative"] = 0;
                    if (AIInfo["TimeSinceLastShot"] >= 7)
                    {
                        
                        BlueBulletSpawn();
                        shootsound();
                        AIInfo["TimeSinceLastShot"] = 0;
                        
                    }
                }
                else
                {
                    AIInfo["RedRelative"] = ((int)RedShip.Position.X + RedShip.Width/2 - (int)BlueShip.Position.X + BlueShip.Width/2) / GraphicsDevice.Viewport.TitleSafeArea.Width * -19;
                }

                if (AIInfo["Direction"] == 0)
                {
                    AIInfo["Direction"] = 1;
                    BlueShip.Position.X += playerMoveSpeed;
                    AIInfo["TravelDistance"] = 0;
                }
                else if (AIInfo["Direction"] == 1)
                {
                    BlueShip.Position.X += playerMoveSpeed;
                    AIInfo["TravelDistance"] += 1;
                    if (BlueShip.Position.X >= GraphicsDevice.Viewport.TitleSafeArea.Width - 65)
                    {
                        BlueBulletSpawn();
                        shootsound();
                        AIInfo["TimeSinceLastShot"] = 0;
                        AIInfo["Direction"] = 2;
                        BlueShip.Position.X -= playerMoveSpeed;
                        AIInfo["TravelDistance"] = 0;
                    }else if (AIInfo["RedRelative"] <= -2)
                    {
                        AIInfo["Direction"] = 2;
                        BlueShip.Position.X -= playerMoveSpeed;
                        AIInfo["TravelDistance"] = 0;
                    }
                    else
                    {
                        if (AIInfo["TravelDistance"] >= 3)
                        {
                            try
                            {
                                if (Random.Next(1, 100 + 5 * AIInfo["RedRelative"]) == 1)
                                {
                                    AIInfo["Direction"] = 2;
                                    BlueShip.Position.X -= playerMoveSpeed;
                                    AIInfo["TravelDistance"] = 0;
                                }
                            }
                            catch
                            {
                                AIInfo["Direction"] = 2;
                                BlueShip.Position.X -= playerMoveSpeed;
                                AIInfo["TravelDistance"] = 0;
                            }
                        }
                    }
                }
                else if (AIInfo["Direction"] == 2)
                {
                    BlueShip.Position.X -= playerMoveSpeed;


                    AIInfo["TravelDistance"] += 1;
                    if (BlueShip.Position.X <= 1)
                    {
                        BlueBulletSpawn();
                        shootsound();
                        AIInfo["TimeSinceLastShot"] = 0;
                        AIInfo["Direction"] = 1;
                        BlueShip.Position.X += playerMoveSpeed;
                        AIInfo["TravelDistance"] = 0;
                    }

                    else if (AIInfo["RedRelative"] >= 2)
                    {
                        AIInfo["Direction"] = 1;
                        BlueShip.Position.X += playerMoveSpeed;
                        AIInfo["TravelDistance"] = 0;
                    }
                    else {
                        if (AIInfo["TravelDistance"] >= 3)
                        {
                            try
                            {
                                if (Random.Next(1, 100 - 5 * AIInfo["RedRelative"]) == 1)
                                {
                                    AIInfo["Direction"] = 1;
                                    BlueShip.Position.X += playerMoveSpeed;
                                    AIInfo["TravelDistance"] = 0;
                                }
                            }
                            catch
                            {
                                AIInfo["Direction"] = 1;
                                BlueShip.Position.X += playerMoveSpeed;
                                AIInfo["TravelDistance"] = 0;
                            }
                        }
                    }
                }
                if (AIInfo["TimeSinceLastShot"] >= 5)
                {
                    if (Random.Next(1, (8 - AIInfo["TimeSinceLastShot"]) + 5) == 1)
                    {
                        BlueBulletSpawn();
                        shootsound();
                        
                        AIInfo["TimeSinceLastShot"] = 0;
                    }
                }
                    AIInfo["TimeSinceLastShot"]++;
                }
            
        }

        protected override void Initialize()
        {

            
            if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stego27\\username.txt"))
            {
                PlayerName = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stego27\\username.txt");
            }
            else
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stego27");
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stego27\\username.txt", Environment.UserName);
                PlayerName = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stego27\\username.txt");
            }

            AIInfo.Add("Direction", 0);
            AIInfo.Add("TraveledDistance", 0);
            AIInfo.Add("TimeSinceLastShot", 0);
            AIInfo.Add("TimeSinceLastEncounter", 0);
            AIInfo.Add("Tactic", 0);
            AIInfo.Add("RedRelative", 0);
            BlueShip = new Player();
            RedShip = new Player();
            Title = new Player();
            Loading = new Player();
            RedIlluminati = new Player();
            BlueIlluminati = new Player();
            Sanic = new Player();
            RedBullets = new List<RedBullet>();
            BlueBullets = new List<BlueBullet>();
            playerMoveSpeed = 12.0f;
            int x = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - BUTTON_WIDTH / 2;
            int y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 -
                NUMBER_OF_BUTTONS / 2 * BUTTON_HEIGHT -
                (NUMBER_OF_BUTTONS % 2) * BUTTON_HEIGHT / 2;
            for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT;
            }
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Vector2 RedPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
            Vector2 BluePosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
            BlueShipTexture = Content.Load<Texture2D>("Graphics\\BlueShip");
            RedShipTexture = Content.Load<Texture2D>("Graphics\\RedShip");
            NathanBlueShip = Content.Load<Texture2D>("Graphics\\NathanBlueShip");
            NathanRedShip = Content.Load<Texture2D>("Graphics\\NathanRedShip");
            NathanBlueBullet = Content.Load<Texture2D>("Graphics\\NathanBlueBullet");
            NathanRedBullet = Content.Load<Texture2D>("Graphics\\NathanRedBullet");
            BlueShip.Initialize(BlueShipTexture, BluePosition);
            RedShip.Initialize(RedShipTexture, RedPosition);
            RedBullet = Content.Load<Texture2D>("Graphics\\RedBullet");
            BlueBullet = Content.Load<Texture2D>("Graphics\\BlueBullet");
            MtnDew = Content.Load<Texture2D>("Graphics\\MtnDew");
            RedBulletTexture = RedBullet;
            BlueBulletTexture = BlueBullet;
            NormalLoading = Content.Load<Texture2D>("Graphics\\Loading");
            IlluminatiLoading = Content.Load<Texture2D>("Graphics\\IlluminatiLoading");
            NathanLoading = Content.Load<Texture2D>("Graphics\\NathanLoading");
            Loading.Initialize(NormalLoading, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - 64, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2 ));
            Title.Initialize(Content.Load<Texture2D>("Graphics\\Title"), new Vector2((GraphicsDevice.Viewport.TitleSafeArea.Width - 1024) / 2, GraphicsDevice.Viewport.TitleSafeArea.Height /2 - 350));
            font = Content.Load<SpriteFont>("Graphics\\Font");
            titlefont = Content.Load<SpriteFont>("Graphics\\TitleFont");
            button_texture[Singleplayer] =
                Content.Load<Texture2D>("Graphics\\Singleplayer");
            button_texture[Multiplayer] =
                Content.Load<Texture2D>("Graphics\\Multiplayer");
            button_texture[Online] =
                Content.Load<Texture2D>("Graphics\\Online");
            Shoot = Content.Load<SoundEffect>("Sounds\\Shoot");
            PoorWeeCrowBoy = Content.Load<SoundEffect>("Sounds\\PoorWeeCrowBoy");
            MLGShot = Content.Load<SoundEffect>("Sounds\\MLGShot");
            RedIlluminati.Initialize(Content.Load<Texture2D>("Graphics\\illuminati"), new Vector2(RedShip.Position.X + RedShip.Width, RedShip.Position.Y + RedShip.Height));
            BlueIlluminati.Initialize(Content.Load<Texture2D>("Graphics\\illuminati"), new Vector2(BlueShip.Position.X, BlueShip.Position.Y));
            Sanic.Initialize(Content.Load<Texture2D>("Graphics\\sanic"), new Vector2(0, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 - 256 / 2));
            Background = Content.Load<Song>("Sounds\\Background");
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        public void shootsound()
        {
            if (!mute)
            {
                switch (Mode)
                {
                    case "Normal":
                        Shoot.Play();
                        break;
                    case "MLG":
                        MLGShot.Play();
                        break;
                    case "Nathan":
                        PoorWeeCrowBoy.Play();
                        break;
                }
            }
        }
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
                if (!songstart)
                {
                    MediaPlayer.Play(Background);
                    songstart = true;
                }

            // TODO: Add your update logic here


            previousKeyboardState = currentKeyboardState;



            // Read the current state of the keyboard and gamepad and store it

            currentKeyboardState = Keyboard.GetState();

            



            //Update the player

            UpdatePlayer(gameTime);
            base.Update(gameTime);
        }
        
        private void UpdatePlayer(GameTime gameTime)
            
        {

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if (HoldingF1 == false)
                {
                    HoldingF1 = true;
                    if (MediaPlayer.IsMuted)
                    {
                        MediaPlayer.IsMuted = false;
                    }
                    else
                    {
                        MediaPlayer.IsMuted = true;
                    }

                }
            }
            else { HoldingF1 = false; }

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                if (HoldingF2 == false)
                {
                    HoldingF2 = true;
                    if (mute)
                    {
                        mute = false;
                    }
                    else
                    {
                        mute = true;
                    }

                }
            }
            else { HoldingF2 = false; }

            if (IsChangingGraphics == true) { IsChangingGraphics = false; graphics.ApplyChanges(); RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64); BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);}
            if (connection != null) {
                if (connection.Connected == true)
                {
                    connection.OnMessage += delegate (object sender, Message m)
                    {
                        if (connection.Connected == true)
                        {
                            switch (m.Type)
                            {
                                case "newusername":
                                    connection.Disconnect();
                                    client.Logout();
                                    PlayerName = PlayerName + " ";
                                    connecting = false;
                                    break;
                                case "join info":
                                    if (m.GetBoolean(0) == false) { connectionnumber++; connection.Disconnect(); connection = client.Multiplayer.CreateJoinRoom("room" + connectionnumber, "Game Room", true, null, null); } else { status = "Waiting For An Opponent"; Refresh = true; remainingDelay = 1.0f; }
                                    break;
                                case "gameready":
                                    if (Screen == "lobby")
                                    {
                                        Refresh = false;
                                        status = "Starting Game";
                                        graphics.PreferredBackBufferWidth = 1920;
                                        graphics.PreferredBackBufferHeight = 1080;
                                        IsChangingGraphics = true;
                                        RedShip.Active = true;
                                        BlueShip.Active = true;
                                        gamemode = "online";
                                        Screen = "gameplay";

                                    }
                                    break;
                                case "packet":
                                    if (m.GetString(2) != PlayerName)
                                    {
                                        if (Screen == "lobby")
                                        {
                                            status = "Starting Game";
                                            graphics.PreferredBackBufferWidth = 1920;
                                            graphics.PreferredBackBufferHeight = 1080;
                                            IsChangingGraphics = true;
                                            RedShip.Active = true;
                                            BlueShip.Active = true;
                                            gamemode = "online";
                                            Screen = "gameplay";
                                        }
                                        EnemyName = m.GetString(2);
                                        BlueShip.Position.X = GraphicsDevice.Viewport.TitleSafeArea.Width - m.GetFloat(0) - BlueShip.Width;

                                    }
                                    break;
                                case "firing":
                                    if (FiringBlueBullet == false)
                                    {
                                        FiringBlueBullet = true;
                                        BlueBulletSpawn();
                                        shootsound();

                                    }
                                    break;
                                case "stoppedfiring":
                                    FiringBlueBullet = false;
                                    break;
                                case "health":
                                    BlueShip.Health = m.GetInt(0);
                                    break;
                                case "lose":

                                    RedWins = 0;
                                    BlueWins = 0;
                                    RedShip.Health = 10;
                                    BlueShip.Health = 10;
                                    RedShip.Active = false;
                                    BlueShip.Active = false;
                                    for (int i = 0; i < RedBullets.Count; i++)
                                    {
                                        RedBullets[i].Active = false;
                                    }
                                    for (int i = 0; i < BlueBullets.Count; i++)
                                    {
                                        BlueBullets[i].Active = false;
                                    }

                                    for (int i = 0; i < RedBullets.Count; i++)
                                    {
                                        RedBullets.RemoveAt(i);
                                    }
                                    for (int i = 0; i < BlueBullets.Count; i++)
                                    {
                                        BlueBullets.RemoveAt(i);
                                    }
                                    connection.Disconnect();
                                    client.Logout();
                                    remainingDelay = 5f;
                                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                                    IsChangingGraphics = true;
                                    Screen = "win";
                                    break;
                                case "disconnected":
                                    RedWins = 0;
                                    BlueWins = 0;
                                    RedShip.Health = 10;
                                    BlueShip.Health = 10;
                                    RedShip.Active = false;
                                    BlueShip.Active = false;
                                    for (int i = 0; i < RedBullets.Count; i++)
                                    {
                                        RedBullets[i].Active = false;
                                    }
                                    for (int i = 0; i < BlueBullets.Count; i++)
                                    {
                                        BlueBullets[i].Active = false;
                                    }


                                    for (int i = 0; i < RedBullets.Count; i++)
                                    {
                                        RedBullets.RemoveAt(i);
                                    }
                                    for (int i = 0; i < BlueBullets.Count; i++)
                                    {
                                        BlueBullets.RemoveAt(i);
                                    }
                                    connection.Disconnect();
                                    client.Logout();
                                    remainingDelay = 5f;
                                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                                    IsChangingGraphics = true;
                                    Screen = "disconnected";
                                    break;
                            }




                        }
                    };
                }
            }
            
            switch (Screen)
            {
                case "gameplay":
                    RedShip.Position = new Vector2(RedShip.Position.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                    BlueShip.Position = new Vector2(BlueShip.Position.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                    IsMouseVisible = false;

                    switch (gamemode)
                    {
                        case "multiplayer":
                            if (currentKeyboardState.IsKeyDown(Keys.Left))

                            {

                                RedShip.Position.X -= playerMoveSpeed;

                            }



                            if (currentKeyboardState.IsKeyDown(Keys.Right))

                            {

                                RedShip.Position.X += playerMoveSpeed;

                            }



                            if (currentKeyboardState.IsKeyDown(Keys.Up))
                            {
                                if (FiringRedBullet == false)
                                {
                                    FiringRedBullet = true;
                                    RedBulletSpawn();
                                    shootsound();
                                }
                            }
                            else
                            {
                                FiringRedBullet = false;
                            }
                            if (currentKeyboardState.IsKeyDown(Keys.A))

                            {

                                BlueShip.Position.X -= playerMoveSpeed;

                            }



                            if (currentKeyboardState.IsKeyDown(Keys.D))

                            {

                                BlueShip.Position.X += playerMoveSpeed;

                            }



                            if (currentKeyboardState.IsKeyDown(Keys.W))

                            {
                                if (FiringBlueBullet == false)
                                {
                                    FiringBlueBullet = true;
                                    BlueBulletSpawn();
                                    shootsound();
                                }
                            }
                            else
                            {
                                FiringBlueBullet = false;
                            }
                            break;
                        case "singleplayer":
                            UpdateAI();
                            if (currentKeyboardState.IsKeyDown(Keys.Left))

                            {

                                RedShip.Position.X -= playerMoveSpeed;

                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.A))

                            {

                                RedShip.Position.X -= playerMoveSpeed;

                            }


                            if (currentKeyboardState.IsKeyDown(Keys.Right))

                            {

                                RedShip.Position.X += playerMoveSpeed;

                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.D))

                            {

                                RedShip.Position.X += playerMoveSpeed;

                            }


                            if (currentKeyboardState.IsKeyDown(Keys.Up))
                            {
                                if (FiringRedBullet == false)
                                {
                                    FiringRedBullet = true;
                                    RedBulletSpawn();
                                    shootsound();
                                }
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.W))

                            {
                                if (FiringRedBullet == false)
                                {
                                    FiringRedBullet = true;
                                    RedBulletSpawn();
                                    shootsound();
                                }
                            }
                            else
                            {
                                FiringRedBullet = false;
                            }
                            break;
                        case "online":
                            connection.Send("packet", RedShip.Position.X, Firing, PlayerName);
                            Firing = false;

                            
                            if (currentKeyboardState.IsKeyDown(Keys.Left))

                            {

                                RedShip.Position.X -= playerMoveSpeed;

                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.A))

                            {

                                RedShip.Position.X -= playerMoveSpeed;

                            }


                            if (currentKeyboardState.IsKeyDown(Keys.Right))

                            {

                                RedShip.Position.X += playerMoveSpeed;

                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.D))

                            {

                                RedShip.Position.X += playerMoveSpeed;

                            }


                            if (currentKeyboardState.IsKeyDown(Keys.Up))
                            {
                                if (FiringRedBullet == false)
                                {
                                    FiringRedBullet = true;
                                    RedBulletSpawn();
                                    shootsound();
                                    connection.Send("firing");
                                }
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.W))

                            {
                                if (FiringRedBullet == false)
                                {
                                    FiringRedBullet = true;
                                    RedBulletSpawn();
                                    shootsound();
                                    connection.Send("firing");
                                }
                            }
                            else
                            {
                                FiringRedBullet = false;
                                connection.Send("stoppedfiring");
                            }
                            break;
                    }
                    
                    // Make sure that the player does not go out of bounds

                    BlueShip.Position.X = MathHelper.Clamp(BlueShip.Position.X, 0, GraphicsDevice.Viewport.Width - BlueShip.Width);
                    RedShip.Position.X = MathHelper.Clamp(RedShip.Position.X, 0, GraphicsDevice.Viewport.Width - RedShip.Width);
                    if (RedShip.Health <= 0)
                    {
                        RedShip.Active = false;
                        BlueShip.Active = false;
                        for (int i = 0; i < RedBullets.Count; i++)
                        {
                            RedBullets[i].Active = false;
                        }
                        for (int i = 0; i < BlueBullets.Count; i++)
                        {
                            BlueBullets[i].Active = false;
                        }

                        var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        remainingDelay -= timer;

                        if (remainingDelay <= 0)
                        {

                            if (gamemode == "online")
                            {
                                connection.Send("lose");
                                RedWins = 0;
                                BlueWins = 0;
                                RedShip.Health = 10;
                                BlueShip.Health = 10;
                                RedShip.Active = false;
                                BlueShip.Active = false;
                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets[i].Active = false;
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets[i].Active = false;
                                }
                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets.RemoveAt(i);
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets.RemoveAt(i);
                                }
                                connection.Disconnect();
                                client.Logout();
                                remainingDelay = 5f;
                                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                                IsChangingGraphics = true;
                                Screen = "lose";
                            }
                            else
                            {
                                BlueShip.Active = true;
                                RedShip.Active = true;
                                AIInfo.Clear();
                                AIInfo.Add("Direction", 0);
                                AIInfo.Add("TraveledDistance", 0);
                                AIInfo.Add("TimeSinceLastShot", 0);
                                AIInfo.Add("TimeSinceLastEncounter", 0);
                                AIInfo.Add("Tactic", 0);
                                AIInfo.Add("RedRelative", 0);
                                RedShip.Health = 10;
                                BlueShip.Health = 10;
                                BlueWins++;
                                RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                                BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                                remainingDelay = delay;
                            }

                        }

                    }
                    if (gamemode != "online")
                    {
                        if (BlueShip.Health <= 0)
                        {
                            RedShip.Active = false;
                            BlueShip.Active = false;
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets[i].Active = false;
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets[i].Active = false;
                            }

                            var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                            remainingDelay -= timer;

                            if (remainingDelay <= 0)
                            {

                                BlueShip.Active = true;
                                RedShip.Active = true;
                                AIInfo.Clear();
                                AIInfo.Add("Direction", 0);
                                AIInfo.Add("TraveledDistance", 0);
                                AIInfo.Add("TimeSinceLastShot", 0);
                                AIInfo.Add("TimeSinceLastEncounter", 0);
                                AIInfo.Add("Tactic", 0);
                                AIInfo.Add("RedRelative", 0);
                                RedShip.Health = 10;
                                BlueShip.Health = 10;
                                RedWins++;

                                RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                                BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                                remainingDelay = delay;
                            }


                        }

                        
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        if (escaping == false)
                        {
                            escaping = true;
                            if (gamemode == "online")
                            {
                                RedWins = 0;
                                BlueWins = 0;
                                RedShip.Health = 10;
                                BlueShip.Health = 10;
                                RedShip.Active = false;
                                BlueShip.Active = false;
                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets[i].Active = false;
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets[i].Active = false;
                                }

                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets.RemoveAt(i);
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets.RemoveAt(i);
                                }
                                connection.Disconnect();
                                client.Logout();
                                remainingDelay = 2.5f;
                                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                                IsChangingGraphics = true;
                                Screen = "title";
                            }
                            else
                            {
                                
                                AIInfo.Clear();
                                AIInfo.Add("Direction", 0);
                                AIInfo.Add("TraveledDistance", 0);
                                AIInfo.Add("TimeSinceLastShot", 0);
                                AIInfo.Add("TimeSinceLastEncounter", 0);
                                AIInfo.Add("Tactic", 0);
                                AIInfo.Add("RedRelative", 0);
                                RedWins = 0;
                                BlueWins = 0;
                                RedShip.Health = 10;
                                BlueShip.Health = 10;
                                RedShip.Active = false;
                                BlueShip.Active = false;
                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets[i].Active = false;
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets[i].Active = false;
                                }
                                RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                                BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                                for (int i = 0; i < RedBullets.Count; i++)
                                {
                                    RedBullets.RemoveAt(i);
                                }
                                for (int i = 0; i < BlueBullets.Count; i++)
                                {
                                    BlueBullets.RemoveAt(i);
                                }
                                Screen = "title";
                            }
                        }
                    }
                    else { escaping = false; }
                    break;
                case "title":
                    frame_time = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

                    // update mouse variables
                    IsMouseVisible = true;
                    MouseState mouse_state = Mouse.GetState();
                    mx = mouse_state.X;
                    my = mouse_state.Y;
                    prev_mpressed = mpressed;
                    mpressed = mouse_state.LeftButton == ButtonState.Pressed;
                    update_buttons();
                    if (Keyboard.GetState().IsKeyDown(Keys.F8))
                    {
                        if (HoldingF8 == false)
                        {
                            HoldingF8 = true;
                            switch (Mode)
                            {
                                case "Normal":
                                    Mode = "MLG";
                                    Loading.PlayerTexture = IlluminatiLoading;
                                    RedBulletTexture = MtnDew;
                                    BlueBulletTexture = MtnDew;
                                    BlueShip.PlayerTexture = BlueShipTexture;
                                    RedShip.PlayerTexture = RedShipTexture;
                                    break;
                                case "MLG":
                                    Mode = "Nathan";
                                    Loading.PlayerTexture = NathanLoading;
                                    RedBulletTexture = NathanRedBullet;
                                    BlueBulletTexture = NathanBlueBullet;
                                    BlueShip.PlayerTexture = NathanBlueShip;
                                    RedShip.PlayerTexture = NathanRedShip;
                                    break;
                                case "Nathan":
                                    Mode = "Normal";
                                    Loading.PlayerTexture = NormalLoading;
                                    RedBulletTexture = RedBullet;
                                    BlueBulletTexture = BlueBullet;
                                    BlueShip.PlayerTexture = BlueShipTexture;
                                    RedShip.PlayerTexture = RedShipTexture;
                                    break;
                            }
                            
                        }
                    }
                    else { HoldingF8 = false; }

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        if (escaping == false)
                        {
                            Exit();
                        }
                    }
                    else { escaping = false; }

                    break;
                case "lobby":
                    
                    if (connecting == false)
                    {
                        connecting = true;

                        client = PlayerIO.Connect(
                                "ship-fighters2-qtzrs3ccj0wdo07oa9bttg",  // Game id (Get your own at gamesnet.yahoo.com. 1: Create user, 2:Goto control panel, 3:Create game, 4: Copy game id inside the "")
                                "public",                       // The id of the connection, as given in the settings section of the control panel. By default, a connection with id='public' is created on all games.
                                PlayerName,                      // The id of the user connecting. This can be any string you like. For instance, it might be "fb10239" if you´re building a Facebook app and the user connecting has id 10239
                                null,
                                null
                            );
                        status = "Finding Game";
                        connectionnumber = 0;
                        //client.Multiplayer.DevelopmentServer = new PlayerIOClient.ServerEndpoint("192.168.1.71", 25565);
                        connection = client.Multiplayer.CreateJoinRoom("room" + connectionnumber, "Game Room", true, null, null);
                        
                    }
                    if (Refresh == true)
                    {
                        var refreshtimer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        remainingDelay -= refreshtimer;

                        if (remainingDelay <= 0)
                        {

                            Refresh = false;
                            connection.Disconnect();
                            connectionnumber = 0;
                            connection = client.Multiplayer.CreateJoinRoom("room" + connectionnumber, "Game Room", true, null, null);
                            
                            
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {

                        if (escaping == false)
                        {
                            escaping = true;
                            RedWins = 0;
                            BlueWins = 0;
                            RedShip.Health = 10;
                            BlueShip.Health = 10;
                            RedShip.Active = false;
                            BlueShip.Active = false;
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets[i].Active = false;
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets[i].Active = false;
                            }

                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets.RemoveAt(i);
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets.RemoveAt(i);
                            }
                            connection.Disconnect();
                            client.Logout();
                            remainingDelay = 2.5f;
                            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            IsChangingGraphics = true;
                            Screen = "title";
                        }
                    }
                    else { escaping = false; }
                    break;
                case "win":
                    
                    var timer2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    remainingDelay -= timer2;

                    if (remainingDelay <= 0)
                    {
                        connecting = false;
                        status = "Connecting To Server";
                        Screen = "lobby";
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        if (escaping == false)
                        {
                            escaping = true;
                            if (connecting == true) { connection.Disconnect(); client.Logout(); connecting = false; }
                            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            graphics.ApplyChanges();
                            AIInfo.Clear();
                            AIInfo.Add("Direction", 0);
                            AIInfo.Add("TraveledDistance", 0);
                            AIInfo.Add("TimeSinceLastShot", 0);
                            AIInfo.Add("TimeSinceLastEncounter", 0);
                            AIInfo.Add("Tactic", 0);
                            AIInfo.Add("RedRelative", 0);
                            RedWins = 0;
                            BlueWins = 0;
                            RedShip.Health = 10;
                            BlueShip.Health = 10;
                            RedShip.Active = false;
                            BlueShip.Active = false;
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets[i].Active = false;
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets[i].Active = false;
                            }
                            RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                            BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets.RemoveAt(i);
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets.RemoveAt(i);
                            }
                            Screen = "title";
                        }
                    }
                    else { escaping = false; }
                    break;
                case "lose":
                    
                    var timer3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    remainingDelay -= timer3;

                    if (remainingDelay <= 0)
                    {
                        connecting = false;
                        status = "Connecting To Server";
                        Screen = "lobby";
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        if (escaping == false)
                        {
                            escaping = true;
                            if (connecting == true) { connection.Disconnect(); client.Logout(); connecting = false; }
                            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            graphics.ApplyChanges();
                            AIInfo.Clear();
                            AIInfo.Add("Direction", 0);
                            AIInfo.Add("TraveledDistance", 0);
                            AIInfo.Add("TimeSinceLastShot", 0);
                            AIInfo.Add("TimeSinceLastEncounter", 0);
                            AIInfo.Add("Tactic", 0);
                            AIInfo.Add("RedRelative", 0);
                            RedWins = 0;
                            BlueWins = 0;
                            RedShip.Health = 10;
                            BlueShip.Health = 10;
                            RedShip.Active = false;
                            BlueShip.Active = false;
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets[i].Active = false;
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets[i].Active = false;
                            }
                            RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                            BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets.RemoveAt(i);
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets.RemoveAt(i);
                            }
                            Screen = "title";
                        }
                    }
                    else { escaping = false; }
                    break;
                case "disconnected":
                    
                    var timer4 = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    remainingDelay -= timer4;

                    if (remainingDelay <= 0)
                    {
                        connecting = false;
                        status = "Connecting To Server";
                        Screen = "lobby";
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        if (escaping == false)
                        {
                            escaping = true;
                            if (connecting == true) { connection.Disconnect(); client.Logout(); connecting = false; }
                            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                            graphics.ApplyChanges();
                            AIInfo.Clear();
                            AIInfo.Add("Direction", 0);
                            AIInfo.Add("TraveledDistance", 0);
                            AIInfo.Add("TimeSinceLastShot", 0);
                            AIInfo.Add("TimeSinceLastEncounter", 0);
                            AIInfo.Add("Tactic", 0);
                            AIInfo.Add("RedRelative", 0);
                            RedWins = 0;
                            BlueWins = 0;
                            RedShip.Health = 10;
                            BlueShip.Health = 10;
                            RedShip.Active = false;
                            BlueShip.Active = false;
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets[i].Active = false;
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets[i].Active = false;
                            }
                            RedShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height - 64);
                            BlueShip.Position = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
                            for (int i = 0; i < RedBullets.Count; i++)
                            {
                                RedBullets.RemoveAt(i);
                            }
                            for (int i = 0; i < BlueBullets.Count; i++)
                            {
                                BlueBullets.RemoveAt(i);
                            }
                            Screen = "title";
                        }
                    }
                    else { escaping = false; }
                    break;
            }




                  
        }

        private Color GetRandomColor()
        {

            return new Color(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
        }

        protected override void Draw(GameTime gameTime)
        {
            
            switch (Screen)
            {
                case "gameplay":
                    switch (Mode) {
                        case "Normal":
                            GraphicsDevice.Clear(Color.White);
                            break;
                        case "MLG":
                            if (Random.Next(65, 70) == 69)
                            {
                                Colour = GetRandomColor();
                                GraphicsDevice.Clear(Colour);
                            }
                            else { GraphicsDevice.Clear(Colour); }
                            break;
                        case "Nathan":
                            GraphicsDevice.Clear(Color.White);
                            break;
                    }
                    
                    break;
                case "title":
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    break;
                case "lobby":
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    break;
                case "win":
                    GraphicsDevice.Clear(Color.Green);
                    break;
                case "lose":
                    GraphicsDevice.Clear(Color.Red);
                    break;
                case "disconnected":
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    break;
            }
            spriteBatch.Begin();
            switch (Screen)
            {
                case "gameplay":
                    if (Mode == "MLG")
                    {
                        Sanic.Draw(spriteBatch);
                        Sanic.Position.X += 69;
                        if (Sanic.Position.X > GraphicsDevice.Viewport.TitleSafeArea.Width) { Sanic.Position.X = 0 - Sanic.Width; }
                        RedIlluminati.Draw(spriteBatch);
                        BlueIlluminati.Draw(spriteBatch);
                        RedIlluminati.rotation += 0.420f;
                        BlueIlluminati.rotation += 0.420f;
                        RedIlluminati.Position = new Vector2(RedShip.Position.X + RedIlluminati.Width / 2, RedShip.Position.Y - RedShip.Height / 2 + RedIlluminati.Height / 2 - 10);
                        BlueIlluminati.Position = new Vector2(BlueShip.Position.X + BlueIlluminati.Width / 2, BlueShip.Position.Y + BlueShip.Height / 2 + BlueIlluminati.Height / 2 + 10);
                    }
                    for (int i = 0; i < RedBullets.Count; i++)

                    {
                        RedBullets[i].Update(gameTime);
                        RedBullets[i].Position.Y -= 12;
                        RedBullets[i].Draw(spriteBatch);
                        RedBulletRectangle = new Rectangle((int)RedBullets[i].Position.X, (int)RedBullets[i].Position.Y, RedBullets[i].Width, RedBullets[i].Height);
                        if (RedBulletRectangle.Intersects(new Rectangle((int)BlueShip.Position.X, (int)BlueShip.Position.Y, BlueShip.Width, BlueShip.Height)))
                        {
                            
                            if (BlueShip.Active == true)

                            {
                                if(gamemode != "online")
                                {
                                    BlueShip.Health -= RedBullets[i].Damage;
                                }
                                
                                RedBullets[i].Active = false;
                            }
                        }
                        if (RedBullets[i].Position.Y > GraphicsDevice.Viewport.Height)
                        {
                            RedBullets[i].Active = false;
                        }
                        if (RedBullets[i].Active == false)
                        {

                            RedBullets.RemoveAt(i);

                        }




                    }
                    for (int i = 0; i < BlueBullets.Count; i++)
                    {
                        
                        BlueBullets[i].Update(gameTime);
                        BlueBullets[i].Position.Y += 12;
                        
                        BlueBullets[i].Draw(spriteBatch);
                        BlueBulletRectangle = new Rectangle((int)BlueBullets[i].Position.X, (int)BlueBullets[i].Position.Y, BlueBullets[i].Width, BlueBullets[i].Height);
                        if (BlueBulletRectangle.Intersects(new Rectangle((int)RedShip.Position.X, (int)RedShip.Position.Y, RedShip.Width, RedShip.Height)))
                        {
                            
                            
                            if (RedShip.Active == true)
                                
                            {
                                RedShip.Health -= BlueBullets[i].Damage;
                                BlueBullets[i].Active = false;
                                if (gamemode == "online") { connection.Send("health", RedShip.Health); } 
                            }

                        }
                        if (BlueBullets[i].Position.Y < 0)
                        {
                            BlueBullets[i].Active = false;
                        }
                        if (BlueBullets[i].Active == false)
                        {

                            BlueBullets.RemoveAt(i);

                        }




                    }
                    if (BlueShip.Active == true)
                    {
                        spriteBatch.DrawString(font, "" + BlueShip.Health, new Vector2(BlueShip.Position.X + BlueShip.Width / 2 - font.MeasureString("" + BlueShip.Health).X / 2, BlueShip.Height - 5), Color.Black);
                        
                        BlueShip.Draw(spriteBatch);
                    }
                    if (RedShip.Active == true)
                    {
                        spriteBatch.DrawString(font, "" + RedShip.Health, new Vector2(RedShip.Position.X + RedShip.Width / 2 - font.MeasureString("" + RedShip.Health).X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height - (RedShip.Height - 5) - font.MeasureString("" + RedShip.Health).Y), Color.Black);
                        
                        RedShip.Draw(spriteBatch);
                    }

                    if (gamemode == "online")
                    {
                        spriteBatch.DrawString(font, PlayerName, new Vector2(10, GraphicsDevice.Viewport.TitleSafeArea.Height - 104), Color.Black);
                        spriteBatch.DrawString(font, EnemyName, new Vector2(10, 79), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Red Wins: " + RedWins, new Vector2(10, GraphicsDevice.Viewport.TitleSafeArea.Height - 104), Color.Black);
                        spriteBatch.DrawString(font, "Blue Wins: " + BlueWins, new Vector2(10, 79), Color.Black);
                    }
                    
                    break;
                case "title":
                    
                    for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
                        spriteBatch.Draw(button_texture[i], button_rectangle[i], button_color[i]);
                    Title.Draw(spriteBatch);
                    spriteBatch.DrawString(font, "Press F1 to mute music and F2 to mute sound effects.", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString("Press F1 to mute music and F2 to mute sound effects.").X, 0), Color.Red);
                    switch (Mode)
                    {
                        case "Normal":
                            spriteBatch.DrawString(font, "(C) Sean Norris 2016", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString("(C) Sean Norris 2016").X - 16, GraphicsDevice.Viewport.TitleSafeArea.Height - font.MeasureString("(C) Sean Norris 2016").Y / 2 - 16), Color.Red);
                            break;
                        case "MLG":
                            spriteBatch.DrawString(titlefont, "MLG MODE ACTIVE!!!!", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString("MLG MODE ACTIVE!!!!").X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 + 256), GetRandomColor());
                            spriteBatch.DrawString(font, "(C) Sean Norris 2016", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString("(C) Sean Norris 2016").X - 16, GraphicsDevice.Viewport.TitleSafeArea.Height - font.MeasureString("(C) Sean Norris 2016").Y / 2 - 16), Color.Red);
                            break;
                        case "Nathan":
                            spriteBatch.DrawString(titlefont, "NATHAN SIMULATOR ACTIVE!!!!", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString("NATHAN SIMULATOR ACTIVE!!!!").X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 + 256), GetRandomColor());
                            spriteBatch.DrawString(font, "(C) Sean Norris 2016", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString("(C) Sean Norris 2016").X - 16, GraphicsDevice.Viewport.TitleSafeArea.Height - font.MeasureString("(C) Sean Norris 2016").Y / 2 - 16), Color.Red);
                            spriteBatch.DrawString(font, "Graphics (C) Nathan Mchallam 2015", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - font.MeasureString("Graphics (C) Nathan Mchallam 2015").X - 16, GraphicsDevice.Viewport.TitleSafeArea.Height - font.MeasureString("Graphics (C) Nathan Mchallam 2015").Y / 2 - font.MeasureString("(C) Sean Norris 2016").Y - 8), Color.Red);

                            break;
                    }
                    break;
                case "lobby":
                    spriteBatch.DrawString(titlefont, status, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString(status).X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 - titlefont.MeasureString(status).Y), Color.Red);
                    Loading.Draw(spriteBatch);
                    Loading.rotation += 0.2f;
                    break;
                case "win":
                    spriteBatch.DrawString(titlefont, "You Beat " + EnemyName + " (" + (int)remainingDelay + ")", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString("You Beat " + EnemyName + " (" + (int)remainingDelay + ")").X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 - titlefont.MeasureString("You Beat " + EnemyName).Y / 2), Color.Red);

                    break;
                case "lose":
                    spriteBatch.DrawString(titlefont, "You Lost To " + EnemyName + " (" + (int)remainingDelay + ")", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString("You Lost To " + EnemyName + " (" + (int)remainingDelay + ")").X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 - titlefont.MeasureString("You Lost To " + EnemyName).Y / 2), Color.Green);

                    break;
                case "disconnected":
                    spriteBatch.DrawString(titlefont, "Disconnected From " + EnemyName + " (" + (int)remainingDelay + ")", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width / 2 - titlefont.MeasureString("Disconnected With " + EnemyName + " (" + (int)remainingDelay + ")").X / 2, GraphicsDevice.Viewport.TitleSafeArea.Height / 2 - titlefont.MeasureString("Disconnected With " + EnemyName).Y / 2), Color.Red);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        private void RedBulletSpawn()
        {
            RedBullet RedBullet = new RedBullet();
            RedBulletPos = new Vector2(RedShip.Position.X + RedShip.Width / 2 - 2, RedShip.Position.Y);
            RedBullet.Initialize(RedBulletTexture, RedBulletPos);
            RedBullets.Add(RedBullet);
        }

        private void BlueBulletSpawn()
        {
            BlueBullet BlueBullet = new BlueBullet();
            BlueBulletPos = new Vector2(BlueShip.Position.X + BlueShip.Width / 2 - 2, BlueShip.Position.Y + BlueShip.Height);
            BlueBullet.Initialize(BlueBulletTexture, BlueBulletPos);
            BlueBullets.Add(BlueBullet);
        }

        bool hit_image_alpha(Rectangle rect, Texture2D tex, int x, int y)
        {
            return hit_image_alpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        // wraps hit_image then determines if hit a transparent part of image 
        bool hit_image_alpha(float tx, float ty, Texture2D tex, int x, int y)
        {
            if (hit_image(tx, ty, tex, x, y))
            {
                uint[] data = new uint[tex.Width * tex.Height];
                tex.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) *
                    tex.Width < tex.Width * tex.Height)
                {
                    return ((data[
                        (x - (int)tx) + (y - (int)ty) * tex.Width
                        ] &
                                0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        // determine if x,y is within rectangle formed by texture located at tx,ty
        bool hit_image(float tx, float ty, Texture2D tex, int x, int y)
        {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }

        // determine state and color of button
        void update_buttons()
        {
            for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
            {

                if (hit_image_alpha(
                    button_rectangle[i], button_texture[i], mx, my))
                {
                    button_timer[i] = 0.0;
                    if (mpressed)
                    {
                        // mouse is currently down
                        button_state[i] = BState.DOWN;
                        button_color[i] = Color.Blue;
                    }
                    else if (!mpressed && prev_mpressed)
                    {
                        // mouse was just released
                        if (button_state[i] == BState.DOWN)
                        {
                            // button i was just down
                            button_state[i] = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        button_state[i] = BState.HOVER;
                        button_color[i] = Color.LightBlue;
                    }
                }
                else
                {
                    button_state[i] = BState.UP;
                    if (button_timer[i] > 0)
                    {
                        button_timer[i] = button_timer[i] - frame_time;
                    }
                    else
                    {
                        button_color[i] = Color.White;
                    }
                }

                if (button_state[i] == BState.JUST_RELEASED)
                {
                    take_action_on_button(i);
                }
            }
        }

            // Logic for each button click goes here
        void take_action_on_button(int i)
        {
            //take action corresponding to which button was clicked
            switch (i)
            {
                case Singleplayer:
                    Screen = "gameplay";
                    gamemode = "singleplayer";
                    RedShip.Active = true;
                    BlueShip.Active = true;
                    break;
                case Multiplayer:
                    Screen = "gameplay";
                    gamemode = "multiplayer";
                    RedShip.Active = true;
                    BlueShip.Active = true;
                    break;
                case Online:
                    connecting = false;
                    status = "Connecting To Server";
                    Screen = "lobby";
                    break;

            }
        }
        
    }
}
