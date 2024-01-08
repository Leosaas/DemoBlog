using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface IDanhMucTinTucRepository : IRepository<DanhMucTinTuc>
    { 

    }
    public class DanhMucTinTucRepository : Repository<DanhMucTinTuc>, IDanhMucTinTucRepository
    {
        public DanhMucTinTucRepository(DemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
