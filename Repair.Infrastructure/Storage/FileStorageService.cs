using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repair.Application.Interfaces;


namespace Repair.Infrastructure.Storage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                    throw new ArgumentException("No image data provided");

                var fileExtension = Path.GetExtension(image.FileName);

                using var stream = image.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(
                        $"campaign_{Guid.NewGuid()}{fileExtension}",
                        stream
                    ),
                    Folder = "Images"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                throw;
            }
        }



        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
                var publicId = GetPublicIdFromUrl(imageUrl);
                if (publicId == null) return;

                var deletionParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deletionParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from Cloudinary");
                throw;
            }
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var pathSegments = uri.Segments;
                var uploadIndex = Array.IndexOf(pathSegments, "upload/");
                if (uploadIndex == -1) return null;

                var publicId = string.Join("", pathSegments.Skip(uploadIndex + 2))
                    .TrimEnd('/')
                    .Split('.')[0];

                return $"campaigns/{publicId}";
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> UploadBase64DocumentAsync(string base64Document)
        {
            try
            {
                // Generate a unique public ID
                var publicId = $"documents/doc_{Guid.NewGuid().ToString("N")[..12]}.pdf";

                // Upload raw document (PDF, Word, etc.)
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription("document.pdf", base64Document),
                    PublicId = publicId
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload document to Cloudinary: {ex.Message}", ex);
            }
        }
    }
}
