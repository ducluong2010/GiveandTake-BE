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
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name); 

            var allReports = await reportRepository.GetListAsync(
                predicate: r => true,
                selector: r => new ReportDTO
                {
                    ReportId = r.ReportId,
                    SenderId = r.SenderId,
                    SenderName = r.SenderId.HasValue && accountDict.ContainsKey(r.SenderId.Value)
                        ? accountDict[r.SenderId.Value]
                        : null,  

                    AccountId = r.AccountId,
                    AccountName = r.AccountId.HasValue && accountDict.ContainsKey(r.AccountId.Value)
                        ? accountDict[r.AccountId.Value]
                        : null,

                    ReportTypeId = r.ReportTypeId,
                    ReportTypeName = r.ReportType.ReportTypeName,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedDate = r.CreatedDate,
                    ReportMediaUrls = r.ReportMedia.Select(m => m.ReportUrl).ToList(),
                    DonationId = r.DonationId,
                    DonationName = r.DonationId.HasValue && donationDict.ContainsKey(r.DonationId.Value)
                        ? donationDict[r.DonationId.Value]
                        : null
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

        public async Task<IGiveandtakeResult> GetReportsByAccountId(int accountId)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var accountExists = await accountRepository.GetAllAsync(a => a.AccountId == accountId)
                                                        .ContinueWith(t => t.Result.Any());

            if (!accountExists)
            {
                return new GiveandtakeResult(-1, "SenderId does not exist.");
            }

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);

            var allReports = await reportRepository.GetListAsync(
                predicate: r => r.AccountId == accountId,
                selector: r => new ReportDTO
                {
                    ReportId = r.ReportId,
                    SenderId = r.SenderId,
                    SenderName = r.SenderId.HasValue && accountDict.ContainsKey(r.SenderId.Value)
                        ? accountDict[r.SenderId.Value]
                        : null,

                    AccountId = r.AccountId,
                    AccountName = r.AccountId.HasValue && accountDict.ContainsKey(r.AccountId.Value)
                        ? accountDict[r.AccountId.Value]
                        : null,

                    ReportTypeId = r.ReportTypeId,
                    ReportTypeName = r.ReportType.ReportTypeName,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedDate = r.CreatedDate,
                    ReportMediaUrls = r.ReportMedia.Select(m => m.ReportUrl).ToList(),
                    DonationId = r.DonationId,
                    DonationName = r.DonationId.HasValue && donationDict.ContainsKey(r.DonationId.Value)
                        ? donationDict[r.DonationId.Value]
                        : null
                },
                include: source => source
                    .Include(r => r.Account)     
                    .Include(r => r.ReportType)  
                    .Include(r => r.ReportMedia) 
            );

            if (allReports == null || !allReports.Any())
            {
                return new GiveandtakeResult(-1, "No reports found for the specified sender.");
            }

            var sortedReports = allReports.OrderByDescending(r => r.CreatedDate).ToList();

            return new GiveandtakeResult(sortedReports);
        }

        public async Task<IGiveandtakeResult> GetReportsByApprovedBy(int approvedBy)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var approverExists = await accountRepository.GetAllAsync(a => a.AccountId == approvedBy)
                                                        .ContinueWith(t => t.Result.Any());

            if (!approverExists)
            {
                return new GiveandtakeResult(-1, "ApprovedBy does not exist.");
            }

            var allAccounts = await accountRepository.GetAllAsync();
            var accountDict = allAccounts.ToDictionary(a => a.AccountId, a => a.FullName);

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);

            var approvedDonationIds = allDonations
                                       .Where(d => d.ApprovedBy == approvedBy)
                                       .Select(d => d.DonationId)
                                       .ToHashSet(); 

            var allReports = await reportRepository.GetListAsync(
                predicate: r => r.DonationId.HasValue && approvedDonationIds.Contains(r.DonationId.Value),
                selector: r => new ReportDTO
                {
                    ReportId = r.ReportId,
                    SenderId = r.SenderId,
                    SenderName = r.SenderId.HasValue && accountDict.ContainsKey(r.SenderId.Value)
                        ? accountDict[r.SenderId.Value]
                        : null,

                    AccountId = r.AccountId,
                    AccountName = r.AccountId.HasValue && accountDict.ContainsKey(r.AccountId.Value)
                        ? accountDict[r.AccountId.Value]
                        : null,

                    ReportTypeId = r.ReportTypeId,
                    ReportTypeName = r.ReportType.ReportTypeName,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedDate = r.CreatedDate,
                    ReportMediaUrls = r.ReportMedia.Select(m => m.ReportUrl).ToList(),
                    DonationId = r.DonationId,
                    DonationName = r.DonationId.HasValue && donationDict.ContainsKey(r.DonationId.Value)
                        ? donationDict[r.DonationId.Value]
                        : null
                },
                include: source => source
                    .Include(r => r.Account)
                    .Include(r => r.ReportType)
                    .Include(r => r.ReportMedia)
            );

            if (allReports == null || !allReports.Any())
            {
                return new GiveandtakeResult(-1, "No reports found for the specified ApprovedBy.");
            }

            var sortedReports = allReports.OrderByDescending(r => r.CreatedDate).ToList();

            return new GiveandtakeResult(sortedReports);
        }

        public async Task<IGiveandtakeResult> GetAllReportsByStaff(int accountId)
        {
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

            var account = await accountRepository.GetAllAsync(a => a.AccountId == accountId && a.RoleId == 2)
                                                 .ContinueWith(t => t.Result.FirstOrDefault());
            if (account == null)
            {
                return new GiveandtakeResult(-1, "Account not found or not a staff member");
            }

            int accountActiveTime = account.ActiveTime ?? -1;
            TimeSpan startTime, endTime;

            if (accountActiveTime == 1)
            {
                startTime = TimeSpan.Zero;
                endTime = new TimeSpan(12, 0, 0);
            }
            else if (accountActiveTime == 2)
            {
                startTime = new TimeSpan(12, 0, 0);
                endTime = new TimeSpan(24, 0, 0);
            }
            else
            {
                return new GiveandtakeResult(-1, "Invalid ActiveTime value in account");
            }

            var allDonations = await donationRepository.GetAllAsync();
            var donationDict = allDonations.ToDictionary(d => d.DonationId, d => d.Name);

            var allReports = await reportRepository.GetListAsync(
                predicate: r => r.CreatedDate.HasValue &&
                               r.CreatedDate.Value.TimeOfDay >= startTime &&
                               r.CreatedDate.Value.TimeOfDay < endTime,
                selector: r => new ReportDTO
                {
                    ReportId = r.ReportId,
                    SenderId = r.SenderId,
                    SenderName = r.SenderId.HasValue && r.SenderId.Value != 0
                        ? r.SenderId.Value.ToString() 
                        : null,

                    AccountId = r.AccountId,
                    AccountName = r.AccountId.HasValue && r.AccountId.Value != 0
                        ? r.Account.FullName 
                        : null,

                    ReportTypeId = r.ReportTypeId,
                    ReportTypeName = r.ReportType.ReportTypeName,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedDate = r.CreatedDate,
                    ReportMediaUrls = r.ReportMedia.Select(m => m.ReportUrl).ToList(),
                    DonationId = r.DonationId,
                    DonationName = r.DonationId.HasValue && donationDict.ContainsKey(r.DonationId.Value)
                        ? donationDict[r.DonationId.Value]
                        : null
                },
                include: source => source
                    .Include(r => r.Account)
                    .Include(r => r.ReportType)
                    .Include(r => r.ReportMedia)
            );

            var sortedReports = allReports.OrderByDescending(r => r.CreatedDate).ToList();

            return new GiveandtakeResult(sortedReports);
        }

        public async Task<IGiveandtakeResult> GetReportById(int reportId)
        {
            var reportRepository = _unitOfWork.GetRepository<Report>();
            var accountRepository = _unitOfWork.GetRepository<Account>();
            var donationRepository = _unitOfWork.GetRepository<Donation>();

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

            var senderName = report.SenderId.HasValue
                ? (await accountRepository.GetAllAsync(a => a.AccountId == report.SenderId.Value))
                    .FirstOrDefault()?.FullName
                : null;

            var accountName = report.AccountId.HasValue
                ? (await accountRepository.GetAllAsync(a => a.AccountId == report.AccountId.Value))
                    .FirstOrDefault()?.FullName
                : null;

            var donationName = report.DonationId.HasValue
                ? (await donationRepository.GetAllAsync(d => d.DonationId == report.DonationId.Value))
                    .FirstOrDefault()?.Name
                : null;

            var reportDTO = new ReportDTO
            {
                ReportId = report.ReportId,
                SenderId = report.SenderId,
                SenderName = senderName, 
                AccountId = report.AccountId,
                AccountName = accountName,  
                ReportTypeId = report.ReportTypeId,
                ReportTypeName = report.ReportType?.ReportTypeName,
                Description = report.Description,
                Status = report.Status,
                CreatedDate = report.CreatedDate,
                ReportMediaUrls = report.ReportMedia?.Select(m => m.ReportUrl).ToList() ?? new List<string>(),
                DonationId = report.DonationId,
                DonationName = donationName
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

            var newReport = new Report
            {
                SenderId = reportCreateDTO.SenderId,
                DonationId = reportCreateDTO.DonationId,
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
