-- Tạo cơ sở dữ liệu ASP6TL_DB
CREATE DATABASE ASP6TL_DB;
GO

-- Sử dụng cơ sở dữ liệu ASP6TL_DB
USE ASP6TL_DB
GO

-- Bảng vai trò (Roles)
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY, -- Mã vai trò, khóa chính
    RoleName NVARCHAR(50) -- Tên vai trò (ví dụ: 'Admin', 'Customer')
);

-- Bảng Admin (Admins)
CREATE TABLE Admins (
    AdminID INT PRIMARY KEY, -- Mã của Admin, khóa chính 
    AdminName NVARCHAR(100), -- Tên Admin
    Email NVARCHAR(100), -- Email của admin
    Password NVARCHAR(50), -- Mật khẩu của tài khoản admin
    Status NVARCHAR(50), -- Trạng thái
    RoleID INT, -- Mã vai trò (liên kết với bảng Roles)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo tài khoản admin
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật tài khoản admin
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) -- Khóa ngoại liên kết với bảng Roles
);

-- Bảng khách hàng (Customers)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY, -- Mã khách hàng, khóa chính
    FullName NVARCHAR(100), -- Tên đầy đủ của khách hàng
    Email NVARCHAR(100), -- Email của khách hàng, cần chỉ mục để tối ưu tìm kiếm
    Address NVARCHAR(200), -- Địa chỉ giao hàng của khách hàng
    PhoneNumber NVARCHAR(20), -- Số điện thoại của khách hàng
    Password NVARCHAR(50), -- Mật khẩu của tài khoản người dùng
    RoleID INT, -- Mã vai trò (liên kết với bảng Roles)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo tài khoản (mặc định là ngày hiện tại)
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật tài khoản
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) -- Khóa ngoại liên kết với bảng Roles
);

-- Bảng danh mục sản phẩm (Categories)
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY, -- Mã danh mục sản phẩm, khóa chính
    CategoryName NVARCHAR(100), -- Tên danh mục sản phẩm
    Description NVARCHAR(500), -- Mô tả danh mục sản phẩm
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo danh mục sản phẩm
    UpdatedAt DATETIME DEFAULT GETDATE() -- Ngày cập nhật danh mục sản phẩm
);

-- Bảng nhà cung cấp (Suppliers)
CREATE TABLE Suppliers (
    SupplierID INT PRIMARY KEY, -- Mã nhà cung cấp, khóa chính
    SupplierName NVARCHAR(100), -- Tên nhà cung cấp
    Email NVARCHAR(100), -- Email của nhà cung cấp
    PhoneNumber NVARCHAR(20), -- Số điện thoại của nhà cung cấp
    Address NVARCHAR(200), -- Địa chỉ nhà cung cấp
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo nhà cung cấp
    UpdatedAt DATETIME DEFAULT GETDATE() -- Ngày cập nhật nhà cung cấp
);

-- Bảng sản phẩm (Products)
CREATE TABLE Products (
    ProductID INT PRIMARY KEY, -- Mã sản phẩm, khóa chính
    ProductName NVARCHAR(100), -- Tên sản phẩm
    Image NVARCHAR(100), -- Hình ảnh sản phẩm
    Material NVARCHAR(100), -- Chất liệu của sản phẩm
    Color NVARCHAR(50), -- Màu sắc của sản phẩm
    Brand NVARCHAR(100), -- Thương hiệu của sản phẩm
    WarrantyPeriod INT, -- Thời gian bảo hành
    ProductDescription NVARCHAR(1000), -- Mô tả chi tiết sản phẩm
    Quantity INT, -- Số lượng sản phẩm hiện tại
    Price DECIMAL(10, 2), -- Giá của sản phẩm
    DiscountAmount DECIMAL(10, 2) NULL, -- Giá trị giảm giá (có thể là số tiền hoặc phần trăm)
    DiscountStartDate DATETIME NULL, -- Ngày bắt đầu giảm giá
    DiscountEndDate DATETIME NULL, -- Ngày kết thúc giảm giá
    SupplierID INT, -- Mã nhà cung cấp
    CategoryID INT, -- Mã danh mục sản phẩm (liên kết với bảng Categories)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo chi tiết sản phẩm
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật chi tiết sản phẩm
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID), -- Khóa ngoại liên kết với bảng Categories
    FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID) -- Khóa ngoại liên kết với bảng Suppliers
);

