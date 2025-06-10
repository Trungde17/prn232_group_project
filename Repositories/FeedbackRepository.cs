using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccess;

namespace Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>
    {
        public FeedbackRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
