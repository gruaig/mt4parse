using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mt4LogParser.Data;
using Mt4LogParser.Models.Entities;
using Mt4LogParser.Models.Enums;
using Mt4LogParser.Models.Extenstions;
using Mt4LogParser.Models.Messages;

namespace Mt4LogParser.Engine;

public class LogUploader
{
    private readonly IDbContextFactory<Mt4Context> _contextFactory;
    private readonly IOptions<LogSettings> _logSettings;

    public LogUploader(IDbContextFactory<Mt4Context> contextFactory, IOptions<LogSettings> logSettings)
    {
        _contextFactory = contextFactory;
        _logSettings = logSettings;
        _isActivitiesEnabled = GetMode("Activities");;
        _isMonitoringEnabled = GetMode("Monitoring");
        _isErrorsEnabled = GetMode("Errors");
        _isOrdersEnabled = GetMode("Orders");
    }
    public async Task Update(List<object> msgs, string path, long position, long metaId)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        foreach (var element in msgs)
        {
            switch (element)
            {
                case LoginMsg login when _isActivitiesEnabled:
                    await ProcessLoginMsg(login, metaId, context);
                    break;
                case TradeMsg trade when _isActivitiesEnabled:
                    await ProcessTradeMsg(trade, metaId, context);
                    break;
                case OrderMsg trade when _isOrdersEnabled:
                    await ProcessOrderMsg(trade, metaId, context);
                    break;
                case ErrorMsg error when _isErrorsEnabled:
                    await ProcessErrorMsg(error, metaId, context);
                    break;
                case MonitorMsg monitor when _isMonitoringEnabled:
                    await ProcessMonitorMsg(monitor, metaId, context);
                    break;
            }
        }

        await Update(path, position, metaId, context);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task ProcessOrderMsg(OrderMsg order, long metaId, Mt4Context context)
    {
        DbOrder dbOrder = order.CreateEx<DbOrder>();
        dbOrder.MetaId = metaId;
        await context.Orders.AddAsync(dbOrder);
    }

    private async Task ProcessMonitorMsg(MonitorMsg monitor, long metaId, Mt4Context context)
    {
        DbMonitor dbMonitor = monitor.CreateEx<DbMonitor>();
        dbMonitor.MetaId = metaId;
        await context.Monitors.AddAsync(dbMonitor);
    }

    private async Task ProcessErrorMsg(ErrorMsg error, long metaId, Mt4Context context)
    {
        DbError dbError = error.CreateEx<DbError>();
        dbError.MetaId = metaId;
        await context.Errors.AddAsync(dbError);
    }

    private async Task ProcessTradeMsg(TradeMsg trade, long metaId, Mt4Context context)
    {
        await Update(trade, metaId, context);
    }

    private async Task ProcessLoginMsg(LoginMsg login, long metaId, Mt4Context context)
    {
        await Update(login, metaId, context);
    }
    

    #region private methods
    private async Task Update(string path, long position, long metaId, Mt4Context context)
    {
        var state = await context.States.FirstOrDefaultAsync(x => x.Meta.Id == metaId);
        if (null == state)
        {
            state = new DbState
            {
                MetaId = metaId
            };
            context.States.Add(state);
        }

        state.Path = path;
        state.Position = position;
    }

    private async Task Update(LoginMsg loginMsg, long metaId,  Mt4Context context)
    {
        IQueryable<DbActivity> activities = context.Activities.Where(x => x.MetaId == metaId);
        activities = activities.Where(x => x.Account == loginMsg.Account);
        activities = activities.Where(x => x.IsInvestor == loginMsg.IsInvestor);
        activities = activities.Where(x => x.Cid == loginMsg.Cid);
        activities = activities.Where(x => x.IpAddress == loginMsg.IpAddress);
        DbDeviceEnum device = loginMsg.GetDbDevice();
        activities = activities.Where(x => x.Device == device);

        var activity = await activities.OrderByDescending(x => x.LastLoginTime).FirstOrDefaultAsync();
        if (null == activity)
        {
            activity = new DbActivity
            {
                MetaId = metaId,
                Account = loginMsg.Account,
                Cid = loginMsg.Cid,
                IpAddress = loginMsg.IpAddress,
                IsInvestor = loginMsg.IsInvestor,
                FirstLoginTime = loginMsg.Time,
                Device = device,
            };
            context.Activities.Add(activity);
            await context.SaveChangesAsync();
        }

        activity.LastLoginTime = loginMsg.Time;

        activity.NumberOfLogins++;

        _activities.RemoveAll(x => x.Account == loginMsg.Account && 
                                   x.IpAddress == loginMsg.IpAddress && 
                                   x.IsInvestor == loginMsg.IsInvestor);
    }

    private async Task Update(TradeMsg tradeMsg, long metaId, Mt4Context context)
    {
        var activity = TryGetFromCache(tradeMsg) ?? await GetFromDatabase(tradeMsg, metaId, context);
        activity.NumberOfOrders++;
    }

    private DbActivity? TryGetFromCache(TradeMsg msg)
    {
        IEnumerable<DbActivity> activities = _activities;
        activities = activities.Where(x => x.Account == msg.Account);
        activities = activities.Where(x => x.IpAddress == msg.IpAddress);
        activities = activities.Where(x => !x.IsInvestor);
        var result = activities.FirstOrDefault();
        return result;
    }

    private async Task<DbActivity> GetFromDatabase(TradeMsg msg, long metaId, Mt4Context context)
    {
        IQueryable<DbActivity> activities = context.Activities.Where(x => x.MetaId == metaId);
        activities = activities.Where(x => x.Account == msg.Account);
        activities = activities.Where(x => x.IpAddress == msg.IpAddress);
        activities = activities.Where(x => !x.IsInvestor);
        var result = await activities.OrderByDescending(x => x.LastLoginTime).FirstOrDefaultAsync();
        if (null == result)
        {
            result = new DbActivity
            {
                MetaId = metaId,
                Account = msg.Account,
                IpAddress = msg.IpAddress,
                FirstLoginTime = msg.Time,
                LastLoginTime = msg.Time,
                NumberOfLogins = 1,
                Cid = string.Empty,
            };
            context.Activities.Add(result);
            await context.SaveChangesAsync();
        }
        _activities.Add(result);
        return result;
    }

    #endregion

    #region static methods
    private bool GetMode(string name)
    {
        var settings = _logSettings.Value;

        switch (name)
        {
            case "Activities":
                return settings.Activities;
            case "Monitoring":
                return settings.Monitoring;
            case "Errors":
                return settings.Errors;
            case "Orders":
                return settings.Orders;
            default:
                return false;
        }
    }
    #endregion

    #region members

    private readonly bool _isActivitiesEnabled ;
    private readonly bool _isMonitoringEnabled;
    private readonly bool _isOrdersEnabled;
    private readonly bool _isErrorsEnabled;
    private readonly List<DbActivity> _activities = new ();
    #endregion
}