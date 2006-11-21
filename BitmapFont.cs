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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using System.IO;

namespace BrickBust
{
    /// <summary>
    ///     Represents a font which uses an image to display text, generated
    ///     from LMNOpc Bitmap Font Builder (http://www.lmnopc.com/bitmapfontbuilder).
    /// </summary>
    /// <remarks>
    ///     To use this bitmap font component, just drag an instance of it onto your
    ///     designer view and then interface with it through code in the Draw method.
    /// </remarks>
    public class BitmapFont : Microsoft.Xna.Framework.GameComponent
    {
        #region Fields

        // Private Fields
        private SpriteBatch m_Sprites;
        private byte[] m_Widths;
        private Texture2D m_Texture;
        private IGraphicsDeviceService m_GraphicsDeviceService;

        // Property Associated
        private SpriteBlendMode m_BlendMode = SpriteBlendMode.AlphaBlend;
        private Color m_Color = Color.White;
        private string m_DataFilename = string.Empty;
        private string m_ImageFilename = string.Empty;

        #endregion Fields

        #region Private Functions

        /// <summary>
        /// Loads the resources into memory.
        /// </summary>
        private void LoadResources()
        {
            m_Texture = Texture2D.FromFile(m_GraphicsDeviceService.GraphicsDevice, m_ImageFilename);
            m_Sprites = new SpriteBatch(m_GraphicsDeviceService.GraphicsDevice);

            using(FileStream stream = new FileStream(m_DataFilename, FileMode.Open))
            {
                m_Widths = new byte[stream.Length];
                stream.Read(m_Widths, 0, (int)stream.Length);
                stream.Close();
            }
        }
        /// <summary>
        /// Releases the context of the sprite batch, texture and widths.
        /// </summary>
        private void ReleaseContent()
        {
            if (m_Texture != null)
            {
                m_Texture.Dispose();
                m_Texture = null;
            }
            if (m_Sprites != null)
            {
                m_Sprites.Dispose();
                m_Sprites = null;
            }
            m_Widths = null;
        }
	
        /// <summary>
        /// Prepares the bitmap font for rendering.
        /// </summary>
        public override void Start()
        {
            // TODO: Add your start up code here
            m_GraphicsDeviceService = this.Game.GameServices.GetService<IGraphicsDeviceService>();
            m_GraphicsDeviceService.DeviceReset += new EventHandler(GraphicsDeviceService_DeviceReset);
            m_GraphicsDeviceService.DeviceResetting += new EventHandler(GraphicsDeviceService_DeviceResetting);
            m_GraphicsDeviceService.DeviceCreated += new EventHandler(GraphicsDeviceService_DeviceCreated);
            m_GraphicsDeviceService.DeviceDisposing += new EventHandler(GraphicsDeviceService_DeviceDisposing);
            LoadResources();
        }

        private void GraphicsDeviceService_DeviceDisposing(object sender, EventArgs e)
        {
            ReleaseContent();
        }

        private void GraphicsDeviceService_DeviceResetting(object sender, EventArgs e)
        {
            ReleaseContent();
        }

        private void GraphicsDeviceService_DeviceCreated(object sender, EventArgs e)
        {
            LoadResources();
        }

        private void GraphicsDeviceService_DeviceReset(object sender, EventArgs e)
        {
            LoadResources();
        }

        #endregion Private Functions

        #region Properties

        /// <summary>
        /// Gets and sets the BlendMode for drawing.  Defaults for alpha blending.
        /// </summary>
        public SpriteBlendMode BlendMode
        {
            get { return m_BlendMode; }
            set { m_BlendMode = value; }
        }

        /// <summary>
        /// Gets and sets the default color the font should draw in.
        /// </summary>
        public Color Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        /// <summary>
        /// Gets the height of each line.
        /// </summary>
        public float LineHeight
        {
            get
            {
                if (m_Texture != null)
                    return m_Texture.Height / 16;
                else
                    return 0;
            }
        }

        /// <summary>
        /// The image file that was generated from LMNOpc Bitmap Font Builder.
        /// </summary>
        public string ImageFilename
        {
            get { return m_ImageFilename; }
            set
            {
                m_ImageFilename = value;
            }
        }

        /// <summary>
        /// The data file generated from LMNOpc Bitmap Font Builder.
        /// </summary>
        public string DataFilename
        {
            get { return m_DataFilename; }
            set
            {
                m_DataFilename = value;
            }
        }

        #endregion Properties

        #region Functions

        #region TextWidth

        /// <summary>
        /// Gets the width of the given text.
        /// </summary>
        /// <param name="text">The text to check the width of.</param>
        /// <returns>The width of the given text.</returns>
        public float TextWidth(string text)
        {
            float width = 0f;
            CharEnumerator chars = text.GetEnumerator();
            while (chars.MoveNext())
                width += m_Widths[(int)chars.Current * 2];
            return width;
        }

        #endregion TextWidth
        
        #region Draw

        public void Draw(float x, float y, string text, Alignment alignment)
        {
            if (text == null)
                return;
            if (text.Length == 0)
                return;

            switch (alignment)
            {
                case Alignment.Center:
                    x -= TextWidth(text) / 2f;
                    break;
                case Alignment.Right:
                    x -= TextWidth(text);
                    break;
            }
            if (m_Sprites != null)
            {
                m_Sprites.Begin(m_BlendMode);
                {
                    float xLocation = x;
                    CharEnumerator chars = text.GetEnumerator();
                    while (chars.MoveNext())
                    {
                        byte ascii = (byte)chars.Current;
                        int charX = (ascii % 16) * m_Texture.Width / 16;
                        int charY = (ascii / 16) * m_Texture.Height / 16;

                        m_Sprites.Draw(m_Texture, new Vector2(xLocation, y), new Rectangle(charX, charY, (int)m_Widths[ascii * 2], m_Texture.Width / 16), m_Color);
                        xLocation += (float)m_Widths[ascii * 2];
                    }
                }
                m_Sprites.End();
            }
        }
        public void Draw(float x, float y, string text)
        {
            Draw(x, y, text, Alignment.Left);
        }

        public void Draw(Vector2 location, string text)
        {
            Draw(location.X, location.Y, text, Alignment.Left);
        }

        public void Draw(Vector2 location, string text, Alignment alignment)
        {
            Draw(location.X, location.Y, text, alignment);
        }
        public void Draw(string text)
        {
            Draw(0f, 0f, text, Alignment.Left);
        }

        #endregion Draw

        #endregion Functions

        #region Enumerations

        public enum Alignment
        {
            Left, Center, Right
        }

        #endregion Enumerations
    }
}