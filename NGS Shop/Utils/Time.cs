using System.Text.RegularExpressions;

namespace NGSShop.Utils
{
    public static class TimeApi
    {
        public static int Convert(string input)
        {
            var units = new Dictionary<string, int>()
            {
                {@"(\d+)(ms|mili[|s]|milisecon[|s])", 1 },
                {@"(\d+)(s|sec|second[|s])", 1000 },
                {@"(\d+)(m|min[|s])", 60000 },
                {@"(\d+)(h|hour[|s])", 3600000 },
                {@"(\d+)(d|day[|s])", 86400000 },
                {@"(\d+)(w|week[|s])", 604800000 },
            };

            var timespan = new TimeSpan();

            foreach (var x in units)
            {
                var matches = Regex.Matches(input, x.Key);
                foreach (Match match in matches)
                {
                    var amount = System.Convert.ToInt32(match.Groups[1].Value);
                    timespan = timespan.Add(TimeSpan.FromMilliseconds(x.Value * amount));
                }
            }

            return (int)timespan.TotalSeconds;
        }
    }
}