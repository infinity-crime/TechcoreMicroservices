using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Domain.Entities;

namespace TechcoreMicroservices.BookOrderService.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.BookId)
            .IsRequired();

        builder.Property(oi => oi.BookTitle)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();
    }
}
