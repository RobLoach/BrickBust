/*
 * $RCSfile$
 * Copyright (C) 2006 Rob Loach (http://www.robloach.net)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace BrickBust
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    partial class BrickBustGame : Microsoft.Xna.Framework.Game
    {

        const float PaddleSpeed = 120f;
        const float BallStartSpeed = 60f;


        public BrickBustGame()
        {
            InitializeComponent();
        }

        protected override void OnStarting()
        {
            // Load game resources and setup graphic device events
            graphics.DeviceReset += new EventHandler(graphics_DeviceReset);
            LoadResources();

            // Setup the game
            m_Paddle.X = graphics.BackBufferWidth / 2f - m_Textures["Paddle"].Width / 2f;
            m_Paddle.Y = graphics.BackBufferHeight - m_Textures["Paddle"].Height * 2f;
            ResetGame();
        }

        void graphics_DeviceReset(object sender, EventArgs e)
        {
            // Reload the resources if the graphics device is lost
            LoadResources();
        }

        private void LoadResources()
        {
            // Load textures and initialize sprite drawing
            m_Sprite = new SpriteBatch(graphics.GraphicsDevice);
            m_Textures["Bricks"] = Texture2D.FromFile(graphics.GraphicsDevice, Path.Combine("Graphics", "Bricks.png"));
            m_Textures["Paddle"] = Texture2D.FromFile(graphics.GraphicsDevice, Path.Combine("Graphics", "Paddle.png"));
            m_Textures["Ball"] = Texture2D.FromFile(graphics.GraphicsDevice, Path.Combine("Graphics", "Ball.png"));
        }

        protected override void Update()
        {
            // The time since Update was called last
            float elapsed = (float)ElapsedTime.TotalSeconds;

            // TODO: Add your game logic here

            // Check player input
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left))
                m_Paddle.X -= PaddleSpeed * elapsed;
            else if(keyboard.IsKeyDown(Keys.Right))
                m_Paddle.X += PaddleSpeed * elapsed;

            // Bound paddle in the game screen
            if (m_Paddle.X < 0)
                m_Paddle.X = 0f;
            else if (m_Paddle.X + m_Textures["Paddle"].Width > graphics.BackBufferWidth)
                m_Paddle.X = graphics.BackBufferWidth - m_Textures["Paddle"].Width;

            // Move the ball 
            m_Ball.X += m_BallSpeed.X * elapsed;
            m_Ball.Y += m_BallSpeed.Y * elapsed;

            // Check ball collisions with walls
            if (m_Ball.X < 0f && m_BallSpeed.X < 0f)
                m_BallSpeed.X *= -1f;
            if (m_Ball.X + m_Textures["Ball"].Width > graphics.BackBufferWidth && m_BallSpeed.X > 0f)
                m_BallSpeed.X *= -1f;
            if (m_Ball.Y < 0f && m_BallSpeed.Y < 0f)
                m_BallSpeed.Y *= -1f;
            if (m_Ball.Y > graphics.BackBufferHeight)
            {
                m_BallsLeft--;
                ResetBall();
                if (m_BallsLeft <= 0)
                    ResetGame();
            }

            // Check ball collisions with bricks
            float brickWidth = m_Textures["Bricks"].Width / 3f;
            float brickHeight = m_Textures["Bricks"].Height / 3f;

            float ballWidth = m_Textures["Ball"].Width;
            float ballHeight = m_Textures["Ball"].Height;
            for (int x = 0; x < m_CurrentLevel.Blocks.GetLength(1); x++)
            {
                for (int y = 0; y < m_CurrentLevel.Blocks.GetLength(0); y++)
                {
                    if (m_CurrentLevel.Blocks[y, x] > 0)
                    {
                        float brickX = x * brickWidth;
                        float brickY = y * brickHeight;

                        bool hit = false;

                        // Check if hit top
                        if (IsPointInRectangle(m_Ball.X + ballWidth / 2f, m_Ball.Y, brickX, brickY, brickWidth, brickHeight))
                        {
                            hit = true;
                            m_BallSpeed.Y *= -1f;
                            m_Ball.Y = brickY + brickHeight;
                        }
                        // Check if hit bottom
                        else if (IsPointInRectangle(m_Ball.X + ballWidth / 2f, m_Ball.Y + ballHeight, brickX, brickY, brickWidth, brickHeight))
                        {
                            hit = true;
                            m_BallSpeed.Y *= -1f;
                            m_Ball.Y = brickY - ballHeight;
                        }
                        // Check if hit left
                        if (IsPointInRectangle(m_Ball.X, m_Ball.Y + ballHeight / 2f, brickX, brickY, brickWidth, brickHeight))
                        {
                            hit = true;
                            m_BallSpeed.X *= -1f;
                            m_Ball.X = brickX + brickWidth;
                        }
                        // Check if hit right
                        else if (IsPointInRectangle(m_Ball.X + ballWidth, m_Ball.Y + ballHeight / 2f, brickX, brickY, brickWidth, brickHeight))
                        {
                            hit = true;
                            m_BallSpeed.X *= -1f;
                            m_Ball.X = brickX - ballWidth;
                        }

                        if (hit && m_CurrentLevel.Blocks[y, x] != 9)
                        {
                            // Decrease the life of the brick and increase the player's score
                            m_Score += m_CurrentLevel.Blocks[y, x]--;

                            // Check winning conditions
                            if (m_CurrentLevel.Blocks[y, x] <= 0)
                                CheckIfWon();
                        }
                    }
                }
            }

            // Check ball collision with paddle
            if (m_BallSpeed.Y > 0f)
            {
                if (m_Ball.Y + ballHeight > m_Paddle.Y
                    && m_Ball.Y < m_Paddle.Y + m_Textures["Paddle"].Height
                    && m_Ball.X < m_Paddle.X + m_Textures["Paddle"].Width
                    && m_Ball.X + ballWidth > m_Paddle.X)
                {
                    // Reverse Y direction while increasing speed
                    m_BallSpeed.Y *= -1.05f;
                    
                    // Check if the player is going to change direction of ball
                    if (keyboard.IsKeyDown(Keys.Left))
                        m_BallSpeed.X = -Math.Abs(m_BallSpeed.X) - BallStartSpeed / 4f;
                    else if(keyboard.IsKeyDown(Keys.Right))
                        m_BallSpeed.X = Math.Abs(m_BallSpeed.X) + BallStartSpeed / 4f;
                }
            }

            // Let the GameComponents update
            UpdateComponents();
        }

        public void CheckIfWon()
        {
            // Go through each block and see if the player won
            bool won = true;
            foreach (int i in m_CurrentLevel.Blocks)
            {
                if (i > 0 && i < 9)
                {
                    won = false;
                    break;
                }
            }
            if (won)
            {
                m_CurrentLevel = new Level(++m_Level);
                ResetBall();
            }
        }

        bool IsPointInRectangle(float x, float y, float rectX, float rectY, float rectWidth, float rectHeight)
        {
            // Return true if the point is in the rectangle
            if ((x >= rectX)
                && (x <= rectX + rectWidth)
                && (y >= rectY)
                && (y <= rectY + rectHeight))
                return true;
            return false;
        }

        protected override void Draw()
        {
            // Make sure we have a valid device
            if (!graphics.EnsureDevice())
                return;

            graphics.GraphicsDevice.Clear(Color.Black);
            graphics.GraphicsDevice.BeginScene();

            // TODO: Add your drawing code here

            // Prepare to draw all game entities
            m_Sprite.Begin(SpriteBlendMode.AlphaBlend);

            // Draw all bricks
            int width = m_Textures["Bricks"].Width / 3;
            int height = m_Textures["Bricks"].Height / 3;

            for (int x = 0; x < m_CurrentLevel.Blocks.GetLength(1); x++)
            {
                for (int y = 0; y < m_CurrentLevel.Blocks.GetLength(0); y++)
                {
                    // Base the brick colour on how much life is left in the ball
                    Rectangle source = Rectangle.Empty;
                    switch (m_CurrentLevel.Blocks[y, x])
                    {
                        case 1: source = new Rectangle(0, 0, width, height); break;
                        case 2: source = new Rectangle(width, 0, width, height); break;
                        case 3: source = new Rectangle(width * 2, 0, width, height); break;
                        case 4: source = new Rectangle(0, height, width, height); break;
                        case 5: source = new Rectangle(width, height, width, height); break;
                        case 6: source = new Rectangle(width * 2, height, width, height); break;
                        case 7: source = new Rectangle(0, height * 2, width, height); break;
                        case 8: source = new Rectangle(width, height * 2, width, height); break;
                        case 9: source = new Rectangle(width * 2, height * 2, width, height); break;
                    }
                    if (source != Rectangle.Empty)
                        m_Sprite.Draw(m_Textures["Bricks"], new Vector2(x * width, y * height), source, Color.White);
                }
            }

            // Draw the paddle and the ball
            m_Sprite.Draw(m_Textures["Ball"], m_Ball, Color.White);
            m_Sprite.Draw(m_Textures["Paddle"], m_Paddle, Color.White);

            m_Sprite.End();

            // Display the textual data to the player using our fancy BitmapFont object
            smallFont.Draw(10f, 5f, "Score: " + m_Score);
            smallFont.Draw(graphics.BackBufferWidth - 10f, 5f, "Level: " + m_Level, BitmapFont.Alignment.Right);
            smallFont.Draw(graphics.BackBufferWidth / 2f, 5f, "Balls: " + m_BallsLeft, BitmapFont.Alignment.Center);

            // Let the GameComponents draw
            DrawComponents();

            graphics.GraphicsDevice.EndScene();
            graphics.GraphicsDevice.Present();
        }

        public void ResetGame()
        {
            // Restart the whole game
            m_Score = 0;
            m_BallsLeft = 5;
            m_CurrentLevel = new Level(m_Level = 1);
            ResetBall();
        }

        public void ResetBall()
        {
            // Reset the position and speed of the ball
            m_Ball.X = m_Paddle.X + m_Textures["Paddle"].Width / 2f - m_Textures["Ball"].Width / 2f;
            m_Ball.Y = m_Paddle.Y;
            m_BallSpeed = new Vector2(BallStartSpeed, -BallStartSpeed);
        }


        #region Fields
        private SpriteBatch m_Sprite;
        private int m_Score = 0;
        private int m_Level = 1;
        private int m_BallsLeft = 5;
        private Level m_CurrentLevel = new Level(1);
        private Vector2 m_Paddle = new Vector2();
        private Vector2 m_Ball = new Vector2();
        private Vector2 m_BallSpeed = new Vector2(BallStartSpeed, -BallStartSpeed);

        private Dictionary<string, Texture2D> m_Textures = new Dictionary<string, Texture2D>();
        #endregion Fields
    }
}