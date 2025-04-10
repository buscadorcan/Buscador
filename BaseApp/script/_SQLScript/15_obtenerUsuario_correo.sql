
create proc sp_obtenerUsuario
@usuario varchar
as
begin
select 
Nombre,Apellido,Email,IdONA,h.CodigoHomologacion
from Usuario u 
inner join Homologacion h on u.IdHomologacionRol = h.IdHomologacion
where u.Nombre = @usuario
end

go
create proc sp_obtenerUsuariosOna
@IdONA int
as
begin
select 
Nombre,Apellido,Email,IdONA,h.CodigoHomologacion
from Usuario u 
inner join Homologacion h on u.IdHomologacionRol = h.IdHomologacion
where u.IdONA = @IdONA
end

