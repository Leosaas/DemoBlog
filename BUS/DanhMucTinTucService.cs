using DAO;
using DTO;

namespace BUS
{
    public interface IDanhMucTinTucService
    {
        Task<bool> Add(DanhMucTinTuc danhMucTinTuc);
        Task<List<DanhMuc>> LayToanBoDanhMucCuaTinTuc(int idTinTuc);
        Task<bool> XoaToanBoDanhMucCuaTinTuc(int idTinTuc);
    }
    public class DanhMucTinTucService : IDanhMucTinTucService
    {
        private readonly IDanhMucTinTucRepository danhMucTinTucRepository;
        private readonly IDanhMucRepository danhMucRepository;
        public DanhMucTinTucService(IDanhMucTinTucRepository danhMucTinTucRepository, IDanhMucRepository danhMucRepository)
        {
            this.danhMucTinTucRepository = danhMucTinTucRepository;
            this.danhMucRepository = danhMucRepository;
        }

        public async Task<bool> Add(DanhMucTinTuc danhMucTinTuc)
        {
            return await danhMucTinTucRepository.Add(danhMucTinTuc);
        }

        public async Task<List<DanhMuc>> LayToanBoDanhMucCuaTinTuc(int idTinTuc)
        {
            var danhSachDanhMuc = danhMucTinTucRepository.GetAll().Where(x => x.IDTintuc.Equals(idTinTuc)).Select(x => x.IDDanhMuc);
            List<DanhMuc> lst = new List<DanhMuc>();
            foreach(int id in danhSachDanhMuc)
            {
                lst.Add(await danhMucRepository.GetById(id));
            }
            return lst;

        }

        public async Task<bool> XoaToanBoDanhMucCuaTinTuc(int idTinTuc)
        {
            var danhSachDanhMuc = danhMucTinTucRepository.GetAll().Where(x => x.IDTintuc.Equals(idTinTuc)).ToList();
            bool success = false;
            foreach(var danhMuc in danhSachDanhMuc)
            {
                success = await danhMucTinTucRepository.Delete(danhMuc);
            }
            return success;
        }
    }
}
