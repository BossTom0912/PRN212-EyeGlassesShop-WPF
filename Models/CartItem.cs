using System;
using System.Collections.Generic;

namespace EyeGlasses_Store.Models;

public partial class CartItem
{
    public long Id { get; set; }

    public long CartId { get; set; }

    public long ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ProductVariant ProductVariant { get; set; } = null!;
}
