using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Toys
{
    public partial class Form2 : Form
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataReader dr;
        public Form2()
        {
            InitializeComponent();
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listView1.SelectedItems.Count > 0)
                {


                    ListViewItem Item = listView1.SelectedItems[0];
                    textBox1.Text = Item.SubItems[0].Text;
                    textBox2.Text = Item.SubItems[1].Text;
                    textBox3.Text = Item.SubItems[2].Text;
                    textBox4.Text = Item.SubItems[3].Text;
                    if (Item != null && Item.SubItems.Count > 5)
                    {
                        string imagePath = Item.SubItems[5].Text;
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
                            pictureBox1.Image = image;
                        }
                        else
                        {
                            pictureBox1.Image = null;
                            MessageBox.Show("Image file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                conn = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= Toys.accdb");
                cmd = new OleDbCommand("Select ToyID, ToyName, ToyType, Model, Price, Images from Toys", conn);
                conn.Open();
                dr = cmd.ExecuteReader();
                listView1.Items.Clear();
                while (dr.Read())
                {
                    ListViewItem item = new ListViewItem(dr["ToyName"].ToString());
                    item.SubItems.Add(dr["ToyType"].ToString());
                    item.SubItems.Add(dr["Model"].ToString());
                    item.SubItems.Add(dr["Price"].ToString());
                    item.SubItems.Add(dr["ToyID"].ToString());
                    item.SubItems.Add(dr["Images"].ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conn.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private string UploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
               string.IsNullOrWhiteSpace(textBox2.Text) ||
               string.IsNullOrWhiteSpace(textBox3.Text) ||
               string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop execution if any textbox is empty
            }
            string ToyID = "";
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count > 4)
                {
                    ToyID = selectedItem.SubItems[4].Text;
                }
            }
            try 
            {
                conn = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = Toys.accdb");
                conn.Open();
                cmd = new OleDbCommand("Update Toys Set ToyName = @UpdateToyName, ToyType = @UpdateToyType, Model = @UpdatedModel, Price = @UpdatePrice, Images = @UpdatedImage where ToyID = @ToyID", conn);
                DialogResult result = MessageBox.Show("Are u sure u want update this item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string imagePath = UploadImage();
                    if (string.IsNullOrWhiteSpace(imagePath))
                    {
                         return;
                    }    
                                cmd.Parameters.AddWithValue("@UpdateToyName", textBox1.Text);
                                cmd.Parameters.AddWithValue("@UpdateToyType", textBox2.Text);
                                cmd.Parameters.AddWithValue("UpdatedModel", textBox3.Text);
                                cmd.Parameters.AddWithValue("@UpdatePrice", textBox4.Text);
                                cmd.Parameters.AddWithValue("@UpdatedImage", imagePath);
                                cmd.Parameters.AddWithValue("@ToyID", ToyID);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                           {
                                MessageBox.Show("Item updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           }
                      else
                      {
                          MessageBox.Show("No rows affected. Item was not updated", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                      }
                      }
                conn.Close();
            }
            
            catch (Exception ex) 
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
