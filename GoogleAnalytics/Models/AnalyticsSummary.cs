using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleAnalytics.Models
{
    public class AnalyticsSummary
    {
        public TimeSpan AveragePageLoadTime { get; set; }
        public TimeSpan AverageTimeOnSite { get; set; }
        public IDictionary<string, int> TopSearches { get; set; }
        public IDictionary<string, int> TopReferrers { get; set; }
        public IDictionary<string, int> PageViews { get; set; }
        public IDictionary<string, string> PageTitles { get; set; }
        public double PercentExitRate { get; set; }
        public double PageviewsPerVisit { get; set; }
        public double EntranceBounceRate { get; set; }
        public double PercentNewVisits { get; set; }
        public int TotalPageViews { get; set; }
        public int TotalVisits { get; set; }
        public List<Visits> Visits { get; set; }
        public List<BrowserView> BrowserViews { get; set; }
        public List<Geolocation> Geolocation { get; set; }
        public List<Languages> Languages { get; set; }
        public List<Statistic> Statistics { get; set; }
    }

    public class Visits
    {
        public string Date { get; set; }
        public int Value { get; set; }
    }

    public class BrowserView
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public class Geolocation
    {
        public string Country { get; set; }
        public int Session { get; set; }
    }

    public class Languages
    {
        public int Session { get; set; }
        public string Language { get; set; }
    }
    public class Statistic
    {
        public int Session { get; set; }
        public string Value { get; set; }
    }
}