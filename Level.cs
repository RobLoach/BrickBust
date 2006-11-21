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
using System.Text;
using System.IO;

namespace BrickBust
{
    public class Level
    {
        // The game field
        int[,] m_Blocks = new int[10,10];

        public Level(int levelNumber)
        {
            // Load the level data
            string levelFile = Path.Combine("Levels", levelNumber.ToString("00") + ".txt");
            if (!File.Exists(levelFile))
                levelFile = Path.Combine("Levels", (levelNumber % 10).ToString("00") + ".txt");
            string[] lines;
            using (StreamReader reader = new StreamReader(levelFile))
                lines = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < (lines.Length < m_Blocks.GetLength(0) ? lines.Length : m_Blocks.GetLength(0)); i++)
            {
                for (int b = 0; b < lines[i].Length; b++)
                {
                    int blockLife;
                    if (!int.TryParse(lines[i][b].ToString(), out blockLife))
                        blockLife = 0;
                    m_Blocks[i,b] = blockLife;
                }
            }
        }

        // Blocks included in the level
        public int[,] Blocks
        {
            get
            {
                return m_Blocks;
            }
        }
    }
}
