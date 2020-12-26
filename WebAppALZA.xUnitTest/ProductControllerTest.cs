using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using WebAppALZA.API;
using WebAppALZA.API.Controllers;
using WebAppALZA.API.Models;
using WebAppALZA.API.Repositories;
using Xunit;

namespace WebAppALZA.xUnitTest
{
    public class ProductControllerTest
    { 
        private ProductController _controller;
        private Mock<IProductRepository> _mockRepository;

        private List<ProductVM> repositoryData;

        private List<Product> TestData => new List<Product>()
            {
                new Product() { Id = 1, Name = "Name1", ImgPath = "Image/1.jpg", Price = 1.00M, Description = null },
                new Product() { Id = 2, Name = "Name2", ImgPath = "Image/2.jpg", Price = 2.50M, Description = "Description2" },
                new Product() { Id = 3, Name = "Name3", ImgPath = "Image/3.jpg", Price = 33.60M, Description = "Description3" },
                new Product() { Id = 4, Name = "Name4", ImgPath = "Image/4.jpg", Price = 4.20M, Description = "Description4" },
                new Product() { Id = 5, Name = "Name5", ImgPath = "Image/5.jpg", Price = 555.99M, Description = null },
                new Product() { Id = 6, Name = "Name6", ImgPath = "Image/6.jpg", Price = 6.10M, Description = "Description5" }
            };

        public ProductControllerTest()
        {
            _mockRepository = new Mock<IProductRepository>();

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string dataType = config["DataType"];

            if (String.IsNullOrWhiteSpace(dataType) || dataType == "1") repositoryData = Util.GetProductsMapper(TestData);            
            else repositoryData = Util.GetProductsMapper(GetDBData(config["ConnectionString"]));                         
        }
        
        private List<Product> GetDBData(string connectionString)
        {
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options;
                AppDbContext appDbContext = new AppDbContext(options);
                return new List<Product>(appDbContext.Products);
            }
            catch 
            {
                return TestData;
            }            
        }

