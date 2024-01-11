using AutoMapper;
using BUS;
using Demo.Models;
using DTO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITinTucService _tinTucService;
        private readonly IMapper _mapper;
        private readonly IDanhMucService _danhMucService;
        private readonly IDanhMucTinTucService _danhMuctinTucService;
        public HomeController(ILogger<HomeController> logger,
             ITinTucService tinTucService,
             IMapper mapper,
             IDanhMucService danhMucService,
             IDanhMucTinTucService danhMuctinTucService)
        {
            _logger = logger;
            _mapper = mapper;
            _tinTucService = tinTucService;
            _danhMucService = danhMucService;
            _danhMuctinTucService = danhMuctinTucService;
        }
        [HttpGet]
       
        public IActionResult Index(List<int> danhSachDanhMuc = null)
        {
            ViewBag.DanhMuc = _danhMucService.GetAll() != null ? _danhMucService.GetAll() : new List<DanhMuc>();
            ViewBag.DanhSachDanhMuc = danhSachDanhMuc;
            var result = new List<TinTucViewModel>();
            if (danhSachDanhMuc is not null)
            {
                result = _mapper.Map<List<TinTucViewModel>>(_danhMuctinTucService.GetTinTucByListIdDanhMuc(danhSachDanhMuc).OrderByDescending(x => x.NgayUpdate)).ToList();
            }
            else
            {
                result = _mapper.Map<List<TinTucViewModel>>(_tinTucService.GetAll().OrderByDescending(x => x.NgayUpdate)).ToList();
            }
            return View(result);
        }
    
    }
}
