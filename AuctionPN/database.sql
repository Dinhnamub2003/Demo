drop database Auction
go
create database Auction
go
use Auction
go
create table Members (
	MemberId int identity(1,1) primary key,
	[Name] nvarchar(max) not null,
	[Address] nvarchar(max) null,
	Email varchar(max) null,
	Expirationdate datetime null,
	[Password] varchar(max) not null
)
go
create table ItemTypes (
	ItemTypeID int identity(1,1) primary key,
	ItemTypeName nvarchar(max) not null
)
go
create table Items (
	ItemID int identity(1,1) primary key,
	ItemName nvarchar(max) not null,
	ItemDescription nvarchar(max) not null,
	MinimumBidIncrement money not null,
	ItemImage nvarchar(max) not null,
	EndDataTime datetime not null,
	CurrentPrice money not null,
	ItemTypeID int foreign key references ItemTypes(ItemTypeID),
	SellerID int foreign key references Members(MemberId)
)
go
create table Bids (
	BidID int identity(1,1) primary key,
	BidDataTime datetime not null,
	BidPrice money null,
	ItemID int foreign key references Items(ItemID),
	BidderID int foreign key references Members(MemberId)
)
go
INSERT INTO Members ([Name], [Address], Email, Expirationdate, [Password])
VALUES 
    ('John Doe', '123 Main St', 'john.doe@example.com', '2024-12-31', 'password1'),
    ('Alice Smith', '456 Oak St', 'alice.smith@example.com', '2024-12-31', 'password2'),
    ('Bob Johnson', '789 Pine St', 'bob.johnson@example.com', '2024-12-31', 'password3'),
    ('Eva Brown', '101 Maple St', 'eva.brown@example.com', '2024-12-31', 'password4'),
    ('Charlie Davis', '202 Cedar St', 'charlie.davis@example.com', '2024-12-31', 'password5'),
    ('Grace White', '303 Elm St', 'grace.white@example.com', '2024-12-31', 'password6'),
    ('David Wilson', '404 Birch St', 'david.wilson@example.com', '2024-12-31', 'password7'),
    ('Emma Miller', '505 Walnut St', 'emma.miller@example.com', '2024-12-31', 'password8');
go
INSERT INTO ItemTypes (ItemTypeName)
VALUES 
    ('Tài sản nhà nước'),
    ('Bất động sản'),
    ('Phương tiện - xe cộ'),
    ('Sưu tầm - nghệ thuật'),
    ('Hàng hiệu - xa xỉ'),
    ('Tang vật bị tịch thu'),
    ('Tài sản khác')
go
INSERT INTO Items (ItemName, ItemDescription, MinimumBidIncrement, EndDataTime, CurrentPrice, ItemTypeID,ItemImage ,SellerID)
VALUES 
    ('Desktop Computer', 'High-performance desktop computer', 50.00, '2024-04-15 18:00:00', 800.00, 1, 'bua.jpg', 1),
    ('Table Lamp', 'Modern and stylish table lamp', 10.00, '2024-04-15 19:00:00', 70.00, 1,  'bua.jpg',2),
    ('Coffee Maker', 'Automatic coffee maker for home use', 20.00, '2024-04-15 20:00:00', 150.00, 1,  'bua.jpg',3),
    ('Vacuum Cleaner', 'Efficient vacuum cleaner for household cleaning', 30.00, '2024-04-15 21:00:00', 200.00, 1, 'bua.jpg',4),
    ('Bluetooth Speaker', 'Portable speaker with wireless connectivity', 15.00, '2024-04-15 22:00:00', 120.00, 1, 'bua.jpg',5),
    ('Beachfront Villa', 'Luxurious villa with panoramic ocean views', 1000.00, '2024-04-15 18:00:00', 1000000.00, 2, 'bua.jpg', 1),
    ('Mountain Cabin', 'Cozy cabin nestled in the mountains', 500.00, '2024-04-15 19:00:00', 50000.00, 2,  'bua.jpg',2),
    ('City Apartment', 'Modern apartment in the heart of the city', 300.00, '2024-04-15 20:00:00', 300000.00, 2, 'bua.jpg',3),
    ('Lake House', 'Charming house overlooking a serene lake', 700.00, '2024-04-15 21:00:00', 70000.00, 2, 'bua.jpg',4),
    ('Rural Farmhouse', 'Quaint farmhouse surrounded by countryside', 400.00, '2024-04-15 22:00:00', 40000.00, 2,  'bua.jpg',5);

go
INSERT INTO Bids (BidDataTime, BidPrice, ItemID, BidderID)
VALUES 
    ('2024-03-31 18:30:00', 550.00, 1, 2),
    ('2024-03-31 20:45:00', 250.00, 2, 3),
    ('2024-03-31 22:15:00', 110.00, 3, 4),
    ('2024-03-31 19:30:00', 60.00, 4, 5),
    ('2024-03-31 21:45:00', 25.00, 5, 6),
    ('2024-03-31 23:15:00', 1050.00, 6, 7),
    ('2024-03-31 17:30:00', 160.00, 7, 8),
    ('2024-03-31 16:45:00', 550.00, 8, 1);
