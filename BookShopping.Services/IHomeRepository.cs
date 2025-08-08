using BookShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopping.Services
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string sTrem = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();       
    }
}
