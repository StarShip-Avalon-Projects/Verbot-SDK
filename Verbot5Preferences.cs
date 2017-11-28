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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace Conversive.Verbot5
{
    [Serializable]
    public class KnowledgeBaseItem : ISerializable
    {
        #region Public Fields

        public int Build;
        public string Filename;
        public string Fullpath;

        [NonSerialized]
        [XmlIgnoreAttribute]
        public KnowledgeBaseInfo Info;

        public bool Trusted;
        public bool Untrusted;
        public bool Used;

        #endregion Public Fields

        #region Public Constructors

        public KnowledgeBaseItem()
        {
            this.Filename = "";
            this.Fullpath = "";
            this.Used = false;
            this.Trusted = false;
            this.Build = -1;
            this.Info = null;
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected KnowledgeBaseItem(SerializationInfo info, StreamingContext context)
        {
            this.Filename = info.GetString("fn");
            this.Fullpath = info.GetString("fp");
            this.Used = info.GetBoolean("u");
            this.Trusted = info.GetBoolean("t");
            this.Build = info.GetInt32("b");
            this.Info = null;
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("fn", this.Filename);
            info.AddValue("fp", this.Fullpath);
            info.AddValue("u", this.Used);
            info.AddValue("t", this.Trusted);
            info.AddValue("b", (Int32)this.Build);
        }

        public override string ToString()
        {
            return this.Filename;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Stores the user's preferences.
    /// </summary>
    public class Verbot5Preferences
    {
        #region Public Fields

        public string AgentFile;

        public int AgentPitch;

        public int AgentSpeed;

        public string AgentTTSMode;

        public int AutoenterTime;

        public int BoredomResponseTime;

        public string CharacterFile;

        public int CharacterTTSMode;

        [XmlArrayItem("KnowledgeBase")]
        public List<KnowledgeBaseItem> KnowledgeBases;

        public string ScheduleFilePath;
        public string SkinPath;
        public bool UseConversiveCharacter = false;

        #endregion Public Fields

        #region Public Constructors

        public Verbot5Preferences()
        {
            this.AgentFile = "merlin.acs";
            this.AgentTTSMode = "";//Whatever the MSAgent wants
            this.AgentSpeed = 0;
            this.AgentPitch = 0;

            this.CharacterFile = "julia.ccs";
            this.CharacterTTSMode = 0;

            this.BoredomResponseTime = 2;
            this.AutoenterTime = 0;
            this.SkinPath = "";
            this.ScheduleFilePath = "";

            this.KnowledgeBases = new List<KnowledgeBaseItem>();
        }

        #endregion Public Constructors
    }
}