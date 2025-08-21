using BookShopping.Models;
using BookShopping.Models.DTOs;
using BookShopping.Services;
using BookShopping.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;

        public GenreController(IGenreRepository genreRepo)
        {
            _genreRepo = genreRepo;
        }

        // GET: /Genre
        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepo.GetGenres();
            return View(genres);
        }

        // GET: /Genre/AddGenre
        public IActionResult AddGenre()
        {
            return View();
        }

        // POST: /Genre/AddGenre
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGenre(GenreDTO genre)
        {
            if (!ModelState.IsValid)
                return View(genre);

            try
            {
                var genreToAdd = new Genre
                {
                    Id = genre.Id,
                    GenreName = genre.GenreName
                };

                await _genreRepo.AddGenre(genreToAdd);
                TempData["successMessage"] = "Genre added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Genre could not be added!";
                return View(genre);
            }
        }

        // GET: /Genre/UpdateGenre/5
        public async Task<IActionResult> UpdateGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre == null)
                throw new InvalidOperationException($"Genre with id {id} not found");

            var genreToUpdate = new GenreDTO
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };

            return View(genreToUpdate);
        }

        // POST: /Genre/UpdateGenre
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGenre(GenreDTO genreToUpdate)
        {
            if (!ModelState.IsValid)
                return View(genreToUpdate);

            try
            {
                var genre = new Genre
                {
                    Id = genreToUpdate.Id,
                    GenreName = genreToUpdate.GenreName
                };

                await _genreRepo.UpdateGenre(genre);
                TempData["successMessage"] = "Genre updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Genre could not be updated!";
                return View(genreToUpdate);
            }
        }

        // GET: /Genre/DeleteGenre/5
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre == null)
                throw new InvalidOperationException($"Genre with id {id} not found");

            await _genreRepo.DeleteGenre(genre);
            TempData["successMessage"] = "Genre deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
