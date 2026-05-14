using System.Reflection;
using System.Text;

namespace Puniemu.Src.TableParser.Logic
{
    //Used to manipulate NHN's table format.
    public class TableParser
    {
        /*
         The table format is essentially a 2D array.
         Each "row" can have multiple items inside, seperated by pipes, and the rows themselves are seperated by asterisks.
        */
        public List<string[]> Table;
        private Dictionary<string, List<int>> _identifierDictionary = new();
        private string? _prefix = null;
        private string? _delimiter = "|"; //Delimiter to be able to parse table that have "^" instead of "|"
        public TableParser(string? src, string prefix = "", string delimiter = "|")
        {
            _delimiter = delimiter;
            if (prefix != "")
            {
                src = src!.Substring(prefix.Length + 1); //+1 for the colon
                _prefix = prefix;
            }
            Table = Load(src!);
            LoadIdentifierTable();
        }
        private List<string[]> Load(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return new List<string[]>();
            }
            string[] rows = src.Split('*');
            List<string[]> table = new List<string[]>(rows.Length);
            for (int i = 0; i < rows.Length; i++)
            {
                table.Add(rows[i].Split(_delimiter));
            }
            return table;
        }

        public void AddRow(string[] row)
        {
            Table.Add(row);
            LoadIdentifierTable(row);
        }
        private void LoadRowIntoIdentifierTable(string[] row, int i)
        {
            foreach (var item in row)
            {
                if (_identifierDictionary.ContainsKey(item))
                {
                    _identifierDictionary[item].Add(i);
                }
                else _identifierDictionary[item] = [i];
            }
        }

        private void LoadIdentifierTable(string[]? addedRow = null)
        {
            _identifierDictionary.Clear();

            if (addedRow == null)
            {
                for (int i = 0; i < Table.Count; i++)
                {
                    var row = Table[i];
                    LoadRowIntoIdentifierTable(row, i);
                }
            }
            else
            {
                LoadRowIntoIdentifierTable(addedRow, Table.Count - 1);
            }

        }

