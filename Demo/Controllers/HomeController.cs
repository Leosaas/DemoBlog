using AutoMapper;
using BUS;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITinTucService _tinTucService;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger,
             ITinTucService tinTucService,
             IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _tinTucService = tinTucService;  
        }

        public IActionResult Index()
        {
         
            return View(_mapper.Map<List<TinTucViewModel>>(_tinTucService.GetAll()));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
