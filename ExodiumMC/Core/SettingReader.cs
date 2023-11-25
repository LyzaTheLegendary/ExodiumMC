using System.Text;

namespace ExodiumMC.Core
{
    public class SettingReader
    {
        private readonly Dictionary<string, string> values = new();
        public SettingReader(string path)
        {
            try
            {
                string content = File.ReadAllText(path);
                content = content.Replace("\r\n", "\n");

                string[] lines = content.Split('\n');
                foreach (string line in lines)
                {
                    if (!line.Contains(':'))
                        continue;

                    if (line.StartsWith("//"))
                        continue;

                    int indexSeperator = line.IndexOf(':');

                    string name = line.Substring(0, indexSeperator);
                    string value = line.Substring(indexSeperator + 1);

                    values[name] = value;
                }
            }
            catch (DirectoryNotFoundException)
            {

            }
        }
        public string GetValue(string value) => values[value];
    }
}
