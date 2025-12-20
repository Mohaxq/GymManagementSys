using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Configuration
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("SessionTimeCheck", "EndDate > startDate");
                tb.HasCheckConstraint("SessionCapacityCheck", "Capacity between 1 and 25");
            });
            builder.HasOne<Category>(s => s.SessionCategory)
                   .WithMany(c => c.Sessions)
                   .HasForeignKey(s => s.CategoryId);
            builder.HasOne<Trainer>(s => s.SessionTrainer)
                   .WithMany(t => t.TrainerSession)
                   .HasForeignKey(s => s.TrainerId);
        }
    }
}
