using GestaoPedidosAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoPedidosAPI.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        //usuário padrão
        builder.HasData(new
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "dev@martech.com",
            PasswordHash = "$2a$11$Uz9ePAPE0q5M3vKjGia7CuxHV9WWpJGeB.abCDCWBtOn81YvOhGn." // Senha@123
        });
    }
}
