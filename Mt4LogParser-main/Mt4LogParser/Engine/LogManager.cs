using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mt4LogParser.Data;

namespace Mt4LogParser.Engine;

public class LogManager(
    ILogger<LogManager> logger,
    LogProcessor? logProcessor,
    IDbContextFactory<Mt4Context> contextFactory,
    IOptions<LogSettings> logSettings,
    DataProccess data,
    LogUploader logUploader)
{
    #region public methods
    public async Task Start(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start requested.");
        await Stop(cancellationToken);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _task = Task.Run(async () => await Loop(_cancellationTokenSource.Token), cancellationToken);
    }
    public async Task Stop(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is { IsCancellationRequested: false })
        {
            await _cancellationTokenSource.CancelAsync();
            logger.LogInformation("Stop requested via CancellationTokenSource.");
        }
        
        if (_task != null)
        {
            try
            {
                await _task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Operation was canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while stopping.");
            }
        }
        
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
    #endregion

    #region private methods
    private async Task Loop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await Step())
            {
                await Task.Delay(Delay, cancellationToken);
            }
        }
    }

    private async Task<bool> Step()
    {
        try
        {
            logProcessor ??= new LogProcessor(logSettings, contextFactory, data, logUploader);
            bool result = await logProcessor.Run();
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error in LogManager {ex.Message}");
            logProcessor?.Dispose();
            logProcessor = null;
            return false;
        }
    }

    #endregion

    #region members
    private Task? _task;
    private CancellationTokenSource? _cancellationTokenSource;
    #endregion

    #region constants

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

    #endregion
}