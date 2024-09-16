using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class Pm
{
    public string Id { get; set; } = null!;

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<AdsAccount> AdsAccounts { get; set; } = new List<AdsAccount>();
}
