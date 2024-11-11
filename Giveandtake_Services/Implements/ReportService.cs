﻿using Giveandtake_Business;
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
        public Task<IGiveandtakeResult> GetAllReportsByStaff(int accountId)
            => _reportBusiness.GetAllReportsByStaff(accountId);
        public Task<IGiveandtakeResult> GetReportsBySenderId(int senderId)
           => _reportBusiness.GetReportsBySenderId(senderId);
        public Task<IGiveandtakeResult> GetReportById(int reportId)
            => _reportBusiness.GetReportById(reportId);
        public Task<IGiveandtakeResult> CreateReport(ReportCreateDTO reportCreateDto) 
            => _reportBusiness.CreateReport(reportCreateDto);
        public Task<IGiveandtakeResult> UpdateReport(int reportId, ReportUpdateDTO reportUpdateDto) 
            => _reportBusiness.UpdateReport(reportId, reportUpdateDto);
        public Task<IGiveandtakeResult> DeleteReport(int reportId)
            => _reportBusiness.DeleteReport(reportId);
        public Task<IGiveandtakeResult> ChangeStatusToProcessing(int reportId)
            => _reportBusiness.ChangeStatusToProcessing(reportId);
        public Task<IGiveandtakeResult> ToggleProcessingStatus(int reportId)
            => _reportBusiness.ToggleProcessingStatus(reportId); 
    }
}
