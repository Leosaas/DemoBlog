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
        private ILogger _logger;
        private IMapper _mapper;
        [SetUp]
        public void Setup()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapping());
            }).CreateMapper();
        }

        [Test]
        public void GetAll_ReturnAllTinTuc()
        {
            var tinTucRepoMock = new Mock<ITinTucRepository>();
            tinTucRepoMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>
        {
            new TinTuc { IDTinTuc = 1, TieuDe = "Test 1" },
            new TinTuc { IDTinTuc = 2, TieuDe = "Test 2" }
        });
            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            // Act
            var result = tinTucService.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[1].IDTinTuc);
        }
        [Test]
        public async Task GetByID_ValidTinTucId_ReturnTinTuc()
        {
            int IDTinTuc = 39;
            var tinTucRepoMock = new Mock<ITinTucRepository>();
            tinTucRepoMock.Setup(repo => repo.GetById(IDTinTuc)).ReturnsAsync(new TinTuc { IDTinTuc = IDTinTuc, TieuDe = "123123"});
            var tinTucService = new TinTucService(tinTucRepoMock.Object);
            var result = await tinTucService.GetByID(IDTinTuc);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.TieuDe, "123123");
            Assert.AreEqual(result.IDTinTuc, 39);
        }

        [Test]
        public async Task Update_ValidTinTuc_CallsRepositoryUpdate()
        {

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var tinTucService = new TinTucService(tinTucRepoMock.Object);
            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            var result = await tinTucService.UpdateTinTuc(exsitingTinTuc);
            tinTucRepoMock.Verify(repo => repo.Update(exsitingTinTuc), Times.Once);
            
        }
        [Test]
        public async Task Delete_ValidTinTuc_CallsRepositoryDelete()
        {

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var tinTucService = new TinTucService(tinTucRepoMock.Object);
            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            var result = await tinTucService.DeleteTinTuc(exsitingTinTuc);
            tinTucRepoMock.Verify(repo => repo.Delete(exsitingTinTuc), Times.Once);

        }
        [Test]
        public async Task Add_ValidTinTuc_CallsRepositoryAdd()
        {

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var tinTucService = new TinTucService(tinTucRepoMock.Object);
            var exsitingTinTuc = new TinTuc { IDTinTuc = 12, TieuDe = "John Doe" };
            var result = await tinTucService.AddTinTuc(exsitingTinTuc);
            tinTucRepoMock.Verify(repo => repo.Add(exsitingTinTuc), Times.Once);

        }
        [Test]
        public void TestXemToanBoTinTuc_TinTucController_ReturnAllTinTuc()
        {

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            tinTucRepoMock.Setup(repo => repo.GetAll()).Returns(new List<TinTuc>
        {
            new TinTuc { IDTinTuc = 1, TieuDe = "Test 1" },
            new TinTuc { IDTinTuc = 2, TieuDe = "Test 2" }
        });
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();
            

            var tinTucService = new TinTucService(tinTucRepoMock.Object);
            
            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object,danhMucRepoMock.Object);


            var controller = new TinTucController(logger.Object,tinTucService,danhMucService,danhMucTinTucService,_mapper,webHostEnv.Object);
            var result = controller.XemToanBoTinTuc() as ViewResult;
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
            var tinTucRepoMock = new Mock<ITinTucRepository>();
            tinTucRepoMock.Setup(repo => repo.GetById(idTinTuc)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 1,
                TieuDe = "Test",
                TrangThai = true
            }) ;
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();


            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object, danhMucRepoMock.Object);


            var controller = new TinTucController(logger.Object, tinTucService, danhMucService, danhMucTinTucService, _mapper, webHostEnv.Object);
            var result = await controller.XemTinTuc(idTinTuc) as ViewResult;
            var model = (TinTucViewModel)result.Model;
            Assert.IsAssignableFrom<TinTucViewModel>(result.ViewData.Model);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.TieuDe, "Test");
           


        }
        [Test]
        public async Task TestAddTinTuc_TinTucController_ReturnNewModelToView()
        {
         
            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();


            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object, danhMucRepoMock.Object);


            var controller = new TinTucController(logger.Object, tinTucService, danhMucService, danhMucTinTucService, _mapper, webHostEnv.Object);
            var result = await controller.AddOrEdit() as ViewResult;
            var model = (TinTucViewModel)result.Model;
            Assert.IsAssignableFrom<TinTucViewModel>(result.ViewData.Model);
            Assert.IsNotNull(model);
            Assert.IsNull(model.TieuDe);
        }
        [Test]
        public async Task TestEditTinTuc_TinTucController_NotFountTinTuc_ReturnNotFound()
        {
            int idTinTuc = 1; //fake id
            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();


            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object, danhMucRepoMock.Object);


            var controller = new TinTucController(logger.Object, tinTucService, danhMucService, danhMucTinTucService, _mapper, webHostEnv.Object);
            var result = await controller.AddOrEdit(idTinTuc) as NotFoundResult;
            
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task TestEditTinTuc_TinTucController_ValidTinTuc_ReturnView()
        {
            int idTinTuc = 1; //fake id

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            tinTucRepoMock.Setup(repo => repo.GetById(idTinTuc)).ReturnsAsync(new TinTuc()
            {
                IDTinTuc = 1,
                TieuDe = "Test",
                TrangThai = true
            });
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            danhMucTinTucRepoMock.Setup(repo => repo.GetAll()).Returns(new List<DanhMucTinTuc>()
            {
                new DanhMucTinTuc(){ IDDanhMuc = 1, IDTintuc = 1}
            });
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();


            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object, danhMucRepoMock.Object);


            var controller = new TinTucController(logger.Object, tinTucService, danhMucService, danhMucTinTucService, _mapper, webHostEnv.Object);
            var result = await controller.AddOrEdit(idTinTuc) as ViewResult;
            var model = result.ViewData.Model as TinTucViewModel;
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.TieuDe,"Test");
        }

        [Test]
        public async Task TestAddTinTucPost_TinTucController_ValidTinTuc_ReturnNewModelToView()
        {

            var tinTucRepoMock = new Mock<ITinTucRepository>();
            var danhMucRepoMock = new Mock<IDanhMucRepository>();
            var danhMucTinTucRepoMock = new Mock<IDanhMucTinTucRepository>();
            var logger = new Mock<ILogger<TinTucController>>();
            var webHostEnv = new Mock<IWebHostEnvironment>();

            
            var tinTucService = new TinTucService(tinTucRepoMock.Object);

            var danhMucService = new DanhMucService(danhMucRepoMock.Object);
            var danhMucTinTucService = new DanhMucTinTucService(danhMucTinTucRepoMock.Object, danhMucRepoMock.Object);

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

            var controller = new TinTucController(logger.Object, tinTucService, danhMucService, danhMucTinTucService, _mapper, webHostEnv.Object);
            var result = await controller.AddOrEdit(0, model) as RedirectToActionResult;
            
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);

            Assert.AreEqual(result.ActionName, "XemToanBoTinTuc");
        }
    }
}