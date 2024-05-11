using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace donhang
{
    public partial class TTDonHang : Form
    {
        private readonly string connectionString = "Data Source=DESKTOP-636OIPO;Initial Catalog=QuanLyBanLatop;Integrated Security=True";

        public TTDonHang()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Orders", connection))
                    {
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

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell content click event if needed
            // You can remove this method if it's not used
        }

        private void btn_them_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsValidOrderData())
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Orders (customer_id, user_id, customer_name, product_name, ngay_mua, so_luong, dia_chi, phone, email, tong_tien) " +
                                                  "VALUES (@CustomerId, @UserId, @CustomerName, @ProductName, @NgayMua, @SoLuong, @DiaChi, @Phone, @Email, @TongTien)";

                            command.Parameters.AddWithValue("@CustomerId", tb_ci.Text);
                            command.Parameters.AddWithValue("@UserId", tb_user.Text);
                            command.Parameters.AddWithValue("@CustomerName", tb_kh.Text);
                            command.Parameters.AddWithValue("@ProductName", tb_sp.Text);
                            command.Parameters.AddWithValue("@SoLuong", tb_sl.Text); // Use Text property here
                            command.Parameters.AddWithValue("@NgayMua", DateTime.Now);
                            command.Parameters.AddWithValue("@DiaChi", tb_dc.Text);
                            command.Parameters.AddWithValue("@Phone", tb_phone.Text);
                            command.Parameters.AddWithValue("@Email", tb_email.Text);
                            command.Parameters.AddWithValue("@TongTien", tb_tong.Text); // Change this based on your requirements

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
                MessageBox.Show("Error adding order: " + ex.Message);
            }
        }

        private bool IsValidOrderData()
        {
            // Add your validation logic here
            // Ensure that the required fields are filled and have valid data
            // Return true if the data is valid, otherwise return false
            return true;
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected
                if (dgv.SelectedRows.Count > 0)
                {
                    // Get the selected row index
                    int rowIndex = dgv.SelectedRows[0].Index;

                    // Assuming you have a unique identifier for each order, e.g., orderId
                    
                    string customerId = dgv.Rows[rowIndex].Cells["customer_id"].Value.ToString();

                    // Enable editing for each field individually
                    // Assuming tb_orderId is your order_id TextBox
                    tb_ci.Enabled = false; // Assuming tb_customer_id is your customer_id TextBox
                                                    // Enable or disable other fields as needed

                    // Populate controls with data from the selected row
                    
                    tb_ci.Text = customerId;
                    tb_user.Text = dgv.Rows[rowIndex].Cells["user_id"].Value.ToString();
                    tb_kh.Text = dgv.Rows[rowIndex].Cells["customer_name"].Value.ToString();
                    tb_sp.Text = dgv.Rows[rowIndex].Cells["product_name"].Value.ToString();
                    tb_sl.Text = dgv.Rows[rowIndex].Cells["so_luong"].Value.ToString();
                    tb_dc.Text = dgv.Rows[rowIndex].Cells["dia_chi"].Value.ToString();
                    tb_phone.Text = dgv.Rows[rowIndex].Cells["phone"].Value.ToString();
                    tb_email.Text = dgv.Rows[rowIndex].Cells["email"].Value.ToString();
                    tb_tong.Text = dgv.Rows[rowIndex].Cells["tong_tien"].Value.ToString();

                    // Now, you can enable editing or perform any necessary adjustments

                    // ... (rest of your update logic)
                }
                else
                {
                    MessageBox.Show("Please select a row in the data grid view to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Assuming you have a unique identifier for each customer, e.g., customerId
                string customerId = tb_ci.Text; // Use your actual textbox or control for customer id

                // Ask for confirmation before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete all orders for this customer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Perform the deletion in the database
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "DELETE FROM Orders WHERE customer_id = @CustomerId";
                            command.Parameters.AddWithValue("@CustomerId", customerId);

                            command.ExecuteNonQuery();
                        }
                    }

                    // Remove the related rows from the DataGridView
                    for (int i = dgv.Rows.Count - 1; i >= 0; i--)
                    {
                        // Check if the cell and its value are not null before accessing
                        if (dgv.Rows[i].Cells["customer_id"] != null && dgv.Rows[i].Cells["customer_id"].Value != null &&
                            dgv.Rows[i].Cells["customer_id"].Value.ToString() == customerId)
                        {
                            dgv.Rows.RemoveAt(i);
                        }
                    }

                    MessageBox.Show("All orders for this customer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            // Reset the text of all TextBoxes
           
            tb_ci.Text = "";
            tb_user.Text = "";
            tb_kh.Text = "";
            tb_sp.Text = "";
            tb_sl.Text = "";
            tb_dc.Text = "";
            tb_phone.Text = "";
            tb_email.Text = "";
            tb_tong.Text = "";

            // Optionally, re-enable any disabled TextBoxes
            
            tb_ci.Enabled = true;
            // Enable or disable other TextBoxes as needed
        }

        private void btn_tim_Click(object sender, EventArgs e)
        {
            // Get the search keyword from TextBoxes or any other controls
            string orderCodeKeyword = tb_timdh.Text;
            string productNameKeyword = tb_sp.Text;

            // Specify the columns to search in
            string orderCodeColumn = "order_id";
            string productNameColumn = "product_name";

            // Iterate through the rows and find the first occurrence of the search keyword in either column
            DataGridViewRow foundRow = null;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if ((row.Cells[orderCodeColumn].Value != null &&
                    row.Cells[orderCodeColumn].Value.ToString().Equals(orderCodeKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (row.Cells[productNameColumn].Value != null &&
                    row.Cells[productNameColumn].Value.ToString().Equals(productNameKeyword, StringComparison.OrdinalIgnoreCase)))
                {
                    foundRow = row;
                    break;
                }
            }

            // Check if a matching row is found
            if (foundRow != null)
            {
                // Select the found row and bring it into view
                dgv.ClearSelection();
                foundRow.Selected = true;
                dgv.CurrentCell = foundRow.Cells[0];
                dgv.FirstDisplayedScrollingRowIndex = foundRow.Index;
            }
            else
            {
                MessageBox.Show("No matching record found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_xuatfile_Click(object sender, EventArgs e)
        {
            try
            {
                // Open a SaveFileDialog to choose the file location
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveFileDialog.Title = "Save Text File";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Open a stream writer to write the TSV content
                        using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8)) // Specify UTF-8 encoding
                        {
                            // Write headers
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                sw.Write(dgv.Columns[i].HeaderText);
                                if (i < dgv.Columns.Count - 1)
                                    sw.Write("\t"); // Use tab as the delimiter
                            }
                            sw.WriteLine();

                            // Write data
                            for (int i = 0; i < dgv.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgv.Columns.Count; j++)
                                {
                                    // Handle Vietnamese time format if the column represents time
                                    if (dgv.Columns[j].DefaultCellStyle.Format == "dd/MM/yyyy HH:mm:ss")
                                    {
                                        DateTime dateTime;
                                        if (DateTime.TryParse(dgv.Rows[i].Cells[j].Value?.ToString(), out dateTime))
                                        {
                                            sw.Write(dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
                                        }
                                    }
                                    else
                                    {
                                        sw.Write(dgv.Rows[i].Cells[j].Value?.ToString());
                                    }

                                    if (j < dgv.Columns.Count + 1)
                                        sw.Write("\t"); // Use tab as the delimiter
                                }
                                sw.WriteLine();
                            }
                        }

                        MessageBox.Show("Data exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
