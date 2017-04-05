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
    public enum CodeLanguages
    {
        CSharp,
        PHP
    }

    /// <summary>
    /// Defines a "virtual" class file for use in the Verbot 5 Engine.
    /// </summary>
    [Serializable]
    public class CodeModule : ISerializable
    {
        #region Public Fields

        [XmlArrayItem("Function")]
        public List<Function> Functions;

        #endregion Public Fields

        #region Private Fields

        private bool changed;
        private string includes;
        private CodeLanguages language;
        private string name;

        private string vars;

        #endregion Private Fields

        #region Public Constructors

        public CodeModule()
        {
            this.name = "";
            this.language = CodeLanguages.CSharp;
            this.includes = "";
            this.vars = "";
            this.changed = false;
            this.Functions = new List<Function>();
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected CodeModule(SerializationInfo info, StreamingContext context)
        {
            this.changed = false;
            this.name = info.GetString("n");
            this.language = (CodeLanguages)info.GetValue("l", typeof(CodeLanguages));
            this.includes = info.GetString("i");
            this.vars = info.GetString("v");
            this.Functions = new List<Function>();
            this.Functions = (List<Function>)info.GetValue("f", typeof(List<Function>));

            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

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

        public string Includes
        {
            get
            {
                return this.includes;
            }
            set
            {
                this.includes = value;
            }
        }

        public CodeLanguages Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
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

        public string Vars
        {
            get
            {
                return this.vars;
            }
            set
            {
                this.vars = value;
            }
        }

        #endregion Public Properties

        /*
		 * Unserialized Attributes
		 */

        #region Public Methods

        public Function AddFunction(string name, string id)
        {
            Function f = new Function();
            f.Name = name;
            f.Id = id;
            this.Functions.Add(f);
            return f;
        }

        public void DeleteFunction(string id)
        {
            Function f = GetFunction(id);
            this.Functions.Remove(f);
        }

        public Function GetFunction(string id)
        {
            foreach (Function f in this.Functions)
            {
                if (f.Id == id)
                    return f;
            }
            return null;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("n", this.name);
            info.AddValue("l", this.language);
            info.AddValue("i", this.includes);
            info.AddValue("v", this.vars);
            info.AddValue("f", this.Functions);
        }

        public override string ToString()
        {
            return this.name;
        }

        #endregion Public Methods
    }//class CodeModule

    [Serializable]
    public class Function : ISerializable
    {
        #region Private Fields

        private string code;
        private string id;

        private string name;

        private string parameters;

        private string returnType;

        #endregion Private Fields

        #region Public Constructors

        public Function()
        {
            this.name = "";
            this.returnType = "string";
            this.parameters = "";
            this.code = "";
            this.id = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected Function(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString("n");
            this.returnType = info.GetString("rt");
            this.parameters = info.GetString("p");
            this.code = info.GetString("c");
            this.id = info.GetString("id");

            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

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

        public string Parameters
        {
            get
            {
                return this.parameters;
            }
            set
            {
                this.parameters = value;
            }
        }

        public string ReturnType
        {
            get
            {
                return this.returnType;
            }
            set
            {
                this.returnType = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("n", this.name);
            info.AddValue("rt", this.returnType);
            info.AddValue("p", this.parameters);
            info.AddValue("c", this.code);
            info.AddValue("id", this.id);
        }

        #endregion Public Methods
    }//class code module
}