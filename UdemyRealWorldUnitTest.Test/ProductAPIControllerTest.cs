using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWordUnitTest.Web.Controllers;
using UdemyRealWordUnitTest.Web.Models;
using UdemyRealWordUnitTest.Web.Repository;
using UdemyRealWorldUnitTest.Web.Controllers;
using Xunit;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductAPIControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsAPIController _controller;
        private List<Product> _products;

        public ProductAPIControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsAPIController(_mockRepo.Object);

            _products = new List<Product>()
            {
                new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product { Id = 2, Name = "Silgi", Price = 100, Stock = 50, Color = "Beyaz" }
            };
        }

        [Fact]
        public async void GetProduct_ActionExecutes_ReturnOkResultWithProduct()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);

            var result = await _controller.GetProduct();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2,returnProduct.ToList().Count);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnOkResult(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);

            var result = await _controller.GetProduct(productId);

            var okResult= Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsType<Product>(okResult.Value);

            Assert.Equal(productId, returnProduct.Id);
        }



    }
}
