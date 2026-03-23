using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OrderStatusHistory
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public long? ChangedByAccountId { get; set; }

    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual Account? ChangedByAccount { get; set; }

    public virtual Order Order { get; set; } = null!;
}
