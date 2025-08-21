using Microsoft.AspNetCore.Http;

namespace BookShopping.Services
{
    public interface IFileService
    {
        void DeleteFile(string fileName);
        Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
    }
}
