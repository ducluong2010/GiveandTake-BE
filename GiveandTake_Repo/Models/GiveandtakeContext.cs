using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GiveandTake_Repo.Models;

public partial class GiveandtakeContext : DbContext
{
    public GiveandtakeContext()
    {
    }

    public GiveandtakeContext(DbContextOptions<GiveandtakeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<DonationImage> DonationImages { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackMedium> FeedbackMedia { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<Rewarded> Rewardeds { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=103.163.25.35;port=3306;database=giveandtake;uid=admin;pwd=Luong123_A", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.0.1-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.HasIndex(e => e.RoleId, "RoleId");

            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.AvatarUrl).HasColumnType("text");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnType("bit(1)");
            entity.Property(e => e.Password).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PremiumUntil).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("Accounts_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PRIMARY");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Donations)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Donations_ibfk_1");

            entity.HasOne(d => d.Category).WithMany(p => p.Donations)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("Donations_ibfk_2");
        });

        modelBuilder.Entity<DonationImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.HasIndex(e => e.DonationId, "DonationId");

            entity.Property(e => e.IsThumbnail).HasColumnType("bit(1)");

            entity.Property(e => e.Url).HasColumnType("text");

            entity.HasOne(d => d.Donation).WithMany(p => p.DonationImages)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("DonationImages_ibfk_1");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PRIMARY");

            entity.ToTable("Feedback");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.DonationId, "DonationId");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Feedback_ibfk_1");

            entity.HasOne(d => d.Donation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("Feedback_ibfk_2");
        });

        modelBuilder.Entity<FeedbackMedium>(entity =>
        {
            entity.HasKey(e => e.FeedbackMediaId).HasName("PRIMARY");

            entity.HasIndex(e => e.FeedbackId, "FeedbackId");

            entity.Property(e => e.MediaType).HasMaxLength(100);
            entity.Property(e => e.MediaUrl).HasColumnType("text");

            entity.HasOne(d => d.Feedback).WithMany(p => p.FeedbackMedia)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FeedbackMedia_ibfk_1");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PRIMARY");

            entity.ToTable("Membership");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.Property(e => e.PremiumUntil).HasColumnType("datetime");
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Membership_ibfk_1");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("Message");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.IsRead).HasColumnType("bit(1)");
            entity.Property(e => e.SendAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Messages)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Message_ibfk_1");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.RewardId).HasName("PRIMARY");

            entity.ToTable("Reward");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.RewardName).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Reward_ibfk_1");
        });

        modelBuilder.Entity<Rewarded>(entity =>
        {
            entity.HasKey(e => e.RewardedId).HasName("PRIMARY");

            entity.ToTable("Rewarded");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.RewardId, "RewardId");

            entity.Property(e => e.ClaimedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Rewardeds)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("Rewarded_ibfk_2");

            entity.HasOne(d => d.Reward).WithMany(p => p.Rewardeds)
                .HasForeignKey(d => d.RewardId)
                .HasConstraintName("Rewarded_ibfk_1");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PRIMARY");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => e.TransactionDetailId).HasName("PRIMARY");

            entity.ToTable("TransactionDetail");

            entity.HasIndex(e => e.DonationId, "DonationId");

            entity.HasIndex(e => e.TransactionId, "TransactionId");

            entity.HasOne(d => d.Donation).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("TransactionDetail_ibfk_2");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionDetails)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("TransactionDetail_ibfk_1");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("UserRole");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
