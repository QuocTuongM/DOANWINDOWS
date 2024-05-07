using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; //Sử dụng thư viện để làm việc SQL server
using System.Runtime.InteropServices; //Sử dụng class 


namespace Quản_lý_Sản_Phẩm
{
    public partial class frmDMLaptop : Form
    {
        DataTable tblLT;
        public frmDMLaptop()
        {
            InitializeComponent();
        }

        private void dgvLaptop_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (btnThem.Enabled == false)
            {
                MessageBox.Show("Đang ở chế độ thêm mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMaLaptop.Focus();
                return;
            }
           // if (tblCL.Rows.Count == 0) //Nếu không có dữ liệu (có dòng này như thiếu database nên chạy ko được thêm vào rồi chạy)
            {
                MessageBox.Show("Không có dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txtMaLaptop.Text = dgvLaptop.CurrentRow.Cells["MaLaptop"].Value.ToString();
            txtTenLaptop.Text = dgvLaptop.CurrentRow.Cells["TenLaptop"].Value.ToString();
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnBoQua.Enabled = true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnBoQua.Enabled = true;
            btnLuu.Enabled = true;
            btnThem.Enabled = false;
            ResetValue(); //Xoá trắng các textbox
            txtMaLaptop.Enabled = true; //cho phép nhập mới
            txtMaLaptop.Focus();
        }
        private void ResetValue()
        {
            txtMaLaptop.Text = "";
            txtTenLaptop.Text = "";
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string sql; //Lưu lệnh sql
            if (txtMaLaptop.Text.Trim().Length == 0) //Nếu chưa nhập mã chất liệu
            {
                MessageBox.Show("Bạn phải nhập mã Laptop", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMaLaptop.Focus();
                return;
            }
            if (txtTenLaptop.Text.Trim().Length == 0) //Nếu chưa nhập tên chất liệu
            {
                MessageBox.Show("Bạn phải nhập tên Laptop", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTenLaptop.Focus();
                return;
            }
            sql = "Select MaLaptop From tblLaptop where MaLaptop=N'" + txtMaLaptop.Text.Trim() + "'";
            //if (Class.Functions.CheckKey(sql))
            //{
            //    MessageBox.Show("Mã chất liệu này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           //     txtMaChatLieu.Focus();
           //     return;
           // }

            sql = "INSERT INTO tblDMLaptop VALUES(N'" +
                txtMaLaptop.Text + "',N'" + txtTenLaptop.Text + "')";
           // Class.Functions.RunSQL(sql); //Thực hiện câu lệnh sql
           // LoadDataGridView(); //Nạp lại DataGridView
            ResetValue();
            btnXoa.Enabled = true;
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnBoQua.Enabled = false;
            btnLuu.Enabled = false;
            txtMaLaptop.Enabled = false;
        }
        //Hàm thực hiện câu lệnh SQL
        public static void RunSQL(string sql)
        {
            SqlCommand cmd; //Đối tượng thuộc lớp SqlCommand
            cmd = new SqlCommand();
            cmd.CommandText = sql; //Gán lệnh SQL
            try
            {
                cmd.ExecuteNonQuery(); //Thực hiện câu lệnh SQL
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            cmd.Dispose();//Giải phóng bộ nhớ
            cmd = null;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string sql; //Lưu câu lệnh sql
            if (tblLT.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtMaLaptop.Text == "") //nếu chưa chọn bản ghi nào
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtTenLaptop.Text.Trim().Length == 0) //nếu chưa nhập tên chất liệu
            {
                MessageBox.Show("Bạn chưa nhập tên chất liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            sql = "UPDATE tblLaptop SET TenLaptop=N'" +
                txtTenLaptop.Text.ToString() +
                "' WHERE MaLaptop=N'" + txtTenLaptop.Text + "'";

            ResetValue();

            btnBoQua.Enabled = false;
        }

        private void frmDMLaptop_Load(object sender, EventArgs e)
        {
            txtMaLaptop.Enabled = false;
            btnLuu.Enabled = false;
            btnBoQua.Enabled = false;
            LoadDataGridView(); //Hiển thị bảng tblChatLieu
        }
        private void LoadDataGridView()
        {
            string sql;
            sql = "SELECT MaLaptop, TenLaptop FROM tblLaptop";
            tblLT =  //Đọc dữ liệu từ bảng
            dgvLaptop.DataSource = tblLT; //Nguồn dữ liệu            
            dgvLaptop.Columns[0].HeaderText = "Mã Latop";
            dgvLaptop.Columns[1].HeaderText = "Mã Laptop";
            dgvLaptop.Columns[0].Width = 100;
            dgvLaptop.Columns[1].Width = 300;
            dgvLaptop.AllowUserToAddRows = false; //Không cho người dùng thêm dữ liệu trực tiếp
            dgvLaptop.EditMode = DataGridViewEditMode.EditProgrammatically; //Không cho sửa dữ liệu trực tiếp
        }
    }
}