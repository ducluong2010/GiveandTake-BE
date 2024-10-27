using Giveandtake_Business;
using GiveandTake_Repo.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IReportService
    {
        Task<IGiveandtakeResult> GetAllReports(int page = 1, int pageSize = 8);
        Task<IGiveandtakeResult> GetReportById(int reportId);
        Task<IGiveandtakeResult> CreateReport(ReportCreateDTO reportCreateDto);
    }
}
