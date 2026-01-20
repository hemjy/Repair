

using Microsoft.AspNetCore.Http;

namespace Repair.Application.Interfaces
{
     public interface IFileStorageService
    {
        Task<string> UploadImageAsync(IFormFile image);
        Task DeleteImageAsync(string imageUrl);
    }
}
