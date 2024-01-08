using DAO;
using DTO;

namespace BUS
{
    public interface ITinTucService
    {
        Task<TinTuc> GetByID(int id);
        List<TinTuc> GetAll();
        Task<bool> AddTinTuc(TinTuc tinTuc);
        Task<bool> UpdateTinTuc(TinTuc tinTuc);
        Task<bool> DeleteTinTuc(TinTuc tinTuc);

    }
    public class TinTucService : ITinTucService
    {
        private readonly ITinTucRepository tinTucRepository;
  
        public TinTucService(ITinTucRepository tinTucRepository)
        {
            this.tinTucRepository = tinTucRepository;
 
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

        public List<TinTuc> GetAll()
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
