using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class VwProductVariantList
{
    public long? VariantId { get; set; }

    public long? ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? CategoryName { get; set; }

    public string? BrandName { get; set; }

    public string? Sku { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Material { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool? VariantIsActive { get; set; }

    public bool? ProductIsActive { get; set; }

    public string? ImageUrl { get; set; }

    public string? BaseImageUrl { get; set; }

    public string? Description { get; set; }
}
