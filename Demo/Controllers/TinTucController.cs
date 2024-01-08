
using AutoMapper;
using BUS;
using DAO;
using Demo.Models;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {

            TinTucViewModel data = new TinTucViewModel();
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI TIN TỨC" : "CẬP NHẬT TIN TỨC";
            ViewBag.DanhMuc = _danhMucService.GetAll();
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
                data.DanhMucs = await _danhMucTinTucService.LayToanBoDanhMucCuaTinTuc(id);
            return View(data);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> UploadImage([FromForm] IFormFile upload)
        {
            if (upload.Length <= 0) return null;

            //your custom code logic here

            //1)check if the file is image

            //2)check if the file is too large

            //etc

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();

            //save file under wwwroot/CKEditorImages folder

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/CKEditorImages",
                fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await upload.CopyToAsync(stream);
            }

            var url = $"{"/CKEditorImages/"}{fileName}";

            var success = new uploadsuccess
            {
                Uploaded = 1,
                FileName = fileName,
                Url = url
            };

            return new JsonResult(success);
        }
        public class uploadsuccess
        {
            public int Uploaded { get; set; }
            public string FileName { get; set; }
            public string Url { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, TinTucViewModel data, IFormFile ImageData)
        {
            
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI TIN TỨC" : "CẬP NHẬT TIN TỨC";
            ViewBag.DanhMuc = _danhMucService.GetAll();
            
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
                    res.NoiDung = Request.Form["textBox"].ToString();
                        res.NgayUpdate = DateTime.Now;
                        await _danhMucTinTucService.XoaToanBoDanhMucCuaTinTuc(id);
                        await _tinTucService.UpdateTinTuc(res);
                        if (data.DanhMucs != null)
                        {
                        var dataSelect = Request.Form["DanhMucs"].ToString().Split(",");

                        foreach (var d in dataSelect)
                        {
                            DanhMucTinTuc dm = new DanhMucTinTuc()
                            {
                                IDDanhMuc = int.Parse(d),
                                IDTintuc = id
                            };
                            await _danhMucTinTucService.Add(dm);
                        }
                    }
                }
                    else
                    {

                    res.NoiDung = Request.Form["textBox"].ToString();
                    res.NgayTao = DateTime.Now;
                        res.NgayUpdate = DateTime.Now;
                        await _tinTucService.AddTinTuc(res);

                    int lastId = _tinTucService.GetAll().ToList().OrderByDescending(x => x.IDTinTuc).First().IDTinTuc;
                    res.Url = "localhost:7117/TinTuc/XemTinTuc/" + lastId;
                    await _tinTucService.UpdateTinTuc(res);
                    if (data.DanhMucs != null)
                    {
                        var dataSelect = Request.Form["DanhMucs"].ToString().Split(",");

                        foreach (var d in dataSelect)
                        {
                            DanhMucTinTuc dm = new DanhMucTinTuc()
                            {
                                IDDanhMuc = int.Parse(d),
                                IDTintuc = lastId
                            };
                            await _danhMucTinTucService.Add(dm);
                        }
                    }
                }
                }
                catch (DbUpdateConcurrencyException)
                {
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
        public IActionResult XemToanBoTinTuc()
        {
            //List<TinTucViewModel> danhSachTinTuc = _mapper.Map<List<TinTucViewModel>>(_tinTucService.GetAll());
             return View(_tinTucService.GetAll());
   
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
            
            await _tinTucService.DeleteTinTuc(res);

            return RedirectToAction("XemToanBoTinTuc");
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
                return RedirectToAction("Index", "Home");
            }
            var modelView = _mapper.Map<TinTucViewModel>(data);
            return View(modelView);
        }
    }
}
