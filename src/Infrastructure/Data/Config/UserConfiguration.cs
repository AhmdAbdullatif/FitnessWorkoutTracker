using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Username).HasMaxLength(30);

            builder.Property(x => x.Email).HasMaxLength(256);

            builder.Property(x => x.HashedPassword).HasMaxLength(256);

            builder.HasIndex(x => x.Email);
        }
    }
}
