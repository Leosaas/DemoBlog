using DAO;
using DTO;

namespace BUS
{
    public interface ITinTucService
    {
        Task<TinTuc> GetByID(int id);
        IQueryable<TinTuc> GetAll();
        Task<bool> AddTinTuc(TinTuc tinTuc);
        Task<bool> UpdateTinTuc(TinTuc tinTuc);
        Task<bool> DeleteTinTuc(TinTuc tinTuc);

    }
    public class TinTucService : ITinTucService
    {
        private readonly ITinTucRepository tinTucRepository;
        private readonly IDanhMucTinTucRepository danhMucTinTucRepository;
        public TinTucService(ITinTucRepository tinTucRepository, IDanhMucTinTucRepository danhMucTinTucRepository)
        {
            this.tinTucRepository = tinTucRepository;
            this.danhMucTinTucRepository = danhMucTinTucRepository;
        }

        public async Task<bool> AddTinTuc(TinTuc tinTuc)
        {
            bool success = await tinTucRepository.Add(tinTuc);
            //DanhMucTinTuc danhMucTinTuc = new DanhMucTinTuc()
            //{
            //    IDDanhMuc = idDanhMuc,
            //    IDTintuc = tinTuc.IDTintuc

            //};
            //success = await danhMucTinTucRepository.Add(danhMucTinTuc);
            return success;
        }

        public async Task<bool> DeleteTinTuc(TinTuc tinTuc)
        {
            return await tinTucRepository.Delete(tinTuc);   
        }

        public IQueryable<TinTuc> GetAll()
        {
            return tinTucRepository.GetAll();
        }

        public async Task<TinTuc> GetByID(int id)
        {
            return await tinTucRepository.GetById(id);
        }

        public async Task<bool> UpdateTinTuc(TinTuc tinTuc)
        {
            return await tinTucRepository.Update(tinTuc);
        }
    }
}
