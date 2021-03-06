using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;

namespace wiki_data_preparator
{
    public partial class Form1 : Form
    {
        public class categoryLinkComparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(x.ToString().Substring(x.ToString().IndexOf(",") + 1), y.ToString().Substring(y.ToString().IndexOf(",") + 1)));
            }
        }
        public class pageLinkComparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {

                return ((new CaseInsensitiveComparer()).Compare(x.ToString().Substring(x.ToString().IndexOf("\'") + 1, x.ToString().LastIndexOf("\'") - x.ToString().IndexOf("\'") - 1), y.ToString().Substring(x.ToString().IndexOf("\'") + 1, y.ToString().LastIndexOf("\'") - y.ToString().IndexOf("\'") - 1)));
            }
        }

        public class categoryLink_id_Comparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(x.ToString().Substring(0, x.ToString().IndexOf(",")), y.ToString().Substring(0, y.ToString().IndexOf(","))));
            }
        }

        public class searchComparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.

            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(x.ToString().Substring(x.ToString().IndexOf(",") + 1), y));
            }

        }
        public class cl_searchComparer : IComparer
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.

            int IComparer.Compare(Object x, Object y)
            {
                return ((new CaseInsensitiveComparer()).Compare(x.ToString().Substring(0, x.ToString().IndexOf(",")), y));
            }

        }

        general_functions gf = new general_functions();
        enum wikipedia_namespaces { ميديا = -2, خاص = -1, نقاش = 1, مستخدم = 2, ويكيبيديا = 4, ملف = 6, ميدياويكي = 8, قالب = 10, مساعدة = 12, تصنيف = 14 };
        //     0                 1              2               3                4                  5              6
        string[] namespaces ={ "ميديا:",          "خاص:",          "",          "نقاش:",         "مستخدم:",         "نقاش المستخدم:", "ويكيبيديا:", 
                               "نقاش ويكيبيديا:", "ملف:",          "نقاش الملف:", "ميدياويكي:",    "نقاش ميدياويكي:", "قالب:",         "نقاش القالب:", 
                               "مساعدة:",         "نقاش المساعدة:", "تصنيف:",     "نقاش التصنيف:", "بوابة:",          "نقاش البوابة:", "ملحق:", 
                               "نقاش الملحق:" };
        string working_dir = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\profile_creation\";
        string cat_det_working_dir = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\";
        string concepts_file_path = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\concepts\";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Encoding.GetEncoding("windows-1256")

            using (StreamReader sr = new StreamReader(working_dir + "category\\category.txt", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "profile.xml", false, Encoding.UTF8))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<wikidb>");

                    string[] separator1 = { ",'", "'," };
                    string[] separator2 = { "," };

                    ArrayList category_titles = new ArrayList();
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string cat_title = word.Substring(word.IndexOf('\'') + 1, word.LastIndexOf('\'') - word.IndexOf('\'') - 1);
                        cat_title = gf.xml_noralization(cat_title);
                        if (category_titles.Contains(cat_title))
                        {
                            MessageBox.Show("the title exist in profile: \"" + cat_title + "\"");
                        }
                        else
                        {
                            category_titles.Add(cat_title);
                            word = word.Remove(word.IndexOf('\''), word.LastIndexOf('\'') - word.IndexOf('\'') + 1);
                            string[] cat_attributes = word.Split(separator2, StringSplitOptions.None);
                            if (cat_attributes.Length != 6)
                            {
                                MessageBox.Show("cat_attributes" + cat_attributes.Length.ToString() + "\n" + word);
                            }
                            else
                            {
                                sw.WriteLine("<category cat_id=\"" + cat_attributes[0] + "\" cat_title=\"" + cat_title + "\" cat_pages=\"" + cat_attributes[2] + "\" cat_subcats=\"" + cat_attributes[3] + "\" cat_files=\"" + cat_attributes[4] + "\" cat_hidden=\"" + cat_attributes[5] + "\" interest_weight=\"\" interest_bool=\"\">");
                                sw.WriteLine("<feature_vector></feature_vector>");
                                sw.WriteLine("<super_categories></super_categories>");
                                sw.WriteLine("<pages></pages>");
                                sw.WriteLine("<sub_categories></sub_categories>");
                                sw.WriteLine("</category>");
                            }
                        }
                    }
                    sw.WriteLine("</wikidb>");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int PG_count_4_split = 0, global_PG_count = 0, file_counter = 1;
            using (StreamReader sr = new StreamReader(working_dir + "pages_articles.xml", Encoding.UTF8))
            {
                string word = string.Empty;
                //string bufferStr = string.Empty;
                StringBuilder strbuilder = new StringBuilder(100000);
                while ((word = sr.ReadLine()) != null)
                {
                    strbuilder.AppendLine(word);
                    //bufferStr += word+"\r\n";
                    if (word.Contains("</page>"))
                    {
                        global_PG_count++;
                        PG_count_4_split++;

                        if (PG_count_4_split == 1000)
                        {
                            using (StreamWriter sw = new StreamWriter(working_dir + "articles\\articles_part" + file_counter.ToString() + ".xml", false, Encoding.UTF8))
                            {
                                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                                sw.WriteLine("<wikidb>");
                                sw.WriteLine(strbuilder.ToString());
                                sw.WriteLine("</wikidb>");
                            }
                            file_counter++;
                            strbuilder.Remove(0, strbuilder.Length);
                            PG_count_4_split = 0;
                        }
                    }

                }
                MessageBox.Show("articles # is" + global_PG_count.ToString() + "\n " + word);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(working_dir + "pages_articles.xml", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "cat_parents.xml", false, Encoding.UTF8))
                {
                    string[] separator1 = { "<title>", "</title>" };
                    string[] separator2 = { "<text xml:space=\"preserve\">", "</text>" };

                    StringBuilder page = new StringBuilder(100000);
                    string word = string.Empty;
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<categories>");
                    while ((word = sr.ReadLine()) != null)
                    {
                        word = word.Trim();
                        //obtain a single page from wikipedia
                        page.Remove(0, page.Length);
                        if (word.Contains("<page>"))
                        {
                            do
                            {
                                page.AppendLine(word);
                                word = sr.ReadLine();
                            } while (!word.Contains("</page>"));
                            page.AppendLine(word);
                        }
                        string pageStr = page.ToString();

                        //
                        if (pageStr.Contains("<title>"))
                        {
                            if (pageStr.Contains("</title>"))
                            {
                                int title_index = pageStr.IndexOf("<id>") + 4;
                                string page_id = pageStr.Substring(title_index, pageStr.IndexOf("</id>") - title_index);
                                string[] strArray = pageStr.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                                string[] Page_text = pageStr.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
                                if (strArray.Length == 3 && Page_text.Length == 3)
                                {
                                    //if (word.Contains(":") && !(word.StartsWith("<title>" + namespaces[0]) || word.StartsWith("<title>" + namespaces[1]) || word.StartsWith("<title>" + namespaces[3]) || word.StartsWith("<title>" + namespaces[4]) || word.StartsWith("<title>" + namespaces[5]) || word.StartsWith("<title>" + namespaces[6]) || word.StartsWith("<title>" + namespaces[7]) || word.StartsWith("<title>" + namespaces[8]) || word.StartsWith("<title>" + namespaces[9]) || word.StartsWith("<title>" + namespaces[10]) || word.StartsWith("<title>" + namespaces[11]) || word.StartsWith("<title>" + namespaces[12]) || word.StartsWith("<title>" + namespaces[13]) || word.StartsWith("<title>" + namespaces[14]) || word.StartsWith("<title>" + namespaces[15]) || word.StartsWith("<title>" + namespaces[16]) || word.StartsWith("<title>" + namespaces[17]) || word.StartsWith("<title>" + namespaces[18]) || word.StartsWith("<title>" + namespaces[19]) || word.StartsWith("<title>" + namespaces[20]) || word.StartsWith("<title>" + namespaces[21])))
                                    //MessageBox.Show(strArray[1] + "   " + namespaces[16]);
                                    if (strArray[1].Contains(namespaces[16]))
                                    {
                                        //if (strArray[1].Contains("<") || strArray[1].Contains(">") || strArray[1].Contains("\"") || strArray[1].Contains("\'") || strArray[1].Contains("&"))
                                        //{
                                        //    MessageBox.Show("contians illegal characters" + strArray[1]);
                                        //}
                                        //else
                                        //{
                                        sw.WriteLine("<category cat_id=\"" + page_id + "\" cat_title=\"" + strArray[1].Remove(0, 6) + "\">");
                                        //sw.WriteLine("<title>" + strArray[1] + ":" + page_id + "</title>");
                                        Regex cat_RE = new Regex(@"\[\[تصنيف:(?<category>.*?)\]\]");

                                        MatchCollection matches = cat_RE.Matches(Page_text[1]);
                                        //MessageBox.Show(matches.Count.ToString());

                                        foreach (Match match in matches)
                                        {
                                            string parent_cat = match.Groups["category"].Value;
                                            //int index = match.Index;
                                            sw.WriteLine("<super_cat>" + parent_cat + "</super_cat>");
                                        }
                                        sw.WriteLine("</category>");
                                        //}
                                    }
                                }
                                else
                                { //MessageBox.Show("some thing wrong 1");
                                }
                            }
                            else { MessageBox.Show("No </title>"); }
                        }
                    }//while close
                    sw.WriteLine("</categories>");
                }//StreamWriter
            }//StreamReader
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string PageString = " <page><title>تصنيف:مقالات تحوي براهين</title><id>459263</id><revision><id>4529197</id><timestamp>2009-12-25T01:18:08Z</timestamp> <contributor> <username>TXiKiBoT</username>  <id>48676</id>  </contributor>  <minor/>  <comment>روبوت إضافة: [[lv:Kategorija:Raksti, kas satur pierādījumus]]</comment>  <text xml:space=\"preserve\">{{بوابة رياضيات/تصنيف}} [[تصنيف:براهين]] [[تصنيف:مبرهنات]] [[bs:Kategorija:Članci koji sadrže dokaz]][[en:Category:Articles containing proofs]][[he:קטגוריה:הוכחות]][[lv:Kategorija:Raksti, kas satur pierādījumus]][[zh:Category:包含证明的条目]]</text>    </revision>  </page>";

            //int title_index = PageString.IndexOf("<id>") + 4;
            //string page_title = PageString.Substring(title_index, PageString.IndexOf("</id>") - title_index);
            //MessageBox.Show(page_title);

            Regex super_category_RE = new Regex(@"\[\[تصنيف:.*?\]\]");
            Regex another = new Regex(@"\[\[تصنيف:(?<category>.*?)\]\]");

            MatchCollection matches = another.Matches(PageString);

            MessageBox.Show(matches.Count.ToString());

            foreach (Match match in matches)
            {
                string word = match.Groups["category"].Value;
                int index = match.Index;
                MessageBox.Show(word + " repeated at position " + index);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //SRCObj = new semantic_relatedness_Class();
            //CD_Obj = new concept_disambiguator();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "xml file (*.xml)|*.xml|all files|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\profile_creation";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //if ((myStream = openFileDialog1.OpenFile()) != null)
                    //{
                    //    using (myStream)
                    //    {
                    //        // Insert code to read the stream here.
                    //    }
                    //}
                    textBox1.Text = openFileDialog1.FileName;
                    working_dir = Path.GetDirectoryName(textBox1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(working_dir + "cat_parents.xml", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "cat_parents.xml", false, Encoding.UTF8))
                {
                    StringBuilder category = new StringBuilder(100000);
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        word = word.Trim();
                        //obtain a single page from wikipedia
                        category.Remove(0, category.Length);
                        if (word.Contains("<category cat_id="))
                        {
                            do
                            {
                                category.AppendLine(word);
                                word = sr.ReadLine();
                            } while (!word.Contains("</category>"));
                            category.AppendLine(word);
                        }
                        string category_Str = category.ToString();
                    }
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(working_dir + "categorylinks_3fields.txt", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "categorylinks_2fields.txt", false, Encoding.UTF8))
                {
                    string[] separator1 = { ",'", "'," };
                    string[] separator2 = { "','" };

                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Contains("\\") || word.Contains("&") || word.Contains(">") || word.Contains("<"))
                        {
                            word = word.Replace("&", "&amp;");
                            word = word.Replace(">", "&gt;");
                            word = word.Replace("<", "&lt;");
                            while (word.Contains("\\"))
                            {
                                int back_index = word.IndexOf('\\');
                                char escape = word[back_index + 1];
                                switch (escape)
                                {
                                    case '\'':
                                        word = word.Remove(back_index, 2);
                                        word = word.Insert(back_index, "&apos;");
                                        break;

                                    case '\"':
                                        word = word.Remove(back_index, 2);
                                        word = word.Insert(back_index, "&quot;");
                                        break;

                                    case '\\':
                                        word = word.Remove(back_index, 2);
                                        word = word.Insert(back_index, "&#92;");
                                        break;

                                    default:
                                        MessageBox.Show("not as you think: " + word);
                                        break;
                                }
                            }
                        }
                        string[] fields = word.Split(separator2, StringSplitOptions.None);
                        if (fields.Length != 2)
                        {
                            MessageBox.Show("Length = " + fields.Length.ToString() + "\n" + word);
                        }
                        else
                        {
                            sw.WriteLine(fields[0]);
                        }
                        //sw.WriteLine(word.Remove(word.LastIndexOf(','), word.Length - word.LastIndexOf(','))); 

                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ns = "0";
            using (StreamReader sr = new StreamReader(working_dir + "page_all_ns.txt", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "page_ns0.txt", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Substring(word.IndexOf(',') + 1, word.IndexOf(',', word.IndexOf(',') + 1) - word.IndexOf(',') - 1).CompareTo(ns) == 0)
                        {
                            sw.WriteLine(word);
                        }
                        //MessageBox.Show(word.Substring(word.IndexOf(',')+1, word.IndexOf(',', word.IndexOf(',') + 1) - word.IndexOf(',')-1));
                    }

                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ArrayList cat_pages_names = new ArrayList();
            ArrayList cat_pages = new ArrayList();

            using (StreamReader sr = new StreamReader(working_dir + "category\\page_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    cat_pages.Add(word);
                    cat_pages_names.Add(gf.xml_noralization(word.Substring(word.IndexOf('\'') + 1, word.IndexOf("\',\'") - word.IndexOf('\'') - 1)));
                }
            }
            using (StreamReader sr = new StreamReader(working_dir + "profile.xml", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "profile_v4.xml", false, Encoding.UTF8))
                {
                    using (StreamWriter sw2 = new StreamWriter(working_dir + "categories_without_pages.txt", false, Encoding.UTF8))
                    {
                        string word = string.Empty;
                        while ((word = sr.ReadLine()) != null)
                        {
                            word = word.Trim();
                            //get category title
                            if (word.StartsWith("<category cat_id"))
                            {
                                string cat_title = word.Substring(word.IndexOf("cat_title=\"") + 11, word.IndexOf("\" cat_pages") - word.IndexOf("cat_title=\"") - 11);
                                int cat_index = cat_pages_names.IndexOf(cat_title);
                                if (cat_index != -1)
                                {
                                    //MessageBox.Show("nice");
                                    string cat_rec_Str = cat_pages[cat_index].ToString();
                                    sw.WriteLine(word.Replace(">", " page_id=\"" + cat_rec_Str.Substring(0, cat_rec_Str.IndexOf(',')) + "\">"));
                                    cat_pages.RemoveAt(cat_index);
                                    cat_pages_names.RemoveAt(cat_index);
                                }
                                else
                                {
                                    sw.WriteLine(word);
                                    sw2.WriteLine(cat_title);
                                }
                            }
                            else
                            {
                                sw.WriteLine(word);
                            }

                            //if category title exists in the "category pages file", append page_id
                            //if category title is not exist in the "category pages file", put in "non_existing_cat_pages" file
                        }
                    }
                }
            }
            using (StreamWriter sw3 = new StreamWriter(working_dir + "cat_have_pages_but_not_Exist_in_cats.txt", false, Encoding.UTF8))
            {
                foreach (string cat in cat_pages)
                    sw3.WriteLine(cat);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            ArrayList ns0_ids = new ArrayList();
            ArrayList ns14_ids = new ArrayList();

            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns0_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            using (StreamReader sr = new StreamReader(working_dir + "category\\page_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns14_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            ns0_ids.Sort();
            ns14_ids.Sort();

            using (StreamReader sr = new StreamReader(working_dir + "categorylinks\\categorylinks_2fields.txt", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "categorylinks_ns0_and_ns14.txt", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string from_id = word.Substring(0, word.IndexOf(','));
                        if (ns0_ids.BinarySearch(from_id) != -1 || ns14_ids.BinarySearch(from_id) != -1)
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ArrayList ns0_ids = new ArrayList();
            ArrayList ns14_ids = new ArrayList();

            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns0_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            using (StreamReader sr = new StreamReader(working_dir + "category\\page_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns14_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            ns0_ids.Sort();
            ns14_ids.Sort();

            ArrayList categoryLinks = new ArrayList();
            using (StreamReader sr = new StreamReader(working_dir + "categorylinks_ns0_and_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    categoryLinks.Add(word);
                }
            }
            IComparer myComp = new categoryLinkComparer();
            categoryLinks.Sort(myComp);

            using (StreamReader sr111 = new StreamReader(working_dir + "profile//profile_v4.xml", Encoding.UTF8))
            {

                using (StreamWriter sw = new StreamWriter(working_dir + "profile_v5.xml", false, Encoding.UTF8))
                {
                    string word_sr1 = string.Empty;
                    while ((word_sr1 = sr111.ReadLine()) != null)
                    {
                        word_sr1 = word_sr1.Trim();
                        //get category title
                        if (word_sr1.StartsWith("<category cat_id"))
                        {
                            string cat_title = word_sr1.Substring(word_sr1.IndexOf("cat_title=\"") + 11, word_sr1.IndexOf("\" cat_pages") - word_sr1.IndexOf("cat_title=\"") - 11);

                            do
                            {
                                sw.WriteLine(word_sr1);
                                word_sr1 = sr111.ReadLine();
                            } while (!word_sr1.Contains("</sub_categories>"));

                            //string word_sr2 = string.Empty;


                            IComparer myComp2 = new searchComparer();
                            int cl_index = -1;
                            while ((cl_index = categoryLinks.BinarySearch(cat_title, myComp2)) > -1)
                            {
                                string page_id = categoryLinks[cl_index].ToString().Substring(0, categoryLinks[cl_index].ToString().IndexOf(','));
                                if (ns14_ids.BinarySearch(page_id) > -1)
                                {
                                    sw.WriteLine("<sub_cat page_id=\"" + page_id + "\"></sub_cat>");
                                }
                                categoryLinks.RemoveAt(cl_index);
                            }
                            sw.WriteLine("</sub_categories>");
                        }
                        else
                        {
                            sw.WriteLine(word_sr1);
                        }
                    }
                }

            }


        }

        private void button12_Click(object sender, EventArgs e)
        {
            ArrayList ns0_ids = new ArrayList();
            ArrayList ns14_ids = new ArrayList();

            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns0_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            using (StreamReader sr = new StreamReader(working_dir + "category\\page_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns14_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            ns0_ids.Sort();
            ns14_ids.Sort();

            ArrayList categoryLinks = new ArrayList();
            using (StreamReader sr = new StreamReader(working_dir + "categorylinks_ns0_and_ns14.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    categoryLinks.Add(word);
                }
            }
            IComparer myComp = new categoryLinkComparer();
            categoryLinks.Sort(myComp);

            using (StreamReader sr111 = new StreamReader(working_dir + "profile_v5.xml", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(working_dir + "profile_v6.xml", false, Encoding.UTF8))
                {
                    string word_sr1 = string.Empty;
                    while ((word_sr1 = sr111.ReadLine()) != null)
                    {
                        word_sr1 = word_sr1.Trim();
                        //get category title
                        if (word_sr1.StartsWith("<category cat_id"))
                        {
                            string cat_title = word_sr1.Substring(word_sr1.IndexOf("cat_title=\"") + 11, word_sr1.IndexOf("\" cat_pages") - word_sr1.IndexOf("cat_title=\"") - 11);

                            do
                            {
                                sw.WriteLine(word_sr1);
                                word_sr1 = sr111.ReadLine();
                            } while (!word_sr1.Contains("</pages>"));

                            IComparer myComp2 = new searchComparer();
                            int cl_index = -1;
                            while ((cl_index = categoryLinks.BinarySearch(cat_title, myComp2)) > -1)
                            {
                                string page_id = categoryLinks[cl_index].ToString().Substring(0, categoryLinks[cl_index].ToString().IndexOf(','));
                                if (ns0_ids.BinarySearch(page_id) > -1)
                                {
                                    sw.WriteLine("<page page_id=\"" + page_id + "\"></page>");
                                }
                                categoryLinks.RemoveAt(cl_index);
                            }
                            sw.WriteLine("</pages>");
                        }
                        else
                        {
                            sw.WriteLine(word_sr1);
                        }
                    }
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int flag = 1;
            if (flag == 0)
            {
                using (StreamReader sr = new StreamReader(working_dir + "profile_v6.xml", Encoding.UTF8))
                {
                    using (StreamWriter sw = new StreamWriter(working_dir + "profile_v7.xml", false, Encoding.UTF8))
                    {
                        string profile_word = string.Empty;
                        while ((profile_word = sr.ReadLine()) != null)
                        {
                            if (profile_word.StartsWith("<category cat_id"))
                            {
                                //string cat_title = word.Substring(word.IndexOf("cat_title=\"") + 11, word.IndexOf("\" cat_pages") - word.IndexOf("cat_title=\"") - 11);
                                string cat_page_id = string.Empty;

                                if (profile_word.Contains("page_id=\""))
                                {
                                    cat_page_id = profile_word.Substring(profile_word.IndexOf("page_id=\"") + 9, profile_word.LastIndexOf("\"") - profile_word.IndexOf("page_id=\"") - 9);
                                    do
                                    {
                                        sw.WriteLine(profile_word);
                                        profile_word = sr.ReadLine();
                                    } while (!profile_word.Contains("</super_categories>"));
                                    // must have page_id
                                    using (StreamReader copy_reader = new StreamReader(working_dir + "copy_profile_v6.xml", Encoding.UTF8))
                                    {
                                        string copy_word = string.Empty;
                                        while ((copy_word = copy_reader.ReadLine()) != null)
                                        {
                                            copy_word = copy_word.Trim();
                                            //get category title
                                            if (copy_word.StartsWith("<category cat_id"))
                                            {
                                                string super_cat_title = copy_word.Substring(copy_word.IndexOf("cat_title=\"") + 11, copy_word.IndexOf("\" cat_pages") - copy_word.IndexOf("cat_title=\"") - 11);
                                                string super_cat_id = string.Empty;
                                                bool contains_page_id = copy_word.Contains("page_id=\"");
                                                if (contains_page_id)
                                                {
                                                    super_cat_id = copy_word.Substring(copy_word.IndexOf("page_id=\"") + 9, copy_word.LastIndexOf("\"") - copy_word.IndexOf("page_id=\"") - 9);
                                                }
                                                do
                                                {
                                                    copy_word = copy_reader.ReadLine();
                                                } while (!copy_word.Contains("<sub_categories>"));
                                                copy_word = copy_reader.ReadLine();
                                                while (copy_word.Contains("<sub_cat page_id"))
                                                {
                                                    string temp_page_id = copy_word.Substring(copy_word.IndexOf("\"") + 1, copy_word.LastIndexOf("\"") - copy_word.IndexOf("\"") - 1);
                                                    if (temp_page_id.CompareTo(cat_page_id) == 0)
                                                    {
                                                        if (contains_page_id)
                                                        {
                                                            sw.WriteLine("<sup_cat page_id=\"" + super_cat_id + "\" cat_title=\"" + super_cat_title + "\"></sup_cat>");
                                                        }
                                                        else
                                                        {
                                                            sw.WriteLine("<sup_cat cat_title=\"" + super_cat_title + "\"></sup_cat>");
                                                        }
                                                    }

                                                    copy_word = copy_reader.ReadLine();
                                                }
                                                //string super_cat_id = 
                                            }
                                        }
                                    }
                                    sw.WriteLine("</super_categories>");
                                }
                                else
                                { sw.WriteLine(profile_word); }
                            }
                            else
                            { sw.WriteLine(profile_word); }
                        }
                    }
                }
            }
            else
            {
                ArrayList ns14_ids = new ArrayList();
                using (StreamReader sr = new StreamReader(working_dir + "category\\page_ns14.txt", Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        ns14_ids.Add(word.Substring(0, word.IndexOf(',')));
                    }
                }

                ns14_ids.Sort();

                ArrayList ns14_categoryLinks = new ArrayList();
                using (StreamReader sr = new StreamReader(working_dir + "categorylinks_ns0_and_ns14.txt", Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (ns14_ids.BinarySearch(word.Substring(0, word.IndexOf(","))) > -1)
                        {
                            ns14_categoryLinks.Add(word);
                        }
                    }
                }
                IComparer myComp = new categoryLink_id_Comparer();
                ns14_categoryLinks.Sort(myComp);
                using (StreamReader sr = new StreamReader(working_dir + "profile_v6.xml", Encoding.UTF8))
                {
                    using (StreamWriter sw = new StreamWriter(working_dir + "profile_v7.xml", false, Encoding.UTF8))
                    {
                        string word = string.Empty;
                        while ((word = sr.ReadLine()) != null)
                        {
                            if (word.StartsWith("<category cat_id"))
                            {
                                //string cat_title = word.Substring(word.IndexOf("cat_title=\"") + 11, word.IndexOf("\" cat_pages") - word.IndexOf("cat_title=\"") - 11);
                                string cat_page_id = string.Empty;

                                if (word.Contains("page_id=\""))
                                {
                                    cat_page_id = word.Substring(word.IndexOf("page_id=\"") + 9, word.LastIndexOf("\"") - word.IndexOf("page_id=\"") - 9);
                                    do
                                    {
                                        sw.WriteLine(word);
                                        word = sr.ReadLine();
                                    } while (!word.Contains("</super_categories>"));
                                    int cl_index = -1;
                                    IComparer cl_search = new cl_searchComparer();
                                    while ((cl_index = ns14_categoryLinks.BinarySearch(cat_page_id, cl_search)) > -1)
                                    {
                                        string temp_cl = ns14_categoryLinks[cl_index].ToString();
                                        sw.WriteLine("<sup_cat cat_title=\"" + temp_cl.Substring(temp_cl.IndexOf(",") + 1) + "\"></sup_cat>");
                                        ns14_categoryLinks.RemoveAt(cl_index);
                                    }
                                    sw.WriteLine(word);
                                }
                                else { sw.WriteLine(word); }
                            }
                            else { sw.WriteLine(word); }

                        }
                    }
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks1.sql", false, Encoding.UTF8))
                {
                    string[] separator1 = { ",'", "'," };
                    string[] separator2 = { "," };
                    bool flag = true;
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.StartsWith("INSERT INTO"))
                        {
                            //if (flag)
                            //{ 
                            //    flag = false;
                            //    MessageBox.Show(word.IndexOf("(").ToString());
                            //}
                            word = word.Remove(word.IndexOf("INSERT INTO"), word.IndexOf("(") + 1);
                            word = word.Replace(");", "");
                            word = word.Replace("),(", "\r\n");
                            sw.WriteLine(word);
                        }
                    }
                }
            }

        }

        private void button15_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks1.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_ns14.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        //MessageBox.Show(word+"\r\n"+word.Substring(word.IndexOf(",") + 1, word.IndexOf(",", word.IndexOf(",") + 1) - word.IndexOf(",") - 1));
                        if (word.Substring(word.IndexOf(",") + 1, word.IndexOf(",", word.IndexOf(",") + 1) - word.IndexOf(",") - 1) == "14")
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            long counter = 0;
            using (StreamReader sr = new StreamReader(working_dir + "all_titles_in_(pages_articles.xml)\\all-titles-in-ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    counter++;
                }
                MessageBox.Show(counter.ToString());
            }
        }

        // Search predicate returns true if a string ends in "saurus".
        static string article_title = string.Empty;
        private static bool with_the_same_title(String s)
        {
            if ((s.Length > 5) &&
                (s.Substring(s.Length - 6).ToLower() == article_title))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void button17_Click(object sender, EventArgs e)
        {
            List<string> distnict_pageLinks = new List<string>();
            List<int> counts_of_in_links = new List<int>();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_concept_concept_links_v5.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    string target = word.Substring(word.IndexOf(',')+1);
                    int pl_index = distnict_pageLinks.IndexOf(target);
                    if (pl_index < 0)
                    {
                        distnict_pageLinks.Add(target);
                        counts_of_in_links.Add(1);
                    }
                    else
                    {
                        counts_of_in_links[pl_index] = counts_of_in_links[pl_index] + 1;
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_inlinks_count.sql", false, Encoding.UTF8))
            {
                for (int i = 0; i < distnict_pageLinks.Count; i++)
                {
                    sw.WriteLine(distnict_pageLinks[i] + "," + counts_of_in_links[i]);
                }
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            List<string> dist_pageLinks = new List<string>();
            List<int> counts_of_out_links = new List<int>();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks_ns0target.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    int sub_length = word.IndexOf(",");
                    if (sub_length > 0)
                    {
                        string page_id = word.Substring(0, sub_length);
                        int pl_index = dist_pageLinks.IndexOf(page_id);
                        if (pl_index < 0)
                        {
                            dist_pageLinks.Add(page_id);
                            counts_of_out_links.Add(1);
                        }
                        else
                        {
                            counts_of_out_links[pl_index] = counts_of_out_links[pl_index] + 1;
                        }
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_outLinks_counts.sql", false, Encoding.UTF8))
            {
                for (int i = 0; i < dist_pageLinks.Count; i++)
                {
                    sw.WriteLine(dist_pageLinks[i] + "," + counts_of_out_links[i]);
                }
            }
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            ArrayList ns0_ids = new ArrayList();
            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns0_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            ns0_ids.Sort();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "outLinks_counts(all_ns-ns0).sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "outLinks_counts(ns0-ns0).sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (ns0_ids.BinarySearch(word.Substring(0, word.IndexOf(","))) > -1)
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            ArrayList ns0_ids = new ArrayList();
            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.txt", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    ns0_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            ns0_ids.Sort();

            List<string> pl_taget = new List<string>();
            List<int> pl_target_count = new List<int>();


            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks\\pagelinks_ns0target.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_ns0target_and_sources.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        //MessageBox.Show(word+"\r\n"+word.Substring(word.IndexOf(",") + 1, word.IndexOf(",", word.IndexOf(",") + 1) - word.IndexOf(",") - 1));
                        if (ns0_ids.BinarySearch(word.Substring(0, word.IndexOf(","))) > -1)
                        {
                            sw.WriteLine(word);
                        }
                        else
                        {
                            int f_index = word.IndexOf("\'");
                            int l_index = word.LastIndexOf("\'");
                            int sub_length = l_index - f_index - 1;
                            if (sub_length > 0)
                            {
                                string page_title = word.Substring(f_index + 1, sub_length);
                                int pl_index = pl_taget.IndexOf(page_title);
                                if (pl_index < 0)
                                {
                                    pl_taget.Add(page_title);
                                    pl_target_count.Add(1);
                                }
                                else
                                {
                                    pl_target_count[pl_index] = pl_target_count[pl_index] + 1;
                                }
                            }
                        }
                    }
                }
            }
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "inLinks_counts_from_all_ns_to_ns0.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "outLinks_counts_from_ns0_to_ns0.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string target_title = word.Substring(0, word.LastIndexOf(","));
                        int pl_index = pl_taget.IndexOf(target_title);
                        if (pl_index > -1)
                        {
                            int ns0_count = int.Parse(word.Substring(word.LastIndexOf(",") + 1)) - pl_target_count[pl_index];
                            sw.WriteLine(target_title + "," + ns0_count);
                        }
                        else
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "ns0pages_id_title.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string id = word.Substring(0, word.IndexOf(","));
                        if (word.IndexOf("','") == word.LastIndexOf("','"))
                        {
                            string title = word.Substring(word.IndexOf(",'") + 2, word.IndexOf("','") - word.IndexOf(",'") - 2);
                            sw.WriteLine(id + "," + title);
                        }
                        else
                        {
                            MessageBox.Show("Oh Oh Oh there are more than one ',' ");
                        }
                    }
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
        }

        private void button23_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(working_dir + "pages_articles.xml", Encoding.UTF8))
            {
                using (StreamWriter asw = new StreamWriter(cat_det_working_dir + "pageLinks_with_LinkFreq_more than_one.sql", false, Encoding.UTF8))
                {
                    string[] separator1 = { "<title>", "</title>" };
                    string[] separator2 = { "<text xml:space=\"preserve\">", "</text>" };

                    StringBuilder page = new StringBuilder(100000);
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        word = word.Trim();
                        //obtain a single page from wikipedia
                        page.Remove(0, page.Length);
                        if (word.Contains("<page>"))
                        {
                            do
                            {
                                page.AppendLine(word);
                                word = sr.ReadLine();
                            } while (!word.Contains("</page>"));
                            page.AppendLine(word);
                        }
                        string pageStr = page.ToString();

                        //
                        if (pageStr.Contains("<title>"))
                        {
                            if (pageStr.Contains("</title>"))
                            {
                                int title_index = pageStr.IndexOf("<id>") + 4;
                                string page_id = pageStr.Substring(title_index, pageStr.IndexOf("</id>") - title_index);
                                string page_title = pageStr.Substring(pageStr.IndexOf("<title>") + 7, pageStr.IndexOf("</title>") - pageStr.IndexOf("<title>") - 7);

                                string[] Page_text = pageStr.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
                                if (Page_text.Length == 3)
                                {
                                    if (!(page_title.StartsWith(namespaces[0]) || page_title.StartsWith(namespaces[1]) || page_title.StartsWith(namespaces[3]) || page_title.StartsWith(namespaces[4]) || page_title.StartsWith(namespaces[5]) || page_title.StartsWith(namespaces[6]) || page_title.StartsWith(namespaces[7]) || page_title.StartsWith(namespaces[8]) || page_title.StartsWith(namespaces[9]) || page_title.StartsWith(namespaces[10]) || page_title.StartsWith(namespaces[11]) || page_title.StartsWith(namespaces[12]) || page_title.StartsWith(namespaces[13]) || page_title.StartsWith(namespaces[14]) || page_title.StartsWith(namespaces[15]) || page_title.StartsWith(namespaces[16]) || page_title.StartsWith(namespaces[17]) || page_title.StartsWith(namespaces[18]) || page_title.StartsWith(namespaces[19]) || page_title.StartsWith(namespaces[20]) || page_title.StartsWith(namespaces[21])))
                                    {
                                        Regex cat_RE = new Regex(@"\[\[(?<category>.*?)\]\]");

                                        MatchCollection matches = cat_RE.Matches(Page_text[1]);
                                        //MessageBox.Show(matches.Count.ToString());
                                        List<string> internal_links = new List<string>();
                                        foreach (Match match in matches)
                                        {
                                            string il_str = match.Groups["category"].Value;
                                            if (il_str.StartsWith(namespaces[0]) || il_str.StartsWith(namespaces[1]) || il_str.StartsWith(namespaces[3]) || il_str.StartsWith(namespaces[4]) || il_str.StartsWith(namespaces[5]) || il_str.StartsWith(namespaces[6]) || il_str.StartsWith(namespaces[7]) || il_str.StartsWith(namespaces[8]) || il_str.StartsWith(namespaces[9]) || il_str.StartsWith(namespaces[10]) || il_str.StartsWith(namespaces[11]) || il_str.StartsWith(namespaces[12]) || il_str.StartsWith(namespaces[13]) || il_str.StartsWith(namespaces[14]) || il_str.StartsWith(namespaces[15]) || il_str.StartsWith(namespaces[16]) || il_str.StartsWith(namespaces[17]) || il_str.StartsWith(namespaces[18]) || il_str.StartsWith(namespaces[19]) || il_str.StartsWith(namespaces[20]) || il_str.StartsWith(namespaces[21]) || il_str.StartsWith("#"))
                                            { }
                                            else if (il_str.Contains("|"))
                                            {
                                                try
                                                {
                                                    il_str = il_str.Substring(0, il_str.IndexOf("|"));
                                                    if (il_str.Contains("#"))
                                                        il_str = il_str.Substring(0, il_str.IndexOf("#"));

                                                    internal_links.Add(il_str);
                                                }
                                                catch (Exception exc)
                                                {
                                                    MessageBox.Show(exc.Message);
                                                }
                                            }
                                            else if (il_str.Contains("#"))
                                            {
                                                try
                                                {
                                                    internal_links.Add(il_str.Substring(0, il_str.IndexOf("#")));
                                                }
                                                catch (Exception exc)
                                                {
                                                    MessageBox.Show(exc.Message);
                                                }
                                            }
                                            else
                                            {
                                                internal_links.Add(il_str);
                                            }
                                            //string parent_cat = match.Groups["category"].Value;
                                            //int index = match.Index;
                                        }

                                        while (internal_links.Count > 0)
                                        {
                                            string internal_link = internal_links[0];
                                            int count_link_freq = 0, il_index = 0;
                                            do
                                            {
                                                internal_links.RemoveAt(il_index);
                                                count_link_freq++;
                                                //   internal_links.
                                            } while ((il_index = internal_links.IndexOf(internal_link)) > -1);

                                            if (count_link_freq > 1)
                                                asw.WriteLine(page_id + "," + internal_link + "," + count_link_freq);
                                        }
                                    }
                                }
                                else
                                { //MessageBox.Show("some thing wrong 1");
                                }
                            }
                            else { MessageBox.Show("No </title>"); }
                        }
                    }//while close
                }//StreamWriter
            }//StreamReader
        }

        private void button24_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "outLinks_counts(ns0-ns0).sql", Encoding.UTF8))
            {
                string word = string.Empty;
                int max = 0;
                while ((word = sr.ReadLine()) != null)
                {
                    if (int.Parse(word.Substring(word.IndexOf(",") + 1)) > max)
                    {
                        max = int.Parse(word.Substring(word.IndexOf(",") + 1));
                    }
                }
                MessageBox.Show(max.ToString());
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            //Category_detector_handler handler = new Category_detector_handler();
            //Dictionary<string, int> concepts_dic = new Dictionary<string, int>();
            
            ////document_frequency Array DFA
            //Dictionary<string, int> DFA = new Dictionary<string, int>();
            
            ////concept_Category Index CCI
            //Dictionary<string, string> CCI = new Dictionary<string, string>();
           
            ////for each category expand the line (virtually don't add these expantion to file)
            //using (StreamReader sr = new StreamReader(cat_det_working_dir + "_category_concepts_index.sql", Encoding.UTF8))
            //{
            //    string word;
            //    while ((word = sr.ReadLine()) != null)
            //    {
            //        handler.get_category_main_concepts(word.Substring(0, word.IndexOf(':')), out concepts_dic);
            //        handler.get_all_concepts_for_a_category(word.Substring(0, word.IndexOf(':')), ref concepts_dic);

            //        //foreach concept add it to DFA and add one to df
            //        foreach (KeyValuePair<string, int> kvp in concepts_dic)
            //        {
            //            if (DFA.ContainsKey(kvp.Key))
            //            {
            //                DFA[kvp.Key]++;
            //            }
            //            else
            //            {
            //                DFA.Add(kvp.Key, 1);
            //            }
            //        }

            //        //for CCI; add concept as Key and concatinate the category at the end of the Value
            //        foreach (KeyValuePair<string, int> kvp in concepts_dic)
            //        {
            //            if (CCI.ContainsKey(kvp.Key))
            //            {
            //                CCI[kvp.Key] += word.Substring(0, word.IndexOf(':')) + ";";
            //            }
            //            else
            //            {
            //                CCI.Add(kvp.Key, word.Substring(0, word.IndexOf(':')) + ";");
            //            }
            //        }
            //    }
            //}

            //using (StreamWriter sw = new StreamWriter(cat_det_working_dir+"document_frequency.sql",false,Encoding.UTF8))
            //{
            //    foreach (KeyValuePair<string, int> kvp in DFA)
            //    {
            //        sw.WriteLine(kvp.Key + "," + kvp.Value);
            //    }
            //}
            //using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "concept_categories_index.sql", false, Encoding.UTF8))
            //{
            //    foreach (KeyValuePair<string, string> kvp in CCI)
            //    {
            //        sw.WriteLine(kvp.Key + ":" + kvp.Value);
            //    }
            //}
        }

        private void button26_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks\\pagelinks_ns0target_and_sources.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_v3.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string title = word.Substring(word.IndexOf('\'') + 1, word.LastIndexOf('\'') - word.IndexOf('\'') - 1).Replace('_', ' ');
                        sw.WriteLine(word.Substring(0, word.IndexOf(',')) + "," + title);
                    }
                }
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "inLinks_counts(ns0-ns0).sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "inLinks_counts(ns0-ns0)_v3.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Substring(word.LastIndexOf(",") + 1) != "0")
                            sw.WriteLine(word.Replace('_', ' '));
                    }
                }
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            string term_concepts_path = @"C:\D\work\Goals\Academic\Master\SWProjects\MasterSW\category_determination\term_concepts_indexes\_term_concepts_index(norm).sql";

            Dictionary<string, string> term_concepts_dic = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(term_concepts_path, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        term_concepts_dic.Add(word.Substring(0, word.IndexOf(':')), word.Substring(word.IndexOf(':') + 1));
                    }
                }
            }
            catch (TypeInitializationException)
            {

            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            List<string> disambig_ids = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disambig_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }

            disambig_ids.Sort();
            using (StreamReader sr = new StreamReader(working_dir + "page_ns0.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "concepts.xml", false, Encoding.UTF8))
                {
                    using (StreamWriter sw2 = new StreamWriter(cat_det_working_dir + "pages_ns0_without_redirect_without_disambig.sql", false, Encoding.UTF8))
                    {
                        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<concepts>");

                        string[] separator1 = { ",'", "'," };
                        string[] separator2 = { "," };

                        ArrayList category_titles = new ArrayList();
                        string word = string.Empty;
                        int counter = 0;
                        while ((word = sr.ReadLine()) != null)
                        {
                            string str1 = word.Substring(word.IndexOf(",\"\",") + 4);
                            string[] str_array = str1.Split(separator2, StringSplitOptions.None);
                            if (str_array.Length == 8)
                            {
                                int dis_index = disambig_ids.BinarySearch(word.Substring(0, word.IndexOf(',')));
                                if (str_array[1] == "0" && dis_index < 0)
                                {
                                    sw2.WriteLine(word);
                                    sw.WriteLine("<concept c_id=\"concept" + (counter++) + "\" page_id=\"" + word.Substring(0, word.IndexOf(',')) + "\">");
                                    sw.WriteLine("<synonyms>");

                                    sw.WriteLine("<synonym page_id=\"" + word.Substring(0, word.IndexOf(',')) + "\">" + gf.xml_noralization(word.Substring(word.IndexOf('\'') + 1, word.IndexOf("\',\"\",") - word.IndexOf('\'') - 1).Replace('_', ' ')) + "</synonym>");

                                    sw.WriteLine("</synonyms>");
                                    sw.WriteLine("</concept>");
                                }
                                else
                                {
                                    if (dis_index > -1 && str_array[1] != "0")
                                        disambig_ids.RemoveAt(dis_index);

                                }
                            }
                            else
                            {
                                MessageBox.Show("Not 8");
                            }
                        }
                    }
                    sw.WriteLine("</concepts>");
                }
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "disamibs.xml", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "disambiguation.xml", false, Encoding.UTF8))
                {
                    string[] separator1 = { "<title>", "</title>" };
                    string[] separator2 = { "<text xml:space=\"preserve\">", "</text>" };
                    string[] separator3 = { "\r\n" };

                    StringBuilder page = new StringBuilder(100000);
                    string word = string.Empty;
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<disambiguations>");
                    while ((word = sr.ReadLine()) != null)
                    {
                        word = word.Trim();
                        //obtain a single page from wikipedia
                        page.Remove(0, page.Length);
                        if (word.Contains("<page>"))
                        {
                            do
                            {
                                page.AppendLine(word);
                                word = sr.ReadLine();
                            } while (!word.Contains("</page>"));
                            page.AppendLine(word);
                        }
                        string pageStr = page.ToString();

                        //
                        if (pageStr.Contains("<title>"))
                        {
                            if (pageStr.Contains("</title>"))
                            {
                                int title_index = pageStr.IndexOf("<id>") + 4;
                                string page_id = pageStr.Substring(title_index, pageStr.IndexOf("</id>") - title_index);
                                string page_title = pageStr.Split(separator1, StringSplitOptions.RemoveEmptyEntries)[1];
                                string[] Page_text = pageStr.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
                                if (Page_text.Length == 3)
                                {
                                    sw.WriteLine("<disambig page_id=\"" + page_id + "\" term=\"" + page_title + "\">");
                                    string[] conc = Page_text[1].Split(separator3, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string con in conc)
                                    {
                                        sw.WriteLine("<concept c_id=\"\" article_link=\"\" page_id=\"\">" + con + "</concept>");
                                    }
                                    sw.WriteLine("</disambig>");
                                }
                                else
                                { //MessageBox.Show("some thing wrong 1");
                                }
                            }
                            else { MessageBox.Show("No </title>"); }
                        }
                    }//while close
                    sw.WriteLine("</disambiguations>");
                }//StreamWriter
            }//StreamReader
        }

        private void button31_Click(object sender, EventArgs e)
        {
            //put all the direct pages in array
            List<string> direct_pages = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_redirect_without_disambig_and_only_ns0_target.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    direct_pages.Add(word);
                }
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(cat_det_working_dir + "_concepts.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList synonymsElements = rootElem.GetElementsByTagName("synonyms");
            for (int i = 0; i < synonymsElements.Count; i++)
            {
                string synStr = synonymsElements[i].ChildNodes[0].FirstChild.Value;
                for (int j = 0; j < direct_pages.Count; j++)
                {
                    if (direct_pages[j].Substring(direct_pages[j].IndexOf(',') + 1) == synStr)
                    {
                        XmlElement newSynElem = xmldoc.CreateElement("synonym");
                        newSynElem.SetAttribute("page_id", direct_pages[j].Substring(0, direct_pages[j].IndexOf(',')));
                        synonymsElements[i].AppendChild(newSynElem);
                        direct_pages.RemoveAt(j);
                        j--;
                    }
                }
            }

            for (int j = 0; j < direct_pages.Count; j++)
            {
                string conceptText = direct_pages[j].Substring(direct_pages[j].IndexOf(',') + 1);
                XmlElement newConceptElem = xmldoc.CreateElement("concept");
                newConceptElem.SetAttribute("c_id", "concept" + (j + 113571));
                //newConceptElem.SetAttribute("page_id", direct_pages[j].Substring(0, direct_pages[j].IndexOf(',')));
                XmlElement newsynonymsElem = xmldoc.CreateElement("synonyms");
                XmlElement newsynonymElem1 = xmldoc.CreateElement("synonym");
                XmlElement newsynonymElem2 = xmldoc.CreateElement("synonym");
                newsynonymElem2.SetAttribute("page_id", direct_pages[j].Substring(0, direct_pages[j].IndexOf(',')));
                XmlText synonym_Title = xmldoc.CreateTextNode(conceptText);
                newsynonymElem1.AppendChild(synonym_Title);
                newsynonymsElem.AppendChild(newsynonymElem1);
                newsynonymsElem.AppendChild(newsynonymElem2);


                while (j < direct_pages.Count - 1 && (direct_pages[j + 1].Substring(direct_pages[j].IndexOf(',') + 1)) == conceptText)
                {
                    j++;
                    XmlElement another_Syn_Elem = xmldoc.CreateElement("synonym");
                    another_Syn_Elem.SetAttribute("page_id", direct_pages[j].Substring(0, direct_pages[j].IndexOf(',')));
                    newsynonymsElem.AppendChild(another_Syn_Elem);
                }

                newConceptElem.AppendChild(newsynonymsElem);
                rootElem.AppendChild(newConceptElem);
            }
            xmldoc.Save(cat_det_working_dir + "test_concepts.xml");
        }

        private void button32_Click(object sender, EventArgs e)
        {
            List<string> disamb_pages_title = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disamb_pages_title.Add(word.Substring(word.IndexOf(',') + 1));
                }
            }
            disamb_pages_title.Sort();

            List<string> disamb_pages_id = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disamb_pages_id.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            disamb_pages_id.Sort();

            List<string> direct_pages = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_redirect.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "redirect_without_disambig.xml", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Substring(word.IndexOf(',') + 1, word.IndexOf(',', word.IndexOf(',') + 1) - word.IndexOf(',') - 1) == "0")
                        {
                            string target = word.Substring(word.IndexOf('\'') + 1, word.LastIndexOf('\'') - word.IndexOf('\'') - 1);
                            //word.Substring(0, word.IndexOf(',')) + ","
                            string final = gf.xml_noralization(target).Replace('_', ' ');
                            int title_index = disamb_pages_title.BinarySearch(final);
                            int id_index = disamb_pages_id.BinarySearch(final);
                            if (title_index < 0 && id_index < 0)
                            {
                                sw.WriteLine(word.Substring(0, word.IndexOf(',')) + "," + final);
                            }
                            else
                            {
                                //direct_pages.Add(target);
                                //MessageBox.Show(word);
                            }
                        }
                    }
                }
            }
        }


        public class redirectPagesComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.LastIndexOf(",")).CompareTo(y.Substring(0, y.LastIndexOf(",")));
            }
        }
        public class redirectSearchComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.IndexOf(",")).CompareTo(y);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            List<string> direct_pages = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_redirect_ns0_pages_records.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    direct_pages.Add(word);
                }
            }
            redirectPagesComparer rpcom = new redirectPagesComparer();
            direct_pages.Sort(rpcom);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(cat_det_working_dir + "_test_concepts.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList synonymElements = rootElem.GetElementsByTagName("synonym");
            redirectSearchComparer RSComp = new redirectSearchComparer();
            for (int i = 0; i < synonymElements.Count; i++)
            {
                //if the syn has no text element get text element
                if (synonymElements[i].ChildNodes.Count == 0)
                {
                    int dir_page_index = direct_pages.BinarySearch(synonymElements[i].Attributes[0].Value, RSComp);
                    //MessageBox.Show(synonymElements[i].Attributes[0].Value+"\r\n"+direct_pages[dir_page_index].Substring(direct_pages[dir_page_index].IndexOf('\'') + 1, direct_pages[dir_page_index].IndexOf("\',\"\",") - direct_pages[dir_page_index].IndexOf('\'') - 1));
                    if (dir_page_index > -1)
                    {
                        XmlText synText = xmldoc.CreateTextNode(direct_pages[dir_page_index].Substring(direct_pages[dir_page_index].IndexOf('\'') + 1, direct_pages[dir_page_index].IndexOf("\',\"\",") - direct_pages[dir_page_index].IndexOf('\'') - 1));
                        synonymElements[i].AppendChild(synText);
                    }
                    //if(dir_page_index < 0)
                    //    MessageBox.Show(synonymElements[i].Attributes[0].Value);
                }
            }
            xmldoc.Save(cat_det_working_dir + "_concepts_with_all_syns.xml");
        }

        private void button34_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc1 = new XmlDocument();
            xmldoc1.Load(cat_det_working_dir + "all_without_700.xml");
            XmlElement rootElem1 = xmldoc1.DocumentElement;
            XmlNodeList synonymElements1 = rootElem1.GetElementsByTagName("synonym");
            List<string> all_synonyms = new List<string>();
            for (int i = 0; i < synonymElements1.Count; i++)
            {
                if (synonymElements1[i].ChildNodes.Count > 0)
                    all_synonyms.Add(synonymElements1[i].FirstChild.Value);
            }
            all_synonyms.Sort();

            XmlDocument xmldoc2 = new XmlDocument();
            xmldoc2.Load(cat_det_working_dir + "con700.xml");
            XmlElement rootElem2 = xmldoc2.DocumentElement;
            XmlNodeList conceptElements2 = rootElem2.GetElementsByTagName("concept");

            for (int i = 0; i < conceptElements2.Count; i++)
            {
                XmlNodeList syns = conceptElements2[i].FirstChild.ChildNodes;
                for (int j = 0; j < syns.Count; j++)
                {
                    if (syns.Count == 2)
                    {
                        if (syns[0].FirstChild.Value == "MBC3")
                        {
                            MessageBox.Show("HERE!");
                        }
                        if (all_synonyms.BinarySearch(syns[0].FirstChild.Value) > -1)
                        {
                            if (syns[1].ChildNodes.Count == 0)
                            {
                                rootElem2.RemoveChild(conceptElements2[i]);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
            xmldoc2.Save(cat_det_working_dir + "refine700.xml");
        }

        private void button35_Click(object sender, EventArgs e)
        {
            List<string> disambig_names = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disambig_names.Add(word.Substring(word.IndexOf(',') + 1));
                }
            }
            disambig_names.Sort();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_inLinks_counts(ns0-ns0)_v3.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "_inLinks_counts(ns0-ns0)_without_disambig_v4.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string xxx = word.Substring(0, word.LastIndexOf(','));
                        if (disambig_names.BinarySearch(xxx) < 0)
                        {
                            sw.WriteLine(word);
                        }
                        else
                        {
                            //MessageBox.Show(xxx);
                        }
                    }
                }
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(cat_det_working_dir + "_concepts.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList synonymElements1 = rootElem.GetElementsByTagName("synonym");
            List<string> all_synonyms = new List<string>();
            for (int i = 0; i < synonymElements1.Count; i++)
            {
                if (synonymElements1[i].ChildNodes.Count > 0)
                    all_synonyms.Add(synonymElements1[i].FirstChild.Value);
            }
            all_synonyms.Sort();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_inLinks_counts(ns0-ns0)_without_disambig_v4.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                int counter = 0;
                while ((word = sr.ReadLine()) != null)
                {
                    string xxx = word.Substring(0, word.LastIndexOf(','));

                    if (xxx.Contains("&"))
                    {

                    }
                    if (all_synonyms.BinarySearch(xxx) < 0)
                    {
                        XmlElement newConceptElem = xmldoc.CreateElement("concept");
                        newConceptElem.SetAttribute("c_id", "concept" + ((counter++) + 114293));

                        XmlElement newsynonymsElem = xmldoc.CreateElement("synonyms");
                        XmlElement newsynonymElem1 = xmldoc.CreateElement("synonym");

                        XmlText synonym_Title = xmldoc.CreateTextNode(xxx);
                        newsynonymElem1.AppendChild(synonym_Title);
                        newsynonymsElem.AppendChild(newsynonymElem1);
                        newConceptElem.AppendChild(newsynonymsElem);
                        rootElem.AppendChild(newConceptElem);
                    }
                    else
                    {
                        //MessageBox.Show(xxx);
                    }
                }
                xmldoc.Save("all_concepts _even_red_links.xml");

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            XmlDocument xmldoc2 = new XmlDocument();
            xmldoc2.Load(cat_det_working_dir + "_all_concepts_even_red_links.xml");
            XmlElement rootElem2 = xmldoc2.DocumentElement;
            XmlNodeList conceptElements2 = rootElem2.GetElementsByTagName("concept");
            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "id_concept_index.sql", false, Encoding.UTF8))
            {
                for (int i = 0; i < conceptElements2.Count; i++)
                {
                    XmlNodeList syns = conceptElements2[i].FirstChild.ChildNodes;
                    for (int j = 0; j < syns.Count; j++)
                    {
                        if (syns[j].ChildNodes.Count != 0 && syns[j].Attributes.Count == 1)
                        {
                            sw.WriteLine(syns[j].Attributes[0].Value + "," + conceptElements2[i].Attributes[0].Value);
                        }
                        //else { MessageBox.Show(syns[j].ChildNodes[0].Value); }
                    }
                }
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            List<string> disambig_ids = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disambig_ids.Add(word.Substring(0, word.IndexOf(',')));
                }
            }
            disambig_ids.Sort();

            List<string> disambig_names = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_disamb_id_title.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    disambig_names.Add(word.Substring(word.IndexOf(',') + 1));
                }
            }
            disambig_names.Sort();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_pagelinks_v3.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "pagelinks_without_disambig_v4.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        string source_id = word.Substring(0, word.IndexOf(','));
                        string target_name = word.Substring(word.IndexOf(',') + 1);
                        if (disambig_ids.BinarySearch(source_id) < 0 && disambig_names.BinarySearch(target_name) < 0)
                        {
                            sw.WriteLine(word);
                        }
                        else
                        {
                            //MessageBox.Show(word);
                        }
                    }
                }
            }
        }

        public class title_concept_indexComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.LastIndexOf(",")).CompareTo(y.Substring(0, y.LastIndexOf(",")));
            }
        }
        public class title_concept_indexSearchComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.LastIndexOf(",")).CompareTo(y);
            }
        }
        public class id_concept_indexComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.IndexOf(",")).CompareTo(y.Substring(0, y.IndexOf(",")));
            }
        }
        public class id_concept_indexSearchComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return x.Substring(0, x.IndexOf(",")).CompareTo(y);
            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            List<string> id_concept_index = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_id_concept_index.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    id_concept_index.Add(word);
                }
            }
            id_concept_indexComparer ICIComp = new id_concept_indexComparer();
            id_concept_index.Sort(ICIComp);

            List<string> title_concept_index = new List<string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_title_concept_index.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    title_concept_index.Add(word);
                }
            }
            title_concept_indexComparer TCIComp = new title_concept_indexComparer();
            title_concept_index.Sort(TCIComp);

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_LinkFreq_more_than_one_(all_have_concept)_v2.sql", Encoding.UTF8))
            {
                using (StreamWriter sw1 = new StreamWriter(cat_det_working_dir + "cc_LinkFreq_more_than_one_(all_have_concept)_v3.sql", false, Encoding.UTF8))
                {
                    using (StreamWriter sw2 = new StreamWriter(cat_det_working_dir + "LinkFreq_ends_have_no_concepts.sql", false, Encoding.UTF8))
                    {
                        string word = string.Empty;
                        while ((word = sr.ReadLine()) != null)
                        {
                            id_concept_indexSearchComparer icisComp = new id_concept_indexSearchComparer();
                            title_concept_indexSearchComparer tcisComp = new title_concept_indexSearchComparer();
                            int source_index = id_concept_index.BinarySearch(word.Substring(0, word.IndexOf(',')), icisComp);
                            string target = gf.form_SQL_to_XML_noralization(word.Substring(word.IndexOf(',') + 1, word.LastIndexOf(',') - word.IndexOf(',') - 1));
                            int target_index = title_concept_index.BinarySearch((target), tcisComp);

                            if (source_index > -1 && target_index > -1)
                            {
                                sw1.WriteLine(id_concept_index[source_index].Substring(id_concept_index[source_index].IndexOf(',') + 1)
                                                + ","
                                                + title_concept_index[target_index].Substring(title_concept_index[target_index].LastIndexOf(',') + 1)
                                                + ","
                                                + word.Substring(word.LastIndexOf(',')+1));
                            }
                            else
                            {
                                if (source_index > -1)
                                {
                                    sw2.WriteLine(id_concept_index[source_index].Substring(id_concept_index[source_index].IndexOf(',') + 1)
                                    +","
                                    + word.Substring(word.IndexOf(',') + 1));
                                }
                                else if (target_index > -1)
                                {

                                    sw2.WriteLine(word.Substring(0, word.IndexOf(','))
                                        + ","
                                        + title_concept_index[target_index].Substring(title_concept_index[target_index].LastIndexOf(',') + 1));
                                }
                                else {
                                    sw2.WriteLine(word);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button40_Click(object sender, EventArgs e)
        {
            //List<string> ts = new List<string>();
            Dictionary<string, int> links_dic = new Dictionary<string, int>();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cc_links(with_linkfreq)_v53.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                string link_as_key = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    link_as_key = word.Substring(0, word.LastIndexOf(','));
                    try
                    {
                        links_dic.Add(link_as_key, int.Parse(word.Substring(word.LastIndexOf(',') + 1)));
                        //links_dic.Add(word,1);
                    }
                    catch (ArgumentException)
                    {
                        if (int.Parse(word.Substring(word.LastIndexOf(',') + 1)) > 1)
                        {
                            links_dic[link_as_key] = links_dic[link_as_key] + int.Parse(word.Substring(word.LastIndexOf(',') + 1)) - 1;
                        }
                        else
                        {
                            links_dic[link_as_key] = links_dic[link_as_key] + int.Parse(word.Substring(word.LastIndexOf(',') + 1));
 
                        }
                        //links_dic[word] = links_dic[word] + 1;
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "cc_linkfreqs_v54.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, int> kvp in links_dic)
                {
                    sw.WriteLine(kvp.Key + "," + kvp.Value);
                }
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cc_LinkFreq_more_than_one_v3.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "_cc_links(without_same_concept)_v51.sql", true, Encoding.UTF8))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(cat_det_working_dir + "_all_concepts_even_red_links.xml");
                    XmlElement rootElem = xmldoc.DocumentElement;
                    XmlNodeList conceptElements1 = rootElem.GetElementsByTagName("concept");

                    for (int i = 0; i < conceptElements1.Count; i++)
                    {
                        sw.WriteLine(conceptElements1[i].Attributes[0].Value + "," + conceptElements1[i].Attributes[0].Value);
                    }
                }
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> links_dic = new Dictionary<string, int>();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_source_concepts.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    try
                    {
                        links_dic.Add(word, 1);
                    }
                    catch (ArgumentException)
                    {
                        links_dic[word] = links_dic[word] + 1;
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "cc_outlink_count.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, int> kvp in links_dic)
                {
                    sw.WriteLine(kvp.Key + "," + kvp.Value);
                }
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_concept_concept_links_(no_duplicates)_v8.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "source_concepts.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(word.Substring(0, word.IndexOf(',')));
                    }
                }
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            //List<string> linkfreqlinks = new List<string>();
            //using (StreamReader sr = new StreamReader(cat_det_working_dir + "_list_of_links_have_more_than_2_target_v2.sql", Encoding.UTF8))
            //{
            //    string word = string.Empty;
            //    while ((word = sr.ReadLine()) != null)
            //    {
            //        linkfreqlinks.Add(word);
            //    }
            //}
            //linkfreqlinks.Sort();
            SortedList<string, string> inlink_counts = new SortedList<string, string>();
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cc_outlink_count.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "cc_outlink_count(ordered).sql", true, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        inlink_counts.Add(word.Substring(0,word.IndexOf(',')), word.Substring(word.IndexOf(',') + 1));
                    }

                    foreach (KeyValuePair<string, string> kvp in inlink_counts)
                    {
                        sw.WriteLine(kvp.Key+","+kvp.Value);
                    }
                }
            }

        }

        private void button45_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cc_linkfreqs_v4.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "linkFreq_more_than_1.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (int.Parse(word.Substring(word.LastIndexOf(',') + 1)) > 1)
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        private void button46_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "pagelinks\\_concept_concept_links_v5.sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "concept_concept_links(without_same_concept)_v51.sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Substring(0, word.IndexOf(',')) != word.Substring(word.IndexOf(',') + 1))
                        {
                            sw.WriteLine(word);
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        private void button47_Click(object sender, EventArgs e)
        {
            Form2 disambig = new Form2();
            disambig.Show(this);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cc_outlink_count(ordered).sql", Encoding.UTF8))
            {
                using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "cc_outlink_count(ordered).sql", false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(word.Replace("concept","c"));
                    }
                }
            }
        }

        private void button50_Click(object sender, EventArgs e)
        {
            SortedDictionary<string, List<string>> concept_concepts_SD = new SortedDictionary<string, List<string>>();

            using (StreamReader sr = new StreamReader(cat_det_working_dir + "_cclinks_with_linkfreqs_v55.sql", Encoding.UTF8))
            {
                string word = string.Empty;
                while ((word = sr.ReadLine()) != null)
                {
                    List<string> concepts_list = new List<string>();
                    string target = word.Substring(word.IndexOf(',') + 1, word.LastIndexOf(',') - word.IndexOf(',') - 1);
                    if (concept_concepts_SD.TryGetValue(target, out concepts_list))
                    {
                        concepts_list.Add(word.Substring(0, word.IndexOf(',')) + ";");
                    }
                    else
                    {
                        concept_concepts_SD.Add(target, new List<string>());
                        concept_concepts_SD[target].Add(word.Substring(0, word.IndexOf(',')) + ";");
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(cat_det_working_dir + "concept_inconcepts_list_index.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, List<string>> kvp in concept_concepts_SD)
                {
                    StringBuilder concepts_string = new StringBuilder(10000);
                    foreach (string concept in kvp.Value)
                    {
                        concepts_string.Append(concept);
                    }
                    sw.WriteLine(kvp.Key + ":" + concepts_string.ToString());
                }
            }
            
        }

        private void button51_Click(object sender, EventArgs e)
        {
            

        }

        private void button49_Click(object sender, EventArgs e)
        {
        }

        private void button52_Click(object sender, EventArgs e)
        {
        }

        private void button53_Click(object sender, EventArgs e)
        {
            categories_creator cc = new categories_creator();
            cc.Show();
        }

        private void button54_Click(object sender, EventArgs e)
        {           }

        private void button22_Click_1(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(@"C:\Documents and Settings\dos\My Documents\Downloads\arwiki-20100123-pages-articles.xml");
            XmlElement rootElem = xmldoc.DocumentElement;
            XmlNodeList conceptElements = rootElem.GetElementsByTagName("page");

        }

        private void button49_Click_1(object sender, EventArgs e)
        {
            //char[] sep = {' '};
            //Regex RE = new Regex(@"\W+");

            //textBox3.Lines = RE.Replace(textBox2.Text, " ").Split(sep, StringSplitOptions.RemoveEmptyEntries);
            
            //string[] tokens = RE.Split(textBox2.Text);            
            //textBox3.Lines = tokens;

            textBox3.Lines = Regex.Split(textBox2.Text, @"[^\wًٌٍَُِّْ]+");

                //foreach (string token in tokens)
                //{
                //    if(token
                //    textBox3.Lines
                //}
        }
    }
}