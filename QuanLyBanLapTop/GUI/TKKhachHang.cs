using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xuathoadon;

namespace In
{
    public partial class TKKhachHang : Form
    {
        private readonly string connectionString = "Data Source=DESKTOP-636OIPO;Initial Catalog=QuanLyBanLatop;Integrated Security=True";

        public TKKhachHang()
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

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Users", connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            // Set the DataSource for dgv
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



        private void dgv_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_dangky_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the required fields are filled
                if (!string.IsNullOrEmpty(tb_username.Text) && !string.IsNullOrEmpty(tb_password.Text)
                    && !string.IsNullOrEmpty(tb_fname.Text) && !string.IsNullOrEmpty(tb_email.Text))
                {
                    // Hash the password (you should use a secure hashing algorithm)
                   

                    // Insert user registration data into the database
                    using (SqlConnection connection = new SqlConnection(connectionString)) // Change 'str' to 'connectionString'
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Users (username, password, full_name, email) VALUES (@Username, @Password, @FullName, @Email)";
                            command.Parameters.AddWithValue("@Username", tb_username.Text);
                            command.Parameters.AddWithValue("@Password", tb_password.Text);
                            command.Parameters.AddWithValue("@FullName", tb_fname.Text);
                            command.Parameters.AddWithValue("@Email", tb_email.Text);

                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Đăng ký tài khoản thành công ", "Thông báo ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // You may want to clear the registration fields after a successful registration
                    tb_username.Text = string.Empty;
                    tb_password.Text = string.Empty;
                    tb_fname.Text = string.Empty;
                    tb_email.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Xin vui lòng điền đầy đủ thông tin vào các ô bắt buộc.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            // Clear the registration fields
            tb_username.Text = string.Empty;
            tb_password.Text = string.Empty;
            tb_fname.Text = string.Empty;
            tb_email.Text = string.Empty;
        }

        private void btn_dangnhap_Click(object sender, EventArgs e)
        {
            try
            {
                // Hardcoded username and hashed password (replace with your actual authentication logic)
                string hardcodedUsername = "admin";
                string hashedPassword = HashPassword("admin123"); // Hash the stored password

                // Get the entered username and password
                string enteredUsername = tb_username.Text;
                string enteredPassword = tb_password.Text;

                // Hash the entered password for comparison
                string hashedEnteredPassword = HashPassword(enteredPassword);

                // Check if the entered username and password match the hardcoded values
                if (enteredUsername == hardcodedUsername && hashedEnteredPassword == hashedPassword)
                {
                    MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Perform additional actions after successful login if needed

                    // Clear the login fields

                    /* CHUYEENR TIEP TRANG KE TIEP
                     * tb_username.Text = string.Empty;
                    tb_password.Text = string.Empty;
                    this.Hide();
                    SanPham sanPham = new SanPham();
                    sanPham.ShowDialog();
                    this.Close();*/
                }
                else
                {
                    MessageBox.Show("Đăng nhập thành công", "thông báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // HashPassword method to hash the password (use a secure hashing algorithm in production)
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }


        private void btn_reset1_Click(object sender, EventArgs e)
        {
            // Clear the registration fields
            tb_username.Text = string.Empty;
            tb_password.Text = string.Empty;
            
        }

        private void btn_tim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = tb_tim.Text.Trim(); // Assuming tb_tim is a TextBox for user input

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        // Assuming "first_name" and "user_id" are the columns you want to search in
                        command.CommandText = "SELECT * FROM Users WHERE username LIKE @SearchKeyword OR user_id = @UserId";
                        command.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");

                        // Try to parse the input as an integer for user_id
                        if (int.TryParse(searchKeyword, out int userId))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                        }
                        else
                        {
                            // If the input is not a valid integer, set the parameter to a value that won't match any user_id
                            command.Parameters.AddWithValue("@UserId", -1); // Adjust this default value based on your requirements
                        }

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
                MessageBox.Show("Error searching for users: " + ex.Message);
            }
        }


       

        private void btn_xoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a row is selected in the dgv
                if (dgv.SelectedRows.Count > 0)
                {
                    // Get the username from the selected row
                    string username = dgv.SelectedRows[0].Cells["username"].Value.ToString();

                    // Confirm with the user before deleting
                    DialogResult result = MessageBox.Show($"Are you sure you want to delete the user '{username}'?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Delete the user from the database
                        DeleteUser(username);

                        // Refresh the data in the DataGridView
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a user from the grid before deleting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser(string username)
        {
            // Implement the deletion of the user in the database based on your schema
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM Users WHERE username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void tb_doigmail_Click(object sender, EventArgs e)
        {
            try
            {
                // Assuming "user_id" is the unique identifier for the user
                int userId = GetUserId(); // Replace this with your logic to get the user ID

                // Assuming "newEmail" is the new email you want to set
                string newEmail = GetNewEmail(); // Replace this with your logic to get the new email

                // Update the user's email in the database
                UpdateUserEmail(userId, newEmail); // Replace this with your logic to update the email

                MessageBox.Show("User email updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user email: " + ex.Message);
            }
        }

        // Example method to get the user ID (replace with your logic)
        private int GetUserId()
        {
            // Replace this with your logic to get the user ID
            return 123; // Example user ID
        }

        // Example method to get the new email (replace with your logic)
        private string GetNewEmail()
        {
            // Replace this with your logic to get the new email
            return "new.email@example.com"; // Example new email
        }

        // Example method to update the user's email (replace with your logic)
        private void UpdateUserEmail(int userId, string newEmail)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Users SET email = @NewEmail WHERE user_id = @UserId";
                    command.Parameters.AddWithValue("@NewEmail", newEmail);
                    command.Parameters.AddWithValue("@UserId", userId);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void btn_rw_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