        #region Products method
        [Fact]
        public void Products_NotFoundResult()
        {
            // Arrange
            List<ProductVM> products = null;
            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetProductsAsync();

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Products_OkResult()
        {
            // Arrange
            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(repositoryData);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetProductsAsync();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
        
        [Fact]
        public void Products_AllItems()
        {
            // Arrange
            var countItems = repositoryData.Count;
            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(repositoryData);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetProductsAsync().Result as OkObjectResult;

            // Assert
            var items = Assert.IsType<List<ProductVM>>(okResult.Value);
            Assert.Equal(countItems, items.Count);
        }
        #endregion

        #region Product method
        [Fact]
        public void Product_NotFoundResult()
        {
            // Arrange
            int invalidID = 0;              
            ProductVM product = repositoryData[0];
            _mockRepository.Setup(m => m.GetProductAsync(1)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetProductAsync(invalidID);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Product_OkResult()
        {
            // Arrange
            ProductVM product = repositoryData[0];           
            _mockRepository.Setup(m => m.GetProductAsync(product.Id)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetProductAsync(product.Id);             

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);             
        }
        #endregion

        #region Update method
        [Fact]
        public void Update_BadRequest()
        {
            // Arrange
            int invalidID = 0;
            ProductVM product = repositoryData[0];
            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var badResult = _controller.UpdateAsync(invalidID, product);

            // Assert
            Assert.IsType<BadRequestResult>(badResult.Result);
        }
        [Fact]
        public void Update_NotFoundResult()
        {
            // Arrange
            int invalidID = 0;
            ProductVM invalidProduct = new ProductVM() { Id = invalidID, Name = "xxx", ImgPath = "Image/1.jpg", Price = 0.00M, Description = null };
            ProductVM product = repositoryData[0];
            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.UpdateAsync(invalidID, invalidProduct);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Update_NoContentResult()
        {
            // Arrange
            ProductVM product = repositoryData[0];
            product.Description = "NewDescription";             
            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _mockRepository.Setup(m => m.GetProductAsync(product.Id)).ReturnsAsync(product);
            _mockRepository.Setup(m => m.ProductExistsAsync(product.Id)).ReturnsAsync(true);
            _controller = new ProductController(_mockRepository.Object);

            // Act             
            var noContent = _controller.UpdateAsync(product.Id, product);

            // Assert
            Assert.IsType<NoContentResult>(noContent.Result);
        }
        #endregion

        #region Page method
        [Fact]
        public void Page_NotFoundResult()
        {
            // Arrange
            List<ProductVM> products = null;
            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetPageAsync(5, 0); 

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Page_OkResult()
        {
            // Arrange
            List<ProductVM> products = repositoryData.GetRange(0, 5);
            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetPageAsync(5, 0);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void Page_PageItems()
        {
            // Arrange
            var countItems = 5;
            List<ProductVM> products = repositoryData.GetRange(0, 5);
            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetPageAsync(5, 0).Result as OkObjectResult;

            // Assert
            var items = Assert.IsType<List<ProductVM>>(okResult.Value);
            Assert.Equal(countItems, items.Count);
        }
        #endregion

        #region UpdateDesc method
        [Fact]
        public void UpdateDesc_NotFoundResult()
        {
            // Arrange
            int invalidID = 0;
            ProductVM product = repositoryData[0];
            _mockRepository.Setup(m => m.UpdateProductDescAsync(product.Id, product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);
            var jsonPatch = new JsonPatchDocument<ProductVM>();

            // Act
            var notFoundResult = _controller.UpdateDescAsync(invalidID, jsonPatch);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }
        
        [Fact]
        public void UpdateDesc_OkResult()
        {
            // Arrange
            string newDescription = "New description patch";
            ProductVM product = repositoryData[0]; 
            _mockRepository.Setup(m => m.UpdateProductDescAsync(product.Id, product)).ReturnsAsync(product);
            _mockRepository.Setup(m => m.GetProductAsync(product.Id)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);            
            var jsonPatch = new JsonPatchDocument<ProductVM>().Replace(p => p.Description, newDescription);

            // Act             
            var okResult = _controller.UpdateDescAsync(product.Id, jsonPatch).Result as OkObjectResult;
            
            // Assert
            var prod = Assert.IsType<ProductVM>(okResult.Value);
            Assert.Equal(newDescription, prod.Description);
        }
        #endregion

        #region Create method
        [Fact]
        public void Create_BadRequest()
        {
            // Arrange            
            ProductVM product = new ProductVM { Id = 1 };
            _mockRepository.Setup(m => m.CreateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var badResult = _controller.CreateAsync(product);

            // Assert
            Assert.IsType<BadRequestResult>(badResult.Result);
        }        
        
        [Fact]
        public void Create_CreatedResult()
        {
            // Arrange            
            ProductVM product = new ProductVM();
            _mockRepository.Setup(m => m.CreateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var createdResult = _controller.CreateAsync(product).Result as CreatedAtActionResult;

            // Assert
            var item = Assert.IsType<ProductVM>(createdResult.Value);
            Assert.Equal(product, item);
        }
        #endregion

        #region Delete method
        [Fact]
        public void Delete_NotFoundResult()
        {
            // Arrange
            int invalidID = 0;            
            _mockRepository.Setup(m => m.DeleteProductAsync(invalidID));
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.DeleteAsync(invalidID);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Delete_NoContentResult()
        {
            // Arrange
            ProductVM product = repositoryData[0];
            _mockRepository.Setup(m => m.DeleteProductAsync(product.Id));
            _mockRepository.Setup(m => m.ProductExistsAsync(product.Id)).ReturnsAsync(true);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var noContent = _controller.DeleteAsync(product.Id);

            // Assert
            Assert.IsType<NoContentResult>(noContent.Result);
        }
        #endregion
    }
}
