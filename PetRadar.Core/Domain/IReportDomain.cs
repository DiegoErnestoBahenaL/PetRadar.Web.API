using Microsoft.AspNetCore.Http;
using PetRadar.Core.Data.Entities;
using PetRadar.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRadar.Core.Domain
{
    public interface IReportDomain
    {
        Task<List<ReportEntity>> GetAllAsync(CancellationToken token);
        Task<List<ReportEntity>> GetAllByUserIdAsync(long userId, CancellationToken token);
        Task<ReportEntity?> FindByIdAsync(long id, CancellationToken token);
        string? GetMainPicturePath(ReportEntity reportDb, CancellationToken token);
        Task<ReportEntity> CreateAsync(ReportCreateModel report, long createdByUserId, CancellationToken token);
        Task<int> UpdateAsync(ReportEntity reportDb, ReportUpdateModel report, long modifiedByUserId, CancellationToken token);
        Task<int> UpdateMainPictureAsync(ReportEntity reportDb, IFormFile file, long modifiedByUserId, CancellationToken token);
        List<string> GetAdditionalPhotoNames(ReportEntity reportDb);
        string? GetAdditionalPhotoPath(string relativePath, string imageName);
        Task<int> UploadAdditionalPhotosAsync(ReportEntity reportDb, List<IFormFile> files, string? guid, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAdditionalPhotoAsync(ReportEntity reportDb, string photoName, long modifiedByUserId, CancellationToken token);
        Task<int> DeleteAsync(ReportEntity report, long modifiedByUserId, CancellationToken token);
    }
}
