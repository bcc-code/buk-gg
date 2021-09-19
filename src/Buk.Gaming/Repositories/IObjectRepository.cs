using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IObjectRepository
    {
        Task<List<Game>> GetGamesAsync();

        Task<List<Category>> GetCategoriesAsync();
    }
}
