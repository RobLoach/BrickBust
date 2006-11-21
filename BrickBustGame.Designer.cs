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

namespace BrickBust
{
    public partial class BrickBustGame
    {
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.graphics = new Microsoft.Xna.Framework.Components.GraphicsComponent();
            this.smallFont = new BrickBust.BitmapFont();
            // 
            // graphics
            // 
            this.graphics.BackBufferHeight = 233;
            this.graphics.BackBufferWidth = 310;
            // 
            // smallFont
            // 
            this.smallFont.BlendMode = Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend;
            this.smallFont.DataFilename = "Fonts/Arial.dat";
            this.smallFont.ImageFilename = "Fonts/Arial.tga";
            this.GameComponents.Add(this.graphics);
            this.GameComponents.Add(this.smallFont);

        }

        private Microsoft.Xna.Framework.Components.GraphicsComponent graphics;
        private BitmapFont smallFont;
    }
}
