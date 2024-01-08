using DAO;
using DTO;

namespace BUS
{
    public interface IDanhMucService
    {
        Task<DanhMuc> GetByID(int id);
        IQueryable<DanhMuc> GetAll();

    }
    public class DanhMucService : IDanhMucService
    {
        private readonly IDanhMucRepository danhMucRepository;
        public DanhMucService(IDanhMucRepository danhMucRepository)
        {
            this.danhMucRepository = danhMucRepository;
        }

        public IQueryable<DanhMuc> GetAll()
        {
            return danhMucRepository.GetAll();
        }

        public async Task<DanhMuc> GetByID(int id)
        {
            return await danhMucRepository.GetById(id);
        }
    }
}
