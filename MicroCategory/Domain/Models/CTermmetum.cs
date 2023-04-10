using System;
using System.Collections.Generic;

namespace MicroCategory.Domain.Models;

public partial class CTermmetum
{
    public int Id { get; set; }

    public long TermId { get; set; }

    public string? MetaKey { get; set; }

    public string? MetaValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public short? DeletedAt { get; set; }
}
