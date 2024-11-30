using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Category;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryBusiness _categoryBusiness;

        public CategoryService()
        {
            _categoryBusiness = new CategoryBusiness();
        }

        public Task<IGiveandtakeResult> CreateCategory(CategoryDTO categoryInfo)
            => _categoryBusiness.CreateCategory(categoryInfo);

        public Task<IGiveandtakeResult> DeleteCategory(int id)
            => _categoryBusiness.DeleteCategory(id);

        public Task<IGiveandtakeResult> GetAllCategories()
            => _categoryBusiness.GetAllCategories();

        public Task<IGiveandtakeResult> GetCategoryById(int id)
            => _categoryBusiness.GetCategoryById(id);

        public Task<IGiveandtakeResult> UpdateCategory(int id, CategoryDTO categoryInfo)
            => _categoryBusiness.UpdateCategory(id, categoryInfo);

        public Task<IGiveandtakeResult> GetCategoryManagers()
            => _categoryBusiness.GetCategoryManagers();
    }
}
