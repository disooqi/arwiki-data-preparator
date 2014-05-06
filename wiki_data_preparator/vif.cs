using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace wiki_data_preparator
{
    class vif
    {
        struct phrase_struct
        {
            string concept;
            int link_counts;
            int total_counts;
        }
        void link_probability_data_extraction()
        {

        }

        void generate_term_concepts_index(string conceptsXML_path, string output_file_name)
        {
            SortedDictionary<string, List<string>> word_concepts_SD = new SortedDictionary<string, List<string>>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(conceptsXML_path);
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList conceptElements = rootElem.GetElementsByTagName("c");
            for (int x = 0; x < conceptElements.Count; x++)
            {
                XmlNodeList synonymElements = conceptElements[x].ChildNodes;
                for (int y = 0; y < synonymElements.Count; y++)
                {
                    try
                    {
                        //List<string> concepts_list  = new List<string>();
                        word_concepts_SD.Add(synonymElements[y].Attributes[0].Value, new List<string>());
                        word_concepts_SD[synonymElements[y].Attributes[0].Value].Add(conceptElements[x].Attributes[0].Value);
                    }
                    catch (ArgumentException)
                    {
                        if (!word_concepts_SD[synonymElements[y].Attributes[0].Value].Contains(conceptElements[x].Attributes[0].Value))
                        {
                            word_concepts_SD[synonymElements[y].Attributes[0].Value].Add(conceptElements[x].Attributes[0].Value);
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(Path.GetFullPath(conceptsXML_path) + output_file_name, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, List<string>> kvp in word_concepts_SD)
                {
                    StringBuilder concepts_string = new StringBuilder(10000);
                    foreach (string concept in kvp.Value)
                    {
                        concepts_string.Append(concept + ";");
                    }
                    sw.WriteLine(kvp.Key + ":" + concepts_string.ToString());
                }
            }
        }
        void analyze_text_in_concepts_file(string conceptsXML_path, string output_file_name)
        {
            arabic_stemmers analyzer = new arabic_stemmers();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(conceptsXML_path + "_concepts_raw_201003211900.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList synomymElements = rootElem.GetElementsByTagName("s");

            for (int i = 0; i < synomymElements.Count; i++)
            {
                synomymElements[i].Attributes[0].Value = analyzer.normalization(synomymElements[i].Attributes[0].Value.Trim(), 0);
                synomymElements[i].Attributes[0].Value = analyzer.stemming(synomymElements[i].Attributes[0].Value.Trim(), 1);
            }

            xmldoc.Save(Path.GetFullPath(conceptsXML_path) + output_file_name);
        }
    }
    class wiki_article
    {
        string page_id;
        string title;
        string name_space;
    }
}
