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
    internal class MemberShipConfiguration : IEntityTypeConfiguration<MemberShip>
    {
        public void Configure(EntityTypeBuilder<MemberShip> builder)
        {
            builder.Property(x=>x.CreatedAt).HasColumnName("StartDate").HasDefaultValueSql("GETDATE()");
            builder.HasKey(x=> new {x.MemberId, x.PlanId});
            builder.Ignore(x=>x.Id);
        }
    }
}
