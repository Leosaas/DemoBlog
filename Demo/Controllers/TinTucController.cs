
using AutoMapper;
using BUS;
using DAO;
using Demo.Models;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Demo.Controllers
{
    public class TinTucController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ILogger<TinTucController> _logger;
        private readonly ITinTucService _tinTucService;
        private readonly IMapper _mapper;
        private readonly IDanhMucService _danhMucService;
        private readonly IDanhMucTinTucService _danhMucTinTucService;
        public TinTucController(ILogger<TinTucController> logger,
        ITinTucService tinTucService,
        IDanhMucService danhMucService,
        IDanhMucTinTucService danhMucTinTucService,
        IMapper mapper,
        IWebHostEnvironment hostingEnv)
        {
            _logger = logger;
            _mapper = mapper;
            _tinTucService = tinTucService;
            _danhMucService = danhMucService;
            _hostingEnv = hostingEnv;
            _danhMucTinTucService = danhMucTinTucService;
        }
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            TinTucViewModel data = new TinTucViewModel();
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI TIN TỨC" : "CẬP NHẬT TIN TỨC";
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll().ToList() : new List<DanhMuc>();
            if (id != 0) //Trường hợp cập nhật
            {
                TinTuc res = await _tinTucService.GetByID(id);
                if (res is null)
                {
                    return NotFound(); //Trả về not found nếu không tìm thấy tin tức
                }
                data = _mapper.Map<TinTucViewModel>(res);
                var danhMuc = await _danhMucTinTucService.LayToanBoDanhMucCuaTinTuc(id);
                data.DanhMucIds = danhMuc is not null ? danhMuc.Select(x => x.IDDanhMuc).ToList() : new List<int>();
            } 
            return View(data);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, TinTucViewModel data, IFormFile ImageData = null)
        {
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI TIN TỨC" : "CẬP NHẬT TIN TỨC";
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll().ToList() : new List<DanhMuc>();
            //data.NoiDung = Request.Form["textBox"].ToString();
            if (ModelState.IsValid)
            {
                TinTuc res = _mapper.Map<TinTuc>(data);
                try
                {
                    if (ImageData != null) //Image data là dữ liệu từ ảnh bìa
                    {
                        string path = DateTime.Now.Ticks.ToString() + "_" + ImageData.FileName;
                        string uploads = Path.Combine(_hostingEnv.WebRootPath, "uploads");
                        string filePath = Path.Combine(uploads, path);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageData.CopyToAsync(fileSteam); //Lưu hình ảnh về server 
                        }
                        string relativePath = "~/uploads/" + path; //lấy đường dẫn tương đối gán vào model
                        res.HinhAnh = relativePath;
                    }
                    if (id != 0)
                    {
                        res.NgayUpdate = DateTime.Now;
                        await _danhMucTinTucService.XoaToanBoDanhMucCuaTinTuc(id); //Trường hợp có cập nhật danh mục cho tin tức, cần xoá toàn bộ danh mục cũ và thêm vào các danh mục mới
                        await _tinTucService.UpdateTinTuc(res);
                        if (data.DanhMucIds != null)
                        {
                            foreach (var d in data.DanhMucIds)
                            {
                                DanhMucTinTuc dm = new DanhMucTinTuc()
                                {
                                    IDDanhMuc = d,
                                    IDTintuc = id
                                };
                                await _danhMucTinTucService.Add(dm);
                            }
                        }
                    }
                    else
                    {
                        res.NgayTao = DateTime.Now;
                        res.NgayUpdate = DateTime.Now;
                        await _tinTucService.AddTinTuc(res);
                        int lastId = 0; 
                        if(_tinTucService.GetAll()?.Count > 0)
                        {
                            lastId = _tinTucService.GetAll().ToList().OrderByDescending(x => x.IDTinTuc).First().IDTinTuc; //Lấy id của tin tức vừa thêm vào
                        }
                        
                        res.Url = "https://localhost:7117/TinTuc/XemTinTuc/" + lastId;
                        await _tinTucService.UpdateTinTuc(res);
                        if (data.DanhMucIds != null)
                        {
                            foreach (var d in data.DanhMucIds)
                            {
                                DanhMucTinTuc dm = new DanhMucTinTuc()
                                {
                                    IDDanhMuc = d,
                                    IDTintuc = lastId
                                };
                                await _danhMucTinTucService.Add(dm);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    
                    return BadRequest();
                }
                return RedirectToAction("XemToanBoTinTuc");
            }
            return View(data);
        }
        [HttpGet]
        public IActionResult XemToanBoTinTuc(List<int> danhSachDanhMuc = null)
        {
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll().ToList() : new List<DanhMuc>();
            ViewBag.DanhSachDanhMuc = danhSachDanhMuc;
            var result = danhSachDanhMuc is not null
                ? _mapper.Map<List<TinTucViewModel>>(_danhMucTinTucService.GetTinTucByListIdDanhMuc(danhSachDanhMuc)).ToList()
                : _mapper.Map<List<TinTucViewModel>>(_tinTucService.GetAll()).ToList();
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            TinTuc res = await _tinTucService.GetByID(id);
            string webRootPath = _hostingEnv.WebRootPath;
            if(res.HinhAnh != null)
            {
                string imgPath = res.HinhAnh.Split("~")[1];
                var fullPath = webRootPath + imgPath;

                if (System.IO.File.Exists(fullPath))
                {

                    System.IO.File.Delete(fullPath); //Xoá hình ảnh đã được lưu ở server
                }
            }
            bool result = await _danhMucTinTucService.XoaToanBoDanhMucCuaTinTuc(id) && await _tinTucService.DeleteTinTuc(res);
            return result ? RedirectToAction("XemToanBoTinTuc") : BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> XemTinTuc(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Home"); //Tin không hợp lệ
            }
            var data = await _tinTucService.GetByID(id);
            if(data == null)
            {
                return NotFound("Tin này không thể tìm thấy");
            }
            var tinKhaDung = await _danhMucTinTucService.LayToanBoTinTucKhaDungTheoDanhMuc();
            if(data.TrangThai == false || !tinKhaDung.Contains(data))
            {
                return BadRequest("Tin này đã bị vô hiệu hoá");
            }
            var soLuongTinTuc = _tinTucService.GetAll() is null ? 0 : _tinTucService.GetAll().Count();
            ViewBag.TinCoLienQuan = new List<TinTucViewModel>();
            var danhMucCuaTinTuc = (await _danhMucTinTucService.LayToanBoDanhMucCuaTinTuc(id)).Select(x => x.IDDanhMuc);
            if (soLuongTinTuc > 1) //Ít nhất 2 tin tức để 1 cái gợi ý cho cái còn lại
            {
                var tinCoLienQuan = (await _danhMucTinTucService.LayNhungTinTucCoLienQuan(id))
                    .OrderBy(x => x.LuotXem) //Sort by luọt xem
                    .Take(3).ToList(); //Lấy tối đa 3 record
                var tinCoLienQuanModelView = _mapper.Map<List<TinTucViewModel>>(tinCoLienQuan);
                ViewBag.TinCoLienQuan = tinCoLienQuanModelView;
            }
            data.LuotXem += 1;
            await _tinTucService.UpdateTinTuc(data);
            var modelView = _mapper.Map<TinTucViewModel>(data);
            return View(modelView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadImage(IFormFile upload)
        {
            if (upload.Length <= 0) return null;

            //your custom code logic here

            //1)check if the file is image

            //2)check if the file is too large

            //etc

           

            //save file under wwwroot/CKEditorImages folder
            string path = DateTime.Now.Ticks.ToString() + "_" + upload.FileName;
            string uploads = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnv.WebRootPath, "uploads");
            string filePath = Path.Combine(uploads, path);
            using (var fileSteam = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(fileSteam); //Lưu hình ảnh về server 
            }

            var url = $"{"/CKEditorImages/"}{path}";

            var successMessage = "image is uploaded";

            dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + path + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
            return Json(success);
        }
    }
}
