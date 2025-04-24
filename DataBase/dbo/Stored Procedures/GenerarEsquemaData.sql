-- =============================================
-- Author:		<Author,,JHONATAN>
-- Create date: <Create Date,09-04-2025,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GenerarEsquemaData] 
(
    @IdOna INT,
    @IdEsquema INT,
    @JsonParametros NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener columnas activas del esquema
    DECLARE @Columnas TABLE (
        ColumnaEsquemaIdH INT,
        ColumnaEsquema NVARCHAR(100)
    );

    DECLARE @IdEsquemaVista INT;

    SELECT 
        @IdEsquemaVista = esVista.IdEsquemaVista
    FROM EsquemaVista esVista 
    WHERE esVista.IdEsquema = @IdEsquema 
      AND esVista.IdONA = @IdOna 
      AND esVista.Estado = 'A';

    INSERT INTO @Columnas (ColumnaEsquemaIdH, ColumnaEsquema)
    SELECT esVistaColum.ColumnaEsquemaIdH, esVistaColum.ColumnaEsquema
    FROM EsquemaVistaColumna esVistaColum 
    WHERE esVistaColum.IdEsquemaVista = @IdEsquemaVista;

    -- Cursor para cada registro del JSON
    DECLARE @Item NVARCHAR(MAX);
    DECLARE cursor_json CURSOR FOR
        SELECT value FROM OPENJSON(@JsonParametros);

    OPEN cursor_json;
    FETCH NEXT FROM cursor_json INTO @Item;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Tabla temporal local para este registro
        DECLARE @Temp TABLE (
            Nombre NVARCHAR(100),
            Valor NVARCHAR(MAX)
        );

        DELETE FROM @Temp; -- Asegura limpieza si se reutiliza

        INSERT INTO @Temp (Nombre, Valor)
        SELECT [key], value
        FROM OPENJSON(@Item);

        DECLARE @JsonData NVARCHAR(MAX); -- Declarar dentro del bucle para evitar acumulación

        SELECT @JsonData = (
            SELECT c.ColumnaEsquemaIdH AS IdHomologacion, t.Valor AS Data
            FROM @Columnas c
            JOIN @Temp t ON t.Nombre = c.ColumnaEsquema
            FOR JSON PATH
        );

		--Insertar em la tabla esquemaFulltext si indexar esta en S
		DECLARE @Indexar NCHAR, 
		        @IdHomologacion INT,
				@valor NVARCHAR(255)

        -- Insertar el resultado limpio
        INSERT INTO EsquemaData (
            IdEsquemaVista,
            VistaFK,
            VistaPK,
            DataEsquemaJson,
            DataFecha
        )
        VALUES (
            @IdEsquemaVista,
            NULL,
            NULL,
            '[' + @JsonData + ']',
            GETDATE()
        );

	    INSERT INTO EsquemaFullText (
		    IdEsquemaData,
			IdHomologacion,
			FullTextData
		  )
	    SELECT (SELECT MAX(IdEsquemaData) FROM EsquemaData),
		        c.ColumnaEsquemaIdH,
		        t.Valor AS Data
            FROM @Columnas c 
			JOIN Homologacion h ON C.ColumnaEsquemaIdH = H.IdHomologacion
            JOIN @Temp t ON t.Nombre = c.ColumnaEsquema
			WHERE h.Indexar = 'S'
      
        FETCH NEXT FROM cursor_json INTO @Item;
    END

    CLOSE cursor_json;
    DEALLOCATE cursor_json;

    SELECT * FROM EsquemaData WHERE idEsquemaVista = @IdEsquemaVista

END



 