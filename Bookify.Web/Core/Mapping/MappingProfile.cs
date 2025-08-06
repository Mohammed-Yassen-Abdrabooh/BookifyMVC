namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping Category
            CreateMap<Category, CategoryViewModel>();//.ForMember(dest=>dest.Name,opt=>opt.MapFrom(src=>src.Name));//if Name Prop in CategoryViewModel is different like"CategoryName" => You Must Use ForMember Method To Map It
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();

            // Mapping Author
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
        }
    }
}
