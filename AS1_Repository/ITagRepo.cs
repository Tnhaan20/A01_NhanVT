using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;
using AS1_DAO;

namespace AS1_Repository
{
    public interface ITagRepo
    {
        public Tag GetTagsId(int id);

        public List<Tag> GetAllTag();

        public void Add(Tag tag);

        public void Update(Tag tag);

        public void Delete(int id);
    }
}
