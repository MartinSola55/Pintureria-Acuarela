USE [master]
GO
/****** Object:  Database [Acuarela]    Script Date: 18/02/2023 07:14:24 p. m. ******/
CREATE DATABASE [Acuarela]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Acuarela', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Acuarela.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Acuarela_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\Acuarela_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Acuarela] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Acuarela].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Acuarela] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Acuarela] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Acuarela] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Acuarela] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Acuarela] SET ARITHABORT OFF 
GO
ALTER DATABASE [Acuarela] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Acuarela] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Acuarela] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Acuarela] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Acuarela] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Acuarela] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Acuarela] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Acuarela] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Acuarela] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Acuarela] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Acuarela] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Acuarela] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Acuarela] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Acuarela] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Acuarela] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Acuarela] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Acuarela] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Acuarela] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Acuarela] SET  MULTI_USER 
GO
ALTER DATABASE [Acuarela] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Acuarela] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Acuarela] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Acuarela] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Acuarela] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Acuarela] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Acuarela] SET QUERY_STORE = OFF
GO
USE [Acuarela]
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Business]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Business](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[adress] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Sucursal] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Capacity]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Capacity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[capacity] [float] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Capacity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_TipoProducto] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Color]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Color](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[rgb_hex_code] [char](7) NOT NULL,
 CONSTRAINT [PK_Color] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL,
	[id_user] [int] NOT NULL,
	[status] [bit] NOT NULL,
	[deleted_at] [datetime] NULL,
	[comment] [varchar](max) NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[description] [varchar](255) NOT NULL,
	[id_brand] [int] NOT NULL,
	[id_category] [int] NULL,
	[id_subcategory] [int] NULL,
	[id_capacity] [int] NULL,
	[id_color] [int] NULL,
	[internal_code] [int] NULL,
	[created_at] [datetime] NOT NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Business]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Business](
	[id_product] [int] NOT NULL,
	[id_business] [int] NOT NULL,
	[minimum_stock] [int] NULL,
	[stock] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_Product_Business] PRIMARY KEY CLUSTERED 
(
	[id_product] ASC,
	[id_business] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Order]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Order](
	[id_product] [int] NOT NULL,
	[id_order] [bigint] NOT NULL,
	[quantity] [int] NOT NULL,
	[status] [bit] NOT NULL,
	[quantity_send] [int] NOT NULL,
	[id_business_sender] [int] NULL,
 CONSTRAINT [PK_Product_Order] PRIMARY KEY CLUSTERED 
(
	[id_product] ASC,
	[id_order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Sell]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Sell](
	[id_product] [int] NOT NULL,
	[id_sell] [int] NOT NULL,
	[quantity] [int] NOT NULL,
 CONSTRAINT [PK_Product_Sell] PRIMARY KEY CLUSTERED 
(
	[id_product] ASC,
	[id_sell] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rol]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rol](
	[id] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sell]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sell](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL,
	[id_user] [int] NOT NULL,
 CONSTRAINT [PK_Sell] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subcategory]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subcategory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Subcategory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 18/02/2023 07:14:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[password] [char](64) NOT NULL,
	[id_rol] [smallint] NOT NULL,
	[id_business] [int] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Brand] ADD  CONSTRAINT [DF_Brand_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Product_Business] ADD  CONSTRAINT [DF_Product_Business_stock]  DEFAULT ((0)) FOR [stock]
GO
ALTER TABLE [dbo].[Product_Business] ADD  CONSTRAINT [DF_Product_Business_created_at]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Product_Order] ADD  CONSTRAINT [DF_Product_Order_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[Product_Order] ADD  CONSTRAINT [DF_Product_Order_quantity_send]  DEFAULT ((0)) FOR [quantity_send]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_User]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Brand] FOREIGN KEY([id_brand])
REFERENCES [dbo].[Brand] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Brand]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Capacity] FOREIGN KEY([id_capacity])
REFERENCES [dbo].[Capacity] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Capacity]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([id_category])
REFERENCES [dbo].[Category] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Color] FOREIGN KEY([id_color])
REFERENCES [dbo].[Color] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Color]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Subcategory] FOREIGN KEY([id_subcategory])
REFERENCES [dbo].[Subcategory] ([id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Subcategory]
GO
ALTER TABLE [dbo].[Product_Business]  WITH CHECK ADD  CONSTRAINT [FK_Product_Business_Business] FOREIGN KEY([id_business])
REFERENCES [dbo].[Business] ([id])
GO
ALTER TABLE [dbo].[Product_Business] CHECK CONSTRAINT [FK_Product_Business_Business]
GO
ALTER TABLE [dbo].[Product_Business]  WITH CHECK ADD  CONSTRAINT [FK_Product_Business_Product] FOREIGN KEY([id_product])
REFERENCES [dbo].[Product] ([id])
GO
ALTER TABLE [dbo].[Product_Business] CHECK CONSTRAINT [FK_Product_Business_Product]
GO
ALTER TABLE [dbo].[Product_Order]  WITH CHECK ADD  CONSTRAINT [FK_Product_Order_Business] FOREIGN KEY([id_business_sender])
REFERENCES [dbo].[Business] ([id])
GO
ALTER TABLE [dbo].[Product_Order] CHECK CONSTRAINT [FK_Product_Order_Business]
GO
ALTER TABLE [dbo].[Product_Order]  WITH CHECK ADD  CONSTRAINT [FK_Product_Order_Order] FOREIGN KEY([id_order])
REFERENCES [dbo].[Order] ([id])
GO
ALTER TABLE [dbo].[Product_Order] CHECK CONSTRAINT [FK_Product_Order_Order]
GO
ALTER TABLE [dbo].[Product_Order]  WITH CHECK ADD  CONSTRAINT [FK_Product_Order_Product] FOREIGN KEY([id_product])
REFERENCES [dbo].[Product] ([id])
GO
ALTER TABLE [dbo].[Product_Order] CHECK CONSTRAINT [FK_Product_Order_Product]
GO
ALTER TABLE [dbo].[Product_Sell]  WITH CHECK ADD  CONSTRAINT [FK_Product_Sell_Product] FOREIGN KEY([id_product])
REFERENCES [dbo].[Product] ([id])
GO
ALTER TABLE [dbo].[Product_Sell] CHECK CONSTRAINT [FK_Product_Sell_Product]
GO
ALTER TABLE [dbo].[Product_Sell]  WITH CHECK ADD  CONSTRAINT [FK_Product_Sell_Sell] FOREIGN KEY([id_sell])
REFERENCES [dbo].[Sell] ([id])
GO
ALTER TABLE [dbo].[Product_Sell] CHECK CONSTRAINT [FK_Product_Sell_Sell]
GO
ALTER TABLE [dbo].[Sell]  WITH CHECK ADD  CONSTRAINT [FK_Sell_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[Sell] CHECK CONSTRAINT [FK_Sell_User]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([id_rol])
REFERENCES [dbo].[Rol] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_Usuario_Rol]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Sucursal] FOREIGN KEY([id_business])
REFERENCES [dbo].[Business] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_Usuario_Sucursal]
GO
USE [master]
GO
ALTER DATABASE [Acuarela] SET  READ_WRITE 
GO
