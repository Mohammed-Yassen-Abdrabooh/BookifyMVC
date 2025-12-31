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

            // Mapping Book  To BookViewModel ==> Details View
            // dest is a BookViewModel , src is a Book  
            CreateMap<Book, BookViewModel>() 
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));

            // Mappind BookCopy To BookCopyViewModel 
            CreateMap<BookCopy, BookCopyViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Book!.Id))
                .ForMember(dest => dest.BookThumbnailUrl, opt => opt.MapFrom(src => src.Book!.ImageThumbnailUrl));

            CreateMap<BookCopy, BookCopyFormViewModel>();
            CreateMap<BookCopyFormViewModel, BookCopyViewModel>();

            // Mapping Users
            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserFormViewModel,ApplicationUser>()
                .ForMember(dest=>dest.NormalizedEmail,opt=>opt.MapFrom(src=>src.Email.ToUpper()))
                .ForMember(dest=>dest.NormalizedUserName,opt=>opt.MapFrom(src=>src.UserName.ToUpper()))
                .ReverseMap();

            // Mapping Areas
            CreateMap<Area, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            // Mapping Governorates
            CreateMap<Governorate, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            // Mapping Subscribers
            CreateMap<SubscriberFormViewModel, Subscriber>().ReverseMap();

            CreateMap<Subscriber, SubscriberSearchResultViewModel>()
                    .ForMember(dest=>dest.FullName , opt=>opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Subscriber, SubscriberDetailsViewModel>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate!.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Subscription, SubscriptionViewModel>();

            // Mapping Rentals
            CreateMap<Rental, RentalViewModel>();
            CreateMap<RentalCopy, RentalCopyViewModel>();

        }
    }
}
