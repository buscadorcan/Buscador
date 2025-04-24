 
CREATE   FUNCTION [dbo].[fnDropSpacesTabs] (@input NVARCHAR(MAX))	
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX);
    
    -- Reemplazar tabulaciones, saltos de línea y retornos de carro por espacios
    SET @result = REPLACE(REPLACE(REPLACE(@input, CHAR(13), ' '), CHAR(10), ' '), CHAR(9), ' ');
    
    -- Eliminar espacios duplicados
    WHILE CHARINDEX('  ', @result) > 0
    BEGIN
        SET @result = REPLACE(@result, '  ', ' ');
    END
    
    -- Retornar el resultado
    RETURN LTRIM(RTRIM(@result));
END;
