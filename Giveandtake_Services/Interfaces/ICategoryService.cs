using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IGiveandtakeResult> GetCategoryById(int id);
        Task<IGiveandtakeResult> GetAllCategories();
        Task<IGiveandtakeResult> UpdateCategory(int id, CategoryDTO categoryInfo);
        Task<IGiveandtakeResult> CreateCategory(CategoryDTO categoryInfo);
        Task<IGiveandtakeResult> DeleteCategory(int id);
        Task<IGiveandtakeResult> GetCategoryManagers();
    }
}
