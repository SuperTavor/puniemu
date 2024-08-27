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
        private List<string[]> _table;
        private Dictionary<string, List<int>> _identifierDictionary = new();
        private string? _prefix = null;
        public TableParser(string src, string prefix = "")
        {
            if (prefix != "")
            {
                src = src.Substring(prefix.Length + 1); //+1 for the colon
                _prefix = prefix;
            }
            _table = Load(src);
            LoadIdentifierTable();
        }
        private List<string[]> Load(string src)
        {
            string[] rows = src.Split('*');
            List<string[]> table = new List<string[]>(rows.Length);
            for (int i = 0; i < rows.Length; i++)
            {
                table.Add(rows[i].Split('|'));
            }
            return table;
        }

        public void AddRow(string[] row)
        {
            _table.Add(row);
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
                for (int i = 0; i < _table.Count; i++)
                {
                    var row = _table[i];
                    LoadRowIntoIdentifierTable(row, i);
                }
            }
            else
            {
                LoadRowIntoIdentifierTable(addedRow, _table.Count - 1);
            }

        }

        public List<T> Deserialize<T>()
        {
            var instance = new List<T>();
            var type = typeof(T);
            foreach (var row in _table)
            {
                foreach (var prop in type.GetProperties())
                {
                    foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var fieldType = field.GetType();
                        if (fieldType != typeof(string))
                        {
                            throw new TypeAccessException("Only string members are supported.");
                        }
                        else
                        {
                            //Set 
                        }
                    }
                }
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_prefix != null)
            {
                sb.Append(_prefix + ":");
            }
            for (int i = 0; i < _table.Count; i++)
            {
                sb.Append(string.Join("|", _table[i]));
                if (i < _table.Count - 1)
                {
                    sb.Append('*');
                }
            }
            return sb.ToString();
        }
    }
}