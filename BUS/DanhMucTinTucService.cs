using DAO;
using DTO;

namespace BUS
{
    public interface IDanhMucTinTucService
    {
        Task<bool> Add(DanhMucTinTuc danhMucTinTuc);
        Task<List<DanhMuc>> LayToanBoDanhMucCuaTinTuc(int idTinTuc);
        Task<bool> XoaToanBoDanhMucCuaTinTuc(int idTinTuc);
        List<TinTuc> GetTinTucByListIdDanhMuc(List<int> idDanhMucs);
        Task<List<TinTuc>> LayNhungTinTucCoLienQuan(int idTinTuc);
    }
    public class DanhMucTinTucService : IDanhMucTinTucService
    {
        private readonly IDanhMucTinTucRepository danhMucTinTucRepository;
        private readonly IDanhMucRepository danhMucRepository;
        private readonly ITinTucRepository tinTucRepository;
        public DanhMucTinTucService(IDanhMucTinTucRepository danhMucTinTucRepository, IDanhMucRepository danhMucRepository, ITinTucRepository tinTucRepository)
        {
            this.danhMucTinTucRepository = danhMucTinTucRepository;
            this.danhMucRepository = danhMucRepository;
            this.tinTucRepository = tinTucRepository;
        }

        public async Task<bool> Add(DanhMucTinTuc danhMucTinTuc)
        {
            return await danhMucTinTucRepository.Add(danhMucTinTuc);
        }

        public async Task<List<DanhMuc>> LayToanBoDanhMucCuaTinTuc(int idTinTuc)
        {
            var danhSachDanhMuc = danhMucTinTucRepository.GetAll().Where(x => x.IDTintuc.Equals(idTinTuc)).Select(x => x.IDDanhMuc);
           
            List<DanhMuc> lst = new List<DanhMuc>();
            if(danhSachDanhMuc is null)
            {
                return null;
            }
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
        public List<TinTuc> GetTinTucByListIdDanhMuc(List<int> idDanhMucs)
        {
            var danhSachTinTucThoaDanhMuc = danhMucTinTucRepository.GetAll().Where(x => idDanhMucs.Contains(x.IDDanhMuc))
                .GroupBy(x => x.IDTintuc)
                .Select(x => new
                {
                    IDTinTuc = x.Key,
                    Count = x.Count()
                }).Where(x => x.Count == idDanhMucs.Count).Select(x => x.IDTinTuc);
            var danhSachTinTuc = tinTucRepository.GetAll();
            var joinTable = danhSachTinTuc.Join(danhSachTinTucThoaDanhMuc, x => x.IDTinTuc, y => y, (tinTuc, danhMucTinTuc) => new { TinTuc = tinTuc, DanhMucTinTuc = danhMucTinTuc })
                .Select(x => x.TinTuc)
                .ToList();
            return joinTable;
        }

        public async Task<List<TinTuc>> LayNhungTinTucCoLienQuan(int idTinTuc)
        {
            var danhMucTinTuc = (await LayToanBoDanhMucCuaTinTuc(idTinTuc)).Select(x => x.IDDanhMuc);
            var danhSachIdTinTucThoaDanhMuc = danhMucTinTucRepository.GetAll().Where(x => danhMucTinTuc.Contains(x.IDDanhMuc)).Select(x => x.IDTintuc).Distinct();
            var result = tinTucRepository.GetAll().Where(x => danhSachIdTinTucThoaDanhMuc.Contains(x.IDTinTuc) && x.IDTinTuc != idTinTuc && x.TrangThai.Equals(true)).ToList();
            return result;

        }
    }
}
