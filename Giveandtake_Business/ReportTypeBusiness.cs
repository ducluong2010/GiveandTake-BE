using GiveandTake_Repo.DTOs.ReportType;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class ReportTypeBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public ReportTypeBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllReportTypes()
        {
            var reportTypeRepository = _unitOfWork.GetRepository<ReportType>();

            var allReportTypes = await reportTypeRepository.GetListAsync(
                predicate: rt => true,
                selector: rt => new ReportTypeDTO
                {
                    ReportTypeId = rt.ReportTypeId,
                    ReportTypeName = rt.ReportTypeName,
                    Description = rt.Description,
                    Status = rt.Status
                }
            );

            return new GiveandtakeResult(allReportTypes.ToList());
        }

        public async Task<IGiveandtakeResult> GetReportTypeById(int reportTypeId)
        {
            var reportTypeRepository = _unitOfWork.GetRepository<ReportType>();

            var reportType = await reportTypeRepository.SingleOrDefaultAsync(
                predicate: rt => rt.ReportTypeId == reportTypeId
            );

            if (reportType == null)
            {
                return new GiveandtakeResult(404, "Report Type not found");
            }

            var reportTypeDTO = new ReportTypeDTO
            {
                ReportTypeId = reportType.ReportTypeId,
                ReportTypeName = reportType.ReportTypeName,
                Description = reportType.Description,
                Status = reportType.Status
            };

            return new GiveandtakeResult(reportTypeDTO);
        }
        public async Task<IGiveandtakeResult> CreateReportType(ReportCreateTypeDTO reportTypeInfo)
        {
            GiveandtakeResult result = new GiveandtakeResult();

            var existingReportType = await _unitOfWork.GetRepository<ReportType>()
                .SingleOrDefaultAsync<ReportType>(
                    selector: r => r,
                    predicate: r => r.ReportTypeName == reportTypeInfo.ReportTypeName
                );


            if (existingReportType != null)
            {
                result.Status = -1;
                result.Message = "Report type with this name already exists.";
                return result; 
            }

            ReportType newReportType = new ReportType
            {
                ReportTypeName = reportTypeInfo.ReportTypeName,
                Description = reportTypeInfo.Description,
                Status = "True"
            };

            await _unitOfWork.GetRepository<ReportType>().InsertAsync(newReportType);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                result.Status = -1;
                result.Message = "Create unsuccessfully";
            }
            else
            {
                result = new GiveandtakeResult(1, "Create Successfully");
            }

            return result;
        }
        public async Task<IGiveandtakeResult> UpdateReportType(int id, ReportUpdateTypeDTO reportTypeInfo)
        {
            var existingReportType = await _unitOfWork.GetRepository<ReportType>()
                .SingleOrDefaultAsync(predicate: r => r.ReportTypeId == id);
            if (existingReportType == null)
            {
                return new GiveandtakeResult(-1, "Report type not found");
            }
            var duplicateReportType = await _unitOfWork.GetRepository<ReportType>()
                    .SingleOrDefaultAsync(predicate: r => r.ReportTypeName == reportTypeInfo.ReportTypeName && r.ReportTypeId != id);

            if (duplicateReportType != null)
            {
                return new GiveandtakeResult(-1, "Report type name already exists");
            }

            existingReportType.ReportTypeName = string.IsNullOrEmpty(reportTypeInfo.ReportTypeName)
                ? existingReportType.ReportTypeName
                : reportTypeInfo.ReportTypeName;

            existingReportType.Description = string.IsNullOrEmpty(reportTypeInfo.Description)
                ? existingReportType.Description
                : reportTypeInfo.Description;

            existingReportType.Status = string.IsNullOrEmpty(reportTypeInfo.Status)
                ? existingReportType.Status
                : reportTypeInfo.Status;

            _unitOfWork.GetRepository<ReportType>().UpdateAsync(existingReportType);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Report type updated successfully")
                : new GiveandtakeResult(-1, "Update unsuccessfully");
        }
        public async Task<IGiveandtakeResult> DeleteReportType(int id)
        {
            var existingReportType = await _unitOfWork.GetRepository<ReportType>()
                .SingleOrDefaultAsync(predicate: r => r.ReportTypeId == id);
            if (existingReportType == null)
            {
                return new GiveandtakeResult(-1, "Report type not found");
            }

            _unitOfWork.GetRepository<ReportType>().DeleteAsync(existingReportType);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Report type deleted successfully")
                : new GiveandtakeResult(-1, "Delete unsuccessfully");
        }

        public async Task<IGiveandtakeResult> ChangeStatusReportType(int id)
        {
            var existingReportType = await _unitOfWork.GetRepository<ReportType>()
                .SingleOrDefaultAsync(predicate: r => r.ReportTypeId == id);
            if (existingReportType == null)
            {
                return new GiveandtakeResult(-1, "Report type not found");
            }
            
            bool currentStatus = existingReportType.Status == "True";

            existingReportType.Status = currentStatus ? "False" : "True"; 

            _unitOfWork.GetRepository<ReportType>().UpdateAsync(existingReportType);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful
                ? new GiveandtakeResult(1, "Report type status changed successfully")
                : new GiveandtakeResult(-1, "Change status unsuccessfully");
        }
    }
}
