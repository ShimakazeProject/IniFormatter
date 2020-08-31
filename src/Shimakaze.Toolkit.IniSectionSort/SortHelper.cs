using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Shimakaze.Struct.Ini;

namespace Shimakaze.Toolkit.IniSectionSort
{
    public static class SortHelper
    {
        public static IEnumerable<string> Sort(IniDocument ini, SortOptions options = null)
        {
            IEnumerable<IniSection> Result;// 约束筛选
            IEnumerable<string> result;// 结果
            int num = options.First;

            if (string.IsNullOrEmpty(options?.KeyConstraint))// 是否有键约束
                Result = ini.Sections;// 没有键约束
            else if (string.IsNullOrEmpty(options?.ValueConstraint))// 是否有值约束
                Result = ini.Sections.Where(i => i.TryGetKey(options.KeyConstraint, out _));// 仅键约束
            else
                Result = ini.Sections.Where(i => i.TryGetKey(options.KeyConstraint)?.Value.Equals(options.ValueConstraint) ?? false);// 值约束

            if (options?.Sort ?? false)// 是否排序
                if (string.IsNullOrEmpty(options?.SortTargetKey))// 是否按目标键的值排序
                    Result = Result.OrderBy(i => i.Name);// 不按目标键的值排序
                else
                    Result = Result
                        .Where(i => i.TryGetKey(options.SortTargetKey, out _))
                        .OrderBy(i => i.TryGetKey(options.SortTargetKey)?.Value.ToString());// 按目标值排序


            if (options?.Digit.HasValue ?? false)
                if (string.IsNullOrEmpty(options?.SummaryKey))
                    result = Result.Select(i => $"{options?.Prefix ?? string.Empty}{num++.ToString().PadLeft(options.Digit.Value, '0')}={i.Name}");
                else
                    result = Result.Select(i => $"{options?.Prefix ?? string.Empty}{num++.ToString().PadLeft(options.Digit.Value, '0')}={i.Name}; {i.TryGetKey(options.SummaryKey)?.Value ?? string.Empty}");
            else
                if (string.IsNullOrEmpty(options?.SummaryKey))
                result = Result.Select(i => $"{options?.Prefix ?? string.Empty}{num++}={i.Name}");
            else
                result = Result.Select(i => $"{options?.Prefix ?? string.Empty}{num++}={i.Name}; {i.TryGetKey(options.SummaryKey)?.Value ?? string.Empty}");

            return result;
        }
    }
    public class SortOptions
    {
        public bool? Sort { get; set; }

        public string Prefix { get; set; }

        public int First { get; set; }

        public int? Digit { get; set; }
        /// <summary>
        /// 根据目标键的指排序
        /// </summary>
        public string SortTargetKey { get; set; }

        public string SummaryKey { get; set; }

        public string KeyConstraint { get; set; }

        public string ValueConstraint { get; set; }
    }
}
