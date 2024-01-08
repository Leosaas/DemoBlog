using BUS;
using DAO;
using DTO;
using Moq;

namespace TestProject
{
    public class TinTucTest
    {
        [SetUp]
        public void Setup()
        {
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

    }
}