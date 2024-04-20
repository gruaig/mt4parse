namespace Mt4LogParser.Data;

public class LogSettings
{
    public string LogDirectory { get; set; }
    public string LogFormat { get; set; }
    public string LogName { get; set; }
    
    public bool Activities { get; set; }
    public bool Monitoring { get; set; }
    public bool Errors { get; set; }
    public bool Orders { get; set; }
}