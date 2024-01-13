using AutoMapper;
using BUS;
using DAO;
using Demo;
using Demo.Controllers;
using Demo.Models;
using DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject
{
    public class TinTucTest
    {
        IMapper _mapper;
        Mock<IDanhMucRepository> _danhMucRepositoryMock;
        Mock<IDanhMucTinTucRepository> _danhMucTinTucRepositoryMock;
        Mock<ITinTucRepository> _tinTucRepositoryMock;
        Mock<IWebHostEnvironment> _webHostEnvMock;
        IDanhMucService _danhMucService;
        ITinTucService _tinTucService;
        IDanhMucTinTucService _danhMucTinTucService;
        TinTucController _tinTucController;
        [SetUp]
        public void Setup()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapping());
            }).CreateMapper();
            _danhMucRepositoryMock = new Mock<IDanhMucRepository> ();
            _danhMucTinTucRepositoryMock = new Mock<IDanhMucTinTucRepository>();
            _tinTucRepositoryMock = new Mock<ITinTucRepository> ();
            _webHostEnvMock = new Mock<IWebHostEnvironment>();
            InitService();
            InitController();
        }
        private void InitService()
        {
            _danhMucService = new DanhMucService(_danhMucRepositoryMock.Object);
            _tinTucService = new TinTucService(_tinTucRepositoryMock.Object);
            _danhMucTinTucService = new DanhMucTinTucService(_danhMucTinTucRepositoryMock.Object, _danhMucRepositoryMock.Object, _tinTucRepositoryMock.Object);
        }
        private void InitController()
        {
            var logger = new Mock<ILogger<TinTucController>>();
            _tinTucController = new TinTucController(logger.Object, _tinTucService, _danhMucService, _danhMucTinTucService, _mapper, _webHostEnvMock.Object);
        }
        [Test]
        public void GetAll_ReturnAllTinTuc()
        {
         
            _tinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>
        {
            new TinTuc { IDTinTuc = 1, TieuDe = "Test 1" },
            new TinTuc { IDTinTuc = 2, TieuDe = "Test 2" }
        });

            // Act
            var result = _tinTucService.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[1].IDTinTuc);
        }
        [Test]
        public async Task GetByID_ValidTinTucId_ReturnTinTuc()
        {
            int IDTinTuc = 39;
            _tinTucRepositoryMock.Setup(repo => repo.GetById(IDTinTuc)).ReturnsAsync(new TinTuc { IDTinTuc = IDTinTuc, TieuDe = "123123"});
            var result = await _tinTucService.GetByID(IDTinTuc);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.TieuDe, "123123");
            Assert.AreEqual(result.IDTinTuc, 39);
        }

        [Test]
        public async Task Update_ValidTinTuc_CallsRepositoryUpdate()
        {

            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            //_tinTucRepositoryMock.Setup(repo => repo.Update(exsitingTinTuc)).Verifiable();
            
            await _tinTucService.UpdateTinTuc(exsitingTinTuc);
            _tinTucRepositoryMock.Verify(repo => repo.Update(exsitingTinTuc), Times.Once);
   
        }
        [Test]
        public async Task Delete_ValidTinTuc_CallsRepositoryDelete()
        {

            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            var result = await _tinTucService.DeleteTinTuc(exsitingTinTuc);
            _tinTucRepositoryMock.Verify(repo => repo.Delete(exsitingTinTuc), Times.Once);

        }
        [Test]
        public async Task Add_ValidTinTuc_CallsRepositoryAdd()
        {

            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            var result = await _tinTucService.AddTinTuc(exsitingTinTuc);
            _tinTucRepositoryMock.Verify(repo => repo.Add(exsitingTinTuc), Times.Once);

        }
        [Test]
        public void TestXemToanBoTinTuc_TinTucController_ReturnAllTinTuc()
        {

            _tinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>
            {
                new TinTuc { IDTinTuc = 1, TieuDe = "Test 1" },
                new TinTuc { IDTinTuc = 2, TieuDe = "Test 2" }
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMuc>
            {
                new DanhMuc(){IDDanhMuc = 1, TenDanhMuc = "Anime"}
            });
            var result = _tinTucController.XemToanBoTinTuc() as ViewResult;
            var model = (List<TinTucViewModel>) result.Model;
            Assert.IsAssignableFrom<List<TinTucViewModel>>(result.ViewData.Model);
            Assert.IsNotNull(model);
            Assert.AreEqual(model[0].TieuDe, "Test 1");
            Assert.AreEqual(model[1].TieuDe, "Test 2");
           

        }
        [Test]
        public async Task TestXemTinTuc_TinTucController_ReturnTinTuc()
        {
            int idTinTuc = 1;

            _tinTucRepositoryMock.Setup(repo => repo.GetById(idTinTuc)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 1,
                TieuDe = "Test",
                TrangThai = true
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMuc>
            {
                new DanhMuc(){IDDanhMuc = 1, TenDanhMuc = "Anime"}
            });
            _danhMucTinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMucTinTuc>
            {
                new DanhMucTinTuc(){IDDanhMuc = 1, IDTintuc = 1}
            });
          
           
            var result = await _tinTucController.XemTinTuc(idTinTuc) as ViewResult;
            var model = (TinTucViewModel)result.Model;
            Assert.IsAssignableFrom<TinTucViewModel>(result.ViewData.Model);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.TieuDe, "Test");
           


        }
        [Test]
        public async Task TestAddTinTuc_TinTucController_ReturnNewModelToView()
        {
            int idTinTuc = 1;
            _tinTucRepositoryMock.Setup(repo => repo.GetById(idTinTuc)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 1,
                TieuDe = "Test",
                TrangThai = true
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMuc>
            {
                new DanhMuc(){IDDanhMuc = 1, TenDanhMuc = "Anime"}
            });
            _danhMucTinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMucTinTuc>
            {
                new DanhMucTinTuc(){IDDanhMuc = 1, IDTintuc = 1}
            });


           
            var result = await _tinTucController.AddOrEdit() as ViewResult;
            var model = (TinTucViewModel)result.Model;
            Assert.IsAssignableFrom<TinTucViewModel>(result.ViewData.Model);
            Assert.IsNotNull(model);
            Assert.IsNull(model.TieuDe);
        }
        [Test]
        public async Task TestEditTinTuc_TinTucController_NotFountTinTuc_ReturnNotFound()
        {
            int idTinTuc = 1; //fake id
            _tinTucRepositoryMock.Setup(repo => repo.GetById(2)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 2,
                TieuDe = "Test",
                TrangThai = true
            });
            _tinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>() 
            {
                new TinTuc()
                {
                    IDTinTuc = 2,
                    TieuDe = "Test",
                    TrangThai = true
                }
                
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMuc>
            {
                new DanhMuc(){IDDanhMuc = 1, TenDanhMuc = "Anime"}
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetById(1)).ReturnsAsync(new DanhMuc()
            {
                IDDanhMuc = 1, TenDanhMuc = "Anime"
            });
            _danhMucTinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMucTinTuc>
            {
                new DanhMucTinTuc(){IDDanhMuc = 1, IDTintuc = 2}
            });

            var result = await _tinTucController.AddOrEdit(idTinTuc);
            Assert.IsAssignableFrom<NotFoundResult>(result);
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task TestEditTinTuc_TinTucController_ValidTinTuc_ReturnView()
        {
            int idTinTuc = 2; //real id


            _tinTucRepositoryMock.Setup(repo => repo.GetById(2)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 2,
                TieuDe = "Test",
                TrangThai = true
            });
            _tinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>()
            {
                new TinTuc()
                {
                    IDTinTuc = 2,
                    TieuDe = "Test",
                    TrangThai = true
                }

            });
            _danhMucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMuc>
            {
                new DanhMuc(){IDDanhMuc = 1, TenDanhMuc = "Anime"}
            });
            _danhMucRepositoryMock.Setup(repo => repo.GetById(1)).ReturnsAsync(new DanhMuc()
            {
                IDDanhMuc = 1,
                TenDanhMuc = "Anime"
            });
            _danhMucTinTucRepositoryMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMucTinTuc>
            {
                new DanhMucTinTuc(){IDDanhMuc = 1, IDTintuc = 2}
            });
            var result = await _tinTucController.AddOrEdit(idTinTuc) as ViewResult;
            var model = result.ViewData.Model as TinTucViewModel;
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.TieuDe,"Test");
        }

        [Test]
        public async Task TestAddTinTucPost_TinTucController_ValidTinTuc_ReturnNewModelToView()
        {

          

            var model = new TinTucViewModel()
            {
                IDTinTuc = 0,
                TieuDe = "Test",
                NgayTao = DateTime.Today,
                NgayUpdate = DateTime.Today,
                DanhMucIds = new List<int>() {
                {
                    1
                } }
            };

            var result = await _tinTucController.AddOrEdit(0, model) as RedirectToActionResult;
            
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);

            Assert.AreEqual(result.ActionName, "XemToanBoTinTuc");
        }
    }
}