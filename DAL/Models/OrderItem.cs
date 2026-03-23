using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ProductVariantId { get; set; }

    public string ProductNameSnapshot { get; set; } = null!;

    public string SkuSnapshot { get; set; } = null!;

    public string ColorSnapshot { get; set; } = null!;

    public string SizeSnapshot { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
