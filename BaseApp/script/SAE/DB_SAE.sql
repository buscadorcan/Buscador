USE master;
GO


IF DB_ID('SAE') IS NOT NULL
BEGIN
    ALTER DATABASE SAE SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SAE;
END
GO

CREATE DATABASE SAE ON PRIMARY 
    (   NAME = 'SAE_data', 
        FILENAME = '/var/opt/mssql/data/SAE.mdf', SIZE = 200MB, MAXSIZE = UNLIMITED, FILEGROWTH = 45MB
    ) 
    LOG ON 
    (   NAME = 'CAN_DB_log', 
        FILENAME = '/var/opt/mssql/data/SAE.ldf', SIZE = 150MB, MAXSIZE = 300MB, FILEGROWTH = 45MB
    )
    WITH CATALOG_COLLATION = DATABASE_DEFAULT;
go

--IF DB_ID('SAE') IS NULL
--    CREATE DATABASE SAE;
--ELSE
--    PRINT 'La base de datos SAE ya existe.';
--GO

-- Validar si el usuario ya existe
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'userSAE')
BEGIN
    -- Crear el usuario
    CREATE LOGIN userSAE WITH PASSWORD = 'passSAE';
    PRINT 'Usuario userSAE creado.';
END
ELSE
    PRINT 'El usuario userSAE ya existe.';
GO

USE SAE;
GO

-- Validar si el usuario está en la base de datos
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'userSAE')
BEGIN
    -- Asociar el usuario a la base de datos
    CREATE USER userSAE FOR LOGIN userSAE;
    PRINT 'Usuario userSAE asociado a la base de datos SAE.';
END
ELSE
    PRINT 'El usuario userSAE ya está asociado a la base de datos SAE.';
go

-- Otorgar permisos de db_owner
IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE role_principal_id = DATABASE_PRINCIPAL_ID('db_owner') AND member_principal_id = USER_ID('userSAE'))
BEGIN
    EXEC sp_addrolemember 'db_owner', 'userSAE';
    PRINT 'Permisos de db_owner otorgados a userSAE.';
END
ELSE
    PRINT 'El usuario userSAE ya tiene permisos de db_owner.';
GO
