using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Helpers
{
    public interface IFileHelperService
    {
        string GetImagePath(string relativePath);
        Task<string> SaveImage(IFormFile image);
        void DeleteImage(string imagePath, ILogger logger);
        Task<string?> SaveImagesInGallery(List<IFormFile> images, string? guid);
        List<string> GetGalleryImageNames(string galleryRelativePath);
    }
}
