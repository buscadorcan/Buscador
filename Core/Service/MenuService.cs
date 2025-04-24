using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class MenuService: IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            this._menuRepository = menuRepository;
        }

        public bool Create(MenuRol data)
        {
           return _menuRepository.Create(data);
        }

        public List<Menus> FindAll()
        {
           return _menuRepository.FindAll();
        }

        public MenuRol? FindById(int idHRol, int idHMenu)
        {
           return _menuRepository.FindById(idHRol, idHMenu);
        }

        public Menus? FindDataById(int idHRol, int idHMenu)
        {
           return _menuRepository.FindDataById(idHRol, idHMenu);
        }

        public List<Menus> GetListByMenusAsync(int idHRol, int idHMenu)
        {
           return _menuRepository.GetListByMenusAsync(idHRol, idHMenu);
        }

        public List<MenuPagina> ObtenerMenusPendingConfig(int idHomologacionRol)
        {
           return _menuRepository.ObtenerMenusPendingConfig(idHomologacionRol);
        }

        public bool Update(MenuRol data)
        {
           return _menuRepository.Update(data);
        }
    }
}
