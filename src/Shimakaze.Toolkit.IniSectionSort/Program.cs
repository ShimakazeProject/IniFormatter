using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Shimakaze.Struct.Ini;

namespace Shimakaze.Toolkit.IniSectionSort
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(new string[] { "-i", "--input-file" }, "将文件作为输入源"){ IsRequired = false },
                new Option<string>(new string[] { "-o", "--output-file" }, "设置输出文件"){ IsRequired = false },

                new Option<int>(new string[] { "-s", "--start-num" }, () => 0, "以此数字开始计数"),
                new Option<int?>(new string[] { "-d", "--digit" }, "以此数字规范长度"),
                new Option<string>(new string[] { "-p", "--prefix" }, "在输出的键前添加一个前缀"),

                new Option<bool>("--nologo") { IsHidden = true },
                new Option<bool>("--sort", () => false, "对输出进行排序"),
                new Option<string>("--sort-key", "根据目标键的指排序"),
                new Option<string>("--output-section" , "在输出结果前添加一个节头"),
                new Option<string>("--output-key", "将此键的值作为注释输出"),
                new Option<string>("--constraint-key", "只有包含此键的节才会被输出"),
                new Option<string>("--constraint-value", "只有当键约束的键值等于这里的值时才会被输出"),
            };
            rootCommand.Description = "INI 重新排序工具 CLI";

            //rootCommand.Handler = CommandHandler.Create(Worka);
            ////<FileInfo, string, int, int?, string, bool, bool, string, string, string, string, string>
            ////Action<FileInfo, string, int, int?, string, bool, bool, string, string, string, string, string>
            //// Parse the incoming args and invoke the handler
            //return await rootCommand.InvokeAsync(args);
        }
        static void Worka(FileInfo input_file, string output_file, string output_section, bool nologo)
        {
            Console.WriteLine("Hello World");
            Console.WriteLine(input_file?.FullName);
            Console.WriteLine(output_file);
            Console.WriteLine(output_section);
            Console.WriteLine(nologo);
        }
        static async void Main(
            FileInfo input_file, string output_file,
            int start_num, int? digit, string prefix,
            bool nologo, bool sort, string sort_key,
            string output_section, string output_key,
            string constraint_key, string constraint_value)
        {
            Console.WriteLine("Hello World");
            using var fs = new FileStream(input_file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var ofs = new FileStream(output_file, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(ofs);
            var options = new SortOptions
            {
                First = start_num,
                Digit = digit,
                Prefix = prefix,
                Sort = sort,
                SortTargetKey = sort_key,
                SummaryKey = output_key,
                KeyConstraint = constraint_key,
                ValueConstraint = constraint_value
            };
            var result = SortHelper.Sort(await IniDocument.ParseAsync(fs), options);

            if (!string.IsNullOrEmpty(output_section)) await sw.WriteLineAsync($"[{output_section}]");
            result.ToList().ForEach(i => sw.WriteLine(i));
        }
    }
}
