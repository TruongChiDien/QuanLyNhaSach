﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using QuanLyNhaSach.DA;
using DGVPrinterHelper;

namespace QuanLyNhaSach.Forms
{
    public partial class TaoPhieuNhap : Form
    {

        #region Properties

        #endregion


        #region Methods
        public TaoPhieuNhap()
        {
            InitializeComponent();

            Load_grid_phieu_nhap();
            Grid_TaoPhieuNhap.Columns["Xoa"].DisplayIndex = 4;
        }

        private void Load_grid_phieu_nhap()
        {
            string query = @"select a.MaSach [Mã sách], a.SL [Số lượng], DGNhap [Đơn giá nhập], DGBan [Đơn giá bán]
                                from CTPHIEUNHAP a, SACH b
                                where MaPN=(select max(MaPN) from PHIEUNHAP) and a.MaSach=b.MaSach";
            Grid_TaoPhieuNhap.DataSource = DataProvider.Instance.ExecuteQuery(query);
        }

        #endregion


        #region Events
        private void button1_Click(object sender, EventArgs e)
        {
            string query = @"delete from SACH";
            DataProvider.Instance.ExecuteNonQuery(query);
            query = @"INSERT INTO SACH SELECT * FROM SACHPHU";
            DataProvider.Instance.ExecuteNonQuery(query);
            query = @"drop table SACHPHU";
            DataProvider.Instance.ExecuteNonQuery(query);
            query = @"delete from CTPHIEUNHAP where MaPN = (select max(MaPN) from PHIEUNHAP)
                    delete from PHIEUNHAP where MaPN = (select max(MaPN) from PHIEUNHAP)";
            DataProvider.Instance.ExecuteNonQuery(query);
            this.Dispose();
        }

        private void BtThemSach_Click(object sender, EventArgs e)
        {
            ThemSach ts = new ThemSach();
            ts.ShowDialog();
            Load_grid_phieu_nhap();
        }

        private void update_SachTonKho ( string MS,string SL)
        {
            string query = @"select TonDau from CTTONKHO where month(ThoiGian) = (select month(getdate()))" + " and MaSach = " + MS;
            object i = DataProvider.Instance.ExecuteScalar(query);
            if ( i == null)
            {
                query = @" insert into CTTONKHO values((select getdate()," + MS + ",0," + SL + "," + SL + ")";
                DataProvider.Instance.ExecuteNonQuery(query);
            }
            else
            {

            }
        }
        private void update_TonKho(string MaPN)
        {
            string query = @"select MaSach, SL from CTPHIEUNHAP where MaPN = " + MaPN;
            DataTable dt;
            dt = DataProvider.Instance.ExecuteQuery(query);
            foreach ( DataRow dr in dt.Rows )
            {
                update_SachTonKho(dr["MaSach"].ToString(), dr["SL"].ToString());
            }
        }
        private void BtXuatPhieu_Click(object sender, EventArgs e)
        {
            string query = @"select a.MaSach [Mã sách], a.SL [Số lượng], DGNhap [Đơn giá nhập], DGBan [Đơn giá bán]
                                from CTPHIEUNHAP a, SACH b
                                where MaPN=(select max(MaPN) from PHIEUNHAP) and a.MaSach=b.MaSach";

            FormTempForPrint temp = new FormTempForPrint();
            temp.Setdtgv(DataProvider.Instance.ExecuteQuery(query));
            query = "select max(MaPN) from PHIEUNHAP ";
            object MaPN = DataProvider.Instance.ExecuteScalar(query);
            DGVPrinter printer = new DGVPrinter();
            printer.Title = "Phiếu nhập - Mã: " + MaPN.ToString();
            query = "select NgayNhap as [GETDATE] from PHIEUNHAP where MaPN = (select max(MaPN) from PHIEUNHAP)";
            object Thoigian = DataProvider.Instance.ExecuteScalar(query);
            printer.SubTitle = "Thời gian: " + Thoigian.ToString();
            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            query = "select sum(SL * DGNhap) from SACH a, CTPHIEUNHAP b"
                    + " where MaPN = (select max(MaPN) from PHIEUNHAP) and a.MaSach = b.MaSach";
            object tong = DataProvider.Instance.ExecuteScalar(query);
            printer.Footer = "Tổng tiền nhập là: " + tong.ToString();
            printer.FooterAlignment = StringAlignment.Near;
            printer.PageNumbers = true;
            printer.PageNumberInHeader = false;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Near;
            printer.PrintPreviewDataGridView(temp.Getdtgv());
            //update_TonKho(MaPN.ToString());
            this.Dispose();
        }

        #endregion


    }
}
