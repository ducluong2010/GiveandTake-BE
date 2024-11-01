using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.DTOs.Feedback;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackBusiness _feedbackBusiness;

        public FeedbackService()
        {
            _feedbackBusiness = new FeedbackBusiness();
        }

        public Task<IGiveandtakeResult> GetAllFeedbacks(int page = 1, int pageSize = 8)
            => _feedbackBusiness.GetAllFeedbacks(page, pageSize);

        public Task<IGiveandtakeResult> GetFeedbackById(int feedbackId)
            => _feedbackBusiness.GetFeedbackById(feedbackId);
        public Task<IGiveandtakeResult> CreateFeedback(CreateFeedbackDTO feedbackInfo)
            => _feedbackBusiness.CreateFeedback(feedbackInfo);

        public Task<IGiveandtakeResult> UpdateFeedback(int id, UpdateFeedbackDTO feedbackInfo)
            => _feedbackBusiness.UpdateFeedback(id, feedbackInfo);
        public Task<IGiveandtakeResult> DeleteFeedback(int id)
            => _feedbackBusiness.DeleteFeedback(id);
        public Task<IGiveandtakeResult> GetFeedbacksBySenderId(int senderId, int page = 1, int pageSize = 8)
            => _feedbackBusiness.GetFeedbacksBySenderId(senderId, page, pageSize);

        public Task<IGiveandtakeResult> GetFeedbacksByAccountId(int accountId, int page = 1, int pageSize = 8)
            => _feedbackBusiness.GetFeedbacksByAccountId(accountId, page, pageSize);

        public Task<IGiveandtakeResult> CreateFeedbackWithoutPoints(CreateFeedbackDTO createFeedbackDto)
            => _feedbackBusiness.CreateFeedbackWithoutPoints(createFeedbackDto);
    }
}
