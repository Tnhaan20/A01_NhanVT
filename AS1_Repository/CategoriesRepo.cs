using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using AS1_DAO;

namespace AS1_Repository
{
    public class CategoriesRepo : ICategoriesRepo
    {
        public void AddCategory(Category a) => CategoryDAO.Instance.AddCategory(a);

        public void DeleteCategory(short id) => CategoryDAO.Instance.DeleteCategory(id);

        public List<Category> getAllCategories () => CategoryDAO.Instance.getAllCategories();

        public Category GetCategoryId(short id)=> CategoryDAO.Instance.GetCategoryId(id);

        public List<object> GetList() => CategoryDAO.Instance.GetSelectList();

        public void UpdateCategory(Category a) => CategoryDAO.Instance.UpdateCategory(a);
    }
}
