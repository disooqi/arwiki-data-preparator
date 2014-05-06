using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace wiki_data_preparator
{
    public partial class categories_creator : Form
    {
        categories cat_Obj = new categories();
        public categories_creator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            cat_Obj.extract_main_categories_and_its_descendants();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cat_Obj.remove_categories_with_8_attributes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cat_Obj.sort_and_remove_duplicate_then_update_profile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cat_Obj.add_concept_id_to_categories();
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            cat_Obj.generate_category_concept_index();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cat_Obj.generate_concept_categories_index();
        }
    }
}