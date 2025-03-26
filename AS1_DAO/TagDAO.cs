using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace AS1_DAO
{
    public class TagDAO
    {

        private FunewsManagementContext _dbContext;
        private static TagDAO instance;

        public TagDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static TagDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TagDAO();
                }
                return instance;
            }
        }


        private readonly object _lock = new object();

public Tag GetTagsId(int id)
{
    lock(_lock)
    {
        return _dbContext.Tags
            .AsNoTracking()
            .SingleOrDefault(a => a.TagId == id);
    }
}

        public List<Tag> GetAllTag()
        {
            return _dbContext.Tags
                .ToList();
        }

        public void Add(Tag tag)
        {
            Tag cur = GetTagsId(tag.TagId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbContext.Tags.Add(tag);
            _dbContext.SaveChanges();
        }

        public void Update(Tag tag)
        {
            Tag cur = GetTagsId(tag.TagId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbContext.Entry(cur).CurrentValues.SetValues(tag);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            Tag cur = GetTagsId(id);
            if (cur != null)
            {
                _dbContext.Tags.Remove(cur);
                _dbContext.SaveChanges(); // Delete the object
            }
        }



    }
}
