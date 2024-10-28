using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiveandTake_Repo.DTOs.Report;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.EntityFrameworkCore;

namespace Giveandtake_Business
{
    public class ReportBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public ReportBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<IGiveandtakeResult> GetAllReports(int page = 1, int pageSize = 8)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();

            var allReports = await reportRepository.GetListAsync(
                predicate: r => true,
                selector: r => new ReportDTO
                {
                    ReportId = r.ReportId,
                    AccountId = r.AccountId,
                    AccountName = r.Account.FullName,
                    ReportTypeId = r.ReportTypeId,
                    ReportTypeName = r.ReportType.ReportTypeName,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedDate = r.CreatedDate,
                    ReportMediaUrls = r.ReportMedia.Select(m => m.ReportUrl).ToList()
                },
                include: source => source
                    .Include(r => r.Account)
                    .Include(r => r.ReportType)
                    .Include(r => r.ReportMedia)
            );

            int totalItems = allReports.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages) page = totalPages;
            if (totalItems == 0)
            {
                return new GiveandtakeResult(new PaginatedResult<ReportDTO>
                {
                    Items = new List<ReportDTO>(),
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                });
            }

            var paginatedReports = allReports
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<ReportDTO>
            {
                Items = paginatedReports,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return new GiveandtakeResult(paginatedResult);
        }
        public async Task<IGiveandtakeResult> GetReportById(int reportId)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();

            var report = await reportRepository.SingleOrDefaultAsync(
                predicate: r => r.ReportId == reportId,
                include: source => source
                    .Include(r => r.Account)
                    .Include(r => r.ReportType)
                    .Include(r => r.ReportMedia)
            );

            if (report == null)
            {
                return new GiveandtakeResult(404, "Report not found");
            }

            var reportDTO = new ReportDTO
            {
                ReportId = report.ReportId,
                AccountId = report.AccountId,
                AccountName = report.Account?.FullName,
                ReportTypeId = report.ReportTypeId,
                ReportTypeName = report.ReportType?.ReportTypeName,
                Description = report.Description,
                Status = report.Status,
                CreatedDate = report.CreatedDate,
                ReportMediaUrls = report.ReportMedia?.Select(m => m.ReportUrl).ToList() ?? new List<string>()
            };

            return new GiveandtakeResult(reportDTO);
        }
        public async Task<IGiveandtakeResult> CreateReport(ReportCreateDTO reportCreateDTO)
        {
            var account = await _unitOfWork.GetRepository<Account>()
                .FirstOrDefaultAsync(a => a.AccountId == reportCreateDTO.AccountId);
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found");
            }

            var reportType = await _unitOfWork.GetRepository<ReportType>()
                .FirstOrDefaultAsync(rt => rt.ReportTypeId == reportCreateDTO.ReportTypeId);
            if (reportType == null)
            {
                return new GiveandtakeResult(-1, "Report type not found");
            }

            var existingReport = await _unitOfWork.GetRepository<Report>()
      .FirstOrDefaultAsync(r => r.Description == reportCreateDTO.Description && r.ReportTypeId == reportCreateDTO.ReportTypeId);
            if (existingReport != null)
            {
                return new GiveandtakeResult(-1, "A report with the same description already exists for this report type.");
            }

            var newReport = new Report
            {
                AccountId = reportCreateDTO.AccountId,
                Description = reportCreateDTO.Description,
                ReportTypeId = reportCreateDTO.ReportTypeId,
                Status = "Pending",
                CreatedDate = reportCreateDTO.CreatedDate ?? DateTime.UtcNow,
                ReportMedia = reportCreateDTO.ReportMediaUrls?.Select(url => new ReportMedium
                {
                    ReportUrl = url
                }).ToList() ?? new List<ReportMedium>()
            };

            await _unitOfWork.GetRepository<Report>().InsertAsync(newReport);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create report unsuccessfully");
            }

            return new GiveandtakeResult(1, "Report created successfully");
        }
        public async Task<IGiveandtakeResult> UpdateReport(int reportId, ReportUpdateDTO reportUpdateDTO)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var report = await reportRepository.SingleOrDefaultAsync(
                predicate: r => r.ReportId == reportId,
                include: source => source
                    .Include(r => r.Account)
                    .Include(r => r.ReportType)
                    .Include(r => r.ReportMedia)
            );
            if (report == null)
            {
                return new GiveandtakeResult(-1, "Report not found");
            }
            if (report.Status != "Pending")
            {
                return new GiveandtakeResult(-1, "Only reports with 'Pending' status can be updated.");
            }
            report.Description = reportUpdateDTO.Description ?? report.Description; 
            report.ReportTypeId = reportUpdateDTO.ReportTypeId ?? report.ReportTypeId; 

            report.ReportMedia = reportUpdateDTO.ReportMediaUrls?.Select(url => new ReportMedium
            {
                ReportUrl = url
            }).ToList() ?? new List<ReportMedium>();

            reportRepository.UpdateAsync(report);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Update report unsuccessfully");
            }

            return new GiveandtakeResult(1, "Report updated successfully");
        }
        public async Task<IGiveandtakeResult> DeleteReport(int reportId)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();

            var report = await reportRepository.SingleOrDefaultAsync(
                predicate: r => r.ReportId == reportId,
                include: source => source.Include(r => r.ReportMedia)
            );

            if (report == null)
            {
                return new GiveandtakeResult(-1, "Report not found");
            }

            if (report.Status != "Pending")
            {
                return new GiveandtakeResult(-1, "Only reports with status 'Pending' can be deleted.");
            }
            var reportMediumRepository = _unitOfWork.GetRepository<ReportMedium>();

            if (report.ReportMedia != null && report.ReportMedia.Any())
            {
                foreach (var media in report.ReportMedia)
                {
                    reportMediumRepository.DeleteAsync(media); 
                }
            }


            reportRepository.DeleteAsync(report);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Delete report unsuccessfully");
            }

            return new GiveandtakeResult(1, "Report deleted successfully");
        }
        public async Task<IGiveandtakeResult> ChangeStatusToProcessing(int reportId)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var report = await reportRepository.SingleOrDefaultAsync(
                predicate: r => r.ReportId == reportId, 
                orderBy: null, 
                include: null  
            );

            if (report == null)
            {
                return new GiveandtakeResult(-1, "Report not found");
            }

            if (report.Status != "Pending")
            {
                return new GiveandtakeResult(-1, "Only reports with status 'Pending' can be changed to 'Processing'");
            }

            report.Status = "Processing";
            reportRepository.UpdateAsync(report);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful
                ? new GiveandtakeResult(1, "Status changed to 'Processing' successfully")
                : new GiveandtakeResult(-1, "Failed to change status");
        }
        public async Task<IGiveandtakeResult> ToggleProcessingStatus(int reportId)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var report = await reportRepository.SingleOrDefaultAsync(
                predicate: r => r.ReportId == reportId,
                orderBy: null,
                include: null
            );

            if (report == null)
            {
                return new GiveandtakeResult(-1, "Report not found");
            }

            report.Status = report.Status == "Processing" ? "Processed" : "Processing";
            reportRepository.UpdateAsync(report);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful
                ? new GiveandtakeResult(1, $"Status changed to '{report.Status}' successfully")
                : new GiveandtakeResult(-1, "Failed to change status");
        }
    }
}
