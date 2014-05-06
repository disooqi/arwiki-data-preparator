using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace wiki_data_preparator
{
    public partial class Form2 : Form
    {
        string disambiguationWorkPath = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\disambiguation\";
        string cat_det_working_dir = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\";
        string conceptsFiles_path = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\concepts\";

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(disambiguationWorkPath + "_test.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList disambigElements = rootElem.GetElementsByTagName("disambig");
            string[] separators = { "[[", "]]" };
            for (int x = 0; x < disambigElements.Count; x++)
            {
                for (int y = 0; y < disambigElements[x].ChildNodes.Count; y++)
                {
                    XmlNode concept_elem = disambigElements[x].ChildNodes[y];
                    if (concept_elem.Attributes[1].Value == "" && concept_elem.ChildNodes.Count > 0)
                    {
                        string str = concept_elem.FirstChild.Value;
                        if (str.Contains("[[") && str.IndexOf("[[") != str.LastIndexOf("[["))
                        {
                            string link_str = "[[" + str.Split(separators, StringSplitOptions.None)[1] + "]]";
                            DialogResult result = MessageBox.Show(str + "\r\n\r\n\r\n\r\n" + link_str,
                                            disambigElements[x].Attributes[1].Value, 
                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, 
                                            MessageBoxDefaultButton.Button1, 
                                            MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

                            if (result == DialogResult.Yes)
                            {
                                
                                concept_elem.Attributes[1].Value = link_str;
                                //XmlNode attr = xmldoc.CreateNode(XmlNodeType.Attribute,"article_link","");
                                //attr.Value = link_str;

                                //concept_elem.Attributes.SetNamedItem(attr);
                            }
                            //else if (result == DialogResult.Cancel)
                            //{
                            //    goto dd;
                            //}
                        }
                    }
                } 
            }
            dd: xmldoc.Save(disambiguationWorkPath + "test_2.xml");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> cat = new Dictionary<string,int>();
            int counter = 0;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(cat_det_working_dir + "_all_concepts_even_red_links.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            
            XmlNodeList synomymElements = rootElem.GetElementsByTagName("s");
            string term = string.Empty;

            for (int i = 0; i < synomymElements.Count; i++)
            {
                //XmlAttribute term_attr = xmldoc.CreateAttribute("term");
                //term_attr.Value = synomymElements[i].FirstChild.Value;
                //MessageBox.Show(synomymElements[i].Attributes[0].Value);
                //synomymElements[i].Attributes[0].Value = synomymElements[i].FirstChild.Value;
                //conceptElements[i].Attributes.Append(term_attr);
                term = synomymElements[i].FirstChild.Value;
                if (term.Contains("(") && term.Contains(")"))
                {
                    if (term.IndexOf("(") == term.LastIndexOf("(") && term.IndexOf("(") == term.LastIndexOf("("))
                    {
                        int start = term.IndexOf("(");
                        int length = term.IndexOf(")") + 1 - term.IndexOf("(");
                        if (length > 0)
                        {
                            synomymElements[i].Attributes[0].Value = term.Remove(start, length).Trim();
                            //        term = term.Substring(start, length);
                            //        try
                            //        {
                            //            cat.Add(term, 1);
                            //        }
                            //        catch (ArgumentException)
                            //        {
                            //            cat[term] = cat[term] + 1;
                            //        }
                        }
                    }
                    else
                    {
                        //MessageBox.Show(term);
                        counter++;
                    }
                }
                else
                {
                    synomymElements[i].Attributes[0].Value = term.Trim();
                }

            }

            xmldoc.Save(cat_det_working_dir + "all_concepts_with_term_attr.xml");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> cat = new Dictionary<string, int>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(cat_det_working_dir + "_all_concepts_with_term_attr.xml");
            XmlElement rootElem = xmldoc.DocumentElement;

            XmlNodeList synomymElements = rootElem.GetElementsByTagName("s");
            string term = string.Empty;

            for (int i = 0; i < synomymElements.Count; i++)
            {
                term = synomymElements[i].Attributes[0].Value;
                if (term.Contains("(") || term.Contains(")"))
                {
                    synomymElements[i].Attributes[0].Value = term.Replace("(","");
                    synomymElements[i].Attributes[0].Value = term.Replace(")", "");

                }
            }

            xmldoc.Save(cat_det_working_dir + "all_concepts_prantheses_removed.xml");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> title_concept_pair = new Dictionary<string, string>();
            char[] trim_ch = {'[',']'};

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_title_concept_index.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while((word = sr.ReadLine()) != null)
                {
                    //try
                    //{
                        title_concept_pair.Add(word.Substring(0, word.LastIndexOf(',')), word.Substring(word.LastIndexOf(',') + 1));
                    //}
                    //catch (ArgumentException)
                    //{ }
                }
            }

            XmlDocument xmldoc1 = new XmlDocument();
            xmldoc1.Load(disambiguationWorkPath + "_test_2.xml");
            XmlElement rootElem = xmldoc1.DocumentElement;
            XmlNodeList disambigElements = rootElem.GetElementsByTagName("disambig");

            //for each word "w" in disambig file
            for (int w = 0; w < disambigElements.Count; w++)
            {
                XmlNodeList different_meaning = disambigElements[w].ChildNodes;
                
                //for each concept "c" in word "w"
                for (int c = 0; c < different_meaning.Count; c++)
                {
                    //if concept "c" exist in concepts file
                    string conceptStr = string.Empty;
                    if (title_concept_pair.TryGetValue(different_meaning[c].Attributes[1].Value.Trim(trim_ch), out conceptStr))
                    {
                        different_meaning[c].Attributes[0].Value = conceptStr;
                    }
                    //add "w" as synonym for concept "c"
                    //else do nothing
                }
            }
            xmldoc1.Save(disambiguationWorkPath + "disambig_with_concepts.xml");       
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Dictionary<string, List<string>> concept_syns = new Dictionary<string, List<string>>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(disambiguationWorkPath + "_disambig_with_concepts.xml");
            XmlElement rootElem1 = xmldoc.DocumentElement;
            XmlNodeList disambigElements = rootElem1.GetElementsByTagName("disambig");

            for (int x = 0; x < disambigElements.Count; x++)
            {
                if (disambigElements[x].Attributes[0].LocalName == "status" && disambigElements[x].Attributes[0].Value =="ok")
                {
                    XmlNodeList meaningElements = disambigElements[x].ChildNodes;
                    for (int y = 0; y < meaningElements.Count; y++)
                    {
                        if (meaningElements[y].Attributes[0].Value != "")
                        {
                            try
                            {
                                concept_syns.Add(meaningElements[y].Attributes[0].Value, new List<string>());
                                concept_syns[meaningElements[y].Attributes[0].Value].Add(disambigElements[x].Attributes[2].Value);
                            }
                            catch (ArgumentException)
                            {
                                concept_syns[meaningElements[y].Attributes[0].Value].Add(disambigElements[x].Attributes[2].Value);
                            }
                        }
                    }
                }
            }

            XmlDocument xmldoc2 = new XmlDocument();
            xmldoc2.Load(cat_det_working_dir + "_all_concepts_(removed_prantheses_from_term).xml");
            XmlElement rootElem2 = xmldoc2.DocumentElement;
            XmlNodeList conceptElements = rootElem2.GetElementsByTagName("c");

            for (int i = 0; i < conceptElements.Count; i++)
            {
                List<string> synonyms_list = new List<string>();
                if (concept_syns.TryGetValue(conceptElements[i].Attributes[0].Value,out synonyms_list))
                {
                    for (int j = 0; j < synonyms_list.Count; j++)
                    {
                        XmlElement syn = xmldoc2.CreateElement("s");
                        syn.SetAttribute("term", synonyms_list[j]);
                        syn.SetAttribute("from_disambig", "yes");
                        conceptElements[i].AppendChild(syn);

                    }

                }
            }
            xmldoc2.Save(cat_det_working_dir + "concept_final.xml");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}