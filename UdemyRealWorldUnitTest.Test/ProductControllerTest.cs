using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
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

            Assert.Equal<int>(2,productList.Count());
        }
    }
}
