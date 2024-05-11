using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
namespace bai2
{
    public partial class Timsp : Form
    {
        private readonly string connectionString = "Data Source=DESKTOP-636OIPO;Initial Catalog=QuanLyBanLatop;Integrated Security=True";

        public Timsp()
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
                        command.CommandText = "SELECT * FROM Orders";

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            dgv3.AutoGenerateColumns = true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.ToString());
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            // Replace "yourDataTable" with the actual DataTable variable
        }




        private void dgv3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
