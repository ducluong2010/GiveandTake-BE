using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Favorite;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class FavoriteService : IFavoriteService
    {
        private readonly FavoriteBusiness _favoriteBusiness;
        public FavoriteService()
        {
            _favoriteBusiness = new FavoriteBusiness();
        }

        public Task<IGiveandtakeResult> AddFavorite(int accountId, FavoriteDTO favoriteDTO)
            => _favoriteBusiness.AddFavorite(accountId, favoriteDTO);

        public Task<IGiveandtakeResult> DeleteFavorite(int id)
            => _favoriteBusiness.DeleteFavorite(id);

        public Task<IGiveandtakeResult> GetAllFavorites()
            => _favoriteBusiness.GetAllFavorites();

        public Task<IGiveandtakeResult> GetFavoriteById(int id)
            => _favoriteBusiness.GetFavoriteById(id);

        public Task<IGiveandtakeResult> GetFavoriteDonationsByCategory(int accountId)
            => _favoriteBusiness.GetFavoriteDonationsByCategory(accountId);

        public Task<IGiveandtakeResult> GetFavoritesByAccountId(int accountId)
            => _favoriteBusiness.GetFavoritesByAccountId(accountId);
    }
}
