using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using Microsoft.EntityFrameworkCore;

namespace AS1_Repository
{
    public interface ICategoriesRepo
    {
        public List<Category> getAllCategories();

        public List<object> GetList();

        public Category GetCategoryId(short id);

        public void AddCategory(Category a);

        public void UpdateCategory(Category a);

        public void DeleteCategory(short id);
    }
}
