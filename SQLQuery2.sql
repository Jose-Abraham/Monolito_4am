/****** Object:  UserDefinedFunction [dbo].[desencriptacon] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[desencriptacon]
(
@clave varbinary(max)
)
RETURNS varchar(50)
AS 
BEGIN
	DECLARE @pass AS varchar(50)
	SET @pass = DECRYPTBYPASSPHRASE('cl@ve',@clave)
	RETURN @pass
END
GO

/****** Object:  UserDefinedFunction [dbo].[encriptacon] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[encriptacon]
(
@clave varchar(50)
)
RETURNS varbinary(max)
AS
BEGIN
	DECLARE @pass AS varbinary(max)
	SET @pass = ENCRYPTBYPASSPHRASE('cl@ve',@clave)
	RETURN @pass
END
GO

/****** Object:  Table [dbo].[tbl_fotos_productos] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_fotos_productos](
	[fpro_id] [int] IDENTITY(1,1) NOT NULL,
	[pro_id] [int] NOT NULL,
	[fpro_ruta_imagen] [varchar](250) NOT NULL,
	[fpro_nombre_archivo] [varchar](150) NULL,
	[fpro_fecha_subida] [datetime] NULL,
	[fpro_es_principal] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[fpro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_productos] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_productos](
	[pro_id] [int] IDENTITY(1,1) NOT NULL,
	[pro_nombre] [varchar](50) NULL,
	[pro_cantidad] [int] NULL,
	[pro_precio] [decimal](18, 2) NULL,
	[pro_estado] [char](1) NULL,
	[prov_id] [int] NULL,
	[pro_codigo] [varchar](50) NULL,
	[pro_descripcion] [varchar](250) NULL,
 CONSTRAINT [PK_tbl_productos] PRIMARY KEY CLUSTERED 
(
	[pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_productos_eliminado] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_productos_eliminado](
	[pro_id] [int] NOT NULL,
	[pro_nombre] [varchar](50) NULL,
	[pro_codigo] [varchar](50) NULL,
	[pro_descripcion] [varchar](250) NULL,
	[pro_cantidad] [int] NULL,
	[pro_precio] [decimal](18, 2) NULL,
	[prov_id] [int] NULL,
	[pro_estado] [char](1) NULL,
	[fecha_eliminacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_proveedor] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_proveedor](
	[prov_id] [int] IDENTITY(1,1) NOT NULL,
	[prov_nombre] [varchar](50) NULL,
	[prov_estado] [char](1) NULL,
	[prov_ruc] [varchar](13) NULL,
	[prov_telefono] [varchar](15) NULL,
	[prov_correo] [varchar](100) NULL,
 CONSTRAINT [PK_tbl_proveedor] PRIMARY KEY CLUSTERED 
(
	[prov_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_proveedor_eliminado] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_proveedor_eliminado](
	[prov_id] [int] NOT NULL,
	[prov_nombre] [varchar](50) NULL,
	[prov_estado] [char](1) NULL,
	[prov_ruc] [varchar](13) NULL,
	[prov_telefono] [varchar](15) NULL,
	[prov_correo] [varchar](100) NULL,
	[fecha_eliminacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[prov_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_tipo_usuario] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_tipo_usuario](
	[tusu_id] [int] IDENTITY(1,1) NOT NULL,
	[tusu_nombre] [varchar](50) NULL,
	[tusu_estado] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[tusu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_usario] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_usario](
	[usu_id] [int] IDENTITY(1,1) NOT NULL,
	[usu_cedula] [varchar](10) NULL,
	[usu_nombres] [varchar](50) NULL,
	[usu_apellidos] [varchar](50) NULL,
	[usu_direccion] [varchar](50) NULL,
	[usu_celular] [varchar](10) NULL,
	[usu_correo] [varchar](150) NULL,
	[usu_fecha_creacion] [datetime] NULL,
	[usu_fecha_cumple] [date] NULL,
	[usu_nick] [varchar](50) NULL,
	[usu_contraseña] [varbinary](max) NULL,
	[usu_intentos] [int] NULL,
	[usu_estado] [char](1) NULL,
	[tusu_id] [int] NULL,
	[usu_fecha_ultimo_intento] [datetime] NULL,
	[usu_codigo_OTP] [varbinary](max) NULL,
	[usu_bloqueado] [bit] NOT NULL,
	[usu_record] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[usu_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[tbl_usuario_foto] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_usuario_foto](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[usu_id] [int] NOT NULL,
	[foto] [varbinary](max) NOT NULL,
	[nombre_archivo] [varchar](150) NULL,
	[fecha_subida] [datetime] NULL,
	[es_principal] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_fotos_productos] ADD DEFAULT (getdate()) FOR [fpro_fecha_subida]
GO
ALTER TABLE [dbo].[tbl_fotos_productos] ADD DEFAULT ((1)) FOR [fpro_es_principal]
GO
ALTER TABLE [dbo].[tbl_productos_eliminado] ADD DEFAULT (getdate()) FOR [fecha_eliminacion]
GO
ALTER TABLE [dbo].[tbl_proveedor_eliminado] ADD DEFAULT (getdate()) FOR [fecha_eliminacion]
GO
ALTER TABLE [dbo].[tbl_usario] ADD DEFAULT ((0)) FOR [usu_bloqueado]
GO
ALTER TABLE [dbo].[tbl_usario] ADD DEFAULT ((0)) FOR [usu_record]
GO
ALTER TABLE [dbo].[tbl_usuario_foto] ADD DEFAULT (getdate()) FOR [fecha_subida]
GO
ALTER TABLE [dbo].[tbl_usuario_foto] ADD DEFAULT ((1)) FOR [es_principal]
GO

ALTER TABLE [dbo].[tbl_fotos_productos] WITH CHECK ADD FOREIGN KEY([pro_id])
REFERENCES [dbo].[tbl_productos] ([pro_id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbl_productos] WITH CHECK ADD CONSTRAINT [FK_tbl_productos_tbl_proveedor] FOREIGN KEY([prov_id])
REFERENCES [dbo].[tbl_proveedor] ([prov_id])
GO
ALTER TABLE [dbo].[tbl_productos] CHECK CONSTRAINT [FK_tbl_productos_tbl_proveedor]
GO

ALTER TABLE [dbo].[tbl_usario] WITH CHECK ADD FOREIGN KEY([tusu_id])
REFERENCES [dbo].[tbl_tipo_usuario] ([tusu_id])
GO

ALTER TABLE [dbo].[tbl_usuario_foto] WITH CHECK ADD FOREIGN KEY([usu_id])
REFERENCES [dbo].[tbl_usario] ([usu_id])
GO

/****** Object:  StoredProcedure [dbo].[sp_ReiniciarIdentityProveedor] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ReiniciarIdentityProveedor]
AS
BEGIN
    DECLARE @MaxID INT = (SELECT ISNULL(MAX(prov_id), 0) FROM tbl_proveedor);
    DBCC CHECKIDENT ('tbl_proveedor', RESEED, @MaxID);
    
    PRINT 'Identity de tbl_proveedor reiniciada correctamente. Próximo ID: ' + CAST(@MaxID + 1 AS VARCHAR(10));
END
GO

/****** Object:  StoredProcedure [dbo].[sp_ReiniciarIntentosDiarios] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ReiniciarIntentosDiarios]
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE tbl_usario
	SET usu_intentos = 0
	WHERE usu_intentos < 3
	 AND CAST (usu_fecha_ultimo_intento AS DATE) < CAST (GETDATE() AS DATE);
END;
GO