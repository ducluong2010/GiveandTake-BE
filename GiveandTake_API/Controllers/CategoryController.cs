﻿using GiveandTake_API.Constants;
using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Category;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController()
        {
            _categoryService = new CategoryService();
        }

        [HttpGet(ApiEndPointConstant.Category.CategoriesEndPoint)]
        [SwaggerOperation(Summary = "Get all Categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.Category.CategoryEndPoint)]
        [SwaggerOperation(Summary = "Get Category by its id")]
        public async Task<IActionResult> GetCategoryInfo(int id)
        {
            var response = await _categoryService.GetCategoryById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpPut(ApiEndPointConstant.Category.CategoryEndPoint)]
        [SwaggerOperation(Summary = "Update Category")]
        public async Task<IActionResult> UpdateCategoryInfo(int id, CategoryDTO category)
        {
            var response = await _categoryService.UpdateCategory(id, category);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response.Message);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpDelete(ApiEndPointConstant.Category.CategoryEndPoint)]
        [SwaggerOperation(Summary = "Delete Category")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategory(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpPost(ApiEndPointConstant.Category.CategoriesEndPoint)]
        [SwaggerOperation(Summary = "Create a new Category")]
        public async Task<IActionResult> CreateCategory(CategoryDTO category)
        {
            var response = await _categoryService.CreateCategory(category);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")]
        [HttpPost(ApiEndPointConstant.Category.CategoryManaEndPoint)]
        [SwaggerOperation(Summary = "Manager category")]
        public async Task<IActionResult> GetCategoryManagers()
        {
            var response = await _categoryService.GetCategoryManagers();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }
    }
}
