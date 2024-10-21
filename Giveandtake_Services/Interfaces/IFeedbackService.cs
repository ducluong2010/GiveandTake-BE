using Giveandtake_Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IGiveandtakeResult> GetAllFeedbacks(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetFeedbackById(int feedbackId);
    }
}
