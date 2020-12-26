using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppALZA.API.Models;

namespace WebAppALZA.API
{
    public static class Util
    {
        public static MapperConfiguration mapperProduct = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductVM>());        

        public static ProductVM GetProductMapper(Product product)
        {
            var mapper = new Mapper(mapperProduct);
            return mapper.Map<ProductVM>(product);
        }

        public static List<ProductVM> GetProductsMapper(List<Product> products)
        {
            var mapper = new Mapper(mapperProduct);
            return mapper.Map<List<Product>, List<ProductVM>>(products);            
        }
    }
}
