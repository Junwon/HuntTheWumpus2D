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

namespace HuntTheWumpus2D
{
    public class Game2D : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont font;

        private Texture2D player;
        private Texture2D wumpus_img;
        private Texture2D superbats_img;
        private Texture2D pit_img;
        private Vector2 playerPosition = Vector2.Zero;


        private Texture2D paths;

        private string console_text, game_state = "OPENING";
        private bool same_setting = false;
        private bool optionPaddle_on = false;
        private bool[] room_entered = new bool[20], room_aimed = new bool[20];
        private int pos = -1, saved_pos = -1, paddlePosX, paddlePosY = 0, paddleThreshold, paddle_increment, option_select = 1, room_select = 0, option_num;
        private int quit_option = 0, setting_select = 0, mouseX = 0, mouseY = 0, aim_count = 0;
        private int[] aim_history = new int[6];
        private Random ran = new Random();
        private Map map = new Map();
        private Map saved_map;
        private Hero hero = new Hero();
        private Wumpus wumpus = new Wumpus();


        private KeyboardState oldState;
        private MouseState moldState;

        private string option_text = "";


        public Game2D()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1000;
        }

        protected override void Initialize()
        {
            //TODO: Add your initialization logic here
            base.Initialize();

            this.IsMouseVisible = true;

            if (same_setting)
                sameSettings();
            else
                newSettings();

            room_entered = new bool[20];
            aim_history = new int[6];
            room_aimed = new bool[20];

            wumpus.setMap(map, map.whereWumpus());
            hero.setMap(map, pos);
        }
       
        protected override void LoadContent()
        {
            base.LoadContent();
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("myFont");

            player = Content.Load<Texture2D>("player");

            wumpus_img = Content.Load<Texture2D>("wumpus");

            superbats_img = Content.Load<Texture2D>("bat");

            pit_img = Content.Load<Texture2D>("pit");

            paths = new Texture2D(GraphicsDevice, 1, 1);
            paths.SetData(new[] { Color.White });
        }
        

        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();

            paths.Dispose(); 
        }


        /*
         *GAME LOOP: HANDLES ALL ACTION AND FLAGS OF THE GAME IN THIS METHOD 
         */
        protected override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState(PlayerIndex.One);

            playerPosition = map.getRoom(pos).getVector();

            room_entered[pos] = true;

            //OPENING
            if (game_state == "OPENING")
            {
                console_text = "Welcome to \"Hunt the Wumpus!\". \nTry to kill the Wumpus before you get yourself killed :)\n\nPress Enter to Start";
               
                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                }
            }
            //WHENEVER THE GAME RESETS DUE TO LOSS OR VICTORY
            else if (game_state == "RESET")
            {
                optionPaddle_on = true;
                setText("Play again? YES NO");

                if (!newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                {
                    leftOption();
                }

                if (!newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                {
                    rightOption();
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    if (quit_option == 0)
                    {    
                        game_state = "SETTING";
                        setPaddle(550, 730);
                        setPaddleInc(55);
                        option_num = 2;
                        resetOption();
                    }
                    else if (quit_option == 1)
                    {
                        this.Exit();
                    }
                }
            }
            //DECIDING TO USE PREVIOUS GAME SETTING OR NEW SETTING
            else if (game_state == "SETTING")
            {
                setText("Would you like to use the same settings? YES NO");

                if (!newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                {
                    leftOption();
                }

                if (!newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                {
                    rightOption();
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    hero = new Hero();

                    if (setting_select == 0)
                    {
                        same_setting = true;
                    }

                    Initialize();
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                }
            }
            //FLAG FOR SHOOTING WUMPUS
            else if (hero.shotWumpus())
            {
                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "RESET";
                    setPaddle(200, 730);
                    setPaddleInc(55);
                    option_num = 2;
                    resetOption();
                }
            }
            //FLAG WHEN ARROWS HAVE BEEN DEPLETED
            else if (!hero.haveArrows())
            {
                setText("You didn't hit anything...\nAnd you just used your last arrow! Can't do anything now...\n\nGame Over!\n\n");

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "RESET";
                    setPaddle(200, 730);
                    setPaddleInc(55);
                    option_num = 2;
                    resetOption();
                }
            }
            //SUPERBATS TELEPORTATION
            else if (game_state == "BATS")
            {
                optionPaddle_on = false;
                setOptionText("");
                setText("Superbats just caught you! They going to drop you in a random room...\n\nPress Enter");
                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    pos = startPosition();
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    resetOption();
                }
            }
            //Check for hazards if player is in the same room as them
            else if (map.isWumpusRoom(pos))
            {
                optionPaddle_on = false;
                setOptionText("");
                setText("...Ooops ... Bumped a Wumpus :[ \r\nHe ate you.\r\n\r\nha ha ha - you lose!\n\nGame Over!\n\n");
                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "RESET";
                    setPaddle(200, 730);
                    setPaddleInc(55);
                    option_num = 2;
                    resetOption();
                }
            }
            else if (map.isSuperBatsRoom(pos))
            {
                game_state = "BATS";
            }
            else if (map.isPitRoom(pos))
            {
                optionPaddle_on = false;
                setOptionText("");
                setText("YYYYIIIIEEEE... Fell in pit!\r\n\r\nha ha ha - you lose!\n\nGame Over!\n\n");
                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "RESET";
                    setPaddle(200, 730);
                    setPaddleInc(55);
                    option_num = 2;
                    resetOption();
                }
            }
            //STATE AFTER SHOOTING ARROW
            else if (game_state == "SHOT")
            {
                setOptionText("");

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    resetOption();
                }
            }
            //STATE DURING ARROW SHOOTING
            else if (game_state == "SHOOT")
            {
                MouseState ms = Mouse.GetState();
                setText("Left click the rooms that you should like to shoot in,\nRight click to undo,\nor press Enter to confirm.\n\n\nPress Backspace to go back...");
                setOptionText("");

                mouseX = ms.X;
                mouseY = ms.Y;

                if (aim_count < 5)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        if (mouseX > (map.getRoom(k).getVector().X) - 15 && mouseX < (map.getRoom(k).getVector().X) + 35 && mouseY > (map.getRoom(k).getVector().Y) - 10 && mouseY < (map.getRoom(k).getVector().Y) + 40 && ms.LeftButton != ButtonState.Pressed && moldState.LeftButton == ButtonState.Pressed && map.getRoom(aim_history[aim_count]).isNeighbor(k + 1) && room_aimed[k] == false)
                        {
                            room_aimed[k] = true;
                            aim_count++;
                            aim_history[aim_count] = k;
                        }
                    }
                }

                for (int k = 0; k < 20; k++)
                {
                    if (mouseX > (map.getRoom(k).getVector().X) - 15 && mouseX < (map.getRoom(k).getVector().X) + 35 && mouseY > (map.getRoom(k).getVector().Y) - 10 && mouseY < (map.getRoom(k).getVector().Y) + 40 && ms.RightButton != ButtonState.Pressed && moldState.RightButton == ButtonState.Pressed && room_aimed[k] == true && k == aim_history[aim_count])
                    {
                        room_aimed[k] = false;
                        aim_history[aim_count] = 0;
                        aim_count--;

                    }
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter) && aim_count != 0)
                {
                    setText(hero.shoot(aim_history));
                    game_state = "SHOT";
                    aim_count = 0;
                    aim_history = new int[6];
                    resetOption();
                    room_aimed = new bool[20];
                }

                if (!newState.IsKeyDown(Keys.Back) && oldState.IsKeyDown(Keys.Back))
                {
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    aim_count = 0;
                    aim_history = new int[6];
                    resetOption();
                    room_aimed = new bool[20];
                }

                moldState = ms;
            }
            //MOVING TO ANOTHER ROOM
            else if (game_state == "MOVE")
            {
                optionPaddle_on = true;

                setText("You chose to move. Where to? " + map.getRoom(pos).printNeighbors());

                if (!newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                {
                    leftOption();
                }
                if (!newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                {
                    rightOption();
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    pos = map.getRoom(pos).getNeighbor(room_select) - 1;
                    hero.movePosition(pos);

                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    resetOption();
                }

                if (!newState.IsKeyDown(Keys.Back) && oldState.IsKeyDown(Keys.Back))
                {
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    resetOption();
                }

            }
            //QUITTING GAME
            else if (game_state == "QUIT")
            {
                optionPaddle_on = true;

                setText("Are you sure you want to quit? YES NO");

                if (!newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                {
                    leftOption();
                }
                if (!newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                {
                    rightOption();
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    if (quit_option == 0)
                    {
                        this.Exit();
                    }
                    else if (quit_option == 1)
                    {
                        game_state = "OPTION";
                        setPaddle(299, 880);
                        setPaddleInc(30);
                        option_num = 3;
                        resetOption();
                    }
                }

                if (!newState.IsKeyDown(Keys.Back) && oldState.IsKeyDown(Keys.Back))
                {
                    game_state = "OPTION";
                    setPaddle(299, 880);
                    setPaddleInc(30);
                    option_num = 3;
                    resetOption();
                }
            }
            //OPTION SCREEN
            else
            {
                console_text = "You entered the game";
                optionPaddle_on = true;
                gameLoop();

                if (!newState.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                {
                    leftOption();
                }
                if (!newState.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                {
                    rightOption();
                }

                if (!newState.IsKeyDown(Keys.Enter) && oldState.IsKeyDown(Keys.Enter))
                {
                    if (option_select == 1)
                    {
                        game_state = "SHOOT";
                        aim_history[0] = pos;
                    }
                    else if (option_select == 2)
                    {
                        game_state = "MOVE";
                        setPaddle(416, 730);
                        setPaddleInc(45);
                        option_num = 3;
                    }
                    else
                    {
                        game_state = "QUIT";
                        setPaddle(414, 730);
                        setPaddleInc(55);
                        option_num = 2;
                    }

                    optionPaddle_on = false;
                }
            }

            oldState = newState;

            base.Update(gameTime);
        }

        private void resetOption()
        {
            option_select = 1;
            room_select = 0;
            quit_option = 0;
            setting_select = 0;
        }

        public void rightOption()
        {
            if (paddlePosX < (paddleThreshold+((option_num-1)*paddle_increment)))
            {
                paddlePosX += paddle_increment;

                if (game_state == "OPTION")
                    option_select++;
                else if (game_state == "MOVE")
                    room_select++;
                else if (game_state == "QUIT")
                    quit_option++;
                else if (game_state == "RESET")
                    quit_option++;
                else if (game_state == "SETTING")
                    setting_select++;
            }
        }

        public void leftOption()
        {
            if (paddlePosX > paddleThreshold)
            {
                paddlePosX -= paddle_increment;

                if (game_state == "OPTION")
                    option_select--;
                else if (game_state == "MOVE")
                    room_select--;
                else if (game_state == "QUIT")
                    quit_option--;
                else if (game_state == "RESET")
                    quit_option--;
                else if (game_state == "SETTING")
                    setting_select--;
            }
        }

        private void setPaddleInc(int inc)
        {
            paddle_increment = inc;
        }

        private void setPaddle(int x, int y)
        {
            paddlePosX = x;
            paddlePosY = y;
            paddleThreshold = x;
        }

        private int getPaddleX()
        {
            return paddlePosX;
        }

        private int getPaddleY()
        {
            return paddlePosY;
        }

        private int getPaddleThresh()
        {
            return paddleThreshold;
        }

        //DRAW METHOD OF GAME. DRAWS ALL SPRITES
        protected override void Draw(GameTime gameTime)
        {        
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            drawMap(gameTime);

            spriteBatch.Begin();
            spriteBatch.Draw(paths, new Rectangle(0, 670, 1000, 230), Color.SlateGray);
            spriteBatch.Draw(paths, new Rectangle(10, 680, 980, 210), Color.SteelBlue);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (map.isWumpusRoom(pos))
                spriteBatch.Draw(wumpus_img, map.getRoom(map.whereWumpus()).getVector(), Color.White);
            else if (map.isPitRoom(pos))
            {
                spriteBatch.Draw(pit_img, playerPosition, Color.White);
            }

            else if (game_state == "BATS")
            {
                spriteBatch.Draw(superbats_img, playerPosition, Color.White);
            }
            else
            {
                spriteBatch.Draw(player, playerPosition, Color.White);

                if(hero.shotWumpus())
                    spriteBatch.Draw(wumpus_img, map.getRoom(map.whereWumpus()).getVector(), Color.White);
            }

           

            
            //CONSOLE TEXT
            drawText(console_text, 30, 700, Color.Black);

            //OPTION TEXT
            drawText(option_text, 30, 790, Color.Pink);

            //OPTION PADDLE
            if(optionPaddle_on)
                spriteBatch.Draw(paths, new Rectangle(paddlePosX, paddlePosY, 20, 4), Color.Red);
            
            spriteBatch.End(); 
        }

        //MAP OF GAME
        private void drawMap(GameTime gt)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            drawPaths();
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            drawRooms();
            spriteBatch.End();
        }

        private void drawText(String s, int x, int y, Color c)
        {
            spriteBatch.DrawString(font, s, new Vector2(x, y), c);
        }

        //SETS TEXT ON UPPER PART OF SCREEN
        public void setText(String text)
        {
            console_text = text;
        }

        //SETS TEXT ON BOTTOM PART OF SCREEN
        private void setOptionText(String text)
        {
            option_text = text;
        }

//======================Original Hunt the Wumpus==========================================================
//========================================================================================================
        //Place the user where the starting location isn't in a danger zone.
        private int startPosition()
        {
            int start = ran.Next(20);

            while (map.checkRoom(start))
            {
                start = ran.Next(20);
            }

            return start;
        }

        //initialize map where hazards and player are in random rooms
        private void newSettings()
        {
            map.initialize_rooms();
            saved_map = map;

            pos = startPosition();
            saved_pos = pos;
        }

        //replays the same setting as last game
        private void sameSettings()
        {
            map = saved_map;

            pos = saved_pos;

            same_setting = false;
        }

        /*
         * Main game loop 
         */
        private void gameLoop()
        {
            string message = "";
            
            /*
                *Give players option of whether to shoot, move, or quit while seeing if player has inputted a proper command. 
                */
            
            message = map.checkNeighbors(pos);
            setOptionText("You are in room " + map.getRoom(pos).getRoomNumber() + "\n" +
                "Tunnels lead to " + map.getRoom(pos).printNeighbors() + "\n" +
                "Shoot, Move, or Quit (S-M-Q)? <Use arrow keys to select>");
            setText(message);      
        }
//========================================================================================================


        /*
         * LINES AND RECTANGLES OF THE MAP
         */
        private void drawPaths()
        {
            spriteBatch.Draw(paths, new Rectangle(200, 50, 10, 550), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(190, 70, 10, 70), null, Color.Gray, MathHelper.ToRadians(295), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(755, 60, 10, 70), null, Color.Gray, MathHelper.ToRadians(65), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(300, 120, 10, 150), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(500, 120, 10, 150), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(470, 140, 10, 20), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(250, 140, 10, 90), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(690, 140, 10, 90), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(400, 237, 10, 90), null, Color.Gray, MathHelper.ToRadians(230), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(560, 233, 10, 90), null, Color.Gray, MathHelper.ToRadians(130), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(275, 260, 10, 75), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(600, 260, 10, 75), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(160, 65, 10, 300), null, Color.Gray, MathHelper.ToRadians(15), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(780, 65, 10, 300), null, Color.Gray, MathHelper.ToRadians(345), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(115, 370, 10, 85), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(425, 370, 10, 100), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(740, 370, 10, 85), null, Color.Gray, MathHelper.ToRadians(270), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(230, 280, 10, 65), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(380, 280, 10, 65), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(560, 280, 10, 65), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(710, 280, 10, 65), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(80, 375, 10, 450), null, Color.Gray, MathHelper.ToRadians(305), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(855, 375, 10, 450), null, Color.Gray, MathHelper.ToRadians(55), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(240, 375, 10, 100), null, Color.Gray, MathHelper.ToRadians(315), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(345, 478, 10, 115), null, Color.Gray, MathHelper.ToRadians(290), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(390, 375, 10, 100), null, Color.Gray, MathHelper.ToRadians(45), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(545, 375, 10, 100), null, Color.Gray, MathHelper.ToRadians(315), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(600, 470, 10, 110), null, Color.Gray, MathHelper.ToRadians(70), Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(paths, new Rectangle(700, 370, 10, 100), null, Color.Gray, MathHelper.ToRadians(45), Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(paths, new Rectangle(470, 520, 10, 100), null, Color.Gray, MathHelper.ToRadians(0), Vector2.Zero, SpriteEffects.None, 0);

        }

        private void drawRooms()
        {
            //Top to Bottom
            Color room_color = Color.BlueViolet;

            //Room 1          
            if (room_aimed[0])
                room_color = Color.LightPink;
            else if (room_entered[0])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
            spriteBatch.Draw(paths, new Rectangle(150, 20, 50, 50), room_color);

            //Room 12 
            if (room_aimed[11])
                room_color = Color.LightPink;
            else if (room_entered[11])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
               spriteBatch.Draw(paths, new Rectangle(750, 20, 50, 50), room_color);

            //Room 6
            if (room_aimed[5])
                room_color = Color.LightPink;
            else if (room_entered[5])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(250, 90, 50, 50), room_color);
            //Room 9     
            if (room_aimed[8])
                room_color = Color.LightPink;
            else if (room_entered[8])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(450, 90, 50, 50), room_color);
            //Room 4
            if (room_aimed[3])
                room_color = Color.LightPink;
            else if (room_entered[3])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(650, 90, 50, 50), room_color);

            //Room 19
            if (room_aimed[18])
                room_color = Color.LightPink;
            else if (room_entered[18])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(450, 160, 50, 50), room_color);

            //Room 14 
            if (room_aimed[13])
                room_color = Color.LightPink;
            else if (room_entered[13])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(225, 230, 50, 50), room_color);
            //Room 5
            if (room_aimed[4])
                room_color = Color.LightPink;
            else if (room_entered[4])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(350, 230, 50, 50), room_color);
            //Room 16 
            if (room_aimed[15])
                room_color = Color.LightPink;
            else if (room_entered[15])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(550, 230, 50, 50), room_color);
            //Room 10
            if (room_aimed[9])
                room_color = Color.LightPink;
            else if (room_entered[9])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(675, 230, 50, 50), room_color);

            //Room 3
            if (room_aimed[2])
                room_color = Color.LightPink;
            else if (room_entered[2])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(75, 340, 50, 50), room_color);


            //Room 17 
            if (room_aimed[16])
                room_color = Color.LightPink;
            else if (room_entered[16])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(200, 340, 50, 50), room_color);
            //Room 13
            if (room_aimed[12])
                room_color = Color.LightPink;
            else if (room_entered[12])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(375, 340, 50, 50), room_color);
            //Room 2  
            if (room_aimed[1])
                room_color = Color.LightPink;
            else if (room_entered[1])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(525, 340, 50, 50), room_color);
            //Room 20
            if (room_aimed[19])
                room_color = Color.LightPink;
            else if (room_entered[19])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(700, 340, 50, 50), room_color);
            //Room 7
            if (room_aimed[6])
                room_color = Color.LightPink;
            else if (room_entered[6])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(825, 340, 50, 50), room_color);

            //Room 8
            if (room_aimed[7])
                room_color = Color.LightPink;
            else if (room_entered[7])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(300, 430, 50, 50), room_color);
            //Room 18
            if (room_aimed[17])
                room_color = Color.LightPink;
            else if (room_entered[17])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(600, 430, 50, 50), room_color);

            //Room 15
            if (room_aimed[14])
                room_color = Color.LightPink;
            else if (room_entered[14])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(450, 500, 50, 50), room_color);

            //Room 11    
            if (room_aimed[10])
                room_color = Color.LightPink;
            else if (room_entered[10])
                room_color = Color.Yellow;
            else
                room_color = Color.BlueViolet;
                spriteBatch.Draw(paths, new Rectangle(450, 610, 50, 50), room_color);
        }
    }

    //spriteBatch.Draw(paths, new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2), (graphics.GraphicsDevice.Viewport.Height / 2), 10, 100), null, Color.Black, MathHelper.ToRadians(45), Vector2.Zero, SpriteEffects.None, 0);
    //spriteBatch.Draw(paths, new Rectangle(100, 100, 10, 100), null, Color.Pink, MathHelper.ToRadians(rotate), Vector2.Zero, SpriteEffects.None, 0);
}
