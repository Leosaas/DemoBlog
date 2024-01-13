using AutoMapper;
using Demo.Models;
using DTO;

namespace Demo
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TinTuc, TinTucViewModel>();
            CreateMap<TinTucViewModel, TinTuc>();

            CreateMap<DanhMuc, DanhMucViewModel>();
            CreateMap<DanhMucViewModel, DanhMuc>();
        }
    }
}
