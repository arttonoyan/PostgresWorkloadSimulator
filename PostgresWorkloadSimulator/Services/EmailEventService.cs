using Microsoft.EntityFrameworkCore;
using PgScalabilityTest.Data;

namespace PgScalabilityTest.Services;

public class EmailEventService(AnalyticsDbContext _context)
{
    //public async Task AddAsync(EmailEvent evt)
    //{
    //    var aggregate = await _context.AggregatedEmailEvents
    //        .FirstOrDefaultAsync(a => a.TenantId == evt.TenantId && a.Date == evt.Timestamp.Date);

    //    if (aggregate == null)
    //    {
    //        aggregate = new AggregatedEmailEvent { TenantId = evt.TenantId, Date = evt.Timestamp.Date, TotalValue = 0 };
    //        await _context.AggregatedEmailEvents.AddAsync(aggregate);
    //    }
    //    aggregate.TotalValue += evt.Value;
    //    await _context.EmailEvents.AddAsync(evt);
    //    await _context.SaveChangesAsync();
    //}

    public async Task AddAsync(EmailEvent evt, CancellationToken token)
    {
        //var roundedMinute = new DateTime(evt.Timestamp.Year, evt.Timestamp.Month, evt.Timestamp.Day, evt.Timestamp.Hour, evt.Timestamp.Minute, 0);
        //var aggregate = await _context.AggregatedEmailEvents.FirstOrDefaultAsync(a => a.TenantId == evt.TenantId && a.Date == roundedMinute);

        var tick = evt.Timestamp.Ticks - evt.Timestamp.Ticks % TimeSpan.TicksPerSecond / 10;
        var roundedTenSeconds = new DateTime(tick, DateTimeKind.Utc);
        var aggregate = await _context.AggregatedEmailEvents
            .FirstOrDefaultAsync(a => a.TenantId == evt.TenantId && a.Date == roundedTenSeconds, token);

        if (aggregate == null)
        {
            //aggregate = new AggregatedEmailEvent { TenantId = evt.TenantId, Date = roundedMinute, TotalValue = 0 };
            aggregate = new AggregatedEmailEvent { TenantId = evt.TenantId, Date = roundedTenSeconds, TotalValue = 0 };
            await _context.AggregatedEmailEvents.AddAsync(aggregate, token);
        }
        aggregate.TotalValue += evt.Value;
        await _context.EmailEvents.AddAsync(evt, token);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(RawEmailEvent evt, CancellationToken token)
    {
        await _context.RawEmailEvents.AddAsync(evt, token);
        await _context.SaveChangesAsync(token);
    }

    public async Task<int?> CalculateAsync(int tenantId, DateTime date, CancellationToken token)
    {
        var aggregate = await _context.AggregatedEmailEvents.FirstOrDefaultAsync(a => a.TenantId == tenantId && a.Date == date, token);
        return aggregate?.TotalValue;
    }

    public async Task<int> CalculateAsync(int tenantId, DateTime startDate, DateTime endDate, CancellationToken token)
    {
        return await _context.AggregatedEmailEvents
            .Where(a => a.TenantId == tenantId && a.Date >= startDate && a.Date <= endDate)
            .SumAsync(a => a.TotalValue, token);
    }

    public async Task<int> CalculateFromRawAsync(int tenantId, DateTime date, CancellationToken token)
    {
        return await _context.RawEmailEvents
            .Where(e => e.TenantId == tenantId && e.Timestamp.Date == date.Date)
            .SumAsync(e => e.Value, token);
    }

    public async Task<int> CalculateFromRawAsync(int tenantId, DateTime startDate, DateTime endDate, CancellationToken token)
    {
        return await _context.RawEmailEvents
            .Where(e => e.TenantId == tenantId && e.Timestamp.Date >= startDate.Date && e.Timestamp.Date <= endDate.Date)
            .SumAsync(e => e.Value, token);
    }
}

