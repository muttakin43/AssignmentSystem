using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Common
{
    public static class QueryableExtensions
    {
        public static async Task<PageResult<T>> ToPagedResultAsync<T>(
       this IQueryable<T> query, PageQuery pagedQuery, CancellationToken ct = default)
        {
            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((pagedQuery.Page - 1) * pagedQuery.PageSize)
                .Take(pagedQuery.PageSize)
                .ToListAsync(ct);

            return new PageResult<T>
            {
                Items = items,
                Page = pagedQuery.Page,
                PageSize = pagedQuery.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
