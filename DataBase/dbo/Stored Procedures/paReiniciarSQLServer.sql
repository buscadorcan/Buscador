CREATE PROCEDURE [dbo].[paReiniciarSQLServer] AS
--| 2K25.FEB.25 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0
--| Descripción:  Reinicia el SQL Server
BEGIN
	EXEC xp_cmdshell 'SCHTASKS /RUN /TN "Reiniciar_SQLServer"';
	--| cmd
	--|	icacls "C:\BuscadorTool" /grant "NT Service\MSSQLSERVER":(F)
	--| taskschd.msc  crear tarea Reiniciar_SQLServer y llamar al bash
	-- Habilitar opciones avanzadas
	--EXEC sp_configure 'show advanced options', 1;
	--RECONFIGURE;
	--GO
	---- Habilitar xp_cmdshell
	--EXEC sp_configure 'xp_cmdshell', 1;
	--RECONFIGURE;
	--GO
END
