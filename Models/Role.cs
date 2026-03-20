using System;
using System.Collections.Generic;

namespace EyeGlasses_Store.Models;

public partial class Role
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
