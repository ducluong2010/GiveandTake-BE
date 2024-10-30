using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.DTOs.Feedback;
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
        Task<IGiveandtakeResult> CreateFeedback(CreateFeedbackDTO feedbackInfo);
        Task<IGiveandtakeResult> UpdateFeedback(int id, UpdateFeedbackDTO feedbackInfo);
        Task<IGiveandtakeResult> DeleteFeedback(int id);
        Task<IGiveandtakeResult> GetFeedbacksBySenderId(int senderId, int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetFeedbacksByAccountId(int accountId, int page = 1, int pageSize = 8);
    }
}
