--| fn_PredictWord
CREATE   FUNCTION [dbo].[fnPredictWord] (@Prefix NVARCHAR(100))
RETURNS @TopWords TABLE (Word NVARCHAR(max))
AS
BEGIN
    INSERT INTO @TopWords (Word)
    SELECT DISTINCT TOP 10 FullTextData
    FROM   [EsquemaFullText]  (NOLOCK)
    WHERE  FullTextData LIKE @Prefix + '%'
	order by 1 asc
    RETURN
END
