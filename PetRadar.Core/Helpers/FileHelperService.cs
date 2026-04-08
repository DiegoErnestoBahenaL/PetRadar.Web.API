using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using PetRadar.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public class FileHelperService : IFileHelperService
    {
        private readonly IWebHostEnvironment _hostEnv;

        public FileHelperService(IWebHostEnvironment hostEnv)
        {
            _hostEnv = hostEnv;
        }

        public string GetImagePath(string relativePath)
        {
            string workingDirectory = _hostEnv.ContentRootPath;
            string absolutePath = Path.Combine(workingDirectory, relativePath);
            if (!File.Exists(absolutePath))
            {
                PetRadarException.ThrowImageNotFoundException();
            }
            return absolutePath;
        }
        public async Task<string> SaveImage(IFormFile image)
        {
            
            ValidateImage(image);

            string extension = Path.GetExtension(image.FileName);

            string workingDirectory = _hostEnv.ContentRootPath;


            string path = Path.Combine(workingDirectory, Constants.SecuredDirectoryName, Constants.ImagesDirectoryName);


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Guid.NewGuid().ToString();

            string absolutePath = Path.Combine(workingDirectory, Constants.SecuredDirectoryName, Constants.ImagesDirectoryName, fileName + extension);

            using (Stream fileStream = new FileStream(absolutePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }


            string relativePath = Path.Combine(Constants.SecuredDirectoryName, Constants.ImagesDirectoryName, fileName + extension);

            return relativePath;
        }

        public async Task<string?> SaveImagesInGallery(List<IFormFile> images, string? guid)
        {
            if (images == null || images.Count == 0)
                PetRadarException.ThrowNoImageProvidedException();

            string workingDirectory = _hostEnv.ContentRootPath;

            string galleryGuid;

            // If no galleryGuid is provided, generate a new one.
            // This allows for both creating new galleries and adding images to existing ones.
            if (string.IsNullOrEmpty(guid))
            {
                galleryGuid = Guid.NewGuid().ToString();
            }
            else
            {
                galleryGuid = guid;
            }

            string galleryRelativePath = Path.Combine(
                Constants.SecuredDirectoryName, 
                Constants.ImagesDirectoryName,
                Constants.AdditionalPhotosDirectoryName,
                galleryGuid
            );

            string path = Path.Combine(workingDirectory, galleryRelativePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int index = 1;
            foreach (var image in images)
            {
                ValidateImage(image);

                string extension = Path.GetExtension(image.FileName);


                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

                string fileName = $"{timestamp}_{index:D3}";


                string absolutePath = Path.Combine(path, fileName + extension);

                using Stream fileStream = new FileStream(absolutePath, FileMode.Create);
                
                await image.CopyToAsync(fileStream);

                index++;
            }

            if (string.IsNullOrEmpty(guid))
            {
                return galleryRelativePath;
            }
            else
            {
                return null;
            }
        }

        public List<string> GetGalleryImageNames(string galleryRelativePath)
        {
            string workingDirectory = _hostEnv.ContentRootPath;
            string galleryAbsolutePath = Path.Combine(workingDirectory, galleryRelativePath);

            if (!Directory.Exists(galleryAbsolutePath))
            {
                PetRadarException.ThrowImageNotFoundException();
            }
            var imageFiles = Directory.GetFiles(galleryAbsolutePath);

            var imageNames = new List<string>();

            foreach (var imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                imageNames.Add(fileName);
            }

            return imageNames;
        }

        public void DeleteImage(string imagePath, ILogger logger)
        {
            try
            {
                File.Delete(imagePath);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error deleting image at path: {ImagePath}", imagePath);
            }
        }

        private void ValidateImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                PetRadarException.ThrowNoImageProvidedException();
            if (image?.Length > Constants.MaxImageSize)
                PetRadarException.ThrowImageSizeTooLargeException();
            string extension = Path.GetExtension(image.FileName);
            if (!Constants.ValidImagesExtensions.Contains(extension))
                PetRadarException.ThrowInvalidExtensionProvidedException();
        }

    }
}
