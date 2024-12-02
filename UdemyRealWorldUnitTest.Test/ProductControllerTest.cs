using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using UdemyRealWordUnitTest.Web.Controllers;
using UdemyRealWordUnitTest.Web.Models;
using UdemyRealWordUnitTest.Web.Repository;
using Xunit;

namespace RealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private readonly List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);

            _products = new List<Product>()
            {
                new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product { Id = 2, Name = "Silgi", Price = 100, Stock = 50, Color = "Beyaz" }
            };
        }

        [Fact]
        public async Task Index_ActionExecutes_ReturnViewResult()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result); // ViewResult dönüp dönmediğini kontrol et
        }

        [Fact]
        public async Task Index_ActionExecutes_ReturnProductList()
        {
            // Mock setup (Repository'den ürünlerin döndüğünü simüle etme)
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToAction()
        {
            var result = await _controller.Details(null);

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);

            var result = await _controller.Details(0);

            var redirect = Assert.IsType<NotFoundResult>(result);

            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public async void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name Alanı Gerekli");

            var result = await _controller.Create(_products.First());

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_ReturnRedirectToActionIndex()
        {
            var result = await _controller.Create(_products.First());

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_ValidModelState_CreateMethodExecute()
        {
            Product product = null;

            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);

            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once());

            Assert.Equal(_products.First().Id, product.Id);
        }

        [Fact]
        public async void CreatePOST_InValidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");

            var result = await _controller.Create(_products.First());

            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never());
        }
    }
}
