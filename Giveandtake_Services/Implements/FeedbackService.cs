using Giveandtake_Business;
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
    }
}
