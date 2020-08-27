using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using Shimakaze.Struct.Ini;

namespace Shimakaze.Toolkit.IniSectionSort
{
    public static class SortHelper
    {
        public static IEnumerable<string> Sort(IniDocument ini, SortOptions options = null)
        {
            IEnumerable<IniSection> constraintResult;// 约束筛选
            if (string.IsNullOrEmpty(options?.KeyConstraint))// 是否有键约束
                constraintResult = ini.Sections;// 没有键约束
            else if (string.IsNullOrEmpty(options?.ValueConstraint))// 是否有值约束
                constraintResult = ini.Sections.Where(i => i.TryGetKey(options.KeyConstraint, out _));// 仅键约束
            else
                constraintResult = ini.Sections.Where(i => i[options.KeyConstraint].Value.Equals(options.ValueConstraint));// 值约束

            IEnumerable<IniSection> sortResult;// 排序
            if (options?.Sort ?? false)// 是否排序
                if (string.IsNullOrEmpty(options?.SortTargetKey))// 是否按目标键的值排序
                    sortResult = constraintResult.OrderBy(i => i.Name);// 不按目标键的值排序
                else
                    sortResult = constraintResult
                        .Where(i => i.TryGetKey(options.SortTargetKey, out _))
                        .OrderBy(i => i[options.SortTargetKey].Value.ToString());// 按目标值排序
            else
                sortResult = constraintResult;// 不排序

            IEnumerable<string> result;// 结果
            int num = options.First;

            if (options?.Digit.HasValue ?? false)
                if (string.IsNullOrEmpty(options?.SummaryKey))
                    result = sortResult.Select(i => $"{options?.Prefix ?? string.Empty}{num++.ToString().PadLeft(options.Digit.Value, '0')}={i.Name}");
                else
                    result = sortResult.Select(i => $"{options?.Prefix ?? string.Empty}{num++.ToString().PadLeft(options.Digit.Value, '0')}={i.Name}; {i.TryGetKey(options.SummaryKey)?.Value ?? string.Empty}");
            else
                if (string.IsNullOrEmpty(options?.SummaryKey))
                result = sortResult.Select(i => $"{options?.Prefix ?? string.Empty}{num++}={i.Name}");
            else
                result = sortResult.Select(i => $"{options?.Prefix ?? string.Empty}{num++}={i.Name}; {i.TryGetKey(options.SummaryKey)?.Value ?? string.Empty}");

            return result;
        }
    }
    public class SortOptions
    {
        [Option('s', "sort", Required = false, Default = false, HelpText = "布尔: 对输入文件进行排序.")]
        public bool? Sort { get; set; }

        [Option('p', "prefix", Required = false, Default = null, HelpText = "字符串: 以此字符串为前缀")]
        public string Prefix { get; set; }

        [Option('f', "first", Required = false, Default = 0, HelpText = "整数: 以此数字开始计数")]
        public int First { get; set; }

        [Option('d', "digit", Required = false, Default = null, HelpText = "整数: 以此数字规范长度")]
        public int? Digit { get; set; }
        /// <summary>
        /// 根据目标键的指排序
        /// </summary>
        [Option('k', "Key", Required = false, Default = null, HelpText = "字符串: 根据目标键的指排序")]
        public string SortTargetKey { get; set; }

        [Option('S', "SummaryKey", Required = false, Default = null, HelpText = "字符串: 将此键的值作为注释输出")]
        public string SummaryKey { get; set; }



        [Option("KeyConstraint", Required = false, Default = null, HelpText = "键约束 字符串: 只有包含此键的节才会被输出")]
        public string KeyConstraint { get; set; }

        [Option("ValueConstraint", Required = false, Default = null, HelpText = "值约束 字符串: 只有当键约束的键值等于这里的值时才会被输出")]
        public string ValueConstraint { get; set; }
    }
}
