using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Data.SqlClient;
namespace Toys
{
    public partial class Form1 : Form
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataReader dr;
        public Form1()
        {
            InitializeComponent();
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
        }
        private void button2_Click(object sender, EventArgs e)
        {                               //Items List
            try
            {
                conn = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= Toys.accdb");
                conn.Open();
                cmd = new OleDbCommand("Select ToyID, ToyName, ToyType, Model, Price, Images from Toys", conn);
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
        private string uploadButton_Click(string imagePath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
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
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
               string.IsNullOrWhiteSpace(textBox3.Text) ||
               string.IsNullOrWhiteSpace(textBox4.Text) ||
               string.IsNullOrWhiteSpace(textBox5.Text))            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop execution if any textbox is empty
            }
            //Items Add
            conn = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= Toys.accdb");
            cmd = new OleDbCommand("INSERT INTO Toys(ToyName, ToyType, Model, Price, Images) VALUES(@ToyName, @ToyType, @Model, @Price, @Images)", conn);
            conn.Open();
            string imagePath = UploadImage();
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }
            using (conn)
            {
                cmd.Parameters.AddWithValue("@ToyName", textBox2.Text);
                cmd.Parameters.AddWithValue("@ToyType", textBox3.Text);
                cmd.Parameters.AddWithValue("@Model", textBox4.Text);
                cmd.Parameters.AddWithValue("@Price", textBox5.Text);
                cmd.Parameters.AddWithValue("@Images", imagePath);
                //             cmd.Parameters.AddWithValue("@Images", imageData); How to add image path to database as path. 
                //  We can upload the image and get the image path. Save as image path into database
                // We can control the image if did upload or not. If image upload prob in the file call "Images"
                //  So add image upload button, can add database after click the button
                
                try
                {
                   
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item Added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No rows affected. Item was not added", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //              It's control values in to textBoxs

        }
        private void button3_Click(object sender, EventArgs e)
        {                           //Delete Item
            string ToyID = "";
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count > 4)
                {
                    ToyID = selectedItem.SubItems[4].Text;
                }
            }
            if (!string.IsNullOrEmpty(ToyID))
            {
                conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source = Toys.accdb");
                cmd = new OleDbCommand("Delete from Toys where ToyID = @ToyID", conn);
                DialogResult result = MessageBox.Show("Are u sure u want to delete this item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                conn.Open();
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ToyID", ToyID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Record deleted succesfully:");
                        }
                        else
                        {
                            MessageBox.Show($"No record deleted:");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    
                
                ListViewItem Item = listView1.SelectedItems[0];

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
      private void button5_click(object sender, EventArgs e)
        {//                         When u click the button show the picture of Item
            // We are going to upgrade this When double clicked on listview item show the item of picture
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count > 5)
                {
                    string ImagePath = selectedItem.SubItems[5].Text;
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromFile(ImagePath);
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
  
        private System.Drawing.Image Image(string ImagePath)
        {
            if (System.IO.File.Exists(ImagePath))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(ImagePath);
                return image;
            }
            else 
            { 
                return null;
            }
            }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'toysDataSet.Toys' table. You can move, or remove it, as needed.
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e, MouseEventArgs me)
        {
        }
        /*        private void GetToyIdFromSelectedListViewItem()
        {
            string ToyID = "";
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                if (selectedItem.SubItems.Count > 4)
                {
                    ToyID = selectedItem.SubItems[4].Text;
                }
            }
        }
*/
        private void listViewItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Double click listview show you the u clicked toy
        }
    
        //Image could added on database
        // Configure OpenFileDialog properties
        /*           openFileDialog1.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*";
                   openFileDialog1.Title = "Select an Image";

                   // Show the OpenFileDialog and get the selected file path
                   if (openFileDialog1.ShowDialog() == DialogResult.OK)
                   {
                       string selectedImagePath = openFileDialog1.FileName;
                       // Do something with the selectedImagePath, such as displaying the path or using it to load the image
                       MessageBox.Show("Selected Image Path: " + selectedImagePath);
                   }
                   using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                   {
                       imageData = new byte[stream.Length];
                       stream.Read(imageData, 0, (int)stream.Length);
                   }
        */
        /*              Control Images Path
         *          if (string.IsNullOrWhiteSpace(imagePath))
                    {
                        MessageBox.Show("Please select an image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
        */
        // Will be upload image on database from visual studio
        /*               On Here I tried: If button click several time shouldn't be show same result on list view. But i failed about it 
         *       private void CheckDatabaseItems()
               {
                   conn = new OleDbConnection("Provider = Microsoft.ACE.OLEDB.12.0; Data Source= Toys.accdb");
                   cmd = new OleDbCommand("Select count(*) from Toys", conn);
                   try
                   {
                       conn.Open();
                       int itemCount = (int)cmd.ExecuteScalar();
                       button2.Enabled = (itemCount > 0);
                       label6.Text = itemCount.ToString();
                       Debug.Write("This is checking database items");
                       Debug.WriteLine($"Number of items in the database: {itemCount}");
                       conn.Close();
                   }
                   catch (Exception ex)
                   {
                       Debug.Write("This is checking database items");
                       MessageBox.Show($"Error checking database items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       Debug.WriteLine($"Error Checking database items: {ex.Message}");
                   }
               }
        HashSet<string> addedItems = new HashSet<string>();
       */

        /*           string newItem = Guid.NewGuid().ToString();
                   if (!addedItems.Contains(newItem))
                   {
                       listView1.Items.Add (newItem);

                       addedItems.Add(newItem);
                   }
                   else
                   {
                       MessageBox.Show("This item has already been added to the list.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   }
        */

        /*       private void ListView1_SelectedIndexChanged()
               {
                    first select the listview column
                    * get id from it
                    * equal to variable call selected
                    * delete the id in database
                    * remove the seleceted column

                   if (listView1.SelectedItems.Count > 0)
                   {
                       int itemId = Convert.ToInt32(listView1.SelectedItems[0].Tag);
                       if (ListView1_SelectedIndexChanged(itemId))
                       {
                           listView1.Items.Remove(listView1.SelectedItems[0]);
                       }
                       else
                       {
                           MessageBox.Show("Failed to delete the item from the database.", "Erro", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                       }
                   }
                   else
                   {
                       MessageBox.Show("No item selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   }
               }
       */

        private void button4_Click(object sender, EventArgs e)
        {
            /*      Update list
             *  click update button
             *  open second form
             *  select the item on listview
             *  get item values on picture box and text box
             *  u can rename it and change the picture box
             *  when u press update button u can ask do u want to change image or not if yes change the image
             *  and update the item on database
             *  
             *  extra u can add picturebox, upload image 
             */
             Form2 secondForm = new Form2();
            secondForm.ShowDialog();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
       
        private void ShowImage(string imagePath)
        {

        }

        /*    private void GetSelectedToyID()
    {
        List<int> list = new List<int>();
        foreach (ListViewItem item in listView1.SelectedItems)
        {
            if (listView1.SelectedItems.Count > 0)
            {                                   
                 if (item.SubItems.Count > 4)
                 {
                      string ToyID = item.SubItems[4].Text;
                      MessageBox.Show(ToyID);
                 }
            }
        }
    } */
        /*private void SelectedItem(int itemID)
        {
                      This func can get ToyID from listview1
             *   if (listView1.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    if (selectedItem.SubItems.Count > 4)
                    {
                        string ToyID = selectedItem.SubItems[4].Text;
                        MessageBox.Show(ToyID);
                    }
                }
           

    } */
        /*       private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
    if (listView1.SelectedItems.Count > 0)
    {
     string selectedItemId = listView1.SelectedItems[0].SubItems[0].Text;
     string imagePath = RetrieveImagePathFromDatabase(selectedItemId);

     if (!string.IsNullOrEmpty(imagePath))
     {
         // Display the image in PictureBox
         pictureBox1.ImageLocation = imagePath;
     }
     else
     {
         MessageBox.Show("No image path found for selected item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
     }
    }
    }
    */
        /*      private string RetrieveImagePathFromDatabase(string itemId)
               {
                   string query = "SELECT Images FROM Toys WHERE ToyID = @ToyID"; // Assuming ToyID is the primary key

                   using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Toys.accdb"))
                   using (OleDbCommand cmd = new OleDbCommand(query, conn))
                   {
                       cmd.Parameters.AddWithValue("@ToyID", itemId);

                       try
                       {
                           conn.Open();
                           object result = cmd.ExecuteScalar();
                           if (result != null && result != DBNull.Value)
                           {
                               return result.ToString(); // Return the image path
                           }
                           else
                           {
                               return null; // Return null if no image path found
                           }
                       }
                       catch (Exception ex)
                       {
                           MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                           return null; // Return null in case of an error
                       }
                   }
               }
        */
        /*       private void ShowImage(Image image)
               {
                   // Create a new form to display the image
                   Form imageForm = new Form();
                   imageForm.Text = "Image Viewer";
                   imageForm.Size = new Size(image.Width, image.Height);

                   // Create a PictureBox to show the image
                   PictureBox pictureBox = new PictureBox();
                   pictureBox.Dock = DockStyle.Fill;
                   pictureBox.Image = image;

                   // Add the PictureBox to the form
                   imageForm.Controls.Add(pictureBox);

                   // Display the form
                   imageForm.ShowDialog();
               }
        */
    }
}
