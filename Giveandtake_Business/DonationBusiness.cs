﻿using GiveandTake_Repo.DTOs.Donation;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Giveandtake_Business
{
    public class DonationBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public DonationBusiness()
        {
            _unitOfWork = new UnitOfWork();
        }

        // Get all donations
        public async Task<IGiveandtakeResult> GetAllDonations()
        {
            var donationList = await _unitOfWork.GetRepository<Donation>()
                .GetListAsync();
            var result = donationList.Select(d => new DonationDTO
            {
                DonationId = d.DonationId,
                AccountId = d.AccountId,
                CategoryId = d.CategoryId,
                Name = d.Name,
                Description = d.Description,
                Point = d.Point,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
                ApprovedBy = d.ApprovedBy,
                TotalRating = d.TotalRating,
                Status = d.Status,
                DonationImages = d.DonationImages.Select(di => di.Url).ToList()
            });
            return new GiveandtakeResult(result);
        }

        // Get donation by id
        public async Task<IGiveandtakeResult> GetDonationById(int donationId)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == donationId,
                                      selector: x => new DonationDTO
                                      {
                                          DonationId = x.DonationId,
                                          AccountId = x.AccountId,
                                          CategoryId = x.CategoryId,
                                          Name = x.Name,
                                          Description = x.Description,
                                          Point = x.Point,
                                          CreatedAt = x.CreatedAt,
                                          UpdatedAt = x.UpdatedAt,
                                          ApprovedBy = x.ApprovedBy,
                                          TotalRating = x.TotalRating,
                                          Status = x.Status,
                                          DonationImages = x.DonationImages.Select(di => di.Url).ToList()
                                      });
            return new GiveandtakeResult(donation);
        }

        // Update donation information using CreateUpdateDonationDTO
        public async Task<IGiveandtakeResult> UpdateDonation(int id, CreateUpdateDonationDTO donationInfo)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == id);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            // Update only the fields that are provided in the DTO
            donation.Name = !string.IsNullOrEmpty(donationInfo.Name) ? donationInfo.Name : donation.Name;
            donation.Description = !string.IsNullOrEmpty(donationInfo.Description) ? donationInfo.Description : donation.Description;
            donation.Point = donationInfo.Point ?? donation.Point;
            donation.ApprovedBy = donationInfo.ApprovedBy ?? donation.ApprovedBy;
            donation.TotalRating = donationInfo.TotalRating ?? donation.TotalRating;
            donation.Status = !string.IsNullOrEmpty(donationInfo.Status) ? donationInfo.Status : donation.Status;
            donation.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Donation>().UpdateAsync(donation);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Donation updated successfully");
        }

        // Create new donation using CreateUpdateDonationDTO
        public async Task<IGiveandtakeResult> CreateDonation(CreateUpdateDonationDTO donationInfo)
        {
            var newDonation = new Donation
            {
                AccountId = donationInfo.AccountId,
                CategoryId = donationInfo.CategoryId,
                Name = donationInfo.Name,
                Description = donationInfo.Description,
                Point = donationInfo.Point,
                CreatedAt = DateTime.Now,
                Status = donationInfo.Status,
                DonationImages = new List<DonationImage>() // Xử lý nếu cần thêm DonationImages
            };

            await _unitOfWork.GetRepository<Donation>().InsertAsync(newDonation);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful)
            {
                return new GiveandtakeResult(-1, "Create donation unsuccessfully");
            }

            return new GiveandtakeResult(1, "Donation created successfully");
        }

        // Delete donation
        public async Task<IGiveandtakeResult> DeleteDonation(int id)
        {
            var donation = await _unitOfWork.GetRepository<Donation>()
                .SingleOrDefaultAsync(predicate: d => d.DonationId == id);

            if (donation == null)
            {
                return new GiveandtakeResult(-1, "Donation not found");
            }

            _unitOfWork.GetRepository<Donation>().DeleteAsync(donation);
            await _unitOfWork.CommitAsync();
            return new GiveandtakeResult(1, "Donation deleted successfully");
        }
    }
}
