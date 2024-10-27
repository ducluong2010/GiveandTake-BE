using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Favorite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<IGiveandtakeResult> GetAllFavorites();
        Task<IGiveandtakeResult> GetFavoriteById(int id);
        Task<IGiveandtakeResult> GetFavoritesByAccountId(int accountId);
        Task<IGiveandtakeResult> AddFavorite(int accountId, FavoriteDTO favoriteDTO);
        Task<IGiveandtakeResult> DeleteFavorite(int id);
        Task<IGiveandtakeResult> GetFavoriteDonationsByCategory(int accountId);
    }
}