        public int FindIndex(string[] identifiers)
        {
            HashSet<int> candidates = [];
            foreach (var identifier in identifiers)
            {
                if (_identifierDictionary.ContainsKey(identifier))
                {
                    var indexes = _identifierDictionary[identifier].ToHashSet();
                    //if we don't have candidates it means we have nothing to filter out, so we first need to have our initial candidates
                    if (candidates.Count == 0)
                    {
                        foreach (var index in indexes) candidates.Add(index);
                    }
                    //otherwise, we can filter!
                    else
                    {
                        HashSet<int> matchingIndexes = new HashSet<int>();
                        foreach (var candidate in candidates)
                        {
                            if (indexes.Contains(candidate))
                            {
                                matchingIndexes.Add(candidate);
                                break;
                            }
                        }
                        if (matchingIndexes.Count == 0) return -1;
                        else candidates = matchingIndexes;
                    }
                }
                else
                {
                    //All identifiers need to match
                    return -1;
                }


                if (candidates.Count == 1)
                {
                    return candidates.FirstOrDefault();
                }

            }
            return -1;
        }
        public virtual void PrepareForToString()
        {
            //does nothing by default. Can be overriden by derived classes to for example convert any structs to members before the toString()
        }
        public override string ToString()
        {
            PrepareForToString();
            StringBuilder sb = new StringBuilder();
            if (_prefix != null)
            {
                sb.Append(_prefix + ":");
            }
            for (int i = 0; i < Table.Count; i++)
            {
                sb.Append(string.Join(_delimiter, Table[i]));
                if (i < Table.Count - 1)
                {
                    sb.Append('*');
                }
            }
            // remove extra *
            var s = sb.ToString();
            if (s.StartsWith("*"))
                s = s.Substring(1);
            if (s.EndsWith("*"))
                s = s.Substring(0, s.Length - 1);
            return s;
        }
    }

    public class TableParser<T> where T : class, new()
    {
        public List<T> Items { get; set; } = new();
        private List<string[]> _rawTable;
        private Dictionary<string, List<int>> _identifierDictionary = new();
        private string? _prefix = null;
        private string? _delimiter = "|";

        public TableParser(string? src = "", string prefix = "", string delimiter = "|")
        {
            _delimiter = delimiter;
            if (prefix != "")
            {
                src = src!.Substring(prefix.Length + 1);
                _prefix = prefix;
            }
            _rawTable = Load(src!);
            LoadIdentifierTable();
            ConvertToObjects();
        }

        private List<string[]> Load(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return new List<string[]>();
            }
            string[] rows = src.Split('*');
            List<string[]> table = new List<string[]>(rows.Length);
            for (int i = 0; i < rows.Length; i++)
            {
                table.Add(rows[i].Split(_delimiter));
            }
            return table;
        }

        private void ConvertToObjects()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            foreach (var row in _rawTable)
            {
                var obj = new T();
                for (int i = 0; i < Math.Min(row.Length, properties.Length); i++)
                {
                    try
                    {
                        var prop = properties[i];
                        object value;
                        if (prop.PropertyType.IsEnum)
                        {
                            value = Enum.Parse(prop.PropertyType, row[i]);
                        }
                        else
                        {
                            value = Convert.ChangeType(row[i], prop.PropertyType);
                        }
                        prop.SetValue(obj, value);
                    }
                    catch
                    {
                        // Handle conversion errors
                    }
                }
                Items.Add(obj);
            }
        }

        public void AddItem(T item)
        {
            Items.Add(item);
            var row = ConvertObjectToRow(item);
            _rawTable.Add(row);
            LoadIdentifierTable(row);
        }

        private string[] ConvertObjectToRow(T item)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Select(p => p.GetValue(item)?.ToString() ?? "").ToArray();
        }

        private void LoadRowIntoIdentifierTable(string[] row, int i)
        {
            foreach (var item in row)
            {
                if (_identifierDictionary.ContainsKey(item))
                {
                    _identifierDictionary[item].Add(i);
                }
                else _identifierDictionary[item] = [i];
            }
        }

        private void LoadIdentifierTable(string[]? addedRow = null)
        {
            if (addedRow == null)
            {
                _identifierDictionary.Clear();
                for (int i = 0; i < _rawTable.Count; i++)
                {
                    LoadRowIntoIdentifierTable(_rawTable[i], i);
                }
            }
            else
            {
                LoadRowIntoIdentifierTable(addedRow, _rawTable.Count - 1);
            }
        }

        public int FindIndex(string[] identifiers)
        {
            HashSet<int> candidates = [];
            foreach (var identifier in identifiers)
            {
                if (_identifierDictionary.ContainsKey(identifier))
                {
                    var indexes = _identifierDictionary[identifier].ToHashSet();
                    if (candidates.Count == 0)
                    {
                        foreach (var index in indexes) candidates.Add(index);
                    }
                    else
                    {
                        HashSet<int> matchingIndexes = new HashSet<int>();
                        foreach (var candidate in candidates)
                        {
                            if (indexes.Contains(candidate))
                            {
                                matchingIndexes.Add(candidate);
                                break;
                            }
                        }
                        if (matchingIndexes.Count == 0) return -1;
                        else candidates = matchingIndexes;
                    }
                }
                else
                {
                    return -1;
                }

                if (candidates.Count == 1)
                {
                    return candidates.FirstOrDefault();
                }
            }
            return -1;
        }

        public virtual void PrepareForToString()
        {
            // Synchronize Items back to _rawTable
            _rawTable.Clear();
            foreach (var item in Items)
            {
                _rawTable.Add(ConvertObjectToRow(item));
            }
        }

        public override string ToString()
        {
            PrepareForToString();
            StringBuilder sb = new StringBuilder();
            if (_prefix != null)
            {
                sb.Append(_prefix + ":");
            }
            for (int i = 0; i < _rawTable.Count; i++)
            {
                sb.Append(string.Join(_delimiter, _rawTable[i]));
                if (i < _rawTable.Count - 1)
                {
                    sb.Append('*');
                }
            }
            var s = sb.ToString();
            if (s.StartsWith("*"))
                s = s.Substring(1);
            if (s.EndsWith("*"))
                s = s.Substring(0, s.Length - 1);
            return s;
        }
    }
}