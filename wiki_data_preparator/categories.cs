using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Collections;

namespace wiki_data_preparator
{
    class categories
    {
        string profile_folder_path = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\profile_creation\profile\";
        string id_concept_index_path = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\";

        public void extract_main_categories_and_its_descendants()
        {
            Queue<string> to_be_visited_ids = new Queue<string>();
            List<string> visited_ids = new List<string>();
            //the first category to add is the Main Category
            //cat_ids.Enqueue("5485");

            XmlDocument xmldoc1 = new XmlDocument();
            xmldoc1.Load(profile_folder_path + "_profile_v7.xml");
            XmlElement rootElem1 = xmldoc1.DocumentElement;
            XmlNodeList categoryElements = rootElem1.GetElementsByTagName("category");

            //XmlDocument xmldoc2 = new XmlDocument();
            //xmldoc2.Load(profile_folder_path + "_profile_v8.xml");
            //XmlElement rootElem2 = xmldoc2.DocumentElement;

            for (int x = 0; x < categoryElements.Count; x++)
            {
                if (categoryElements[x].Attributes[1].Value == "ÇáÊÕäíÝ ÇáÑÆíÓí")
                {
                    //cat_ids.Enqueue(categoryElements[x].Attributes[0].Value);
                   //XmlNode tempNode = 
                    visited_ids.Add(categoryElements[x].Attributes[8].Value);
                    XmlNodeList subCategories = categoryElements[x].ChildNodes[3].ChildNodes;
                    for (int y = 0; y < subCategories.Count; y++)
                    {
                        to_be_visited_ids.Enqueue(subCategories[y].Attributes[0].Value);
                    }
                    break;
                }
            }
            //xmldoc2.Save(profile_folder_path + "profile_v8.xml");
            while (to_be_visited_ids.Count > 0)
            {
                categoryElements = rootElem1.GetElementsByTagName("category");
                for (int i = 0; i < categoryElements.Count; i++)
                {
                    if (categoryElements[i].Attributes.Count == 9 && categoryElements[i].Attributes[8].Value == to_be_visited_ids.Peek())
                    {
                        XmlNodeList subCategories = categoryElements[i].ChildNodes[3].ChildNodes;
                        for (int y = 0; y < subCategories.Count; y++)
                        {
                            if (!visited_ids.Contains(subCategories[y].Attributes[0].Value))
                            {
                                to_be_visited_ids.Enqueue(subCategories[y].Attributes[0].Value);
                            }
                        }
                        break;                        
                    }
                }
                visited_ids.Add(to_be_visited_ids.Dequeue());
            }
            using(StreamWriter sw = new StreamWriter(profile_folder_path + "maincategory.txt",false,Encoding.UTF8))
            {
                foreach (string page_id in visited_ids)
                {
                    sw.WriteLine(page_id);
                }
            }
        }
        public void remove_categories_with_8_attributes()
        {
            XmlDocument xmldoc1 = new XmlDocument();
            xmldoc1.Load(profile_folder_path + "_profile_v7.xml");
            XmlElement rootElem1 = xmldoc1.DocumentElement;
            XmlNodeList categoryElements = rootElem1.GetElementsByTagName("category");

            for (int i = 0; i < categoryElements.Count; i++)
            {
                if (categoryElements[i].Attributes.Count == 9)
                {
                    rootElem1.RemoveChild(categoryElements[i]);
                    i--;
                }
            }
            xmldoc1.Save(profile_folder_path + "profile_v8.xml");
        }
        public void sort_and_remove_duplicate_then_update_profile()
        {
            List<string> page_ids = new List<string>();

            using (StreamReader sr = new StreamReader(profile_folder_path + "maincategory.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    if (!page_ids.Contains(word))
                    {
                        page_ids.Add(word);
                    }
                }
            }
            page_ids.Sort();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(profile_folder_path + "_profile_v7.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList categoryElements = rootElem.GetElementsByTagName("category");

            for (int i = 0; i < categoryElements.Count; i++)
            {
                if (categoryElements[i].Attributes.Count != 9)
                {
                    rootElem.RemoveChild(categoryElements[i]);
                    i--;
                }
                else if (page_ids.BinarySearch(categoryElements[i].Attributes[8].Value) < 0)
                {
                    rootElem.RemoveChild(categoryElements[i]);
                    i--;
                }
            }
            xmldoc.Save(profile_folder_path + "profile_v9.xml");

        }
        public void add_concept_id_to_categories()
        {
            Dictionary<string, string> page_id__c_id = new Dictionary<string, string>();
            List<string> have_no_concept_list = new List<string>();

            using (StreamReader sr = new StreamReader(id_concept_index_path + "x_id_concept_index.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    page_id__c_id.Add(word.Substring(0, word.IndexOf(',')), word.Substring(word.IndexOf(',') + 1));
                }
            }
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(profile_folder_path + "_profile_v9.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList pageElements = rootElem.GetElementsByTagName("page");

            for (int x = 0; x < pageElements.Count; x++)
            {
                string concept_id = string.Empty;
                if (page_id__c_id.TryGetValue(pageElements[x].Attributes[1].Value, out concept_id))
                {
                    pageElements[x].Attributes[0].Value = concept_id;
                }
                else
                {
                    have_no_concept_list.Add(pageElements[x].Attributes[1].Value);
                }
            }
            xmldoc.Save(profile_folder_path + "profile_v10.xml");
        }
        
        public void generate_category_concept_index()
        {
            Dictionary<string, Dictionary<string, int>> category_concepts_index = new Dictionary<string, Dictionary<string, int>>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(profile_folder_path + "_profile_v10.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList categoryElements = rootElem.GetElementsByTagName("category");
            
            for (int i = 0; i < categoryElements.Count; i++)
            {
                XmlNodeList pageElements = categoryElements[i].ChildNodes[2].ChildNodes;
                
                Dictionary<string, int> concepts_freqs_dic = new Dictionary<string, int>();

                for (int j = 0; j < pageElements.Count; j++)
                {
                    int freq = 0;
                    if (concepts_freqs_dic.TryGetValue(pageElements[j].Attributes[0].Value, out freq))
                    {
                        concepts_freqs_dic[pageElements[j].Attributes[0].Value] = freq;
                    }
                    else 
                    {
                        concepts_freqs_dic.Add(pageElements[j].Attributes[0].Value, 1);
                    }
                }
                if (concepts_freqs_dic.Count > 0)
                {
                    category_concepts_index.Add(categoryElements[i].Attributes[0].Value, concepts_freqs_dic);
                }
            }
            using (StreamWriter sw = new StreamWriter(id_concept_index_path + "category_concepts_index.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp1 in category_concepts_index)
                {
                    StringBuilder concepts_string = new StringBuilder(10000);

                    foreach (KeyValuePair<string, int> kvp2 in kvp1.Value)                    
                    {
                        concepts_string.Append(kvp2.Key+","+kvp2.Value+";");
                    }
                    sw.WriteLine(kvp1.Key + ":" + concepts_string.ToString());
                }
            }
        }

        public void generate_concept_categories_index()
        {
            Dictionary<string, List<string>> concept_categories_index = new Dictionary<string, List<string>>();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(profile_folder_path + "_profile_v10.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList categoryElements = rootElem.GetElementsByTagName("category");

            for (int i = 0; i < categoryElements.Count; i++)
            {
                XmlNodeList pageElements = categoryElements[i].ChildNodes[2].ChildNodes;

                List<string> temp_cat_list = new List<string>();

                for (int j = 0; j < pageElements.Count; j++)
                {
                    if (concept_categories_index.TryGetValue(pageElements[j].Attributes[0].Value, out temp_cat_list))
                    {
                        temp_cat_list.Add(categoryElements[i].Attributes[0].Value);
                        concept_categories_index[pageElements[j].Attributes[0].Value] = temp_cat_list;
                    }
                    else
                    {
                        List<string> categories_list = new List<string>();                  
                        categories_list.Add(categoryElements[i].Attributes[0].Value);
                        concept_categories_index.Add(pageElements[j].Attributes[0].Value, categories_list);
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(id_concept_index_path + "category_concepts_index.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, List<string>> kvp in concept_categories_index)
                {
                    StringBuilder concepts_string = new StringBuilder(10000);

                    foreach (string category in kvp.Value)
                    {
                        concepts_string.Append(category + ";");
                    }
                    sw.WriteLine(kvp.Key + ":" + concepts_string.ToString());
                }
            }
        }
    }
}
