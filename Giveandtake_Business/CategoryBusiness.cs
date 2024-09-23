using GiveandTake_Repo.DTOs.Category;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class CategoryBusiness
    {
        private UnitOfWork _unitOfWork;
        public CategoryBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        // Get all categories
        public async Task<IGiveandtakeResult> GetAllCategories()
        {
            var categoryList = await _unitOfWork.GetRepository<Category>()
                .GetListAsync();
            var result = categoryList.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Status = c.Status
            });
            return new GiveandtakeResult(result);
        }

        // Get category by id
        public async Task<IGiveandtakeResult> GetCategoryById(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category>()
                .SingleOrDefaultAsync(predicate: c => c.CategoryId == categoryId,
                                      selector: x => new CategoryDTO
                                      {
                                          CategoryId = x.CategoryId,
                                          CategoryName = x.CategoryName,
                                          Description = x.Description,
                                          ImageUrl = x.ImageUrl,
                                          Status = x.Status
                                      });
            return new GiveandtakeResult(category);
        }

        // Update category information
        public async Task<IGiveandtakeResult> UpdateCategory(int id, CategoryDTO categoryInfo)
        {
            Category category = await _unitOfWork.GetRepository<Category>()
                .SingleOrDefaultAsync(predicate: c => c.CategoryId == id);
            if (category == null)
            {
                return new GiveandtakeResult(-1, "Category not found");
            }
            category.CategoryName = String.IsNullOrEmpty(categoryInfo.CategoryName) ? category.CategoryName : categoryInfo.CategoryName;
            category.Description = String.IsNullOrEmpty(categoryInfo.Description) ? category.Description : categoryInfo.Description;
            category.ImageUrl = String.IsNullOrEmpty(categoryInfo.ImageUrl) ? category.ImageUrl : categoryInfo.ImageUrl;
            category.Status = String.IsNullOrEmpty(categoryInfo.Status) ? category.Status : categoryInfo.Status;

            _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Category updated successfully");
        }

        // Add new category
        public async Task<IGiveandtakeResult> CreateCategory(CategoryDTO categoryInfo)
        {
            GiveandtakeResult result = new GiveandtakeResult();
            Category newCategory = new Category
            {
                CategoryName = categoryInfo.CategoryName,
                Description = categoryInfo.Description,
                ImageUrl = categoryInfo.ImageUrl,
                Status = categoryInfo.Status
            };
            await _unitOfWork.GetRepository<Category>().InsertAsync(newCategory);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Create unsuccessfully";
            }
            else
            {
                result = new GiveandtakeResult(1, "Create Susscessfull");
            }
            return result;
        }

        // Delete category
        public async Task<IGiveandtakeResult> DeleteCategory(int id)
        {
            Category category = await _unitOfWork.GetRepository<Category>()
                .SingleOrDefaultAsync(predicate: c => c.CategoryId == id);
            if (category == null)
            {
                return new GiveandtakeResult(-1, "Category not found");
            }
            _unitOfWork.GetRepository<Category>().DeleteAsync(category);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Category deleted successfully");
        }
    }
}
