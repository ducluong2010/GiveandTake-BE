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
        Task<IGiveandtakeResult> GetAllReportsByStaff(int accountId);
        Task<IGiveandtakeResult> GetReportsByAccountId(int accountId);
        Task<IGiveandtakeResult> GetReportsByApprovedBy(int approvedBy);
        Task<IGiveandtakeResult> GetReportById(int reportId);
        Task<IGiveandtakeResult> CreateReport(ReportCreateDTO reportCreateDto);
        Task<IGiveandtakeResult> UpdateReport(int reportId, ReportUpdateDTO reportUpdateDto);
        Task<IGiveandtakeResult> DeleteReport(int reportId);
        Task<IGiveandtakeResult> ChangeStatusToProcessing(int reportId);
        Task<IGiveandtakeResult> ToggleProcessingStatus(int reportId); 
    }
}
