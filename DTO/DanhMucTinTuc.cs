using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DTO
{
    [Table("DanhMucTinTuc")]
    [PrimaryKey(nameof(IDDanhMuc), nameof(IDTintuc))]
  
    public class DanhMucTinTuc
    {

        public int IDDanhMuc { get; set; }
        //public DanhMuc DanhMuc { get; set; }

        public int IDTintuc { get; set; }
        //public TinTuc TinTuc { get; set; }
  
    }
}
