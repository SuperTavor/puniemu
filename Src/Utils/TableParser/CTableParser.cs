using System.Text;

namespace Puniemu.Src.Utils.TableParser
{
    //Used to manipulate NHN's table format.
    public class CTableParser
    {
        /*
         The table format is essentially a 2D array.
         Each "row" can have multiple items inside, seperated by pipes, and the rows themselves are seperated by asterisks.
        */
        private List<string[]> _table;

        public CTableParser(string src,bool hasPrefix,string prefix = "")
        {
            if(hasPrefix)
            {
                src = src.Substring(prefix.Length+1); //+1 for the colon
            }
            _table = Load(src);
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
        }

        public int FindIndex(string[] identifiers)
        {
            for (int i = 0; i < _table.Count; i++)
            {
                string[] row = _table[i];
                bool match = true;
                for (int j = 0; j < identifiers.Length; j++)
                {
                    if (j >= row.Length || row[j] != identifiers[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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