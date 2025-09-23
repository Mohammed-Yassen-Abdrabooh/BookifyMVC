using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping Categories
            CreateMap<Category, CategoryViewModel>();//.ForMember(dest=>dest.Name,opt=>opt.MapFrom(src=>src.Name));//if Name Prop in CategoryViewModel is different like"CategoryName" => You Must Use ForMember Method To Map It
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();
            // SelectListItem ==> convert Values For Categories (Id , Name) عشان استخدمهم ف الدروب داون ليست في ال فيو بتاع كرييت البووك
            CreateMap<Category, SelectListItem>()
                     .ForMember(dest=>dest.Value,opt=>opt.MapFrom(src=>src.Id))
                     .ForMember(dest=>dest.Text,opt=>opt.MapFrom(src=>src.Name));

            // Mapping Authors
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
            // SelectListItem ==> convert Values For Authors (Id , Name) عشان استخدمهم ف الدروب داون ليست في ال فيو بتاع كرييت البووك
            CreateMap<Author, SelectListItem>()
                    .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            // Mapping Books
            CreateMap<BooksFormViewModel, Book>()
                    .ReverseMap()
                    .ForMember(dest=>dest.Categories ,opt=>opt.Ignore()); // Ignore Categories for Mapping because we will handle it manually in the controller
        
            // Mapping Book  To BookViewModel
            CreateMap<Book, BookViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));
        }
    }
}
