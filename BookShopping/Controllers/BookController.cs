using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using BookShopping.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShopping.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepo;
        private readonly IFileService _fileService;
        private readonly IGenreRepository _genreRepo;

        public BookController(IBookRepository bookRepo, IGenreRepository genreRepo, IFileService fileService)
        {
            _bookRepo = bookRepo;
            _genreRepo = genreRepo;
            _fileService = fileService;
        }

        // ✅ Index with Pagination
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var allBooks = await _bookRepo.GetBooks();
            int totalItems = allBooks.Count();

            var books = allBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<Book>
            {
                Data = books,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(pagedResult);
        }

        public async Task<IActionResult> AddBook()
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString()
            });

            BookDTO BookToAdd = new() { GenreList = genreSelectList };
            return View(BookToAdd);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookDTO bookToAdd)
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString()
            });
            bookToAdd.GenreList = genreSelectList;

            if (!ModelState.IsValid) return View(bookToAdd);

            try
            {
                if (bookToAdd.ImageFile != null)
                {
                    if (bookToAdd.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file cannot exceed 1 MB");

                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string imageName = await _fileService.SaveFile(bookToAdd.ImageFile, allowedExtensions);
                    bookToAdd.Image = imageName;
                }

                Book book = new()
                {
                    Id = bookToAdd.Id,
                    BookName = bookToAdd.BookName,
                    AuthorName = bookToAdd.AuthorName,
                    GenreId = bookToAdd.GenreId,
                    Image = bookToAdd.Image,
                    Price = bookToAdd.Price,
                    DiscountPercentage = bookToAdd.DiscountPercentage   // ✅ Added
                };
                await _bookRepo.AddBook(book);
                TempData["successMessage"] = "Book added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToAdd);
            }
        }

        public async Task<IActionResult> UpdateBook(int id)
        {
            var book = await _bookRepo.GetBookById(id);
            if (book == null)
            {
                TempData["errorMessage"] = $"Book with id: {id} not found";
                return RedirectToAction(nameof(Index));
            }

            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == book.GenreId
            });

            BookDTO bookToUpdate = new()
            {
                Id = book.Id,
                BookName = book.BookName,
                AuthorName = book.AuthorName,
                GenreId = book.GenreId,
                Price = book.Price,
                DiscountPercentage = book.DiscountPercentage,  // ✅ Added
                Image = book.Image,
                GenreList = genreSelectList // genrelist hocce dropdown ar jonno option list
            };
            return View(bookToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookDTO bookToUpdate)
        {
            var genreSelectList = (await _genreRepo.GetGenres()).Select(genre => new SelectListItem
            {
                Text = genre.GenreName,
                Value = genre.Id.ToString(),
                Selected = genre.Id == bookToUpdate.GenreId
            });
            bookToUpdate.GenreList = genreSelectList;

            if (!ModelState.IsValid) return View(bookToUpdate);

            try
            {
                string oldImage = "";
                if (bookToUpdate.ImageFile != null)
                {
                    if (bookToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("Image file cannot exceed 1 MB");

                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string imageName = await _fileService.SaveFile(bookToUpdate.ImageFile, allowedExtensions);
                    oldImage = bookToUpdate.Image;
                    bookToUpdate.Image = imageName;
                }

                Book book = new()
                {
                    Id = bookToUpdate.Id,
                    BookName = bookToUpdate.BookName,
                    AuthorName = bookToUpdate.AuthorName,
                    GenreId = bookToUpdate.GenreId,
                    Price = bookToUpdate.Price,
                    DiscountPercentage = bookToUpdate.DiscountPercentage,   // ✅ Added
                    Image = bookToUpdate.Image
                };
                await _bookRepo.UpdateBook(book);

                if (!string.IsNullOrWhiteSpace(oldImage))
                    _fileService.DeleteFile(oldImage);

                TempData["successMessage"] = "Book updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(bookToUpdate);
            }
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookRepo.GetBookById(id);
                if (book == null)
                {
                    TempData["errorMessage"] = $"Book with id: {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                await _bookRepo.DeleteBook(book);
                if (!string.IsNullOrWhiteSpace(book.Image))
                    _fileService.DeleteFile(book.Image);

                TempData["successMessage"] = "Book deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
