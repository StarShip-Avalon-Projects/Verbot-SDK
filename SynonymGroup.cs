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
using System.Text;
using System.Xml.Serialization;

namespace Conversive.Verbot5
{
    [Serializable]
    public class Phrase : ISerializable, IComparable
    {
        #region Private Fields

        private string id;

        private string text;

        #endregion Private Fields

        #region Public Constructors

        public Phrase()
        {
            this.text = "";
            this.id = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected Phrase(SerializationInfo info, StreamingContext context)
        {
            this.text = info.GetString("t");
            this.id = info.GetString("i");
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public int CompareTo(Object other)
        {
            Phrase siOther = (Phrase)other;
            return ((-1) * this.text.Length.CompareTo(siOther.text.Length));
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("t", this.text);
            info.AddValue("i", this.id);
        }

        #endregion Public Methods
    }

    [Serializable]
    public class Synonym : ISerializable
    {
        #region Public Fields

        [XmlArrayItem("Phrase")]
        public List<Phrase> Phrases;

        #endregion Public Fields

        #region Private Fields

        private string id;
        private string name;

        #endregion Private Fields

        #region Public Constructors

        public Synonym()
        {
            this.name = "";
            this.Phrases = new List<Phrase>();
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected Synonym(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString("n");
            this.Phrases = (List<Phrase>)info.GetValue("p", typeof(List<Phrase>));
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public string AddPhrase(string phraseText)
        {
            Phrase phraseNew = new Phrase();
            phraseNew.Id = GetNewPhraseId();
            phraseNew.Text = phraseText;
            this.Phrases.Add(phraseNew);
            return phraseNew.Id;
        }

        public string GetNewPhraseId()
        {
            return TextToolbox.GetNewId();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("n", this.name);
            info.AddValue("p", this.Phrases);
        }

        /*
		 * Modifier Methods
		 */
        /*
		 * Accessor Methods
		 */

        public Phrase GetPhrase(string id)
        {
            //return null if not found
            foreach (Phrase p in this.Phrases)
            {
                if (p.Id == id)
                {
                    return p;
                }
            }
            return null;
        }//GetPhrase(string id)

        public string GetPhrases()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (Phrase p in this.Phrases)
            {
                if (!first)
                    sb.Append("|");
                sb.Append(p.Text);
                first = false;
            }
            return sb.ToString();
        }

        #endregion Public Methods

        //GetPhrases()
    }

    /// <summary>
    /// Defines a list of equivalent words or phrases.
    /// </summary>
    public class SynonymGroup
    {
        #region Public Fields

        [XmlArrayItem("Synonym")]
        public List<Synonym> Synonyms;

        #endregion Public Fields

        /*
		 * Unserialized Attributes
		 */

        #region Private Fields

        private bool changed;

        #endregion Private Fields

        #region Public Constructors

        public SynonymGroup()
        {
            this.changed = false;
            this.Synonyms = new List<Synonym>();
        }

        #endregion Public Constructors

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

        /*
		 * Modifier Methods
		 */

        #region Public Methods

        public string AddSynonym(string synonymName)
        {
            Synonym synonymNew = new Synonym();
            synonymNew.Id = GetNewSynoymId();
            synonymNew.Name = synonymName;
            this.Synonyms.Add(synonymNew);
            return synonymNew.Id;
        }

        /*
		 * Accessor Methods
		 */

        public string GetNewSynoymId()
        {
            return TextToolbox.GetNewId();
        }

        public Synonym GetSynonym(string id)
        {
            //return null if not found
            foreach (Synonym s in this.Synonyms)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }

        #endregion Public Methods

        //GetPhrase(string id)
    }//class SynonymGroup

    //class Synonym

    //Synonym
}//namespace Conversive.Verbot5