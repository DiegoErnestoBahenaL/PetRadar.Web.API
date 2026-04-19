using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Common
{
    public static class Constants
    {
        public const int SuperAdminId = 1;

        public static readonly DateTimeOffset SuperAdminCreatedAt = new DateTimeOffset(new DateTime(2026, 1, 1));

        public const int MaxImageSize = 5242880;

        public static List<string> ValidImagesExtensions = [".jpg", ".jpeg", ".png"];

        public const int MaxAdditionalPhotos = 5;

        public const string ImagesDirectoryName = "Images";

        public const string AdditionalPhotosDirectoryName = "AdditionalPhotos";

        public const string SecuredDirectoryName = "Secured";

        public const string MediaTypeNamesImagePng = "image/png";


        #region Exception messages

        public const string ImageSizeTooLarge = "Image size is too large.";

        public const string NoImageProvided = "No image file provided.";

        public const string InvalidExtensionProvided = "The extension of the image is invalid.";

        public const string ImageNotFound = "Image not found.";

        #endregion

        public static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            string mimeType = "";

            switch (extension)
            {
                

                case ".jpg":
                case ".jpeg":

                    mimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;

                    break;

                case ".png":

                    mimeType = MediaTypeNamesImagePng;

                    break;

                default:

                    mimeType = System.Net.Mime.MediaTypeNames.Application.Octet;

                    break;
            }

            return mimeType;
        }
    }
}