-- Bảng đơn hàng (Orders)
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY, -- Mã đơn hàng, khóa chính
    CustomerID INT, -- Mã khách hàng (liên kết với bảng Customers)
    CustomerName NVARCHAR(100), -- Tên khách hàng
    OrderDate DATETIME DEFAULT GETDATE(), -- Ngày đặt hàng (mặc định là ngày hiện tại)
    OrderStatus NVARCHAR(50), -- Trạng thái đơn hàng (ví dụ: Đang xử lý, Đã giao)
    TotalAmount DECIMAL(10, 2), -- Tổng giá trị đơn hàng
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo đơn hàng
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật đơn hàng
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) -- Khóa ngoại liên kết với bảng Customers
);

-- Bảng chi tiết đơn hàng (OrderDetails)
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY, -- Mã chi tiết đơn hàng
    OrderID INT, -- Mã đơn hàng (liên kết với bảng Orders)
    ProductID INT, -- Mã sản phẩm (liên kết với bảng Products)
    ProductName NVARCHAR(100), -- Tên sản phẩm
    Quantity INT, -- Số lượng sản phẩm
    UnitPrice DECIMAL(10, 2), -- Giá đơn vị của sản phẩm tại thời điểm đặt hàng
    Total DECIMAL(10, 2), -- Tổng tiền cho sản phẩm này (Quantity * UnitPrice)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo chi tiết đơn hàng
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật chi tiết đơn hàng
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID), -- Khóa ngoại liên kết với bảng Orders
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) -- Khóa ngoại liên kết với bảng Products
);

-- Bảng sản phẩm yêu thích (Wishlist)
CREATE TABLE Wishlist (
    WishlistID INT PRIMARY KEY, -- Mã danh sách yêu thích, khóa chính
    CustomerID INT, -- Mã khách hàng (liên kết với bảng Customers)
    ProductID INT, -- Mã sản phẩm (liên kết với bảng Products)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày thêm sản phẩm vào danh sách yêu thích
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật sản phẩm yêu thích
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID), -- Khóa ngoại liên kết với bảng Customers
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) -- Khóa ngoại liên kết với bảng Products
);

-- Bảng mã giảm giá (Discounts)
CREATE TABLE Discounts (
    DiscountID INT PRIMARY KEY, -- Mã giảm giá, khóa chính
    DiscountCode NVARCHAR(50), -- Mã giảm giá (ví dụ: SUMMER2024)
    DiscountAmount DECIMAL(10, 2), -- Giá trị giảm giá (có thể là số tiền hoặc phần trăm)
    StartDate DATETIME, -- Ngày bắt đầu giảm giá
    EndDate DATETIME, -- Ngày kết thúc giảm giá
    Active BIT, -- Trạng thái giảm giá (1: hoạt động, 0: không hoạt động)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo mã giảm giá
    UpdatedAt DATETIME DEFAULT GETDATE() -- Ngày cập nhật mã giảm giá
);

-- Bảng Blog (Blogs)
CREATE TABLE Blogs (
    BlogID INT PRIMARY KEY, -- Mã của Blog, khóa chính 
    BlogTitle NVARCHAR(100), -- Tiêu đề Blog
    BlogContent TEXT, -- Nội dung blog
    AdminID INT, -- Mã của Admin (liên kết với bảng Admins)
    CreatedAt DATETIME DEFAULT GETDATE(), -- Ngày tạo bài viết
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Ngày cập nhật bài viết
    FOREIGN KEY (AdminID) REFERENCES Admins(AdminID) -- Khóa ngoại liên kết với bảng Admins
);
