namespace ExtendedCategoriesService.Tests
{
    using System;
    using ExtendedCategoriesService.WebApi.Abstractions;
    using ExtendedCategoriesService.WebApi.Abstractions.Dto;
    using ExtendedCategoriesService.WebApi.Controllers;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class InventoryControllerTests
    {
        private ICategoriesProxyService categoriesProxyService;
        [SetUp]
        public void SetUp()
        {
            categoriesProxyService = Substitute.For<ICategoriesProxyService>();
        }
        
        private InventoryController CreateInstance() => new InventoryController(categoriesProxyService);

        [TestCase(2, 100)]
        [TestCase(1, 50)]
        public void Should_get_categories(int pageNumber, int pageSize)
        {
            var controller = CreateInstance();

            Should.NotThrow(controller.GetCategoriesAsync(pageNumber, pageSize));

            categoriesProxyService.Received(1).GetCategoriesWithProducts(pageSize, pageNumber);
        }
        
        [TestCase("test1")]
        [TestCase("test2")]
        public void Should_create_categories(string name)
        {
            var controller = CreateInstance();

            Should.NotThrow(controller.CreateCategoriesAsync(new NewCategory(){ CategoryName = name }));

            categoriesProxyService.Received(1).CreateCategoryAsync(name);
        }
        
        [Test]
        public void Should_delete_categories()
        {
            var id = Guid.NewGuid();
            
            var controller = CreateInstance();

            Should.NotThrow(controller.DeleteCategoriesAsync(id));

            categoriesProxyService.Received(1).DeleteCategoryByIdAsync(id);
        }
    }
}