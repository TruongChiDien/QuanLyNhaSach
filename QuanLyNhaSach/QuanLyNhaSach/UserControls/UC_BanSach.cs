﻿using QuanLyNhaSach.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuanLyNhaSach.DA;
using DGVPrinterHelper;
namespace QuanLyNhaSach.UserControls
{
    public partial class UC_BanSach : UserControl
    {
        #region Properties
        int n = 0;
        YesNo msb = new YesNo();
        int tienthu;
        int total = 0;
        int soluong;

        #endregion

        #region Method
        public UC_BanSach()
        {
            InitializeComponent();
            Grid_tb_loadData();

        }

        private void Grid_tb_loadData()
        {
            string query = "SELECT MaSach as 'Mã sách', TenSach as 'Tên sách',TheLoai as 'Thể loại',TacGia as 'Tác giả',DGBan as 'DG bán',SoLuong as 'Số lượng' FROM SACH";

            dtgvSach.DataSource = DataProvider.Instance.ExecuteQuery(query);
        }

        private void Search_book()
        {

            string chude = "MaSach";
            switch (CbChude.SelectedIndex)
            {
                case -1:
                    MessageBox.Show("Chọn chủ dề!");
                    return;
                case 1:
                    chude = "TenSach";

                    break;
                case 2:
                    chude = "TheLoai";

                    break;
                case 3:
                    chude = "TacGia";

                    break;
            }
            try
            {

                string query = "SELECT * FROM SACH WHERE + " + chude + " like '" + TxTimkiem.Text + "%'";

                dtgvSach.DataSource = DataProvider.Instance.ExecuteQuery(query);
            }
            catch (Exception)
            {

            }
        }

        private void Print_HoaDon()
        {
            if(txtMaKH.Text == "")
            {
                msb.Messageshow("Nhập mã khách hàng!");
                return;
            }
            string query = string.Format("select HoTenKH from khachhang where MaKH = {0}", txtMaKH.Text);
            string tenKH = DataProvider.Instance.ExecuteScalar(query).ToString();
            if (tenKH == "")
            {
                msb.Messageshow("Khách hàng không tồn tại!");
                return;
            }

            query = string.Format("update ctcongno set PhatSinh += {0}, NoCuoi += {1} where MaKH = {2} and month(Thoigian) = {3} and year(ThoiGian) = {4}", txtTongcong.Text, txtTongcong.Text, txtMaKH.Text, DateTime.Now.Month, DateTime.Now.Year);
            DataProvider.Instance.ExecuteNonQuery(query);
            query = string.Format("update khachhang set NoKH += {0} where MaKH = {1} ", txtTongcong.Text, txtMaKH.Text);
            DataProvider.Instance.ExecuteNonQuery(query);

            DGVPrinter printer = new DGVPrinter();
            printer.Title = "HÓA ĐƠN";
            printer.SubTitle = string.Format("Tên khách hàng: {0}                  Ngày lập hóa đơn: {1}/{2}/{3}", tenKH, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            printer.SubTitleAlignment = StringAlignment.Near;
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.PrintPreviewDataGridView(dtgvHoaDon);
        }

        public void PrintPhieuThu()
        {

            tienthu = Convert.ToInt32(txtTongcong.Text);
            string query = string.Format("select * from ctcongno where MaKH = {0} and month(ThoiGian) = {1} and year(ThoiGian) = {2}", Convert.ToInt32(txtMaKH.Text), DateTime.Now.Month, DateTime.Now.Year);
            DataTable dtt = DataProvider.Instance.ExecuteQuery(query);
            int NoCuoi = Convert.ToInt32(dtt.Rows[0]["NoCuoi"]);
            int PhatSinh = Convert.ToInt32(dtt.Rows[0]["PhatSinh"]);
            while (tienthu > NoCuoi)
            {
                msb.Messageshow("Số tiền thu không được phép lớn hơn số tiền nợ!");
                Dialog_ThuTien t_thutien1 = new Dialog_ThuTien();
                t_thutien1.ShowDialog();
                if (t_thutien1.Ok == false) return;
                tienthu = t_thutien1.Tienthu;
            }
            query = string.Format("update ctcongno set PhatSinh = {0}, NoCuoi = {1} where MaKH = {2} and month(ThoiGian) = {3} and year(ThoiGian) = {4}", PhatSinh - tienthu, NoCuoi - tienthu, Convert.ToInt32(txtMaKH.Text), DateTime.Now.Month, DateTime.Now.Year);
            DataProvider.Instance.ExecuteNonQuery(query);
            query = string.Format("update khachhang set NoKH = {0} where MaKH = {1}", NoCuoi - tienthu, Convert.ToInt32(txtMaKH.Text));
            DataProvider.Instance.ExecuteNonQuery(query);

            this.prtDoc.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 970, 234);
            this.prtprvDoc.Document = this.prtDoc;
            if (this.prtprvDoc.ShowDialog() == DialogResult.OK)
                this.prtDoc.Print();
        }

        private void SetFontAndColors()
        {
            dtgvSach.Font = new Font("Tahoma", 15);
            dtgvSach.ForeColor = Color.Blue;
            dtgvSach.BackColor = Color.Beige;

        }
        void showTotal()
        {
            int total = 0;
            for (int i = 0; i < dtgvHoaDon.Rows.Count; i++)
            {
                total += Convert.ToInt32(dtgvHoaDon.Rows[i].Cells[2].Value);
            }
            txtTongcong.Text = total.ToString();
        }



        #endregion

        #region Event

