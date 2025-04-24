create view [dbo].[vwBorrar] as
select o.RazonSocial,	v.VistaOrigen,		c.ColumnaEsquema  ,  e.MostrarWeb
from ona					o				
join EsquemaVista			v	on	v.IdONA				=  o.IdONA	
join esquema				e	on  e.IdEsquema		=  v.IdEsquema
join EsquemaVistaColumna	c	on  c.IdEsquemaVista	=  v.IdEsquemaVista
where o.estado ='A' and o.IdONA = 1
and   v.estado ='A' -- and v.IdEsquemaVista = 41
and   c.estado ='A' 
and   e.estado ='A'
