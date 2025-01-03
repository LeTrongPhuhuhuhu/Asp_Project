using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Models;

public partial class Db6TlContext : DbContext
{
    public Db6TlContext()
    {
    }

    public Db6TlContext(DbContextOptions<Db6TlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductColor> ProductColors { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<WebsiteInfo> WebsiteInfos { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DB_6TL;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {

            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E87A0B2101");

            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E87F72B5D4");


            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.AdminName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins)
                .HasForeignKey(d => d.RoleId)

                .HasConstraintName("FK__Admins__RoleID__6C190EBB");


        });

        modelBuilder.Entity<Cart>(entity =>
        {

            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B7F4978FC7");

            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B7F3CD1AB2");


            entity.ToTable("Cart");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");


    


            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Product");
        });

        modelBuilder.Entity<Category>(entity =>
        {

            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BBDAF0536");

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB6643E41AB").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB6998E1E3B").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB6B7D58DA3").IsUnique();

            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B3107F464");

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB636486D1D").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB66E10A029").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Categori__BC7B5FB69705D0FC").IsUnique();


            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Slug).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_Category_ParentCategory");
        });

        modelBuilder.Entity<Color>(entity =>
        {

            entity.HasKey(e => e.ColorId).HasName("PK__Colors__8DA7674DB28303E4");

            entity.Property(e => e.ColorCode).HasMaxLength(50);

            entity.HasKey(e => e.ColorId).HasName("PK__Colors__8DA7674DAF507780");

            entity.Property(e => e.ColorName).HasMaxLength(100);
        });

        modelBuilder.Entity<Contact>(entity =>
        {

            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C66259B389365BA");

            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C66259BAF2D24FB");


            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Customer>(entity =>
        {

            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B82B00D0AE");

            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8D5C81C68");


            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Customers)
                .HasForeignKey(d => d.RoleId)

                .HasConstraintName("FK__Customers__RoleI__70DDC3D8");

       

        });

        modelBuilder.Entity<Discount>(entity =>
        {

            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF6AF17C81A");

            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF658447C0E");


            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountName).HasMaxLength(100);
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MinOrderValue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Order>(entity =>
        {

            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFA9130D07");

            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF3C756C47");


            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderStatus).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)

                .HasConstraintName("FK__Orders__Customer__75A278F5");



        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {

            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderDet__08D097C15615C8DE");

            entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderDet__08D097C1C7EAADA8");


            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)

                .HasConstraintName("FK__OrderDeta__Order__73BA3083");


            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)

                .HasConstraintName("FK__OrderDeta__Produ__74AE54BC");

        });

        modelBuilder.Entity<Product>(entity =>
        {

            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED8C764A37");

            entity.HasIndex(e => e.Slug, "UQ__Products__BC7B5FB689904554").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Products__BC7B5FB6A0C31C3B").IsUnique();

            entity.HasIndex(e => e.Slug, "UQ__Products__BC7B5FB6C1215808").IsUnique();


            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.Image).HasMaxLength(100);
            entity.Property(e => e.Material).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductDescription).HasMaxLength(1000);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(150);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)

                .HasConstraintName("FK__Products__Catego__7A672E12");

            entity.HasOne(d => d.Discount).WithMany(p => p.Products)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Products__Discou__7D439ABD");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Products__Suppli__00200768");

        });

        modelBuilder.Entity<ProductColor>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProductColor");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Color).WithMany()
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK_ProductColor_Color");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("ProductID");
        });

        modelBuilder.Entity<Review>(entity =>
        {

            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE995156EF");


            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Review");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Review");
        });

        modelBuilder.Entity<Role>(entity =>
        {

            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A424D3D7D");


            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {

            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE66694C31EF2A9");


            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SupplierName).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<WebsiteInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WebsiteInfo");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FacebookUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InstagramUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TwitterUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.YouTubeUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {

            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189CB6CA3273E");


            entity.ToTable("Wishlist");

            entity.Property(e => e.WishlistId).HasColumnName("WishlistID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.CustomerId)

                .HasConstraintName("FK__Wishlist__Custom__04E4BC85");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Wishlist__Produc__07C12930");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
