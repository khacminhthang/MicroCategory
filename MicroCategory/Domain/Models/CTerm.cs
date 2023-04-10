using System;
using System.Collections.Generic;

namespace MicroCategory.Domain.Models;

public partial class CTerm
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Type { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public short? DeletedAt { get; set; }

    public string? AppCode { get; set; }

    public string? SiteId { get; set; }
}
