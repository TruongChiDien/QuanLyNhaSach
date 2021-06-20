﻿CREATE DATABASE QLNS
GO

USE QLNS
GO

CREATE TABLE NHANVIEN (
	MaNV INT IDENTITY PRIMARY KEY,
	HoTenNV NVARCHAR(100),
	DiaChi NVARCHAR(100),
	DienThoai VARCHAR(13),
	Email VARCHAR(50),
	NgayVaoLam DATETIME
)
GO


CREATE TABLE SACH (
	MaSach INT IDENTITY PRIMARY KEY,
	TenSach NVARCHAR(100),
	TheLoai NVARCHAR(100),
	TacGia NVARCHAR(100),
	DGNhap INT
)
GO



CREATE TABLE PHIEUNHAP (
	MaPN INT IDENTITY PRIMARY KEY,
	MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
	NgayNhap DATETIME
)
GO

CREATE TABLE CTPHIEUNHAP (
	MaCTPN INT IDENTITY PRIMARY KEY,
	MaPN INT FOREIGN KEY REFERENCES PHIEUNHAP(MaPN),
	MaSach INT FOREIGN KEY REFERENCES SACH(MaSach),
	SL INT
)
GO

CREATE TABLE THAMSO (
	MaTS INT IDENTITY PRIMARY KEY,
	TenTS VARCHAR(100),
	GiaTri INT
)
GO

CREATE TABLE HOADON (
	MaHD INT IDENTITY PRIMARY KEY,
	MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
	NgayLapHD DATETIME
)
GO

CREATE TABLE CTHOADON (
	MaCTHD INT IDENTITY PRIMARY KEY,
	MaHD INT FOREIGN KEY REFERENCES HOADON(MaHD),
	MaSach INT FOREIGN KEY REFERENCES SACH(MaSach),
	SL INT
)
GO

CREATE TABLE KHACHHANG (
	MaKH INT IDENTITY PRIMARY KEY,
	HoTenKH NVARCHAR(100),
	DiaChi NVARCHAR(100),
	DienThoai TINYINT,
	Email VARCHAR(50),
	NoKH INT
)
GO

CREATE TABLE PHIEUTHU (
	MaPhieuThu INT IDENTITY PRIMARY KEY,
	MaKH INT FOREIGN KEY REFERENCES KHACHHANG(MaKH),
	MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
	NgayThu DATETIME,
	SoTienThu INT
)
GO

CREATE TABLE TONKHO (
	MaTon INT IDENTITY PRIMARY KEY,
	MaNV INT FOREIGN KEY REFERENCES NHANVIEN(MaNV),
	ThoiGian DATETIME
)
GO

CREATE TABLE CTTONKHO (
	MaCTTK INT IDENTITY PRIMARY KEY,
	MaTon INT FOREIGN KEY REFERENCES TONKHO (MaTon),
	MaSach INT FOREIGN KEY REFERENCES SACH (MaSach),
	TonDau INT,
	PhatSinh INT,
	TonCuoi INT
)
GO

CREATE TABLE CONGNO (
	MaNo INT IDENTITY PRIMARY KEY,
	ThoiGian DATETIME
)
GO

CREATE TABLE CTCONGNO (
	MaCTCN INT IDENTITY PRIMARY KEY,
	MaNo INT FOREIGN KEY REFERENCES CONGNO (MaNo),
	MaKH INT FOREIGN KEY REFERENCES KHACHHANG (MaKH),
	NoDau INT,
	PhatSinh INT,
	NoCuoi INT
)
GO




INSERT INTO THELOAI (MaTL, TenTL)
VALUES(1, N'Lập trình')
INSERT INTO THELOAI (MaTL, TenTL)
VALUES(2, N'Giáo dục')
GO

INSERT INTO TACGIA (MaTG, TenTG)
VALUES(1, N'Trương Chí Diễn')
INSERT INTO TACGIA (MaTG, TenTG)
VALUES(2, N'Trần Hoàn Đức Duy')
INSERT INTO TACGIA (MaTG, TenTG)
VALUES(3, N'Nguyễn Anh Dũng')
INSERT INTO TACGIA (MaTG, TenTG)
VALUES(4, N'Lê Anh Nhân')
GO

INSERT INTO SACH (MaSach, TenSach, MaTL, MaTG, DGNhap)
VALUES(1, N'Python', 1, 1, 10000)
INSERT INTO SACH (MaSach, TenSach, MaTL, MaTG, DGNhap)
VALUES(2, N'Java', 1, 2, 15000)
INSERT INTO SACH (MaSach, TenSach, MaTL, MaTG, DGNhap)
VALUES(3, N'C++', 2, 3, 20000)
INSERT INTO SACH (MaSach, TenSach, MaTL, MaTG, DGNhap)
VALUES(4, N'C#', 2, 3, 25000)
INSERT INTO SACH (MaSach, TenSach, MaTL, MaTG, DGNhap)
VALUES(5, N'Golang', 1, 1, 100000)
GO

--DROP TABLE CTCONGNO;
--DROP TABLE CONGNO;
--DROP TABLE CTTONKHO;
--DROP TABLE TONKHO;
--DROP TABLE PHIEUTHU;
--DROP TABLE KHACHHANG
--DROP TABLE CTHOADON;
--DROP TABLE HOADON;
--DROP TABLE THAMSO;
--DROP TABLE CTPHIEUNHAP;
--DROP TABLE PHIEUNHAP;
--DROP TABLE SACH;
--DROP TABLE THELOAI;
--DROP TABLE TACGIA;
--DROP TABLE NHANVIEN;

--DELETE FROM SACH;
--DELETE FROM THELOAI;
--DELETE FROM TACGIA;