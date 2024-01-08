using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface IDanhMucRepository : IRepository<DanhMuc>
    { 

    }
    public class DanhMucRepository : Repository<DanhMuc>, IDanhMucRepository
    {
        public DanhMucRepository(DemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
