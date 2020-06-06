namespace ExtendedCategoriesService.WebApi
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;

    public class WebApiProfile : Profile
    {
        public WebApiProfile()
        {
            CreateMap<Category, WebApi.Abstractions.Dto.Category>()
                .ReverseMap();
            
            
            CreateMap<CustomScriptResult, IEnumerable<WebApi.Abstractions.Dto.Category>>()
                .ConvertUsing((source, target) =>
                {
                    var list = new List<WebApi.Abstractions.Dto.Category>();
                    foreach (var row in source.Results)
                    {
                        if (row.TryGetValue(nameof(Abstractions.Dto.Category.CategoryId), out var id) &&
                            row.TryGetValue(nameof(Abstractions.Dto.Category.CategoryName), out var name) &&
                            row.TryGetValue(nameof(Abstractions.Dto.Category.ProductsCount), out var count))
                        {
                            list.Add(new WebApi.Abstractions.Dto.Category()
                            {
                                CategoryId = id is string s ? Guid.Parse(s) : (Guid)id,
                                CategoryName = name as string,
                                ProductsCount = count is string value ? long.Parse(value) : (long)count ,
                            });
                        }
                    }

                    return list;
                });
        }
    }
}