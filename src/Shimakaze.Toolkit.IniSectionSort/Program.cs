using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommandLine;

using Shimakaze.Struct.Ini;

namespace Shimakaze.Toolkit.IniSectionSort
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ParserResult = Parser.Default.ParseArguments<Options>(args);
            await ParserResult.WithParsedAsync(async i =>
            {
                if (!i.NoLogo)
                {
                    Console.WriteLine("******************");
                    Console.WriteLine("* IniSectionSort *");
                    Console.WriteLine("******************");
                }
                using var fs = new FileStream(i.Input, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var ofs = new FileStream(i.Output, FileMode.Create, FileAccess.Write, FileShare.Read);
                using var sw = new StreamWriter(ofs);

                var result = SortHelper.Sort(await IniDocument.ParseAsync(fs), i);

                if (!string.IsNullOrEmpty(i.TargetSectionName))
                    await sw.WriteLineAsync($"[{i.TargetSectionName}]");
                result.ToList().ForEach(async i => await sw.WriteLineAsync(i));
            });
        }
    }
    public class Options : SortOptions
    {
        [Option('i', "Input", Required = true, HelpText = "文件路径: 将文件作为输入源输入")]
        public string Input { get; set; }
        [Option('o', "Output", Required = true, HelpText = "文件路径: 将文件作为输出源输出")]
        public string Output { get; set; }
        [Option("NoLogo", Required = false, Default = false, Hidden = true)]
        public bool NoLogo { get; set; }
        /// <summary>
        /// 注册表节名
        /// </summary>
        [Option('t', "Target", Required = false, HelpText = "字符串: 在输出结果前添加一个节头")]
        public string TargetSectionName { get; set; }
    }
}
