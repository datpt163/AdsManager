using System;
using System.Collections.Generic;
using FBAdsManager.Common.Database.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace FBAdsManager.Common.Database.DbContexts;

public partial class DbAdsmanagerContext : DbContext
{
    public DbAdsmanagerContext()
    {
    }

    public DbAdsmanagerContext(DbContextOptions<DbAdsmanagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ads> Ads { get; set; }

    public virtual DbSet<AdsAccount> AdsAccounts { get; set; }

    public virtual DbSet<Adset> Adsets { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Insight> Insights { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_unicode_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Ads>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ads");

            entity.HasIndex(e => e.AdsetId, "adset_id");

            entity.Property(e => e.Id)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(250)
                .HasColumnName("action_type");
            entity.Property(e => e.Adcreatives)
                .HasMaxLength(250)
                .HasColumnName("adcreatives");
            entity.Property(e => e.AdsetId)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("adset_id");
            entity.Property(e => e.ConfiguredStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("configured_status");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("created_time");
            entity.Property(e => e.EffectiveStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("effective_status");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.StartTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.TrackingSpecs)
                .HasMaxLength(250)
                .HasColumnName("tracking_specs");
            entity.Property(e => e.UpdateDataTime)
                .HasColumnType("datetime")
                .HasColumnName("update_data_time");
            entity.Property(e => e.UpdatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("updated_time");

            entity.HasOne(d => d.Adset).WithMany(p => p.Ads)
                .HasForeignKey(d => d.AdsetId)
                .HasConstraintName("ads_ibfk_1");
        });

        modelBuilder.Entity<AdsAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.ToTable("adsAccounts");

            entity.HasIndex(e => e.EmployeeId, "employeeId");

            entity.Property(e => e.AccountId)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.AccountStatus).HasColumnName("account_status");
            entity.Property(e => e.AmountSpent)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("amount_spent");
            entity.Property(e => e.Balance)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("balance");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("created_time");
            entity.Property(e => e.Currency)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("currency");
            entity.Property(e => e.DisableReason).HasColumnName("disable_reason");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeId");
            entity.Property(e => e.InforCardBanking)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("inforCardBanking");
            entity.Property(e => e.IsPersonal).HasColumnName("is_personal");
            entity.Property(e => e.MinCampaignGroupSpendCap)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("min_campaign_group_spend_cap");
            entity.Property(e => e.MinDailyBudget).HasColumnName("min_daily_budget");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .HasColumnName("name");
            entity.Property(e => e.Owner)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("owner");
            entity.Property(e => e.SpendCap)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("spend_cap");
            entity.Property(e => e.TimezoneName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("timezone_name");
            entity.Property(e => e.TypeCardBanking).HasColumnName("typeCardBanking");
            entity.Property(e => e.UpdateDataTime)
                .HasColumnType("datetime")
                .HasColumnName("update_data_time");

            entity.HasOne(d => d.Employee).WithMany(p => p.AdsAccounts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("adsAccounts_ibfk_1");
        });

        modelBuilder.Entity<Adset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("adsets");

            entity.HasIndex(e => e.CampaignId, "campaign_id");

            entity.Property(e => e.Id)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.BudgetRemaining)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("budget_remaining");
            entity.Property(e => e.CampaignId)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("campaign_id");
            entity.Property(e => e.ConfiguredStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("configured_status");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("created_time");
            entity.Property(e => e.DailyBudget)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("daily_budget");
            entity.Property(e => e.EffectiveStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("effective_status");
            entity.Property(e => e.LifetimeBudget)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("lifetime_budget");
            entity.Property(e => e.LifetimeImps).HasColumnName("lifetime_imps");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.PromoteObjectPageId)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("Promote_object_page_id");
            entity.Property(e => e.StartTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.Targeting)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("targeting");
            entity.Property(e => e.UpdateDataTime)
                .HasColumnType("datetime")
                .HasColumnName("update_data_time");
            entity.Property(e => e.UpdatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("updated_time");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Adsets)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("adsets_ibfk_1");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("branch");

            entity.HasIndex(e => e.OrganizationId, "branch_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeleteDate)
                .HasColumnType("datetime")
                .HasColumnName("deleteDate");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.OrganizationId).HasColumnName("organizationId");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Organization).WithMany(p => p.Branches)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("branch_ibfk_1");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("campaigns");

            entity.HasIndex(e => e.AccountId, "account_id");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("account_id");
            entity.Property(e => e.BudgetRebalanceFlag)
                .HasColumnType("bit(1)")
                .HasColumnName("budget_rebalance_flag");
            entity.Property(e => e.BudgetRemaining)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("budget_remaining");
            entity.Property(e => e.BuyingType)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("buying_type");
            entity.Property(e => e.ConfiguredStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("configured_status");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("created_time");
            entity.Property(e => e.DailyBudget)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("daily_budget");
            entity.Property(e => e.EffectiveStatus)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("effective_status");
            entity.Property(e => e.LifetimeBudget)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("lifetime_budget");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.Objective)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("objective");
            entity.Property(e => e.SpecialAdCategory)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("special_ad_category");
            entity.Property(e => e.SpecialAdCategoryCountry)
                .HasMaxLength(250)
                .IsFixedLength()
                .HasColumnName("special_ad_category_country");
            entity.Property(e => e.StartTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.UpdateDataTime)
                .HasColumnType("datetime")
                .HasColumnName("update_data_time");
            entity.Property(e => e.UpdatedTime)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("updated_time");

            entity.HasOne(d => d.Account).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("campaigns_ibfk_1");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employee");

            entity.HasIndex(e => e.GroupId, "employee_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeleteDate)
                .HasColumnType("datetime")
                .HasColumnName("deleteDate");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Group).WithMany(p => p.Employees)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("employee_ibfk_1");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("group");

            entity.HasIndex(e => e.BranchId, "group_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branchId");
            entity.Property(e => e.DeleteDate)
                .HasColumnType("datetime")
                .HasColumnName("deleteDate");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("updateDate");

            entity.HasOne(d => d.Branch).WithMany(p => p.Groups)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("group_ibfk_1");
        });

        modelBuilder.Entity<Insight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("insight");

            entity.HasIndex(e => e.AdsId, "ads_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Actions)
                .HasMaxLength(250)
                .HasColumnName("actions");
            entity.Property(e => e.AdsId)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("ads_id");
            entity.Property(e => e.Clicks)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("clicks");
            entity.Property(e => e.Cpc)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("cpc");
            entity.Property(e => e.Cpm)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("cpm");
            entity.Property(e => e.Cpp)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("cpp");
            entity.Property(e => e.Ctr)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("ctr");
            entity.Property(e => e.DateAt)
                .HasColumnType("datetime")
                .HasColumnName("dateAt");
            entity.Property(e => e.Frequency)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("frequency");
            entity.Property(e => e.Impressions)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("impressions");
            entity.Property(e => e.Reach)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("reach");
            entity.Property(e => e.Spend)
                .HasMaxLength(60)
                .IsFixedLength()
                .HasColumnName("spend");
            entity.Property(e => e.UpdateDataTime)
                .HasColumnType("datetime")
                .HasColumnName("Update_Data_Time");

            entity.HasOne(d => d.Ads).WithMany(p => p.Insights)
                .HasForeignKey(d => d.AdsId)
                .HasConstraintName("insight_ibfk_1");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("organization");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeleteDate)
                .HasColumnType("datetime")
                .HasColumnName("deleteDate");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.RoleId, "roleId");

            entity.HasIndex(e => e.GroupId, "ten_khoa_ngoai");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessTokenFb)
                .HasMaxLength(250)
                .IsFixedLength()
                .HasColumnName("accessTokenFb");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.GroupId).HasColumnName("groupId");
            entity.Property(e => e.IsActive)
                .HasColumnType("bit(1)")
                .HasColumnName("isActive");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("ten_khoa_ngoai");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
