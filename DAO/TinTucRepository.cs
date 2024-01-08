using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface ITinTucRepository : IRepository<TinTuc>
    { 

    }
    public class TinTucRepository : Repository<TinTuc>, ITinTucRepository
    {
        public TinTucRepository(DemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
