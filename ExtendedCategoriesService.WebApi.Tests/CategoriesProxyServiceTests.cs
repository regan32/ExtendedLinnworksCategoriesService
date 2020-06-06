using NUnit.Framework;

namespace ExtendedCategoriesService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Exceptions;
    using ExtendedCategoriesService.WebApi;
    using ExtendedCategoriesService.WebApi.Abstractions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using NSubstitute;
    using Shouldly;
    using Category = ExtendedCategoriesService.WebApi.Abstractions.Dto.Category;

    public class CategoriesProxyServiceTests
    {
        private ICategoriesClient categoriesClient;
        private IDashboardsClient dashboardsClient;
        private IHttpContextAccessor httpContextAccessor;
        private IMapper mapper;
        
        [SetUp]
        public void Setup()
        {
            categoriesClient = Substitute.For<ICategoriesClient>();
            dashboardsClient = Substitute.For<IDashboardsClient>();
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            mapper = new Mapper(new MapperConfiguration(o => o.AddProfile<WebApiProfile>()));
        }

        [TestCase("TestName1")]
        [TestCase("TestName2")]
        public void Should_create_category(string name)
        {
            //given
            categoriesClient.CreateCategoryAsync(default, default, default)
                .ReturnsForAnyArgs((a) => new LinnSystemsClient.Abstractions.Dto.Category()
                {
                    CategoryId = Guid.NewGuid(), 
                    CategoryName = a.ArgAt<string>(0)
                });
            
            //when
            var categoriesProxyService = CreateInstance();
            var result = Should.NotThrow(categoriesProxyService.CreateCategoryAsync(name));
            
            //then
            result.CategoryName.ShouldBe(name);
        }

        [Test]
        public void Should_delete_category()
        {
            //given
            var categoryId = Guid.NewGuid();
            
            //when
            var categoriesProxyService = CreateInstance();
            Should.NotThrow(categoriesProxyService.DeleteCategoryByIdAsync(categoryId));
            
            //then
            categoriesClient.Received(1).DeleteCategoryByIdAsync(categoryId, Arg.Any<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>());
        }
        
        [TestCase(10, 1)]
        [TestCase(20, 2)]
        [TestCase(30, 3)]
        public void Should_get_categories(int pageSize, int pageNumber)
        {
            //given
            string five = "five";
            string ten = "ten";
            string fifteen = "fifteen";
            
            string script = string.Format(CategoriesProxyService.PagedCategoryWithProductsQueryFormat, pageSize, pageNumber);
            
            dashboardsClient.ExecuteCustomScriptCustomerAsync(default, default)
                .ReturnsForAnyArgs( 
                    new CustomScriptResult()
                    {
                        IsError = false,
                        Results = new List<IDictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                {nameof(Category.CategoryId), Guid.NewGuid()},
                                {nameof(Category.CategoryName), five},
                                {nameof(Category.ProductsCount), 5.ToString()}
                            },
                            new Dictionary<string, object>
                            {
                                {nameof(Category.CategoryId), Guid.NewGuid()},
                                {nameof(Category.CategoryName), ten},
                                {nameof(Category.ProductsCount), 10.ToString()}
                            },
                            new Dictionary<string, object>
                            {
                                {nameof(Category.CategoryId), Guid.NewGuid()},
                                {nameof(Category.CategoryName), fifteen},
                                {nameof(Category.ProductsCount), 15.ToString()}
                            }
                        },
                        
                    } 
                    );
            
            //when
            var categoriesProxyService = CreateInstance();
            var results = Should.NotThrow(categoriesProxyService.GetCategoriesWithProducts(pageSize, pageNumber));
            
            //then
            dashboardsClient.Received(1).ExecuteCustomScriptCustomerAsync(Arg.Is<string>(a => a.Equals(script)), Arg.Any<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>());
            
            results.Count().ShouldBe(3);
            results.ShouldContain(a => a.CategoryName == five && a.ProductsCount == 5 );
            results.ShouldContain(a => a.CategoryName == ten && a.ProductsCount == 10 );
            results.ShouldContain(a => a.CategoryName == fifteen && a.ProductsCount == 15 );
        }
        
        [Test]
        public void Should_handle_script_result_error()
        {
            //given
            string error = "testError";

            dashboardsClient.ExecuteCustomScriptCustomerAsync(default, default)
                .ReturnsForAnyArgs( 
                    new CustomScriptResult()
                    {
                        IsError = true,
                        ErrorMessage = error
                        
                    } 
                    );
            
            //when
            var categoriesProxyService = CreateInstance();
            var results = Should.Throw<ApiException>(categoriesProxyService.GetCategoriesWithProducts(1, 1));

            //then
            results.ResponseText.ShouldBe(error);
        }
        
        [Test]
        public void Should_proxy_authorization()
        {
            //given
            var name = "TextCategoryName";
            var sessionId = Guid.NewGuid();
            
            httpContextAccessor
                .HttpContext
                .Returns(new DefaultHttpContext
                {
                    Request =
                    {
                        Headers =
                        {
                            [HeaderNames.Authorization] = sessionId.ToString(),
                        }
                    }
                });
            
            //when
            var categoriesProxyService = CreateInstance();
            Should.NotThrow(categoriesProxyService.CreateCategoryAsync(name));
            
            //then
            categoriesClient.Received(1).CreateCategoryAsync(
                name,
                Arg.Is<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(a => a[HeaderNames.Authorization].Contains(sessionId.ToString())), 
                default);
        }
        
        private ICategoriesProxyService CreateInstance()
        {
            return new CategoriesProxyService(categoriesClient, dashboardsClient, httpContextAccessor, mapper);
        }
    }
}