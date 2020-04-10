using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
  public class ReviewConfiguration : IEntityTypeConfiguration<Review>
  {
    public void Configure(EntityTypeBuilder<Review> builder)
    {
      builder.Property(r => r.Rating)
            .HasMaxLength(1)
            .IsRequired();

      builder.Property(r => r.Comment)
            .HasMaxLength(500);

      builder.Property(r => r.CreatedAt)
            .HasMaxLength(30)
            .IsRequired();


      builder.HasKey(r => new { r.UserName, r.SportObjectId });

      builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserName)
            .HasPrincipalKey(u => u.UserName);

      builder.HasOne(r => r.SportObject)
            .WithMany(so => so.Reviews)
            .HasForeignKey(r => r.SportObjectId);
    }
  }
}