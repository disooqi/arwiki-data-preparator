using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace wiki_data_preparator
{
    class general_functions
    {
        public string retrieval_normalization(string page_name)
        {
            return page_name;
        }

        public string xml_noralization(string page_name)
        {
            if (page_name.Contains("&") || page_name.Contains(">") || page_name.Contains("<"))
            {
                page_name = page_name.Replace("&", "&amp;");
                page_name = page_name.Replace(">", "&gt;");
                page_name = page_name.Replace("<", "&lt;");
            }
            if (page_name.Contains("\\"))
            {
                while (page_name.Contains("\\"))
                {
                    int back_index = page_name.IndexOf('\\');
                    char escape = page_name[back_index + 1];
                    switch (escape)
                    {
                        case '\'':
                            page_name = page_name.Remove(back_index, 2);
                            page_name = page_name.Insert(back_index, "&apos;");
                            break;

                        case '\"':
                            page_name = page_name.Remove(back_index, 2);
                            page_name = page_name.Insert(back_index, "&quot;");
                            break;

                        case '\\':
                            page_name = page_name.Remove(back_index, 2);
                            page_name = page_name.Insert(back_index, "&#92;");
                            break;

                        default:
                            MessageBox.Show("not as you think: " + page_name);
                            break;
                    }
                }
            }
            return page_name;
        }

        public string form_SQL_to_XML_noralization(string str)
        {
            if (str.Contains("\\"))
            {
                while (str.Contains("\\"))
                {
                    int back_index = str.IndexOf('\\');
                    char escape = str[back_index + 1];
                    switch (escape)
                    {
                        case '\'':
                            str = str.Remove(back_index, 2);
                            str = str.Insert(back_index, "\'");
                            break;

                        case '\"':
                            str = str.Remove(back_index, 2);
                            str = str.Insert(back_index, "\"");
                            break;

                        case '\\':
                            str = str.Remove(back_index, 2);
                            str = str.Insert(back_index, "&#92;");
                            break;

                        default:
                            MessageBox.Show("not as you think: " + str);
                            str = str.Remove(back_index, 1);
                            str = str.Insert(back_index, "&#92;");
                            break;
                    }
                }
            }
            return str.Replace("&#92;", "\\");
        }
    }
}
