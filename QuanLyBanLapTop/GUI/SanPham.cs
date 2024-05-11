using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace xuathoadon
{
    public partial class SanPham : Form
    {
        SqlConnection connection;
        SqlCommand command;
        string str = "Data Source=DESKTOP-636OIPO;Initial Catalog=Sale;Integrated Security=True;";
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table = new DataTable();

        void LoadData()
        {
            command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Products";
            adapter.SelectCommand = command;
            table.Clear();
            adapter.Fill(table);
            dgv.DataSource = table;
        }

        // SanitizeFileName method to remove illegal characters from file names
        private string SanitizeFileName(string fileName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }

        // ResizeImage method to resize the image while maintaining its aspect ratio
        private System.Drawing.Image ResizeImage(System.Drawing.Image originalImage, int targetWidth, int targetHeight)
        {
            Bitmap resizedImage = new Bitmap(targetWidth, targetHeight);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
            }

            return resizedImage;
        }

        public SanPham()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'saleDataSet1.Product1' table. You can move, or remove it, as needed.
            //this.product1TableAdapter.Fill(this.saleDataSet1.Product1);
            // TODO: This line of code loads data into the 'saleDataSet.Products' table. You can move, or remove it, as needed.
            //this.productsTableAdapter.Fill(this.saleDataSet.Products);
            connection = new SqlConnection(str);

            // Check if the connection is not open before opening it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            LoadData();
        }

        private void LoadImageFromSelectedRow()
        {
            // Assuming the "image_path" column exists in the Products table
            int imagePathColumnIndex = table.Columns["image_path"]?.Ordinal ?? -1;

            if (imagePathColumnIndex != -1)
            {
                int rowIndex = dgv.CurrentRow.Index;
                string imagePath = dgv.Rows[rowIndex].Cells[imagePathColumnIndex].Value?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Sanitize the file name
                    string sanitizedFileName = SanitizeFileName(imagePath);

                    // You need to adjust the path based on your image storage strategy
                    string fullImagePath = Path.Combine(Application.StartupPath, "ProductImages", sanitizedFileName);

                    if (File.Exists(fullImagePath))
                    {
                        // Use the fully qualified name for System.Drawing.Image
                        System.Drawing.Image image = System.Drawing.Image.FromFile(fullImagePath);
                        pictureBox1.Image = image;
                    }
                    else
                    {
                        MessageBox.Show($"Image not found: {fullImagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        pictureBox1.Image = null; // Clear PictureBox if the image is not found
                    }
                }
                else
                {
                    // Clear PictureBox if the image path is empty
                    pictureBox1.Image = null;
                }
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count)
            {
                int columnIndex = dgv.Columns["product_id"]?.Index ?? -1;

                if (columnIndex != -1)
                {
                    int i = e.RowIndex;
                    tb_productid.Text = dgv.Rows[i].Cells[columnIndex].Value?.ToString() ?? string.Empty;
                    tb_productname.Text = dgv.Rows[i].Cells["product_name"].Value?.ToString() ?? string.Empty;
                    tb_mota.Text = dgv.Rows[i].Cells["mo_ta"].Value?.ToString() ?? string.Empty;
                    tb_gia.Text = dgv.Rows[i].Cells["gia"].Value?.ToString() ?? string.Empty;
                    tb_soluong.Text = dgv.Rows[i].Cells["so_luong"].Value?.ToString() ?? string.Empty;
                    tb_upload.Text = dgv.Rows[i].Cells["image_path"].Value?.ToString() ?? string.Empty;

                    // Display the image
                    LoadImageFromSelectedRow();
                }
                else
                {
                    MessageBox.Show("Column 'product_id' not found in DataGridView.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tb_productname.Text) && !string.IsNullOrEmpty(tb_mota.Text) && !string.IsNullOrEmpty(tb_gia.Text) && !string.IsNullOrEmpty(tb_soluong.Text) && !string.IsNullOrEmpty(tb_upload.Text))
                {
                    using (connection = new SqlConnection(str))
                    {
                        connection.Open();

                        command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO Products (product_name, mo_ta, gia, so_luong, image_path) VALUES (@ProductName, @MoTa, @Gia, @SoLuong, @Anh)";
                        command.Parameters.AddWithValue("@ProductName", tb_productname.Text);
                        command.Parameters.AddWithValue("@MoTa", tb_mota.Text);
                        command.Parameters.AddWithValue("@Gia", tb_gia.Text);
                        command.Parameters.AddWithValue("@SoLuong", tb_soluong.Text);
                        command.Parameters.AddWithValue("@Anh", tb_upload.Text);

                        command.ExecuteNonQuery();
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tb_productname.Text) && !string.IsNullOrEmpty(tb_mota.Text) && !string.IsNullOrEmpty(tb_gia.Text) && !string.IsNullOrEmpty(tb_soluong.Text) && !string.IsNullOrEmpty(tb_upload.Text))
                {
                    using (connection = new SqlConnection(str))
                    {
                        connection.Open();

                        command = connection.CreateCommand();
                        command.CommandText = "UPDATE Products SET product_name = @ProductName, mo_ta = @MoTa, gia = @Gia, so_luong = @SoLuong, image_path = @Anh WHERE product_id = @ProductId";
                        command.Parameters.AddWithValue("@ProductId", tb_productid.Text);
                        command.Parameters.AddWithValue("@ProductName", tb_productname.Text);
                        command.Parameters.AddWithValue("@MoTa", tb_mota.Text);
                        command.Parameters.AddWithValue("@Gia", tb_gia.Text);
                        command.Parameters.AddWithValue("@SoLuong", tb_soluong.Text);
                        command.Parameters.AddWithValue("@Anh", tb_upload.Text);

                        command.ExecuteNonQuery();
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tb_productid.Text))
                {
                    command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Products WHERE product_id = @ProductId";

                    command.Parameters.AddWithValue("@ProductId", tb_productid.Text);

                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please select a record to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            try
            {
                // Create and configure the OpenFileDialog
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                openFileDialog.Title = "Select an Image File";

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string imagePath = openFileDialog.FileName;

                    // Display the image in a PictureBox (optional)
                    pictureBox1.Image = System.Drawing.Image.FromFile(imagePath);

                    // Process the selected image as needed (save to database, display in DataGridView, etc.)
                    // For example, you can update the 'tb_anh' TextBox with the file name or path:
                    tb_upload.Text = Path.GetFileName(imagePath);

                    // If you need to save the image to a specific location, you can use File.Copy:
                    string destinationPath = Path.Combine(Application.StartupPath, "ProductImages", SanitizeFileName(tb_upload.Text));
                    File.Copy(imagePath, destinationPath, true);

                    // Now you can update your DataGridView or perform any other necessary actions
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Create and configure the OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
            openFileDialog.Title = "Select an Image File";

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                string imagePath = openFileDialog.FileName;

                // Display the image in the PictureBox
                pictureBox1.Image = System.Drawing.Image.FromFile(imagePath);
            }
        }
    }
}
