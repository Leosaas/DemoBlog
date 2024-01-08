using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO
{
    [Table("DanhMuc")]
    public class DanhMuc
    {
        [Key]
        public int IDDanhMuc { get; set; }
        public string? TenDanhMuc { get; set; }
        public bool TrangThai { get; set; }
   //     public List<DanhMucTinTuc> DanhMucTinTucs { get; set; }
    }
}
