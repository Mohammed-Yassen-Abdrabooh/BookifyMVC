namespace Bookify.Web.Servieces
{
    public interface IImageService
    {
        Task<(bool IsUploaded,string? ErrorMessage)> UploadImageAsync(IFormFile imageFile,string imageName,string folderPath,bool hasThumbnail);
    
        void DeleteImage(string imagePath,string? imageThumbnailPath = null);
    }
}
