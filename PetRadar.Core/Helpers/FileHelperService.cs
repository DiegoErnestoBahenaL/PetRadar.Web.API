using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using PetRadar.Common;
using System;
using System.Collections.Generic;
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
            if (image == null || image.Length == 0)

                PetRadarException.ThrowNoImageProvidedException();

            if (image?.Length > Constants.MaxImageSize)

                PetRadarException.ThrowImageSizeTooLargeException();

            string extension = Path.GetExtension(image.FileName);

            if (!Constants.ValidImagesExtensions.Contains(extension))

                PetRadarException.ThrowInvalidExtensionProvidedException();


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


            string relativePath = Path.Combine(Constants.SecuredDirectoryName,Constants.ImagesDirectoryName, fileName + extension);

            return relativePath;
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
    }
}
