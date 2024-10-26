using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveandTake_Repo.DTOs.Favorite
{
    public class FavoriteDTO
    {
        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }
    }

    public class GetFavoriteDTO
    {
        public int FavoriteId { get; set; }

        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }
    }
}
