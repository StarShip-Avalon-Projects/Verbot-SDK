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
    public class InputReplacement : ISerializable
    {
        #region Private Fields

        private string textToFind;

        private string textToInput;

        #endregion Private Fields

        #region Public Constructors

        public InputReplacement()
        {
            this.textToFind = "";
            this.textToInput = "";
        }

        public InputReplacement(string stTextToFind, string stTextToInput)
        {
            this.textToFind = stTextToFind;
            this.textToInput = stTextToInput;
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected InputReplacement(SerializationInfo info, StreamingContext context)
        {
            this.TextToFind = info.GetString("ttf");
            this.textToInput = info.GetString("tti");
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string TextToFind
        {
            get
            {
                return this.textToFind;
            }
            set
            {
                this.textToFind = value;
            }
        }

        public string TextToInput
        {
            get
            {
                return this.textToInput;
            }
            set
            {
                this.textToInput = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ttf", this.TextToFind);
            info.AddValue("tti", this.TextToInput);
        }

        #endregion Public Methods
    }

    [Serializable]
    public class Replacement : ISerializable
    {
        #region Private Fields

        private string textForAgent;
        private string textForOutput;
        private string textToFind;

        #endregion Private Fields

        #region Public Constructors

        public Replacement()
        {
            this.TextToFind = "";
            this.TextForAgent = "";
            this.TextForOutput = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected Replacement(SerializationInfo info, StreamingContext context)
        {
            this.TextToFind = info.GetString("ttf");
            this.TextForAgent = info.GetString("tfa");
            this.TextForOutput = info.GetString("tfo");
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string TextForAgent
        {
            get
            {
                return this.textForAgent;
            }
            set
            {
                this.textForAgent = value;
            }
        }

        public string TextForOutput
        {
            get
            {
                return this.textForOutput;
            }
            set
            {
                this.textForOutput = value;
            }
        }

        public string TextToFind
        {
            get
            {
                return this.textToFind;
            }
            set
            {
                this.textToFind = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ttf", this.TextToFind);
            info.AddValue("tfa", this.TextForAgent);
            info.AddValue("tfo", this.TextForOutput);
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Replaces input and output text with the given strings.
    /// </summary>
    public class ReplacementProfile
    {
        #region Public Fields

        [XmlArrayItem("InputReplacement")]
        public List<InputReplacement> InputReplacements;

        [XmlArrayItem("Replacement")]
        public List<Replacement> Replacements;

        #endregion Public Fields

        #region Private Fields

        private bool changed;

        #endregion Private Fields

        #region Public Constructors

        public ReplacementProfile()
        {
            this.Replacements = new List<Replacement>();
            this.InputReplacements = new List<InputReplacement>();
        }

        #endregion Public Constructors

        /*
		 * Unserialized Attributes
		 */

        #region Public Properties

        [XmlIgnoreAttribute]
        public bool Changed
        {
            get
            {
                return this.changed;
            }
            set
            {
                this.changed = value;
            }
        }

        #endregion Public Properties
    }

    //class Replacement

    //class InputReplacement
}