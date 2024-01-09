using DTO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TinTucViewModel
    {
        public int IDTinTuc { get; set; }
        [Required(ErrorMessage = "Tiêu đề bắt buộc phải có")]
        [DisplayName("Tiêu đề tin tức")]
        public string TieuDe { get; set; }
        public string? Url { get; set; }
        [DisplayName("Tóm tắt nội dung")]
        public string? TomTat { get; set; }
        [DisplayName("Nội dung tin tức")]
        public string? NoiDung { get; set; }
        [DisplayName("Ảnh bìa")]
       
        public string? HinhAnh { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? NgayTao { get; set; }
        [DisplayName("Ngày chỉnh sửa")]
        public DateTime? NgayUpdate { get; set; }
        [DisplayName("Lượt xem")]
        public int? LuotXem { get; set; }
        [DisplayName("Trạng thái")]
        public bool TrangThai { get; set; } = true;
        [Required(ErrorMessage = "Phải có ít nhất một danh mục")]
        public List<DanhMuc> DanhMucs { get; set; }
    }
}
