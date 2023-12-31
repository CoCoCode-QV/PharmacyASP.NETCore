﻿create database QLPharmacy

use QlPharmacy 
go

create table Account(
	AccountID int identity Primary key,
	DisplayName nvarchar(100) not null,
	UserName nvarchar(100) not null,
	Password nvarchar(100) not null,
	Type int not null default 0
)	

ALTER TABLE dbo.Account
ALTER COLUMN UserName nvarchar(100) COLLATE SQL_Latin1_General_CP1_CS_AS;
ALTER TABLE dbo.Account
ALTER COLUMN Password nvarchar(100) COLLATE SQL_Latin1_General_CP1_CS_AS;

create table Supplier(
	SupplierID int identity Primary key,
	SupplierName nvarchar(500),
	SupplierEmail nvarchar(100),
	SupplierPhone nvarchar(20),
	SupplierAddress nvarchar(500)
)

create table Customer (
	CustomerID int identity Primary key,
	CustomerName nvarchar(100),
	CustomerPhone nvarchar(20),
	CustomerAddress nvarchar(20),
	CustomerEmail nvarchar(100),
	CustomerSex int
)

alter table Customer add CustomerAge int, CustomerAllergies nvarchar(200)

create table Category(
	CategoryID int IDENTITY PRIMARY KEY,
	CategoryName nvarchar(100) not null default N'Chưa đặt tên'
)

Create table Discount(
	DiscountID int identity primary key,
	DiscountPercent float default 0
)
alter table Discount add
DiscountName nvarchar(100),
DiscountStartDate date,
DiscountEndDate date

create table Product(
	ProductID int Identity primary key,
	ProductName nvarchar(500),
	ProductPrice float not null default 0,
	ProductDetail nvarchar(max),
	ProductImage nvarchar(500),
	ProductInventory float not null default 0,
	CategoryID int not null,
	SupplierID int not null,
	DiscountID int not null,
	foreign key (CategoryID) references dbo.Category(CategoryID),
	foreign key (SupplierID) references dbo.Supplier(SupplierID),
	foreign key (DiscountID) references dbo.Discount(DiscountID)
)

alter table Product add ProductExpiryDate Date, ProductIngredients nvarchar(200), ProductPrescription bit, ProductActive Bit

create table Cart(
	CartID int identity primary key,
	CartTotalPrice float default 0,
	CustomerID int not null,
	foreign key (CustomerID) references dbo.Customer(CustomerID)
)

create table CartDetail(
	CartDetailID int identity primary key,
	CartDetailQuantity int default 0,
	CartDetailTemporaryPrice float default 0,
	CartID int not null,
	ProductID int not null,
	foreign key (CartID) references dbo.Cart(CartID),
	foreign key (ProductID) references dbo.Product(ProductID)
)
ALTER TABLE CartDetail
ADD CartDetailPriceCurrent float default 0;

create table Orders (
	OrderID int identity primary key,
	OrderDate datetime  NOT NULL DEFAULT GETDATE(),
	OrderAddress nvarchar(500),
	OrderStatus int not null default 0,
	CustomerID int not null,
	Foreign key (CustomerID)references dbo.Customer(CustomerID)
)

create table OrderDetails(
	OrderDetails int identity primary key,
	OrderDetailsQuantity int default 0,
	OrderDetailsPrice float default 0,
	OrderID int not null,
	ProductID int not null,
	Foreign key (OrderID) references dbo.Orders(OrderID),
	Foreign key (ProductID) references dbo.Product(ProductID)
)
ALTER TABLE OrderDetails
ADD OrderDetailsTemporaryPrice  float default 0;


--ACCOUNT
INSERT INTO dbo.Account(userName,displayName,password,Type)
VALUES(
	N'Admin',
	N'Admin',
	N'Admin123',
	1
)
select * from Account

INSERT INTO dbo.Account(userName,displayName,password,Type)
VALUES(
	N'Staff',
	N'Staff',
	N'Staff123',
	0
)
--supplier
INSERT INTO dbo.Supplier(SupplierName,SupplierEmail, SupplierPhone, SupplierAddress)
VALUES(
	N'GH',
	N'GHPharmacy@gmail.com',
	N'0906422111',
	N'Nhà bè, TP.HCM'
)

INSERT INTO dbo.Category(CategoryName)
VALUES(
	N'Dược phẩm'
)

INSERT INTO dbo.Discount(DiscountName ,DiscountPercent, DiscountStartDate, DiscountEndDate)
VALUES(
	N'GH',
	10,
	GETDATE(),
	GETDATE()
	
)

select * from Discount

select * from Category

select * from Supplier
