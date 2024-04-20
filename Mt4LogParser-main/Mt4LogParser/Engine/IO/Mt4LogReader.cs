using System.Globalization;

namespace Mt4LogParser.Engine.IO;

internal class Mt4LogReader : IDisposable
{
    private const string LogFormat = "yyyyMMdd";
    public Mt4LogReader(string path, long position)
    {
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        Date = DateTime.ParseExact(name, LogFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
        Path = path;
        _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1);
        position = Math.Min(_stream.Length, position);
        _stream.Position = position;
    }

    public string Path { get; }
    public DateTime Date { get; }
    public long Position => _stream.Position - _data.Count;

    #region methods
    public int ReadNext()
    {
        int result = TryReadNext();
        if (result < 0)
        {
            ReadNextBuffer();
            result = TryReadNext();
        }
        return result;
    }
    public int TryReadNext()
    {
        if (_data.Count > 0)
        {
            int result = _data.Dequeue();
            return result;
        }
        return -1;
    }
    private void ReadNextBuffer()
    {
        byte[] data = new byte[BufferSize];
        long position = _stream.Position;
        _stream.Read(data, 0, data.Length);

        int index = 0;
        for (; index < data.Length; index++)
        {
            byte value = data[index];
            if (value > 0)
            {
                _data.Enqueue(value);                    
            }
            else
            {
                break;
            }
        }
        _stream.Position = position + index;
    }
    #endregion

    #region IDisposable
    public void Dispose()
    {
        _stream.Dispose();
    }
    #endregion

    #region members
    private const int BufferSize = 4096;
    private readonly FileStream _stream;
    private readonly Queue<byte> _data = new ();
    #endregion
}