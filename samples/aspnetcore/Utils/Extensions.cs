﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.PairwiseApi;

namespace WebAgent.Utils
{
    public static class Extensions
    {
        public static string TimeAgo(this DateTime dateTime)
        {
            string result;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(15))
            {
                result = "Just now";
            }
            else if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = $"{timeSpan.Seconds} seconds ago";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} minutes ago" : "About a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} hours ago" : "About an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ? $"{timeSpan.Days} days ago" : "Yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30
                    ? $"{timeSpan.Days / 30} months ago"
                    : "About a month ago";
            }
            else
            {
                result = timeSpan.Days > 365
                    ? $"{timeSpan.Days / 365} years ago"
                    : "About a year ago";
            }

            return result;
        }

        public static List<(string Key, string Value)> TagContents(this RecordBase record, bool sort = true)
        {
            var type = record.GetType();
            var propInfo = type.GetProperty("Tags",
                BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            var dict = propInfo.GetValue(record) as Dictionary<string, string>;
            var list = new List<(string Key, string Value)>();
            foreach (var keyValuePair in dict)
            {
                list.Add((keyValuePair.Key, keyValuePair.Value));
            }
            if (sort)
                list.Sort((a, b) => String.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        public static string AsJsonKeyValueObject(this List<(string Key, string Value)> kvp)
        {
            var sb = new StringBuilder("{\n");
            kvp.ForEach(pair =>
            {
                if (pair.Key.EndsWith("AtUtc", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(pair.Value, out long secsSinceEpoch))
                    {
                        var dtm = DateTime.FromBinary(secsSinceEpoch);
                        sb.Append($"  \"{pair.Key}\": \"{dtm:o}\"\n");
                    }
                    else
                        sb.Append($"  \"{pair.Key}\": \"{pair.Value}\"\n");
                }
                else    
                    sb.Append($"  \"{pair.Key}\": \"{pair.Value}\"\n");
            });
            sb.Append('}');
            return sb.ToString();
        }
    }
}