using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mt4LogParser.Data;
using Mt4LogParser.Engine.IO;
using Mt4LogParser.Models.Entities;
using Mt4LogParser.Models.Messages;

namespace Mt4LogParser.Engine;

public class LogProcessor : IDisposable
{
    #region construction
    private Task? _initializationTask;
    private readonly Regex _datePattern = new (@"\b(?<Time>\d{2}:\d{2}:\d{2}\.\d{3})\b", RegexOptions.Compiled);
    private readonly IOptions<LogSettings> _logSettings;
    private readonly IDbContextFactory<Mt4Context> _contextFactory;
    private readonly LogUploader _logUploader;
    private readonly DataProccess _dataProccess;
    private string _directory = String.Empty;
    private bool _initialized;
    private long _metaId;

    public LogProcessor(IOptions<LogSettings> logSettings, 
        IDbContextFactory<Mt4Context> contextFactory,
        DataProccess dataProccess, 
        LogUploader logUploader)
    {
        _logUploader = logUploader;
        _logSettings = logSettings;
        _contextFactory = contextFactory;
        _dataProccess = dataProccess;
    }   
    
    private Task EnsureInitializedAsync()
    {
        if (_initialized)
            return Task.CompletedTask;

        if (_initializationTask == null)
        {
            _initializationTask = InitializeImplAsync();
        }

        return _initializationTask;
    }

    private async Task InitializeImplAsync()
    {
        if (!_initialized)
        {
            _directory = _logSettings.Value.LogDirectory.ToLowerInvariant();
            _metaId = await Initialize(_logSettings.Value.LogName);
            await Seek(_metaId);
            _initialized = true;
        }
    }

    private async Task<long> Initialize(string name)
    {
        DbMeta? meta = null;

        try
        {
            var context = await _contextFactory.CreateDbContextAsync();

            // Log the start of the initialization process
            Console.WriteLine($"Initializing Meta with name: {name}");

            meta = await context.Metas.AsNoTracking().SingleOrDefaultAsync(x => x.Name == name);

            if (meta == null)
            {
                Console.WriteLine($"Cannot find Meta by name: {name}");
                throw new Exception($"Cannot find Meta by name: {name}");
            }

            // Log the retrieval of the Meta record
            Console.WriteLine($"Retrieved Meta record: {meta}");

            var state = await context.States.AsNoTracking().SingleOrDefaultAsync(x => x.MetaId == meta.Id);

            if (state == null)
            {
                state = new DbState
                {
                    MetaId = meta.Id
                };
                context.States.Add(state);

                // Log the creation of a new State record
                Console.WriteLine("Creating new State record.");

                await context.SaveChangesAsync();

                // Log the successful creation of the State record
                Console.WriteLine("New State record created successfully.");
            }

            // Log the successful initialization of Meta
            Console.WriteLine($"Meta initialized successfully with Id: {meta.Id}");

            return meta.Id;
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during initialization
            Console.WriteLine($"Error during Meta initialization: {ex.Message}");
            throw;
        }
    }


    private async Task Seek(long metaId)
    {
         var context =  await _contextFactory.CreateDbContextAsync();
         var state = await context.States.SingleAsync(x => x.MetaId == metaId);
        
        _reader = new Mt4MsgReader(_directory, state.Path, state.Position);
    }

    #endregion

    #region methods
    public async Task<bool> Run()
    {
        await EnsureInitializedAsync();
        string? line = _reader!.ReadNext();
        if (line != null)
        {
            Console.WriteLine($"Processing line: {line}");
            await Process(line);
            return true;
        }
        Console.WriteLine("Line is null, flushing and finishing.");
        await Flush();
        return false;
    }
    #endregion
    private async Task Process(string line)
    {
        var timestamp = ExtractTimestamp(line); 
        
        var msg = await ParseLog(line, timestamp);
        if (msg != null)
        {
            _msgs.Add(msg);
        }
    }

    private async Task<object?> ParseLog(string line, DateTime time)
    {
        object? msg = null;

        try
        {
            if (LoginMsg.TryParse(line, time, out var login))
            {
                msg = login;
            }
            else if (TradeMsg.TryParse(line, time, out var trade))
            {
                msg = trade;
            }
            else if (ErrorMsg.TryParse(line, time, out var error))
            {
                msg = error;
            }
            else if (OrderMsg.TryParse(line, time, out var order))
            {
                msg = order;
            }
            else if (_monitor != null && _monitor.TryParseAdditional(line))
            {
                msg = _monitor;
                _monitor = null;
            }
            else if (MonitorMsg.TryParse(line, time, out var monitor))
            {
                _monitor = monitor;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't process line: {line}", ex);
        }
        
        await FlushIfNeeded();

        return msg;
    }

    private async Task FlushIfNeeded()
    {
        if (_msgs.Count >= MaxMsgsNumber)
        {
            await Flush();
        }            
    }
    
    private async Task Flush()
    {
        if (_msgs.Count > 0)
        {
            await _dataProccess.Clean(_metaId);
            await _logUploader.Update(_msgs, _reader?.Path ?? string.Empty, _reader?.Position ?? 0, _metaId);
            _msgs.Clear();
        }
    }
    
    private DateTime ExtractTimestamp(string logLine)
    {
        Match match = _datePattern.Match(logLine);
        if (!match.Success)
        {
            throw new ArgumentException("Invalid log line format");
        }
        
        return DateTime.SpecifyKind(DateTime.ParseExact(match.Groups["Time"].Value, "HH:mm:ss.fff", CultureInfo.InvariantCulture), DateTimeKind.Utc);
    }
    
    private Mt4MsgReader? _reader;
    private MonitorMsg? _monitor;
    private readonly List<object> _msgs = new ();
    private const int MaxMsgsNumber = 1000;
    public void Dispose()
    {
        _reader?.Dispose();
    }
}