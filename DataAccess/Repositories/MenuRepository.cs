using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;
using SharedApp.Services;

namespace DataAccess.Repositories
{
    public class MenuRepository : BaseRepository, IMenuRepository
    {
        private readonly IJwtService _jwtService;
        public MenuRepository(
          IJwtService jwtService,
          ILogger<MenuRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }

        public bool Create(MenuRol data)
        {
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.MenuRol.Add(data);
                return context.SaveChanges() >= 0;
            });
        }

        public Menus? FindDataById(int idHRol, int idHMenu)
        {
            return ExecuteDbOperation(context =>
                (from mr in context.MenuRol.AsNoTracking()
                 join hm in context.Homologacion.AsNoTracking() on mr.IdHMenu equals hm.IdHomologacion
                 join hr in context.Homologacion.AsNoTracking() on mr.IdHRol equals hr.IdHomologacion
                 where mr.IdHRol == idHRol && mr.IdHMenu == idHMenu /*&& mr.Estado == "A"*/ && hm.Estado == "A" && hr.Estado == "A"
                 select new Menus
                 {
                     IdMenuRol = mr.IdMenuRol,
                     IdHRol = mr.IdHRol,
                     Rol = hr.MostrarWeb,
                     IdHMenu = mr.IdHMenu,
                     Menu = hm.MostrarWeb,
                     Estado = mr.Estado,
                     FechaCreacion = mr.FechaCreacion
                 }).FirstOrDefault()
            );
        }
        public MenuRol? FindById(int idHRol, int idHMenu)
        {
            return ExecuteDbOperation(context =>
                (from mr in context.MenuRol.AsNoTracking()
                 join hm in context.Homologacion.AsNoTracking() on mr.IdHMenu equals hm.IdHomologacion
                 join hr in context.Homologacion.AsNoTracking() on mr.IdHRol equals hr.IdHomologacion
                 where mr.IdHRol == idHRol && mr.IdHMenu == idHMenu && /*mr.Estado == "A" && */ hm.Estado == "A" && hr.Estado == "A"
                 select new MenuRol
                 {
                     IdMenuRol = mr.IdMenuRol,
                     IdHRol = mr.IdHRol,
                     IdHMenu = mr.IdHMenu,
                     Estado = mr.Estado,
                     FechaCreacion = mr.FechaCreacion
                 }).FirstOrDefault()
            );
        }

        public List<Menus> FindAll()
        {
            return ExecuteDbOperation(context =>
                (from mr in context.MenuRol.AsNoTracking()
                 join hm in context.Homologacion.AsNoTracking() on mr.IdHMenu equals hm.IdHomologacion
                 join hr in context.Homologacion.AsNoTracking() on mr.IdHRol equals hr.IdHomologacion
                 where /*mr.Estado == "A" && */hm.Estado == "A" && hr.Estado == "A"
                 select new Menus
                 {
                     IdMenuRol = mr.IdMenuRol,
                     IdHRol = mr.IdHRol,
                     Rol = hr.MostrarWeb,
                     IdHMenu = mr.IdHMenu,
                     Menu = hm.MostrarWeb,
                     Estado = mr.Estado,
                     FechaCreacion = mr.FechaCreacion,
                 }).ToList()
            );
        }

        public List<Menus> GetListByMenusAsync(int idHRol, int idHMenu)
        {
            return ExecuteDbOperation(context =>
                context.Menus
                    .AsNoTracking()
                    .Where(c => c.IdHRol == idHRol && c.Estado == "A")
                    .ToList()
            );
        }


        public bool Update(MenuRol newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdMenuRol == newRecord.IdMenuRol);

                _exits.FechaCreacion = DateTime.Now;

                context.MenuRol.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public List<MenuPagina> ObtenerMenusPendingConfig(int idHomologacionRol)
        {
            return ExecuteDbOperation(context =>
            {
                var homologaciones = context.Homologacion
                    .Where(h => h.IdHomologacionGrupo == 3 && h.Estado == "A")
                    .Select(h => h.IdHomologacion);

                var menusExcluidos = context.MenuRol
                    .Where(mr => mr.IdHRol == idHomologacionRol)
                    .Select(mr => mr.IdHMenu);

                var resultado = context.Homologacion
                    .Where(h => homologaciones.Contains(h.IdHomologacion) && !menusExcluidos.Contains(h.IdHomologacion))
                    .Select(h => new MenuPagina
                    {
                        IdHomologacion = h.IdHomologacion,
                        MostrarWeb = h.MostrarWeb
                    })
                    .ToList();

                return resultado;
            });
        }

    }
}
