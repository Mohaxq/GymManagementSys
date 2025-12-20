using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Configuration
{
    internal class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(x => x.Email).HasColumnType("varchar").HasMaxLength(100);
            builder.Property(x => x.Phone).HasColumnType("varchar").HasMaxLength(11);
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("GymUserValidEmailCheck", "Email Like '_%@_%._%'");
                tb.HasCheckConstraint("GymUserValidPhoneCheck", "Phone Like '01%' and Phone not Like '%[^0-9]%'");
            });
            builder.HasIndex(x=> x.Email).IsUnique();
            builder.HasIndex(x=> x.Phone).IsUnique();
            builder.OwnsOne(x=>x.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street).HasColumnType("varchar").HasMaxLength(100).HasColumnName("Street");
                addressBuilder.Property(a => a.City).HasColumnType("varchar").HasMaxLength(50).HasColumnName("City");
                addressBuilder.Property(a => a.BuildingNumber).HasColumnType("varchar").HasMaxLength(50).HasColumnName("BuldingNumber");
                
            });
        }
    }
}
