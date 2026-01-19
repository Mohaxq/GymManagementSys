using GymManagementBLL.Services.InterFaces.AttachmentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        private readonly long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public AttachmentService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        public bool Delete(string folderName, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(folderName) || string.IsNullOrEmpty(fileName))
                {
                    return false;
                }
                var filePath = Path.Combine(_webHost.WebRootPath, "images", folderName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string? Upload(string folderName, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0 || folderName == null)
                {
                    return null;
                }
                if (file.Length > MaxFileSizeInBytes)
                {
                    return null;
                }
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    return null;
                }
                var folderPath = Path.Combine(_webHost.WebRootPath, "images", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(folderPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
                return fileName;
            }
            catch
            {
                return null;
            }
        }
    }
}
