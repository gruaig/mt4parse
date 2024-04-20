using System.Text;

namespace Mt4LogParser.Engine.IO;

public class Mt4MsgReader
{
   #region construction
   public Mt4MsgReader(string directory, string fileName, long position)
   {
       _reader = new Mt4LogsReader(directory, fileName, position);
   }
   #endregion
   #region methods
   public string? ReadNext()
   {
       while (TryReadBuffer())
       {
           byte[] data = _buffer.ToArray();
           _buffer.Clear();

           string result = Encoding.UTF8.GetString(data);
           if (!string.IsNullOrEmpty(result))
           {
               return result;
           }
       }
       return null;
   }
   #endregion
   #region properties
   public DateTime Date => _reader.Date;
   public string? Path => _reader.Path;
   public long? Position => _reader.Position;
   #endregion
   #region private methods
   private bool TryReadBuffer()
   {
       for (; ; )
       {
           int ch = _reader.ReadNext();
           if (ch < 0)
           {
               return _buffer.Count > 0;
           }
           if ('\r' == ch)
           {
               continue;
           }
           if ('\n' == ch)
           {
               return true;
           }
           _buffer.Add((byte)ch);
       }
   }
   #endregion

   #region IDisposable
   public void Dispose()
   {
       _reader.Dispose();
   }
   #endregion

   #region members
   private readonly Mt4LogsReader _reader;
   private readonly List<byte> _buffer = new ();
   #endregion
}