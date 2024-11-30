using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Category
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? Status { get; set; }

        public int Point { get; set; }
    }

    public class CategoryManagerDTO
    {
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int AccountId { get; set; }

        public string? AccountName { get; set; }

        public string? Status { get; set; }

    }
}
