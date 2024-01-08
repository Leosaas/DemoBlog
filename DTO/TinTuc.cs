using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO
{
    [Table("TinTuc")]
    public class TinTuc
    {
        [Key]
        public int IDTinTuc { get; set; }
        public string TieuDe { get; set; }
        public string? Url { get; set; }
        public string? TomTat { get; set; }
        public string? NoiDung { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime NgayTao { get; set; }

        public DateTime NgayUpdate { get; set; }
        public int LuotXem { get; set; }
        public bool TrangThai { get; set; }
      //  public List<DanhMucTinTuc> DanhMucTinTucs { get; set; }    
        
    }
}
