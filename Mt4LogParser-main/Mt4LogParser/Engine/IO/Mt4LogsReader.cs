using System.IO;
using System.Text.RegularExpressions;

namespace Mt4LogParser.Engine.IO;

 class Mt4LogsReader : IDisposable
    {
        #region construciton
        public Mt4LogsReader(string directory, string path, long position)
        {
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));
            _directory = _directory.ToLowerInvariant();
            _reader = CreateReader(path, position);
        }

        private Mt4LogReader? CreateReader(string path, long position)
        {
            List<string> paths = GetFiles();

            foreach (var element in paths)
            {
                int order = string.Compare(path, element, StringComparison.Ordinal);

                if (0 == order)
                {
                    return new Mt4LogReader(element, position);
                }

                if (order < 0)
                {
                    return new Mt4LogReader(element, 0);
                }
            }

            return null;
        }

        #endregion

        #region properties
        public DateTime Date => _reader.Date;
        public string? Path => _reader?.Path;
        public long? Position => _reader?.Position;
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;
        }
        #endregion

        #region read next
        public int ReadNext()
        {
            if (_reader == null)
            {
                MoveToNext();
            }

            for (; null != _reader; MoveToNext())
            {
                int result = _reader.ReadNext();
                if (result >= 0)
                {
                    return result;
                }

                string? path = GetPath();
                if (null == path)
                {
                    return -1;
                }

                result = _reader.ReadNext();
                if (result >= 0)
                {
                    return result;
                }
            }

            return -1;
        }

        private void MoveToNext()
        {
            string? path = GetPath();
            _reader?.Dispose();
            _reader = null;

            if (null != path)
            {
                Console.WriteLine($"Moving to the next file: {path}");
                _reader = new Mt4LogReader(path, 0);
            }
            
        }

        #endregion

        #region common methods
        private string? GetPath()
        {
            List<string> paths = GetFiles();
            string? path = _reader?.Path;
            path ??= string.Empty;
            int index = paths.IndexOf(path) + 1;

            if (index >= paths.Count)
            {
                return null;
            }

            string result = paths[index];
            return result;
        }

        private List<string> GetFiles()
        {
            List<string> result = Directory.GetFiles(_directory, "*.log")
                .Where(path => Pattern.IsMatch(path))
                .OrderBy(path => path)
                .Select(x => x.ToLowerInvariant())
                .ToList();

            return result;
        }

        #endregion

        #region input
        private readonly string _directory;
        private Mt4LogReader? _reader;
        #endregion

        #region state
        private static readonly Regex Pattern = new (@"^.*\\\d{8,8}\.log");
        #endregion
    }