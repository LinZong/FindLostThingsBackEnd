using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using FindLostThingsBackEnd.Persistence.Model;

namespace FindLostThingsBackEnd.Persistence.DAO.Context
{
    public partial class LostContext : DbContext
    {
        public LostContext()
        {
        }

        public LostContext(DbContextOptions<LostContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LostThingsRecord> LostThingsRecord { get; set; }
        public virtual DbQuery<SchoolBuildingInfo> SchoolInfo { get; set; }
        public virtual DbSet<SupportSchool> SupportSchool { get; set; }
        public virtual DbSet<ThingsCategory> ThingsCategory { get; set; }
        public virtual DbSet<ThingsDetail> ThingsDetail { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LostThingsRecord>(entity =>
            {
                entity.ToTable("lost_things_record", "lost");

                entity.HasIndex(e => e.FoundTime)
                    .HasName("things_search_index_2");

                entity.HasIndex(e => e.PublishTime)
                    .HasName("things_search_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FoundAddrDescription)
                    .HasColumnName("found_addr_description")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FoundAddress)
                    .IsRequired()
                    .HasColumnName("found_address")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.FoundTime)
                    .HasColumnName("found_time")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Given)
                    .HasColumnName("given")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.GivenContacts)
                    .HasColumnName("given_contacts")
                    .IsUnicode(false);

                entity.Property(e => e.GivenTime)
                    .HasColumnName("given_time")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Isgiven)
                    .HasColumnName("isgiven")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PublishTime)
                    .HasColumnName("publish_time")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Publisher)
                    .IsRequired()
                    .HasColumnName("publisher")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PublisherContacts)
                    .HasColumnName("publisher_contacts")
                    .IsUnicode(false);

                entity.Property(e => e.ThingAddiDescription)
                    .HasColumnName("thing_addi_description")
                    .IsUnicode(false);

                entity.Property(e => e.ThingCatId)
                    .HasColumnName("thing_cat_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ThingDetailId)
                    .HasColumnName("thing_detail_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ThingPhotoUrls)
                    .HasColumnName("thing_photo_urls")
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Query<SchoolBuildingInfo>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BuilddingAddress)
                    .HasColumnName("building_address")
                    .IsUnicode(false);

                entity.Property(e => e.BuildingName)
                    .IsRequired()
                    .HasColumnName("building_name")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");
            });

            modelBuilder.Entity<SupportSchool>(entity =>
            {
                entity.ToTable("support_school", "lost");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SchoolAddrTbName)
                    .IsRequired()
                    .HasColumnName("school_addr_tb_name")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ThingsCategory>(entity =>
            {
                entity.ToTable("things_category", "lost");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ThingsDetail>(entity =>
            {
                entity.ToTable("things_detail", "lost");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("category_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("user_info", "lost");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasColumnName("access_token")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AndroidDevId)
                    .IsRequired()
                    .HasColumnName("android_dev_id")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nickname)
                    .HasColumnName("nickname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Openid)
                    .IsRequired()
                    .HasColumnName("openid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.QQ)
                    .HasColumnName("qq")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.WxID)
                    .HasColumnName("wxid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RealPersonValid)
                .HasColumnName("real_person_vaild")
                .HasColumnType("int(11)");

                entity.Property(e => e.Login)
                .HasColumnName("login")
                .HasColumnType("int(11)");

                entity.Property(e => e.RealPersonIdentity)
                .HasColumnName("real_person_identity");
            });
        }
    }
}
