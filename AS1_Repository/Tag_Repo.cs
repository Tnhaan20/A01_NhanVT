using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using AS1_DAO;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace AS1_Repository
{
    public class Tag_Repo : ITagRepo
    {


        public Tag GetTagsId(int id) => TagDAO.Instance.GetTagsId(id);

        public List<Tag> GetAllTag() => TagDAO.Instance.GetAllTag();

        public void Add(Tag tag) => TagDAO.Instance.Add(tag);

        public void Update(Tag tag) => TagDAO.Instance.Update(tag);

        public void Delete(int id) => TagDAO.Instance.Delete(id);

    }
}
