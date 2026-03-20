using System;
using System.Collections.Generic;

namespace EyeGlasses_Store.Models;

public partial class Account
{
    public long Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public long RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> OrderAccounts { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderApprovedByOwners { get; set; } = new List<Order>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
}
