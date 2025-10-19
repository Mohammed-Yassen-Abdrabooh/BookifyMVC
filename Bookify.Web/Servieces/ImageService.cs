
using Bookify.Web.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Bookify.Web.Servieces
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment; // Used to get the WWWroot path for file uploads "Saved Files Upload in wwwroot"

        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };
        private int _maxFileSize = 2097152; // 2 MB = 2 * 1024 * 1024;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(bool IsUploaded, string? ErrorMessage)> UploadImageAsync(IFormFile imageFile, string imageName, string folderPath, bool hasThumbnail)
        {
            var extension = Path.GetExtension(imageFile.FileName);
            if (!_allowedExtensions.Contains(extension))
                return(IsUploaded: false, ErrorMessage: Errors.NotAllowedExtensionError);
            if (imageFile.Length > _maxFileSize)
                return (IsUploaded: false, ErrorMessage: Errors.MaxSizeError);


            
            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}", imageName);

            using var stream = File.Create(path);
            await imageFile.CopyToAsync(stream);
            stream.Dispose();


            if(hasThumbnail)
            {
                var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}/thumb", imageName);
                using var loadedImage = Image.Load(imageFile.OpenReadStream());
                var ratio = (float)loadedImage.Width / 200;
                var height = (int)(loadedImage.Height / ratio);
                loadedImage.Mutate(i => i.Resize(width: 200, height: height));
                loadedImage.Save(thumbPath);
            }

            return (IsUploaded: true, ErrorMessage: null);
        }


        public void DeleteImage(string imagePath, string? imageThumbnailPath = null)
        {
            var oldImagePath = $"{_webHostEnvironment.WebRootPath}{imagePath}";

            if (File.Exists(oldImagePath))
                File.Delete(oldImagePath);

            if (!string.IsNullOrEmpty(imageThumbnailPath))
            {
                var oldThumbPath = $"{_webHostEnvironment.WebRootPath}{imageThumbnailPath}";

                if (File.Exists(oldThumbPath))
                    File.Delete(oldThumbPath);
            }
        }

    }
}
