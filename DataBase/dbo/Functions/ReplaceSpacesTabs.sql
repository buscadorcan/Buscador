--ModoBuscar 
--4 = Buscar con sinonimos
--5 = Buscar vectorizacion

--SELECT	*
--FROM	OrganizacionFullText
--WHERE	FullTextOrganizacion LIKE  '%rdTipoCertificacion%'
CREATE   FUNCTION [dbo].[ReplaceSpacesTabs](@input NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @pos INT;
    DECLARE @length INT;
    DECLARE @result NVARCHAR(MAX);
    
    SET @result = @input;
    SET @pos = 1;
    SET @length = LEN(@result);
    
    WHILE @pos < @length
    BEGIN
        IF SUBSTRING(@result, @pos, 1) IN (' ', CHAR(9)) AND SUBSTRING(@result, @pos + 1, 1) IN (' ', CHAR(9))
        BEGIN
            SET @result = STUFF(@result, @pos + 1, 1, '');
            SET @length = LEN(@result);
        END
        ELSE
        BEGIN
            SET @pos = @pos + 1;
        END
    END
    
    RETURN @result;
END;
