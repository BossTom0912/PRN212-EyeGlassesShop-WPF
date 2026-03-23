using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Order
{
    public long Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public long AccountId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public long? ApprovedByOwnerId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancelReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Account? ApprovedByOwner { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
}
