using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ProductVariant
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string Size { get; set; } = null!;

    public string? Material { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}
