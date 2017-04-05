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
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace Conversive.Verbot5
{
    public enum ResourceFileType
    {
        SynonymFile,
        VerbotPluginFile,
        ReplacementProfileFile,
        TemplateDataFile,
        CodeModuleFile,
        Other
    };

    //NOTE: This is only used for copy/paste not for binary serialization to a file
    [Serializable()]
    public class Input
    {
        #region Private Fields

        private string condition;

        private string id;

        private string text;

        #endregion Private Fields

        #region Public Constructors

        public Input()
        {
            this.id = "";
            this.text = "";
            this.condition = "";
        }

        #endregion Public Constructors

        #region Public Properties

        public string Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
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

        public override bool Equals(object obj)
        {
            bool bRet = false;
            if (obj is Input)
            {
                Input otherInput = (Input)obj;
                if (this.condition == otherInput.Condition && this.text == otherInput.Text)
                {
                    bRet = true;
                }
            }
            else if (obj != null)
            {
                if (this.ToString() == obj.ToString())
                {
                    bRet = true;
                }
            }
            return bRet;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string ToRTF()
        {
            string stCond = "";
            if (this.Condition != null && this.Condition != "")
                stCond = "|Cond: " + this.Condition + "\r\n";
            return "\\cf1 Input Text: " + @"\cf3 " + this.Text.Replace("\r\n", "\\par\r\n") + "\\par\r\n" + stCond;
        }

        public override string ToString()
        {
            string stCond = "";
            if (this.Condition != null && this.Condition != "")
                stCond = "|Cond: " + this.Condition + "\r\n";
            return "Input Text: " + this.Text + "\r\n" + stCond;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Represents a KnowledgeBase that is used by the Verbot5Engine.
    /// </summary>
    public class KnowledgeBase
    {
        #region Public Fields

        public KnowledgeBaseInfo Info;

        [XmlArrayItem("ResourceFile")]
        public List<ResourceFile> ResourceFiles;

        [XmlArrayItem("Rule")]
        public List<Rule> Rules;

        #endregion Public Fields

        #region Private Fields

        private int build;
        private bool changed;
        private string id;

        private object Lock = new object();

        private string version;

        #endregion Private Fields

        #region Public Constructors

        public KnowledgeBase()
        {
            this.Rules = new List<Rule>();
            this.ResourceFiles = new List<ResourceFile>();
            this.version = "1.0";
            this.build = 0;
            this.Changed = false;
            this.Info = new KnowledgeBaseInfo();
        }

        #endregion Public Constructors

        #region Public Properties

        public int Build
        {
            get
            {
                return this.build;
            }
            set
            {
                this.build = value;
            }
        }

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

        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        #endregion Public Properties

        /*
		 * Unserialized Attributes
		 */
        /*
		 * Modifier Methods
		 */

        #region Public Methods

        public Rule AddRule()
        {
            Rule ruleNew = new Rule();
            ruleNew.Id = GetNewRuleId();
            this.Rules.Add(ruleNew);
            return ruleNew;
        }

        public string AddRule(string ruleName)
        {
            Rule ruleNew = this.AddRule();
            ruleNew.Name = ruleName;
            return ruleNew.Id;
        }

        public string AddRuleChild(Rule parent, string ruleName)
        {
            if (parent != null)
            {
                Rule ruleNew = new Rule();
                ruleNew.Id = GetNewRuleId();
                ruleNew.Name = ruleName;
                parent.Children.Add(ruleNew);
                return ruleNew.Id;
            }
            else
                return AddRule(ruleName);
        }

        public Input CloneInput(Input i, Rule r)
        {
            Input iClone = new Input();
            iClone.Text = i.Text;
            iClone.Condition = i.Condition;
            iClone.Id = r.GetNewInputId();
            return iClone;
        }

        public Output CloneOutput(Output o, Rule r)
        {
            Output oClone = new Output();
            oClone.Text = o.Text;
            oClone.Cmd = o.Cmd;
            oClone.Condition = o.Condition;
            oClone.Id = r.GetNewOutputId();
            return oClone;
        }

        public Rule CloneRule(Rule r)
        {
            Rule rClone = new Rule();
            rClone.Name = r.Name;
            rClone.Id = this.GetNewRuleId();
            foreach (Input i in r.Inputs)
                rClone.Inputs.Add(this.CloneInput(i, r));
            foreach (Output o in r.Outputs)
                rClone.Outputs.Add(this.CloneOutput(o, r));
            foreach (string id in r.VirtualParents)
                rClone.VirtualParents.Add(id);
            foreach (Rule rRecursive in r.Children)
                rClone.Children.Add(this.CloneRule(rRecursive));
            return rClone;
        }

        public Rule createRuleFromTemplateRow(Rule baseRule, System.Data.DataColumnCollection columns, System.Data.DataRow row, int index)
        {
            Rule newRule = new Rule();
            newRule.Id = baseRule.Id + "_" + index;
            newRule.Name = this.replaceTemplateData(baseRule.Name, columns, row);
            foreach (Input i in baseRule.Inputs)
            {
                Input newInput = new Input();
                newInput.Id = i.Id + "_" + index;
                newInput.Text = this.replaceTemplateData(i.Text, columns, row);
                newInput.Condition = this.replaceTemplateData(i.Condition, columns, row);
                newRule.Inputs.Add(newInput);
            }
            foreach (Output o in baseRule.Outputs)
            {
                Output newOutput = new Output();
                newOutput.Id = o.Id + "_" + index;
                newOutput.Text = this.replaceTemplateData(o.Text, columns, row);
                newOutput.Cmd = this.replaceTemplateData(o.Cmd, columns, row);
                newOutput.Condition = this.replaceTemplateData(o.Condition, columns, row);
                newRule.Outputs.Add(newOutput);
            }
            foreach (Rule child in baseRule.Children)
            {
                newRule.Children.Add(this.createRuleFromTemplateRow(child, columns, row, index));
            }
            newRule.VirtualParents = baseRule.VirtualParents;
            return newRule;
        }

        public KnowledgeBase DecompressTemplates(string path)
        {
            List<System.Data.DataTable> dataTables = loadTemplateData(path);
            if (dataTables.Count == 0)
                return null;

            KnowledgeBase newKB = new KnowledgeBase();
            newKB.Build = this.Build;
            newKB.Changed = true;
            newKB.Id = this.Id;
            newKB.Version = this.Version;
            newKB.Info = this.Info;
            newKB.Rules = this.Rules;

            foreach (ResourceFile rf in this.ResourceFiles)
            {
                if (rf.Filetype != ResourceFileType.TemplateDataFile)
                {
                    newKB.ResourceFiles.Add(rf);
                }
            }

            foreach (System.Data.DataTable dataTable in dataTables)
            {
                if (this.containsTemplateRule(newKB.Rules, dataTable))
                {
                    newKB.Rules = this.decompressRules(newKB.Rules, dataTable);
                }
            }//foreach dataTable in dataTables
            return newKB;
        }

        public void DeleteRule(string ruleId)
        {
            Rule r = this.GetRule(ruleId);
            deleteRule(r, this.Rules);
        }//DeleteRule(int ruleId)

        public string GetNewRuleId()
        {
            return TextToolbox.GetNewId();
        }

        public Rule GetRule(string id)
        {
            return this.getRule(id, this.Rules);
        }

        public Rule GetRuleByNameOrId(string stName)
        {
            return this.getRuleByNameOrId(stName, this.Rules);
        }

        public void IncBuild()
        {
            this.build++;
        }

        public bool IsDescendant(Rule rParent, Rule rChild)
        {
            return this.isDescendant(rParent, rChild, rParent.Children);
        }

        #endregion Public Methods

        #region Private Methods

        private bool containsTemplateRule(List<Rule> rules, System.Data.DataTable dataTable)
        {
            foreach (Rule r in rules)
            {
                if (this.isTemplateRule(r, dataTable))
                    return true;
                if (this.containsTemplateRule(r.Children, dataTable))
                    return true;
            }
            return false;
        }

        private List<Rule> decompressRules(List<Rule> rules, System.Data.DataTable dataTable)
        {
            List<Rule> newRules = new List<Rule>();
            foreach (Rule r in rules)
            {
                if (this.isTemplateRule(r, dataTable))
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        newRules.Add(this.createRuleFromTemplateRow(r, dataTable.Columns, dataTable.Rows[i], i));
                    }
                }
                else
                {
                    Rule newRule = this.CloneRule(r);
                    newRule.Children = decompressRules(r.Children, dataTable);
                    newRules.Add(newRule);
                }
            }
            return newRules;
        }

        private void deleteRule(Rule ruleToDelete, List<Rule> rules)
        {
            if (rules.Contains(ruleToDelete))
            {
                rules.Remove(ruleToDelete);
            }
            else
            {
                foreach (Rule r in rules)
                {
                    deleteRule(ruleToDelete, r.Children);
                }
            }
        }//deleteRule(Rule ruleToDelete, List<Rule> rules)

        //DecompressTemplates(string path)

        //decompressRule(Rule r, System.Data.DataTable dataTable)

        private Rule getRule(string id, List<Rule> rules)
        {
            //return null if not found

            if (rules != null)
            {
                foreach (Rule r in rules)
                {
                    if (r.Id == id)
                    {
                        return r;
                    }
                    Rule subRule = getRule(id, r.Children);
                    if (subRule != null)
                        return subRule;
                }
            }
            return null;
        }

        private Rule getRuleByNameOrId(string stName, List<Rule> rules)
        {
            //return null if not found
            if (rules != null)
            {
                string searchName = stName.ToLower().Trim();
                string subName = "";
                int slashPos = searchName.IndexOf('/');
                //if it's a path like "top/child1/child2"
                if (slashPos != -1)
                {
                    if (slashPos != searchName.Length)//if the slash isn't at the end
                        subName = searchName.Substring(slashPos + 1);
                    searchName = searchName.Substring(0, slashPos);
                }
                else
                {
                    slashPos = searchName.IndexOf('\\');
                    //if it's a path like "top/child1/child2"
                    if (slashPos != -1)
                    {
                        if (slashPos != searchName.Length)//if the slash isn't at the end
                            subName = searchName.Substring(slashPos + 1);
                        searchName = searchName.Substring(0, slashPos);
                    }
                }
                List<Rule> todo = new List<Rule>();
                todo.AddRange(rules);
                lock (Lock)
                {
                    while (todo.Count != 0)
                    {
                        Rule r = (Rule)todo[0];
                        todo.RemoveAt(0);
                        if (r.Name.ToLower().Trim() == searchName || r.Id.ToLower().Trim() == searchName)
                        {
                            if (subName != "")
                            {
                                Rule subRule = getRuleByNameOrId(subName, r.Children);
                                if (subRule != null)
                                    return subRule;
                                //if we didn't find it here, keep looking
                            }
                            else
                            {
                                return r;
                            }
                        }//end if name or matches
                         //for breadth first search
                        if (r.Children != null && r.Children.Count != 0)
                            todo.AddRange(r.Children);
                    }//end foreach rule
                }
            }//if rules isn't null
            return null;
        }

        private bool isDescendant(Rule rParent, Rule rChild, List<Rule> children)
        {
            if (rParent.Children.Contains(rChild))
                return true;
            foreach (Rule r in children)
                return isDescendant(r, rChild, r.Children);
            return false;
        }

        private bool isTemplateRule(Rule r, System.Data.DataTable dataTable)
        {
            if (dataTable != null)
            {
                foreach (System.Data.DataColumn col in dataTable.Columns)
                {
                    //check the rule name
                    if (r.Name.ToLower().IndexOf("#" + col.ColumnName) != -1)
                        return true;
                    //check the inputs
                    foreach (Input i in r.Inputs)
                    {
                        if (i.Text.ToLower().IndexOf("#" + col.ColumnName) != -1)
                            return true;
                    }
                    //check the outputs and commands
                    foreach (Output o in r.Outputs)
                    {
                        if (o.Text.ToLower().IndexOf("#" + col.ColumnName) != -1)
                            return true;
                        if (o.Cmd.ToLower().IndexOf("#" + col.ColumnName) != -1)
                            return true;
                    }
                }//foreach column
            }//if(dataTable != null)
            return false;
        }

        private List<System.Data.DataTable> loadTemplateData(string path)
        {
            List<System.Data.DataTable> dataTables = new List<System.Data.DataTable>();
            System.Data.DataTable dataTable = null;
            foreach (ResourceFile rf in this.ResourceFiles)
            {
                if (rf.Filetype == ResourceFileType.TemplateDataFile)
                {
                    string dataFilename = path + rf.Filename;
                    StreamReader sr = new StreamReader(dataFilename, System.Text.Encoding.Default, true);
                    string data = sr.ReadToEnd();
                    List<List<string>> lines = ConversiveGeneralTextToolbox.SplitCSV(data, ',', '"');
                    if (lines.Count > 1)
                    {
                        dataTable = new System.Data.DataTable();
                        foreach (string colName in lines[0])
                        {
                            System.Data.DataColumn dc = new System.Data.DataColumn();
                            dc.DataType = System.Type.GetType("System.String");
                            dc.ColumnName = colName.ToLower();
                            dc.Caption = colName;
                            dataTable.Columns.Add(dc);
                        }
                        for (int i = 1; i < lines.Count; i++)
                        {
                            List<string> fieldValues = lines[i];
                            System.Data.DataRow dr = dataTable.NewRow();
                            for (int j = 0; j < fieldValues.Count; j++)
                            {
                                dr[j] = (string)fieldValues[j];
                            }
                            dataTable.Rows.Add(dr);
                        }//for each data line
                        dataTables.Add(dataTable);
                    }//if we have a valid file
                }//end if TemplateDataFile
            }//foreach resource file
            return dataTables;
        }//loadTemplateData(List<System.Data.DataTable> resourceFiles)

        //containsTemplateRule(List<Rule> rules, System.Data.DataTable dataTable)

        //isTemplateRule(Rule r, System.Data.DataTable dataTable)

        //createRuleFromTemplateRow(Rule baseRow, DataRow row, int index)

        private string replaceTemplateData(string text, System.Data.DataColumnCollection columns, System.Data.DataRow row)
        {
            foreach (System.Data.DataColumn col in columns)
            {
                int start = text.ToLower().IndexOf("#" + col.ColumnName);
                while (start != -1)
                {
                    int end = start + col.ColumnName.Length + 1;
                    if (end < text.Length)
                        text = text.Substring(0, start) + row[col.ColumnName] + text.Substring(end);
                    else
                        text = text.Substring(0, start) + row[col.ColumnName];
                    if (start + 1 < text.Length)
                        start = text.ToLower().IndexOf("#" + col.ColumnName, start + 1);
                    else
                        start = -1;
                }
            }
            return text;
        }

        #endregion Private Methods

        //replaceTemplateData(string text, DataRow row)

        /*
		 * Accessor Methods
		 *
		 */
        //getRuleByNameOrId(string stName, List<Rule> rules)

        //GetRule(int id)

        //getRule(int id, List<Rule> rules)
    }//class KnowledgeBase

    [Serializable]
    public class KnowledgeBaseInfo : ISerializable
    {
        #region Public Fields

        public string Author;

        public string AuthorWebsite;

        public CategoryType Category;

        public string Copyright;

        public System.DateTime CreationDate;

        public LanguageType Language;

        public System.DateTime LastUpdateDate;

        public string License;

        public KnowledgeBaseRating Rating;

        #endregion Public Fields

        #region Private Fields

        private string comment;

        #endregion Private Fields

        #region Public Constructors

        public KnowledgeBaseInfo()
        {
            this.Author = "";
            this.Copyright = "";
            this.License = "";
            this.AuthorWebsite = "";
            this.CreationDate = System.DateTime.Now;
            this.LastUpdateDate = System.DateTime.Now;
            this.Rating = new KnowledgeBaseRating();
            this.Language = LanguageType.English;
            this.Category = CategoryType.Other;
            this.Comment = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected KnowledgeBaseInfo(SerializationInfo info, StreamingContext context)
        {
            this.Author = (string)info.GetString("a");
            this.Copyright = (string)info.GetString("copy"); ;
            this.License = (string)info.GetString("lic"); ;
            this.AuthorWebsite = (string)info.GetString("aw");
            this.CreationDate = (System.DateTime)info.GetValue("cd", typeof(System.DateTime));
            this.LastUpdateDate = (System.DateTime)info.GetValue("lud", typeof(System.DateTime));
            this.Rating = (KnowledgeBaseRating)info.GetValue("r", typeof(KnowledgeBaseRating));
            this.Language = (LanguageType)info.GetValue("lang", typeof(LanguageType));
            this.Category = (CategoryType)info.GetValue("cat", typeof(CategoryType));
            this.Comment = info.GetString("comment");
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Enums

        public enum CategoryType
        {
            Conversational,
            Educational,
            Personal_Assistant,
            Entertainment,
            Reference,
            Other
        };

        public enum LanguageType
        {
            Dutch,
            English,
            French,
            German,
            Italian,
            Japanese,
            Korean,
            Portuguese,
            Russian,
            Spanish,
            Other
        };

        #endregion Public Enums

        //drop description in favor of this

        #region Public Properties

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
                int index = this.comment.IndexOf('\n');
                if (index != -1 && (index == 0 || this.comment[index - 1] != '\r'))
                    this.comment = this.comment.Replace("\n", "\r\n");
            }
        }

        #endregion Public Properties

        #region Public Methods

        //KnowledgeBaseInfo
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("a", this.Author);
            info.AddValue("copy", this.Copyright);
            info.AddValue("lic", this.License);
            info.AddValue("aw", this.AuthorWebsite);
            info.AddValue("cd", this.CreationDate);
            info.AddValue("lud", this.LastUpdateDate);
            info.AddValue("r", this.Rating);
            info.AddValue("lang", this.Language);
            info.AddValue("cat", this.Category);
            info.AddValue("comment", this.Comment);
        }

        public override string ToString()
        {
            string stRet = "";
            stRet += "Author: " + this.Author + "\r\n";
            stRet += "Author's Website: " + this.AuthorWebsite + "\r\n";
            stRet += "Copyright: " + this.Copyright + "\r\n";
            stRet += "License: " + this.License + "\r\n";
            stRet += "Creation Date: " + this.CreationDate.ToString("G", null) + "\r\n";
            stRet += "Last Update Date: " + this.LastUpdateDate.ToString("G", null) + "\r\n";
            stRet += "Rating: " + this.Rating.ToString() + "\r\n";
            stRet += "Category: " + this.Category.ToString() + "\r\n";
            stRet += "Language: " + this.Language.ToString() + "\r\n";
            stRet += "Comment: " + this.Comment;
            return stRet;
        }

        #endregion Public Methods

        //ToString()
    }//KnowledgeBaseInfo

    [Serializable]
    public class KnowledgeBaseRating : ISerializable
    {
        #region Public Fields

        public string Description;

        public bool Language;

        public bool Other;

        public RatingLevel Rating;

        //contains strong language
        public bool Sexual;

        //contains sexual context (probably strong language too)
        public bool Violence;

        #endregion Public Fields

        #region Public Constructors

        public KnowledgeBaseRating()
        {
            this.Rating = RatingLevel.Unknown;
            this.Language = false;
            this.Sexual = false;
            this.Violence = false;
            this.Other = false;
            this.Description = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        //people getting hurt by others (fictional or based on reality?)
        //some other crazy reason
        //tell us why you rated it that way
        protected KnowledgeBaseRating(SerializationInfo info, StreamingContext context)
        {
            this.Rating = (RatingLevel)info.GetValue("r", typeof(RatingLevel));
            this.Language = info.GetBoolean("l");
            this.Sexual = info.GetBoolean("s");
            this.Violence = info.GetBoolean("v");
            this.Other = info.GetBoolean("o");
            this.Description = info.GetString("d");
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Enums

        public enum RatingLevel
        {
            Kids,//Targeted for kids (educational value, positive, etc...)
            General,//This is for everyone
            Teens,//Teens and above.  Think twice before letting a kid use it
            MatureAudience,//Adults only
            Unknown//Not rated
        };

        #endregion Public Enums

        #region Public Methods

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("r", this.Rating);
            info.AddValue("l", this.Language);
            info.AddValue("s", this.Sexual);
            info.AddValue("v", this.Violence);
            info.AddValue("o", this.Other);
            info.AddValue("d", this.Description);
        }

        public override string ToString()
        {
            string stRet = "";
            stRet += Rating.ToString();
            if (this.Language || this.Sexual || this.Violence || this.Other)
            {
                stRet += " for: ";
                if (Language)
                    stRet += "Language ";
                if (Sexual)
                    stRet += "Sexual ";
                if (Violence)
                    stRet += "Violence ";
                if (Other)
                    stRet += "Other ";
            }
            stRet += "\r\nRating Description: " + this.Description + "\r\n";
            return stRet;
        }

        #endregion Public Methods

        //ToString()
    }//class KnowledgeBaseRating

    [Serializable]
    public class Output : ISerializable
    {
        #region Private Fields

        private string cmd;
        private string condition;
        private string id;

        private string text;

        #endregion Private Fields

        #region Public Constructors

        public Output()
        {
            this.id = "";
            this.text = "";
            this.condition = "";
            this.cmd = "";
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected Output(SerializationInfo info, StreamingContext context)
        {
            this.id = (string)info.GetString("i");
            this.text = (string)info.GetString("t");
            this.cmd = (string)info.GetString("c");
            try
            {
                this.condition = (string)info.GetString("cond");
            }
            catch
            {
                this.condition = "";
            }
            //use a try/catch block around any new vales
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Cmd
        {
            get
            {
                return this.cmd;
            }
            set
            {
                this.cmd = value;
            }
        }

        public string Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
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

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                int index = this.text.IndexOf('\n');
                if (index != -1 && (index == 0 || this.text[index - 1] != '\r'))
                    this.text = this.text.Replace("\n", "\r\n");
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override bool Equals(object obj)
        {
            bool bRet = false;
            if (obj is Output)
            {
                Output otherOutput = (Output)obj;
                if (this.condition == otherOutput.Condition && this.text == otherOutput.Text && this.cmd == otherOutput.Cmd)
                {
                    bRet = true;
                }
            }
            else if (obj != null)
            {
                if (this.ToString() == obj.ToString())
                {
                    bRet = true;
                }
            }
            return bRet;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("i", this.id);
            info.AddValue("t", this.text);
            info.AddValue("c", this.cmd);
            info.AddValue("cond", this.condition);
        }

        public string ToRTF()
        {
            string stCmd = "";
            if (this.Cmd != null && this.Cmd != "")
                stCmd = "\\cf1 |Cmd: \\cf4 " + this.Cmd + "\\par\r\n";
            string stCond = "";
            if (this.Condition != null && this.Condition != "")
                stCond = "\\cf1 |Cond: \\cf4 " + this.Condition + "\\par\r\n";
            return "\\cf1 Output Text: " + @"\cf4 " + this.Text.Replace("\r\n", "\\par\r\n") + "\\par\r\n" + stCmd + stCond;
        }

        public override string ToString()
        {
            string stCmd = "";
            if (this.Cmd != null && this.Cmd != "")
                stCmd = "|Cmd: " + this.Cmd + "\r\n";
            string stCond = "";
            if (this.Condition != null && this.Condition != "")
                stCond = "|Cond: " + this.Condition + "\r\n";
            return "Output Text: " + this.Text + "\r\n" + stCmd + stCond;
        }

        #endregion Public Methods
    }

    //class Output
    public class ResourceFile
    {
        #region Private Fields

        private string filename;

        private ResourceFileType filetype;

        #endregion Private Fields

        #region Public Constructors

        public ResourceFile()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.filename = value;
            }
        }

        public ResourceFileType Filetype
        {
            get
            {
                return this.filetype;
            }
            set
            {
                this.filetype = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return this.filename;
        }

        #endregion Public Methods
    }

    //NOTE: This is only used for copy/paste not for binary serialization to a file
    [Serializable()]
    public class Rule
    {
        #region Public Fields

        [XmlArrayItem("Rule")]
        public List<Rule> Children;

        [XmlArrayItem("Input")]
        public List<Input> Inputs;

        [XmlArrayItem("Output")]
        public List<Output> Outputs;

        [XmlArrayItem("VirtualParent")]
        public List<string> VirtualParents;

        #endregion Public Fields

        #region Private Fields

        private string id;

        private string name;

        #endregion Private Fields

        #region Public Constructors

        public Rule()
        {
            this.Inputs = new List<Input>();
            this.Outputs = new List<Output>();
            this.Children = new List<Rule>();
            this.VirtualParents = new List<string>();
        }

        #endregion Public Constructors

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

        //Backreference from ActivationList
        /*
		 * Modifier Methods
		 */

        #region Public Methods

        public void AddInput(string stText, string stCond)
        {
            Input inputNew = new Input();
            inputNew.Text = stText;
            inputNew.Condition = stCond;
            inputNew.Id = this.GetNewInputId();
            this.Inputs.Add(inputNew);
        }

        public void AddOutput(string stText, string stCond, string stCmd)
        {
            Output outputNew = new Output();
            outputNew.Text = stText;
            outputNew.Condition = stCond;
            outputNew.Cmd = stCmd;
            outputNew.Id = this.GetNewOutputId();
            this.Outputs.Add(outputNew);
        }

        public Input GetInput(string id)
        {
            foreach (Input i in this.Inputs)
            {
                if (i.Id == id)
                    return i;
            }
            return null;
        }

        public string GetNewInputId()
        {
            return TextToolbox.GetNewId();
        }

        public string GetNewOutputId()
        {
            return TextToolbox.GetNewId();
        }

        public Output GetOutput(string id)
        {
            foreach (Output o in this.Outputs)
            {
                if (o.Id == id)
                    return o;
            }
            return null;
        }

        public string ToRTF()
        {
            return this.ToRTF(0);
        }

        public string ToRTF(int spaces)
        {
            string stPar = @"\pard ";
            if (spaces > 0)
                stPar = @"\pard\li" + 15 * spaces + " ";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(stPar + @"\cf1 " + "Rule Name: " + @"\cf2 " + this.Name + "\\par\r\n");
            foreach (Input i in this.Inputs)
            {
                sb.Append(i.ToRTF());
            }
            foreach (Output o in this.Outputs)
            {
                sb.Append(o.ToRTF());
            }
            foreach (Rule rc in this.Children)
            {
                sb.Append(rc.ToRTF(spaces + 10));
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Rule Name: " + this.Name + "\r\n");
            foreach (Input i in this.Inputs)
            {
                sb.Append(i.ToString());
            }
            foreach (Output o in this.Outputs)
            {
                sb.Append(o.ToString());
            }
            foreach (Rule r in this.Children)
            {
                sb.Append(r.ToString());
            }
            return sb.ToString();
        }

        public void UpdateInput(string stText, string stCond, string id)
        {
            Input i = GetInput(id);
            i.Text = stText;
            i.Condition = stCond;
        }

        public void UpdateOutput(string stText, string stCond, string stCmd, string id)
        {
            Output o = GetOutput(id);
            o.Text = stText;
            o.Condition = stCond;
            o.Cmd = stCmd;
        }

        #endregion Public Methods

        /*
		 * Accessor Methods
		 *
		 */
    }//class Rule

    //class Input

    //class ResourceFile
}//namespace Conversive.Verbot5