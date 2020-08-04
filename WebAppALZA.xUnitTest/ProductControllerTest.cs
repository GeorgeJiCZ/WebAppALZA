using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
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

        private List<Product> repositoryData;

        private List<Product> TestData => new List<Product>()
            {
                new Product() { Id = 1, Name = "Name1", ImgUri = "Image/1.jpg", Price = 1.00M, Description = null },
                new Product() { Id = 2, Name = "Name2", ImgUri = "Image/2.jpg", Price = 2.50M, Description = "Description2" },
                new Product() { Id = 3, Name = "Name3", ImgUri = "Image/3.jpg", Price = 33.60M, Description = "Description3" },
                new Product() { Id = 4, Name = "Name4", ImgUri = "Image/4.jpg", Price = 4.20M, Description = "Description4" },
                new Product() { Id = 5, Name = "Name5", ImgUri = "Image/5.jpg", Price = 555.99M, Description = null },
                new Product() { Id = 6, Name = "Name6", ImgUri = "Image/6.jpg", Price = 6.10M, Description = "Description5" }
            };

        public ProductControllerTest()
        {
            _mockRepository = new Mock<IProductRepository>();

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string dataType = config["DataType"];

            if (String.IsNullOrWhiteSpace(dataType) || dataType == "1") repositoryData = TestData;            
            else repositoryData = GetDBData(config["ConnectionString"]);                         
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

        #region GetAll method
        [Fact]
        public void GetAll_ReturnsNotFoundResult()
        {
            // Arrange
            List<Product> products = null;

            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetAllAsync();

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void GetAll_ReturnsOkResult()
        {
            // Arrange
            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(repositoryData);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetAllAsync();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
        
        [Fact]
        public void GetAll_ReturnsAllItems()
        {
            // Arrange
            var countItems = repositoryData.Count;
            _mockRepository.Setup(m => m.GetProductsAsync()).ReturnsAsync(repositoryData);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetAllAsync().Result as OkObjectResult;

            // Assert
            var items = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(countItems, items.Count);
        }
        #endregion

        #region GetById method
        [Fact]
        public void GetById_ReturnsNotFoundResult()
        {
            // Arrange
            int invalidID = 0;              
            Product product = repositoryData[0];
            _mockRepository.Setup(m => m.GetProductAsync(1)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetByIdAsync(invalidID);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void GetById_ReturnsOkResult()
        {
            // Arrange
            Product product = repositoryData[0];             
            
            _mockRepository.Setup(m => m.GetProductAsync(product.Id)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetByIdAsync(product.Id);             

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);             
        }
        #endregion

        #region Put method
        [Fact]
        public void Put_BadRequest()
        {
            // Arrange
            int invalidID = 0;
            Product product = repositoryData[0];
            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var badResult = _controller.PutAsync(invalidID, product);

            // Assert
            Assert.IsType<BadRequestResult>(badResult.Result);
        }
        [Fact]
        public void Put_ReturnsNotFoundResult()
        {
            // Arrange
            int invalidID = 0;
            Product invalidProduct = new Product() { Id = invalidID, Name = "xxx", ImgUri = "Image/1.jpg", Price = 0.00M, Description = null };
            Product product = repositoryData[0];
            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.PutAsync(invalidID, invalidProduct);

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void Put_ReturnsNoContentResult()
        {
            // Arrange
            Product product = repositoryData[0];
            product.Description = "NewDescription";

            _mockRepository.Setup(m => m.UpdateProductAsync(product)).ReturnsAsync(product);
            _mockRepository.Setup(m => m.GetProductAsync(product.Id)).ReturnsAsync(product);

            _controller = new ProductController(_mockRepository.Object);

            // Act             
            var noContent = _controller.PutAsync(product.Id, product);

            // Assert
            Assert.IsType<NoContentResult>(noContent.Result);
        }
        #endregion

        #region GetPage method
        [Fact]
        public void GetPage_ReturnsNotFoundResult()
        {
            // Arrange
            List<Product> products = null;

            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var notFoundResult = _controller.GetPageAsync(5, 0); 

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public void GetPage_ReturnsOkResult()
        {
            // Arrange
            List<Product> products = repositoryData.GetRange(0, 5);            

            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetPageAsync(5, 0);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetPage_ReturnsPageItems()
        {
            // Arrange
            var countItems = 5;
            List<Product> products = repositoryData.GetRange(0, 5);

            _mockRepository.Setup(m => m.GetProductsAsync(5, 0)).ReturnsAsync(products);
            _controller = new ProductController(_mockRepository.Object);

            // Act
            var okResult = _controller.GetPageAsync(5, 0).Result as OkObjectResult;

            // Assert
            var items = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(countItems, items.Count);
        }
        #endregion
    }
}
