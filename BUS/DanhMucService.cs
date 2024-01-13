using DAO;
using DTO;

namespace BUS
{
    public interface IDanhMucService
    {
        Task<DanhMuc> GetByID(int id);
        List<DanhMuc> GetAll();
        Task<bool> AddDanhMuc(DanhMuc danhMuc);
        Task<bool> UpdateDanhMuc(DanhMuc danhMuc);
        Task<bool> DeleteDanhMuc(DanhMuc danhMuc);

    }
    public class DanhMucService : IDanhMucService
    {
        private readonly IDanhMucRepository danhMucRepository;
        public DanhMucService(IDanhMucRepository danhMucRepository)
        {
            this.danhMucRepository = danhMucRepository;
        }

        public async Task<bool> AddDanhMuc(DanhMuc danhMuc)
        {
            return await danhMucRepository.Add(danhMuc);
        }

        public async Task<bool> DeleteDanhMuc(DanhMuc danhMuc)
        {
            return await danhMucRepository.Delete(danhMuc);
        }

        public List<DanhMuc> GetAll()
        {
            return danhMucRepository.GetAll();
        }

        public async Task<DanhMuc> GetByID(int id)
        {
            return await danhMucRepository.GetById(id);
        }

        public async Task<bool> UpdateDanhMuc(DanhMuc danhMuc)
        {
            return await danhMucRepository.Update(danhMuc);
        }
    }
}