        private void txbTimKiem_TextChanged(object sender, EventArgs e)
        {
            Search_book();
            if (String.IsNullOrEmpty(txbSoluong.Text))
            {
                MessageBox.Show("Nhập số lượng sách", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //dataGridView1.Rows.Clear();
            }

        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            
            if (dtgvHoaDon.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào được chọn");
            }
            else
            {
                int qty = 0;

                foreach (DataGridViewRow row in dtgvHoaDon.Rows)
                {
                    qty = Convert.ToInt32((row.Cells["SL"].Value).ToString());
                    string t_sach = (row.Cells["Tensach"].Value).ToString();
                    
                    string query = string.Format("update SACH set SoLuong= SoLuong-{0} where TenSach={1}", qty, t_sach);
                    DataProvider.Instance.ExecuteNonQuery(query);
                }

                this.Print_HoaDon();
            }
        }

        private void dtgvHoaDon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.CurrentRow.Selected = true;
            //txtTensach.Text = dataGridView1.Rows[e.RowIndex].Cells["TenSach"].FormattedValue.ToString();
            //txtTacgia.Text = dataGridView1.Rows[e.RowIndex].Cells["TacGia"].FormattedValue.ToString();


        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txbSoluong.Text == "")
            {
                MessageBox.Show("Nhập số lượng");
                return;
            }
            else
            {
                try
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dtgvHoaDon);
                    newRow.Cells[0].Value = txtTensach.Text;
                    newRow.Cells[1].Value = txbSoluong.Text;
                    newRow.Cells[2].Value = txtGiatien.Text;

                    dtgvHoaDon.Rows.Add(newRow);
                    showTotal();
                }
                catch { }
            }
            //dataGridView2.Rows.Add(newRow);
         
        }

        private void dtgvSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
   
            int num_row = 0;
            for (int i = 1; i < dtgvSach.Rows.Count; i++)
            {
                num_row=num_row+1;
            }
            if (e.RowIndex == 0)
            {
                return;
            }
            if (e.RowIndex > 0&& e.RowIndex<num_row)
            {
                dtgvSach.CurrentRow.Selected = true;
                txtTensach.Text = dtgvSach.Rows[e.RowIndex].Cells["Tên sách"].FormattedValue.ToString();
                txtTacgia.Text = dtgvSach.Rows[e.RowIndex].Cells["Tác giả"].FormattedValue.ToString();
                txtTheloai.Text = dtgvSach.Rows[e.RowIndex].Cells["Thể loại"].FormattedValue.ToString();
                txtGiatien.Text = dtgvSach.Rows[e.RowIndex].Cells["DG bán"].FormattedValue.ToString();
                soluong = Convert.ToInt32(dtgvSach.Rows[e.RowIndex].Cells["DG bán"].FormattedValue.ToString());
                if (txbSoluong.Text == "")
                {
                    return;
                }
                total = Convert.ToInt32(txbSoluong.Text) * Convert.ToInt32(txtGiatien.Text);
                txtTongtien.Text = Convert.ToString(total);
            }
            else
            {
                return;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            int rowIndex = dtgvHoaDon.CurrentCell.RowIndex;
            dtgvHoaDon.Rows.RemoveAt(rowIndex);
        }

        private void txbSoluong_TextChanged(object sender, EventArgs e)
        {
            if (txbSoluong.Text != "")
            {
                if (Convert.ToInt32(txbSoluong.Text) > soluong)
                {
                    MessageBox.Show("Số lượng hàng không đủ");
                }
                else
                {
                    total = Convert.ToInt32(txbSoluong.Text) * Convert.ToInt32(txtGiatien.Text);
                }
            }
          
            txtTongtien.Text = Convert.ToString(total);

        }

        private void txtSoluong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void dtgvHoaDon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewCell oneCell in dtgvHoaDon.SelectedCells)
            {
                if (oneCell.Selected)
                    dtgvHoaDon.Rows.RemoveAt(oneCell.RowIndex);
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (dtgvHoaDon.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có sản phẩm nào được chọn");
            }
            else
            {
                this.Print_HoaDon();
                this.PrintPhieuThu();
            }
            
        }

        private void prtDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string query = string.Format("select * from khachhang where MaKH  = {0}", txtMaKH.Text);
            DataTable t = DataProvider.Instance.ExecuteQuery(query);
            string TenKH = t.Rows[0]["HoTenKH"].ToString();
            string DiaChi = t.Rows[0]["DiaChi"].ToString();
            string DienThoai = t.Rows[0]["DienThoai"].ToString();
            string Email = t.Rows[0]["Email"].ToString();

            e.Graphics.DrawRectangle(new Pen(Brushes.Black), new Rectangle(12, 9, 926, 50));
            e.Graphics.DrawString("PHIẾU THU TIỀN", new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(376, 26));
            e.Graphics.DrawString("Họ tên khách hàng: " + TenKH, new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(32, 86));
            e.Graphics.DrawString("Địa chỉ: " + DiaChi, new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(533, 86));
            e.Graphics.DrawString("Điện thoại: " + DienThoai, new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(32, 124));
            e.Graphics.DrawString("Email: " + Email, new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(533, 124));
            e.Graphics.DrawString("Ngày thu tiền: " + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(32, 170));
            e.Graphics.DrawString("Số tiền thu: " + tienthu.ToString(), new Font("Times New Roman", 14, FontStyle.Regular), Brushes.Black, new Point(533, 170));

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            dtgvHoaDon.Rows.Clear();
        }
    }
    #endregion

}
