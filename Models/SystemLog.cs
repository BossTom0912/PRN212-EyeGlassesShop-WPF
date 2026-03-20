using System;
using System.Collections.Generic;

namespace EyeGlasses_Store.Models;

public partial class SystemLog
{
    public long Id { get; set; }

    public long? ActorAccountId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public long? EntityId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Account? ActorAccount { get; set; }
}
