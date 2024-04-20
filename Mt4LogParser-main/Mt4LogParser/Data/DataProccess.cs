using Microsoft.EntityFrameworkCore;

namespace Mt4LogParser.Data;

public class DataProccess(IDbContextFactory<Mt4Context> contextFactory)
{
    #region members
    private static readonly TimeSpan Interval = TimeSpan.FromDays(7);
    #endregion

    #region methods
    public async Task Clean(long metaId)
    {
        var context = await contextFactory.CreateDbContextAsync();
        DateTime threshold = DateTime.UtcNow - Interval;
        var monitorsToDelete = context.Monitors
            .Where(m => m.Timestamp <= threshold && m.MetaId == metaId)
            .ToList();
        context.Monitors.RemoveRange(monitorsToDelete);

        var errorsToDelete = context.Errors
            .Where(e => e.Timestamp <= threshold && e.MetaId == metaId)
            .ToList();
        context.Errors.RemoveRange(errorsToDelete);

        await context.SaveChangesAsync();
    }
    #endregion
}