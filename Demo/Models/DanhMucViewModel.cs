using DTO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class DanhMucViewModel
    {
        public int IDDanhMuc { get; set; }
        [Required(ErrorMessage = "Tên bắt buộc phải có")]
        [DisplayName("Tên danh mục")]
        public string TenDanhMuc { get; set; }
        [DisplayName("Trạng thái")]
        public bool TrangThai { get; set; } = true;
    }
}
