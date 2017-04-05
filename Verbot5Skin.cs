/*
	Copyright 2004-2006 Conversive, Inc.
	http://www.conversive.com
	3806 Cross Creek Rd., Unit F
	Malibu, CA 90265

	This file is part of Verbot 5 Library: a natural language processing engine.

    Verbot 5 Library is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Verbot 5 Library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Verbot 5 Library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

	Verbot 5 Library may also be available under other licenses.
*/

using System.Xml.Serialization;

namespace Conversive.Verbot5
{
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = 3,
        Underline = 4,
        BoldUnderline = 5,
        ItalicUnderline = 6,
        BoldItalicUnderline = 7,
        Strikeout = 8,
        BoldStrikeout = 9,
        ItalicStrikeout = 10,
        BoldItalicStrikeout = 11,
        UnderlineStrikeout = 12,
        BoldUnderlineStrikeout = 13,
        ItalicUnderlineStrikeout = 14,
        BoldItalicUnderlineStrikeout = 15
    };

    public class Font
    {
        #region Public Fields

        public string FontName;
        public float FontSize;
        public FontStyle FontStyle;

        #endregion Public Fields

        #region Public Constructors

        public Font()
        {
            this.FontName = "Microsoft Sans Serif";
            this.FontSize = (float)8.25;
            this.FontStyle = FontStyle.Regular;
        }

        #endregion Public Constructors
    }

    /// <summary>
    /// Defines the look of the player.
    /// </summary>
    public class Verbot5Skin
    {
        #region Public Fields

        public string AgentFileName;
        public string AgentPanelBackgroundColor;
        public int AgentPitch;
        public int AgentSpeed;
        public string AgentTTSMode;
        public bool AllowWindowResize;
        public string AppBackgroundColor;
        public string BackgroundImageFileName;

        [XmlIgnoreAttribute]
        public bool Changed;

        public string CharacterFile;
        public int CharacterTTSMode;
        public string InputBackgroundColor;
        public Font InputFont;
        public string InputTextColor;
        public int LanguageId;
        public string OutputBackgroundColor;
        public Font OutputFont;
        public string OutputTextColor;
        public bool UseConversiveCharacter = false;

        public int WindowHeight;
        public int WindowWidth;

        #endregion Public Fields

        #region Public Constructors

        public Verbot5Skin()
        {
            this.AgentFileName = "merlin.acs";
            this.LanguageId = 1033;
            this.AgentTTSMode = "";
            this.AgentSpeed = 0;
            this.AgentPitch = 0;

            this.CharacterFile = "julia.ccs";
            this.CharacterTTSMode = 0;

            this.WindowHeight = 0;
            this.WindowWidth = 0;
            this.AllowWindowResize = true;
            this.BackgroundImageFileName = "";
            this.AppBackgroundColor = "#D4D0C8";
            this.AgentPanelBackgroundColor = "#D4D0C8";

            this.InputFont = new Font();
            this.OutputFont = new Font();

            this.InputTextColor = "#000000";
            this.InputBackgroundColor = "#FFFFFF";
            this.OutputTextColor = "#000000";
            this.OutputBackgroundColor = "#FFFFFF";
        }

        #endregion Public Constructors

        #region Public Methods

        public int HexToRGB(string Hex, int iDefault)
        {
            int iColor = iDefault;
            if (Hex.Length == 7 && Hex[0] == '#')
            {
                iColor = int.Parse(("FF" + Hex[1] + Hex[2] + Hex[3] + Hex[4] + Hex[5] + Hex[6]), System.Globalization.NumberStyles.HexNumber);
            }
            return iColor;
        }

        public string RGBToHex(int RGB)
        {
            string r = ((byte)((RGB & 0x00FF0000) >> 16)).ToString("X");
            if (r.Length == 1)
                r = "0" + r;
            string g = ((byte)((RGB & 0x0000FF00) >> 8)).ToString("X");
            if (g.Length == 1)
                g = "0" + g;
            string b = ((byte)((RGB & 0x000000FF) >> 0)).ToString("X");
            if (b.Length == 1)
                b = "0" + b;
            return "#" + r + g + b;
        }

        #endregion Public Methods
    }

    //class Font
}//namespace Conversive.Verbot5