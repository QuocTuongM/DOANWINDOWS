using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Sale
{
    public partial class ThongTin : Form
    {
        private readonly string connectionString = "Data Source=DESKTOP-636OIPO;Initial Catalog=QuanLyBanLatop;Integrated Security=True";

        public ThongTin()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Customers";

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            dgv.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }


        private void LoadProductData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Products";

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            dgv2.DataSource = table;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product data: " + ex.Message);
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadProductData(); // Add this line to load product data
        }


        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = dgv.CurrentRow.Index;
            tb_customerId.Text = dgv.Rows[i].Cells["customer_id"].Value.ToString();
            tb_customerName.Text = dgv.Rows[i].Cells["first_name"].Value.ToString();
            tb_diaChi.Text = dgv.Rows[i].Cells["last_name"].Value.ToString();
            tb_email.Text = dgv.Rows[i].Cells["email"].Value.ToString();
            tb_phone.Text = dgv.Rows[i].Cells["phone"].Value.ToString();
            tb_timkh.Text = dgv.Rows[i].Cells["timkh"].Value.ToString();
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidCustomerData())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            // Omit customer_id from the column list to allow the database to generate the value
                            command.CommandText = "INSERT INTO Customers (customer_name, dia_chi, email, phone) VALUES (@CustomerName, @DiaChi, @Email, @Phone)";
                            command.Parameters.AddWithValue("@CustomerName", tb_customerName.Text);
                            command.Parameters.AddWithValue("@DiaChi", tb_diaChi.Text);
                            command.Parameters.AddWithValue("@Email", tb_email.Text);
                            command.Parameters.AddWithValue("@Phone", tb_phone.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer: " + ex.Message);
            }
        }


        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidCustomerData())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "UPDATE Customers SET customer_name = @CustomerName, dia_chi = @DiaChi, email = @Email, phone = @Phone WHERE customer_id = @CustomerId";
                            command.Parameters.AddWithValue("@CustomerId", tb_customerId.Text);
                            command.Parameters.AddWithValue("@CustomerName", tb_customerName.Text);
                            command.Parameters.AddWithValue("@DiaChi", tb_diaChi.Text);
                            command.Parameters.AddWithValue("@Email", tb_email.Text);
                            command.Parameters.AddWithValue("@Phone", tb_phone.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message);
            }
        }

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tb_customerId.Text))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "DELETE FROM Customers WHERE customer_id = @CustomerId";
                            command.Parameters.AddWithValue("@CustomerId", tb_customerId.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Please provide a customer ID to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer: " + ex.Message);
            }
        }

        private bool IsValidCustomerData()
        {
            return
                !string.IsNullOrEmpty(tb_customerId.Text) &&
                !string.IsNullOrEmpty(tb_customerName.Text) &&
                !string.IsNullOrEmpty(tb_diaChi.Text) &&
                !string.IsNullOrEmpty(tb_email.Text) &&
                !string.IsNullOrEmpty(tb_phone.Text);
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            // Clear textboxes
            tb_customerId.Text = string.Empty;
            tb_customerName.Text = string.Empty;
            tb_diaChi.Text = string.Empty;
            tb_email.Text = string.Empty;
            tb_phone.Text = string.Empty;

            // Deselect the DataGridView row
            dgv.ClearSelection();
        }

        private void btn_xuat_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a SaveFileDialog to prompt the user for the file location
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Export to Text File";
                    saveFileDialog.DefaultExt = "txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the chosen file path
                        string filePath = saveFileDialog.FileName;

                        // Open a StreamWriter to write to the text file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            // Write the header (column names)
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                writer.Write(dgv.Columns[i].HeaderText);
                                if (i < dgv.Columns.Count - 1)
                                    writer.Write("\t");
                            }
                            writer.WriteLine();

                            // Write the data
                            for (int i = 0; i < dgv.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgv.Columns.Count; j++)
                                {
                                    writer.Write(dgv.Rows[i].Cells[j].Value);
                                    if (j < dgv.Columns.Count - 1)
                                        writer.Write("\t");
                                }
                                writer.WriteLine();
                            }

                            MessageBox.Show("Data exported successfully!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message);
            }
        }

        private void dgv2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = dgv.CurrentRow.Index;
            tb_productId.Text = dgv.Rows[i].Cells["product_id"].Value.ToString();
            tb_productName.Text = dgv.Rows[i].Cells["product_name"].Value.ToString();
            tb_moTa.Text = dgv.Rows[i].Cells["mo_ta"].Value.ToString();
            tb_gia.Text = dgv.Rows[i].Cells["gia"].Value.ToString();
            tb_soLuong.Text = dgv.Rows[i].Cells["so_luong"].Value.ToString();
            tb_timsp.Text = dgv.Rows[i].Cells["timsp"].Value.ToString();
        }

        private void btn_them1_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidProductData())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Products (product_name, mo_ta, gia, so_luong) " +
                                                  "VALUES (@ProductName, @MoTa, @Gia, @SoLuong); " +
                                                  "SELECT SCOPE_IDENTITY()";
                            command.Parameters.AddWithValue("@ProductName", tb_productName.Text);
                            command.Parameters.AddWithValue("@MoTa", tb_moTa.Text);
                            command.Parameters.AddWithValue("@Gia", tb_gia.Text);
                            command.Parameters.AddWithValue("@SoLuong", tb_soLuong.Text);

                            // ExecuteScalar to get the identity value generated by the database
                            int generatedProductId = Convert.ToInt32(command.ExecuteScalar());
                            MessageBox.Show($"Product added successfully with ID: {generatedProductId}");
                        }
                    }

                    LoadProductData(); // Reload product data after adding a new product
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }

        private bool IsValidProductData()
        {
            return
                !string.IsNullOrEmpty(tb_productId.Text) &&
                !string.IsNullOrEmpty(tb_productName.Text) &&
                !string.IsNullOrEmpty(tb_moTa.Text) &&
                !string.IsNullOrEmpty(tb_gia.Text) &&
                !string.IsNullOrEmpty(tb_soLuong.Text);
        }

        private void btn_sua1_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidProductData())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "UPDATE Products SET product_name = @ProductName, " +
                                                  "mo_ta = @MoTa, gia = @Gia, so_luong = @SoLuong " +
                                                  "WHERE product_id = @ProductId";
                            command.Parameters.AddWithValue("@ProductId", tb_productId.Text);
                            command.Parameters.AddWithValue("@ProductName", tb_productName.Text);
                            command.Parameters.AddWithValue("@MoTa", tb_moTa.Text);
                            command.Parameters.AddWithValue("@Gia", tb_gia.Text);
                            command.Parameters.AddWithValue("@SoLuong", tb_soLuong.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadProductData(); // Reload product data after updating a product
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message);
            }
        }

        private void btn_xoa1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tb_productId.Text))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "DELETE FROM Products WHERE product_id = @ProductId";
                            command.Parameters.AddWithValue("@ProductId", tb_productId.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    LoadProductData(); // Reload product data after deleting a product
                }
                else
                {
                    MessageBox.Show("Please provide a product ID to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message);
            }
        }

        private void btn_reset1_Click(object sender, EventArgs e)
        {
            // Clear textboxes for product data
            tb_productId.Text = string.Empty;
            tb_productName.Text = string.Empty;
            tb_moTa.Text = string.Empty;
            tb_gia.Text = string.Empty;
            tb_soLuong.Text = string.Empty;

            // Deselect the DataGridView row for product data
            dgv2.ClearSelection();
        }

        private void btn_xuat1_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a SaveFileDialog to prompt the user for the file location
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Export Product Data to Text File";
                    saveFileDialog.DefaultExt = "txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the chosen file path
                        string filePath = saveFileDialog.FileName;

                        // Open a StreamWriter to write to the text file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            // Write the header (column names)
                            for (int i = 0; i < dgv2.Columns.Count; i++)
                            {
                                writer.Write(dgv2.Columns[i].HeaderText);
                                if (i < dgv2.Columns.Count - 1)
                                    writer.Write("\t");
                            }
                            writer.WriteLine();

                            // Write the data
                            for (int i = 0; i < dgv2.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgv2.Columns.Count; j++)
                                {
                                    writer.Write(dgv2.Rows[i].Cells[j].Value);
                                    if (j < dgv2.Columns.Count - 1)
                                        writer.Write("\t");
                                }
                                writer.WriteLine();
                            }

                            MessageBox.Show("Product data exported successfully!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting product data: " + ex.Message);
            }
        }

        private void btn_timkh_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = tb_timkh.Text.Trim(); // Assuming tb_timkh is a TextBox for user input

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        // Assuming "first_name" and "customer_id" are the columns you want to search in
                        command.CommandText = "SELECT * FROM Customers WHERE customer_name LIKE @SearchKeyword OR customer_id LIKE @SearchKeyword";
                        command.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            dgv.DataSource = table;

                            if (table.Rows.Count == 0)
                            {
                                MessageBox.Show("No matching records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching for customers: " + ex.Message);
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btn_timsp_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = tb_timsp.Text.Trim(); // Assuming tb_timsp is a TextBox for user input

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        // Assuming "product_name" and "product_id" are the columns you want to search in
                        command.CommandText = "SELECT * FROM Products WHERE product_name LIKE @SearchKeyword OR product_id LIKE @SearchKeyword";
                        command.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            dgv2.DataSource = table;

                            if (table.Rows.Count == 0)
                            {
                                MessageBox.Show("No matching records found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching for products: " + ex.Message);
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
