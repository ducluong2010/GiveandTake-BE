using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Report;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly ReportBusiness _reportBusiness;

        public ReportService()
        {
            _reportBusiness = new ReportBusiness();
        }

        public Task<IGiveandtakeResult> GetAllReports(int page = 1, int pageSize = 8)
            => _reportBusiness.GetAllReports(page, pageSize);

        public Task<IGiveandtakeResult> GetReportById(int reportId)
            => _reportBusiness.GetReportById(reportId);
        public Task<IGiveandtakeResult> CreateReport(ReportCreateDTO reportCreateDto) 
            => _reportBusiness.CreateReport(reportCreateDto);
    }
}
