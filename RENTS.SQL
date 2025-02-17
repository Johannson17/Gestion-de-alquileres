USE master;
GO

-- Forzar la desconexión de todos los usuarios y eliminar la base de datos si existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Rents')
BEGIN
    ALTER DATABASE Rents SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Rents;
END
GO

-- Crear la base de datos
CREATE DATABASE Rents;
GO

USE Rents;
GO

-- Eliminar restricciones de clave foránea si existen
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
              + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys;
EXEC sp_executesql @sql;
GO

-- Eliminar tablas si existen
DECLARE @sqlDropTables NVARCHAR(MAX) = N'';
SELECT @sqlDropTables += 'DROP TABLE IF EXISTS ' + QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
EXEC sp_executesql @sqlDropTables;
GO

-- Crear tablas en el orden correcto

CREATE TABLE [dbo].[Person](
	[IdPerson] [uniqueidentifier] NOT NULL,
	[IdUser] [varchar](5000) NULL,
	[NamePerson] [varchar](500) NULL,
	[LastNamePerson] [varchar](500) NULL,
	[NumberDocumentPerson] [int] NULL,
	[TypeDocumentPerson] [varchar](50) NULL,
	[PhoneNumberPerson] [varchar](50) NULL,
	[EmailPerson] [varchar](50) NULL,
	[DomicilePerson] [varchar](200) NULL,
	[TypePerson] [varchar](50) NULL,
 CONSTRAINT [PK__Person__A5D4E15BAA03F4A3] PRIMARY KEY CLUSTERED 
(
	[IdPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Property](
	[IdProperty] [uniqueidentifier] NOT NULL,
	[DescriptionProperty] [varchar](8000) NULL,
	[CountryProperty] [varchar](50) NULL,
	[ProvinceProperty] [varchar](50) NULL,
	[MunicipalityProperty] [varchar](50) NULL,
	[AddressProperty] [varchar](50) NULL,
	[StatusProperty] [varchar](50) NULL,
 CONSTRAINT [PK__Property__842B6AA713CBE153] PRIMARY KEY CLUSTERED 
(
	[IdProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Contract](
	[IdContract] [uniqueidentifier] NOT NULL,
	[FkIdProperty] [uniqueidentifier] NULL,
	[FkIdTenant] [uniqueidentifier] NULL,
	[DateStartContract] [datetime] NULL,
	[DateFinalContract] [datetime] NULL,
	[MensualRentPrice] [float] NULL,
	[StatusContract] [varchar](50) NULL,
	[Hash] [varchar](200) NULL,
	[SignedContract] [image] NULL,
 CONSTRAINT [PK__Contract__856E762CFD47E922] PRIMARY KEY CLUSTERED 
(
	[IdContract] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContractClause](
	[IdContractClause] [uniqueidentifier] NOT NULL,
	[FkIdContract] [uniqueidentifier] NULL,
	[TitleClause] [varchar](100) NULL,
	[DetailClause] [varchar](500) NULL,
	[IdAuxiliar] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK__Contract__423F9DC6A96E5408] PRIMARY KEY CLUSTERED 
(
	[IdContractClause] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[InventoryProperty](
	[IdInventoryProperty] [uniqueidentifier] NOT NULL,
	[FkIdProperty] [uniqueidentifier] NULL,
	[NameInventory] [varchar](100) NULL,
	[DescriptionInventory] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdInventoryProperty] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MaintenanceRequest](
	[IdRequest] [uniqueidentifier] NOT NULL,
	[FkIdProperty] [uniqueidentifier] NULL,
	[FkIdPerson] [uniqueidentifier] NULL,
	[TitleTicket] [varchar](100) NULL,
	[DescriptionTicket] [varchar](300) NULL,
	[StatusTicket] [varchar](50) NULL,
	[ImageTicket] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[IdRequest] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[PropertyPerson](
	[IdPropertyPerson] [uniqueidentifier] NOT NULL,
	[FkIdProperty] [uniqueidentifier] NULL,
	[FkIdPerson] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[IdPropertyPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Restaurar claves foráneas

ALTER TABLE dbo.ContractClause 
ADD CONSTRAINT FK_ContractClause_FkIdContract FOREIGN KEY (FkIdContract)
REFERENCES dbo.Contract (IdContract);
GO

ALTER TABLE dbo.InventoryProperty 
ADD CONSTRAINT FK_InventoryProperty_FkIdProperty FOREIGN KEY (FkIdProperty)
REFERENCES dbo.Property (IdProperty);
GO

ALTER TABLE dbo.MaintenanceRequest 
ADD CONSTRAINT FK_MaintenanceRequest_FkIdProperty FOREIGN KEY (FkIdProperty)
REFERENCES dbo.Property (IdProperty);
GO

ALTER TABLE dbo.MaintenanceRequest 
ADD CONSTRAINT FK_MaintenanceRequest_FkIdPerson FOREIGN KEY (FkIdPerson)
REFERENCES dbo.Person (IdPerson);
GO

ALTER TABLE dbo.PropertyPerson 
ADD CONSTRAINT FK_PropertyPerson_FkIdProperty FOREIGN KEY (FkIdProperty)
REFERENCES dbo.Property (IdProperty);
GO

ALTER TABLE dbo.PropertyPerson 
ADD CONSTRAINT FK_PropertyPerson_FkIdPerson FOREIGN KEY (FkIdPerson)
REFERENCES dbo.Person (IdPerson);
GO

ALTER TABLE dbo.Contract
ADD CONSTRAINT FK_Contract_FkIdProperty FOREIGN KEY (FkIdProperty)
REFERENCES dbo.Property (IdProperty);
GO
