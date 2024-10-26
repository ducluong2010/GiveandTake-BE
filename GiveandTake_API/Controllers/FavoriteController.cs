using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Favorite;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;
        public FavoriteController()
        {
            _favoriteService = new FavoriteService();
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoritesEndPoint)]
        [SwaggerOperation(Summary = "Get all Favorites")]
        public async Task<IActionResult> GetAllFavorites()
        {
            var response = await _favoriteService.GetAllFavorites();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoriteEndPoint)]
        [SwaggerOperation(Summary = "Get Favorite by its id")]
        public async Task<IActionResult> GetFavoriteById(int id)
        {
            var response = await _favoriteService.GetFavoriteById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoriteDonationsByCategoryEndPoint)]
        [SwaggerOperation(Summary = "Get Favorite Donations by Category")]
        public async Task<IActionResult> GetFavoriteDonationsByCategory()
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _favoriteService.GetFavoriteDonationsByCategory(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoritesByAccountEndPoint)]
        [SwaggerOperation(Summary = "Get Favorites by Account ID")]
        public async Task<IActionResult> GetFavoritesByAccountId(int accountId)
        {
            var response = await _favoriteService.GetFavoritesByAccountId(accountId);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpPost(ApiEndPointConstant.Favorite.AddFavoriteEndPoint)]
        [SwaggerOperation(Summary = "Add a new Favorite")]
        public async Task<IActionResult> AddFavorite(FavoriteDTO favoriteDTO)
        {
            int accountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId").Value);

            var response = await _favoriteService.AddFavorite(accountId, favoriteDTO);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpDelete(ApiEndPointConstant.Favorite.DeleteFavoriteEndPoint)]
        [SwaggerOperation(Summary = "Delete Favorite")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var response = await _favoriteService.DeleteFavorite(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}
