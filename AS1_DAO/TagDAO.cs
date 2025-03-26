using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS1_BusinessModel;

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

        
    }
}
