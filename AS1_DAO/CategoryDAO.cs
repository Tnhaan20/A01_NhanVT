using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using Microsoft.EntityFrameworkCore;

namespace AS1_DAO
{
    public class CategoryDAO
    {
        private FunewsManagementContext _dbcontext;
        private static CategoryDAO instance;

        public CategoryDAO()
        {
            _dbcontext = new FunewsManagementContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public List<Category> getAllCategories()
        {
            return _dbcontext.Categories.ToList();
        }

        


        public List<object> GetSelectList()
        {
            return _dbcontext.Categories
                .Select(c => new
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName.ToString()
                })
                .ToList<object>();
        }

        public Category GetCategoryId(short id)
        {
            return _dbcontext.Categories
                .SingleOrDefault(a => a.CategoryId.Equals(id));
        }

        public void AddCategory(Category a)
        {
            Category cur = GetCategoryId(a.CategoryId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbcontext.Categories.Add(a);
            _dbcontext.SaveChanges();
        }

        public void UpdateCategory(Category a)
        {
            Category cur = GetCategoryId(a.CategoryId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbcontext.Entry(cur).CurrentValues.SetValues(a);
            _dbcontext.SaveChanges();
        }

        public void DeleteCategory(short id)
        {
            Category cur = GetCategoryId(id);
            if (cur != null)
            {
                _dbcontext.Categories.Remove(cur);
                _dbcontext.SaveChanges(); // Delete the object
            }
        }


        public List<object> GetCategoryList()
        {
            return _dbcontext.Categories
                .Select(c => new
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName.ToString()
                })
                .ToList<object>();
        }




    }
}
