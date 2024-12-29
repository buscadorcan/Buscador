DROP TABLE if exists Cliente;
create TABLE Cliente (
	 IdCliente 	integer PRIMARY KEY identity(10,2)
  	,Cedula 	varchar(10)
  	,nombre 	varchar(100)
);

INSERT into cliente ( cedula, nombre) values
 ( '1105976870','patmic paccha')
,( '1105976870','michael paccha')
,('1105976870','leo paccha');



------------------------------------
DROP TABLE if exists Producto;
create table Producto
(
	 IdProducto 	INTEGER  IDENTITY(10,2)
  	,codigo 		VARCHAR(10)
  	,nombre 		VARCHAR(100)
  	,precio 		FLOAT
	,cantidad		INTEGER

    ,CONSTRAINT  [PK_P_IdProducto]  PRIMARY KEY CLUSTERED (IdProducto)
);


------------------------------------


DROP TABLE if exists Venta;
create table Venta
(
	 IdVenta 	    INTEGER IDENTITY(10,2)
  	,IdProducto_ 	INTEGER
  	,IdCliente 		INTEGER
  	,CostoTotal		FLOAT
    ,Estado         VARCHAR(1) NOT NULL DEFAULT('A')

    ,CONSTRAINT  [PK_V_IdVenta]     PRIMARY KEY CLUSTERED (IdVenta)
    ,CONSTRAINT  [FK_V_IdProducto_] FOREIGN KEY(IdProducto_) REFERENCES Producto (IdProducto)
    ,CONSTRAINT  [CK_V_Estado]		CHECK   (Estado IN ('A', 'X'))
);

------------------------------------

select * from Cliente;
select * from producto ;