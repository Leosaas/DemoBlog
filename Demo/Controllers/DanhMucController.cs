
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
    public class DanhMucController : Controller
    {
        private readonly ILogger<DanhMucController> _logger;
        private readonly IMapper _mapper;
        private readonly IDanhMucService _danhMucService;
        public DanhMucController(ILogger<DanhMucController> logger,
        IDanhMucService danhMucService,
        IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _danhMucService = danhMucService;
        }
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            DanhMucViewModel data = new DanhMucViewModel();
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI DANH MỤC" : "CẬP NHẬT DANH MỤC";
            if (id != 0)
            {
                DanhMuc res = await _danhMucService.GetByID(id);
                if (res is null)
                {
                    return NotFound();
                }
                data = _mapper.Map<DanhMucViewModel>(res);        
            }
            return View(data);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, DanhMucViewModel data)
        {
            ViewBag.RenderedHtmlTitle = id == 0 ? "THÊM MỚI DANH MỤC" : "CẬP NHẬT DANH MỤC";
            if (ModelState.IsValid)
            {
                bool result;
                DanhMuc res = _mapper.Map<DanhMuc>(data);
                try
                {
                    result = id != 0 ? await _danhMucService.UpdateDanhMuc(res) : await _danhMucService.AddDanhMuc(res);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
                return result ? RedirectToAction("XemToanBoDanhMuc") : ValidationProblem();
            }
            return View(data);
        }
        [HttpGet]
        public IActionResult XemToanBoDanhMuc()
        {
            var result = _mapper.Map<List<DanhMucViewModel>>(_danhMucService.GetAll()).ToList();
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            DanhMuc res = await _danhMucService.GetByID(id);
            var result = await _danhMucService.DeleteDanhMuc(res);
            return result ? RedirectToAction("XemToanBoDanhMuc") : ValidationProblem();
        }
 
    }
}
