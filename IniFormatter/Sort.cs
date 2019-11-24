namespace IniFormatter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Sort
    {
        public static async Task<string> SortStart(string ipath, string opath,string[] sections, uint first = 0, uint? digit = null, uint? prefix = null)
        {
            string workSections = string.Empty;
            var isr = new StreamReader(new FileStream(ipath, FileMode.Open));
            var osw = new StreamWriter(new FileStream(opath, FileMode.Create));
            bool work = false;
            uint num = (uint)first;
            while (!isr.EndOfStream)
            {
                string line = await isr.ReadLineAsync();

                if (line.StartsWith("["))
                {
                    num = (uint)first;
                    work = false;
                    if (sections.Contains(line.Substring(1, line.IndexOf("]") - 1)))
                    {
                        work = true;
                        workSections += line + "\n";
                    }
                }

                if (work && line.Split(';')[0].Contains("="))
                {
                    var tmp = line.Split('=');
                    string temp = string.Empty;
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (i == 0) continue;
                        temp += "=" + tmp[i];
                    }
                    var tmpstr = string.Empty;
                    if (prefix != null)
                        tmpstr += prefix;                    
                    if (digit != null)
                        tmpstr += num.ToString($"#{new string('0', (int)digit.GetValueOrDefault())}");
                    else
                        tmpstr += num.ToString();
                    line = tmpstr + temp;
                    num++;
                }
                await osw.WriteLineAsync(line);
            }
            await osw.FlushAsync();
            osw.Close();
            isr.Close();
            return workSections;
        }
    }
}