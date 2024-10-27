using Giveandtake_Business;
using GiveandTake_Repo.DTOs.ReportType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Interfaces
{
    public interface IReportTypeService
    {
        Task<IGiveandtakeResult> GetReportTypeById(int id);
        Task<IGiveandtakeResult> GetAllReportTypes();
        Task<IGiveandtakeResult> CreateReportType(ReportCreateTypeDTO reportTypeInfo);
        Task<IGiveandtakeResult> UpdateReportType(int id, ReportUpdateTypeDTO reportTypeInfo);
        Task<IGiveandtakeResult> DeleteReportType(int id);
        Task<IGiveandtakeResult> ChangeStatusReportType(int id);
    }
}
