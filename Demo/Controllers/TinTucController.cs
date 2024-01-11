
using AutoMapper;
using BUS;
using DAO;
using Demo.Models;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Data.SqlTypes;
using System.Diagnostics;

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
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll() : new List<DanhMuc>();

            if (id != 0)
            {
                TinTuc res = await _tinTucService.GetByID(id);
                data = _mapper.Map<TinTucViewModel>(res);
                if (data == null)
                {
                    return NotFound();
                }
            }
            if(id != 0)
            {
                var danhMuc = await _danhMucTinTucService.LayToanBoDanhMucCuaTinTuc(id);

                data.DanhMucIds = danhMuc != null ? danhMuc.Select(x=> x.IDDanhMuc).ToList() : [];
            }
            return View(data);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, TinTucViewModel data, IFormFile ImageData = null)
        {
            
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI TIN TỨC" : "CẬP NHẬT TIN TỨC";
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll() : new List<DanhMuc>();
            //data.NoiDung = Request.Form["textBox"].ToString();
            if (ModelState.IsValid)
            {
                TinTuc res = _mapper.Map<TinTuc>(data);
                try
                {

                    //if (ImageData != null)
                    //{

                    //    using (var memoryStream = new MemoryStream())
                    //    {
                    //        //Stream stream = ImageData.OpenReadStream();
                    //        //stream.CopyTo(memoryStream);
                    //        ImageData.CopyTo(memoryStream);
                    //        byte[] hinhAnhTinTuc = memoryStream.ToArray();
                    //        res.HinhAnh = Convert.ToBase64String(hinhAnhTinTuc);
                    //    }
                    //}
                    if (ImageData != null)
                    {

                        string path = DateTime.Now.Ticks.ToString() + "_" + ImageData.FileName;
                        string uploads = Path.Combine(_hostingEnv.WebRootPath, "uploads");
                        string filePath = Path.Combine(uploads, path);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageData.CopyToAsync(fileSteam);
                        }
                        string relativePath = "~/uploads/" + path;
                        res.HinhAnh = relativePath;

                    }





                    if (id != 0)
                    {
                        //if (image == null && data.HinhAnh != null)
                        //{
                        //    //	byte[] dbImage = productService.GetProduct(id).pImage;
                        //    byte[] dbImage = Convert.FromBase64String(data.ImageByBase64);
                        //    res.pImage = dbImage;
                        //}
                        res.NgayUpdate = DateTime.Now;
                        await _danhMucTinTucService.XoaToanBoDanhMucCuaTinTuc(id);
                        await _tinTucService.UpdateTinTuc(res);
                        if (data.DanhMucIds != null)
                        {
                           // var dataSelect = Request.Form["DanhMucs"].ToString().Split(",");

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
                            lastId = _tinTucService.GetAll().ToList().OrderByDescending(x => x.IDTinTuc).First().IDTinTuc;
                        }
                        
                        res.Url = "https://localhost:7117/TinTuc/XemTinTuc/" + lastId;
                        await _tinTucService.UpdateTinTuc(res);
                        if (data.DanhMucIds != null)
                        {
                           // var dataSelect = Request.Form["DanhMucs"].ToString().Split(",");

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
                    Console.WriteLine(e.StackTrace);
                    return NotFound();
                }



                // return RedirectToAction("XemToanBoTinTuc", "TinTuc");

                //data.Units = unitService.GetUnits();
                //data.Categories = categoryService.GetCategories();
                //if (!data.Units.Any() || !data.Categories.Any())
                //{
                //    return Content("Không thể thêm sản phẩm do chưa có đơn vị tính hoặc loại rau củ");
                //}
                return RedirectToAction("XemToanBoTinTuc");
            }
            return View(data);
        }
        [HttpGet]
        public IActionResult XemToanBoTinTuc(List<int> danhSachDanhMuc = null)
        {
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll() : new List<DanhMuc>();
            ViewBag.DanhSachDanhMuc = danhSachDanhMuc;
            var result = new List<TinTucViewModel>();
            if (danhSachDanhMuc is not null)
            {
                result = _mapper.Map<List<TinTucViewModel>>(_danhMucTinTucService.GetTinTucByListIdDanhMuc(danhSachDanhMuc)).ToList();
            }
            else
            {
                result = _mapper.Map<List<TinTucViewModel>>(_tinTucService.GetAll()).ToList();
            }
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

                    System.IO.File.Delete(fullPath);
                }
            }
            bool result = await _danhMucTinTucService.XoaToanBoDanhMucCuaTinTuc(id);
            result = await _tinTucService.DeleteTinTuc(res);

            return result ? RedirectToAction("XemToanBoTinTuc") : BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> XemTinTuc(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Home");
            }
        
            
            
            
            var data = await _tinTucService.GetByID(id);
            if(data == null)
            {
                return NotFound("Tin này không thể tìm thấy");
            }
            if(data.TrangThai == false)
            {
                return BadRequest("Tin này đã bị vô hiệu hoá");
            }
            var soLuongTinTuc = _tinTucService.GetAll() == null ? 0 : _tinTucService.GetAll().Count();
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
    }
}
