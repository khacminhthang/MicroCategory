﻿using System.ComponentModel.DataAnnotations;

namespace MicroCategory.Domain.Common.Queries
{
    public class PagedQueryBase : IPagedQuery
    {
        [Required]
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string OrderColumn { get; set; }

        public bool OrderValue { get; set; }
    }
}
