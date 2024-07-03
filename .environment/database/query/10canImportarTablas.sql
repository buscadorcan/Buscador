DROP TABLE IF EXISTS ImportarTablas;
GO

CREATE TABLE ImportarTablas (
    idImportarTablas INT PRIMARY KEY IDENTITY(1,1),
    DataTipo NVARCHAR(max) NOT NULL,
    DataSistemaOrigen NVARCHAR(max) NOT NULL,
    DataSistemaOrigenId NVARCHAR(max) NOT NULL,
    DataSistemaFecha DATETIME	NOT NULL DEFAULT(GETDATE()),
    DataSistema NVARCHAR(max) NOT NULL,
    DataPais NVARCHAR(max) NOT NULL,
    H40 NVARCHAR(max) NOT NULL,
    H41 NVARCHAR(max) NOT NULL,
    H42 NVARCHAR(max) NOT NULL,
    H43 NVARCHAR(max) NOT NULL,
    H44 NVARCHAR(max) NOT NULL,
    H45 NVARCHAR(max) NOT NULL,
    H46 NVARCHAR(max) NOT NULL,
    H47 NVARCHAR(max) NOT NULL,
    H48 NVARCHAR(max) NOT NULL,
    H49 NVARCHAR(max) NOT NULL,
    H50 NVARCHAR(max) NOT NULL,
    H51 NVARCHAR(max) NOT NULL,
    H52 NVARCHAR(max) NOT NULL,
    H53 NVARCHAR(max) NOT NULL,
    H54 NVARCHAR(max) NOT NULL,
    H55 NVARCHAR(max) NOT NULL,
    H56 NVARCHAR(max) NOT NULL,
    H57 NVARCHAR(max) NOT NULL
);
GO



CREATE OR ALTER VIEW vwEsq01 AS
  SELECT 
    DataTipo,
    DataSistemaOrigen,
    DataSistemaOrigenId,
    DataSistemaFecha,
    2 as IdHomologacionEsquema,
    DataSistema,
    DataPais,
    H51,
    H52,
    H53
  FROM ImportarTablas
GO

CREATE OR ALTER VIEW vwEsq02 AS
  SELECT 
    DataTipo,
    DataSistemaOrigen,
    DataSistemaOrigenId,
    DataSistemaFecha,
    3 as IdHomologacionEsquema,
    DataSistema,
    DataPais,
    H55,
    H56,

  FROM ImportarTablas
GO

SELECT * FROM vwEsq01;

INSERT INTO ImportarTablas (DataTipo, DataSistemaOrigen, DataSistemaOrigenId, DataSistemaFecha, DataSistema, DataPais, H40, H41, H42, H43, H44, H45, H46, H47, H48, H49, H50, H51, H52, H53, H54, H55, H56, H57)
VALUES
('ORGANIZACION', 'DSO1', 'DSOId1', '2024-06-11', 'INACAL-DA', 'Colombia', 'H40-1', 'H41-1', 'H42-1', 'H43-1', 'H44-1', 'H45-1', 'H46-1', 'H47-1', 'H48-1', 'H49-1', 'H50-1', 'H51-1', 'H52-1', 'H53-1', 'H54-1', 'H55-1', 'H56-1', 'H57-1'),
('ORGANIZACION', 'DSO1', 'DSOId1', '2024-06-11', 'INACAL-DA', 'Colombia', 'H40-2', 'H41-2', 'H42-2', 'H43-2', 'H44-2', 'H45-2', 'H46-2', 'H47-2', 'H48-2', 'H49-2', 'H50-2', 'H51-2', 'H52-2', 'H53-2', 'H54-2', 'H55-2', 'H56-2', 'H57-2'),
('ORGANIZACION', 'DSO1', 'DSOId1', '2024-06-11', 'INACAL-DA', 'Colombia', 'H40-3', 'H41-3', 'H42-3', 'H43-3', 'H44-3', 'H45-3', 'H46-3', 'H47-3', 'H48-3', 'H49-3', 'H50-3', 'H51-3', 'H52-3', 'H53-3', 'H54-3', 'H55-3', 'H56-3', 'H57-3'),
('ORGANIZACION', 'DSO1', 'DSOId1', '2024-06-11', 'INACAL-DA', 'Colombia', 'H40-4', 'H41-4', 'H42-4', 'H43-4', 'H44-4', 'H45-4', 'H46-4', 'H47-4', 'H48-4', 'H49-4', 'H50-4', 'H51-4', 'H52-4', 'H53-4', 'H54-4', 'H55-4', 'H56-4', 'H57-4'),
('ORGANIZACION', 'DSO1', 'DSOId1', '2024-06-11', 'INACAL-DA', 'Colombia', 'H40-5', 'H41-5', 'H42-5', 'H43-5', 'H44-5', 'H45-5', 'H46-5', 'H47-5', 'H48-5', 'H49-5', 'H50-5', 'H51-5', 'H52-5', 'H53-5', 'H54-5', 'H55-5', 'H56-5', 'H57-5'),
('ORGANIZACION', 'DSO2', 'DSOId2', '2024-06-11', 'SAE', 'Ecuador', 'H40-6', 'H41-6', 'H42-6', 'H43-6', 'H44-6', 'H45-6', 'H46-6', 'H47-6', 'H48-6', 'H49-6', 'H50-6', 'H51-6', 'H52-6', 'H53-6', 'H54-6', 'H55-6', 'H56-6', 'H57-6'),
('ORGANIZACION', 'DSO2', 'DSOId2', '2024-06-11', 'SAE', 'Ecuador', 'H40-7', 'H41-7', 'H42-7', 'H43-7', 'H44-7', 'H45-7', 'H46-7', 'H47-7', 'H48-7', 'H49-7', 'H50-7', 'H51-7', 'H52-7', 'H53-7', 'H54-7', 'H55-7', 'H56-7', 'H57-7'),
('ORGANIZACION', 'DSO2', 'DSOId2', '2024-06-11', 'SAE', 'Ecuador', 'H40-8', 'H41-8', 'H42-8', 'H43-8', 'H44-8', 'H45-8', 'H46-8', 'H47-8', 'H48-8', 'H49-8', 'H50-8', 'H51-8', 'H52-8', 'H53-8', 'H54-8', 'H55-8', 'H56-8', 'H57-8'),
('ORGANIZACION', 'DSO2', 'DSOId2', '2024-06-11', 'SAE', 'Ecuador', 'H40-9', 'H41-9', 'H42-9', 'H43-9', 'H44-9', 'H45-9', 'H46-9', 'H47-9', 'H48-9', 'H49-9', 'H50-9', 'H51-9', 'H52-9', 'H53-9', 'H54-9', 'H55-9', 'H56-9', 'H57-9'),
('ORGANIZACION', 'DSO2', 'DSOId2', '2024-06-11', 'SAE', 'Ecuador','H40-10', 'H41-10', 'H42-10', 'H43-10', 'H44-10', 'H45-10', 'H46-10', 'H47-10', 'H48-10', 'H49-10', 'H50-10', 'H51-10', 'H52-10', 'H53-10', 'H54-10', 'H55-10', 'H56-10', 'H57-10')