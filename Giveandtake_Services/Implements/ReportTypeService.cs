using Giveandtake_Business;
using GiveandTake_Repo.DTOs.ReportType;
using Giveandtake_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Services.Implements
{
    public class ReportTypeService : IReportTypeService
    {
        private readonly ReportTypeBusiness _reportTypeBusiness;

        public ReportTypeService()
        {
            _reportTypeBusiness = new ReportTypeBusiness();
        }

        public Task<IGiveandtakeResult> CreateReportType(ReportCreateTypeDTO reportTypeInfo)
            => _reportTypeBusiness.CreateReportType(reportTypeInfo);

        public Task<IGiveandtakeResult> GetAllReportTypes()
            => _reportTypeBusiness.GetAllReportTypes();

        public Task<IGiveandtakeResult> GetReportTypeById(int id)
            => _reportTypeBusiness.GetReportTypeById(id);

        public Task<IGiveandtakeResult> UpdateReportType(int id, ReportUpdateTypeDTO reportTypeInfo)
            => _reportTypeBusiness.UpdateReportType(id, reportTypeInfo);

        public Task<IGiveandtakeResult> DeleteReportType(int id)
       => _reportTypeBusiness.DeleteReportType(id);

        public Task<IGiveandtakeResult> ChangeStatusReportType(int id)
            => _reportTypeBusiness.ChangeStatusReportType(id);
    }
}
