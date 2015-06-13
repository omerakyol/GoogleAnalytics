using Google.GData.Analytics;
using GoogleAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GoogleAnalytics.Controllers
{

    public class HomeController : Controller
    {
        private static string username;
        private static string pass;
        private static string siteID;

        AnalyticsService analytics = new AnalyticsService("WebApp");

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void SaveInfo(string Username, string Password, string SiteID)
        {
            username = Username;
            pass = Password;
            siteID = SiteID;
        }

        [HttpGet]
        public async Task<JsonResult> BindDailyStatistic()
        {
            analytics.setUserCredentials(username, pass);
            List<Statistic> DailyStatistics = new List<Statistic>();
            DateTime today = DateTime.Now;
            var daily = new DataQuery(siteID, today, today)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:hour",
                Sort = "ga:hour"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(daily).Entries)
                {
                    var session = entry.Metrics.First().IntegerValue;
                    var time = entry.Dimensions.Single(x => x.Name == "ga:hour").Value.ToString() + ":00";
                    Statistic dailyStatistic = new Statistic
                    {
                        Session = session,
                        Value = time
                    };
                    DailyStatistics.Add(dailyStatistic);
                }
            });
            return Json(DailyStatistics, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> BindWeeklyStatistic()
        {
            analytics.setUserCredentials(username, pass);
            List<Statistic> WeeklyStatistics = new List<Statistic>();
            DateTime today = DateTime.Now;
            DateTime aWeekBefore = today.AddDays(-7);
            var weekly = new DataQuery(siteID, aWeekBefore, today)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:date",
                Sort = "ga:date"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(weekly).Entries)
                {
                    var session = entry.Metrics.First().IntegerValue;
                    var date = entry.Dimensions.Single(x => x.Name == "ga:date").Value.ToString();
                    var formattedDate = date.Substring(6, 2) + "." + date.Substring(4, 2) + "." + date.Substring(0, 4);
                    Statistic weeklyStatistic = new Statistic
                    {
                        Session = session,
                        Value = formattedDate
                    };
                    WeeklyStatistics.Add(weeklyStatistic);
                }
            });
            return Json(WeeklyStatistics, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> BindMonthlyStatistic()
        {
            analytics.setUserCredentials(username, pass);
            List<Statistic> MonthlyStatistics = new List<Statistic>();
            DateTime today = DateTime.Now;
            DateTime aMonthBefore = today.AddMonths(-1);
            var weekly = new DataQuery(siteID, aMonthBefore, today)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:date",
                Sort = "ga:date"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(weekly).Entries)
                {
                    var session = entry.Metrics.First().IntegerValue;
                    var date = entry.Dimensions.Single(x => x.Name == "ga:date").Value.ToString();
                    var formattedDate = date.Substring(6, 2) + "." + date.Substring(4, 2) + "." + date.Substring(0, 4);
                    Statistic monthlyStatistic = new Statistic
                    {
                        Session = session,
                        Value = formattedDate
                    };
                    MonthlyStatistics.Add(monthlyStatistic);
                }
            });
            return Json(MonthlyStatistics, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> BindYearlyStatistic()
        {
            analytics.setUserCredentials(username, pass);
            List<Statistic> YearlyStatistics = new List<Statistic>();
            DateTime today = DateTime.Now;
            var year = today.Year;
            DateTime startOfYear = Convert.ToDateTime("01.01." + year);
            var yearly = new DataQuery(siteID, startOfYear, today)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:month",
                Sort = "ga:month"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(yearly).Entries)
                {
                    var session = entry.Metrics.First().IntegerValue;
                    var month = entry.Dimensions.Single(x => x.Name == "ga:month").Value;
                    var formattedMonth = month.StartsWith("0") ? "Month " + month.Substring(1, 1) : "Month " + month.ToString();
                    Statistic monthlyStatistic = new Statistic
                    {
                        Session = session,
                        Value = formattedMonth
                    };
                    YearlyStatistics.Add(monthlyStatistic);
                }
            });
            return Json(YearlyStatistics, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> AnalyticsSummary(DateTime startDate, DateTime endDate)
        {
            analytics.setUserCredentials(username, pass);
            var to = endDate;
            var from = startDate;
            var model = new AnalyticsSummary
            {
                Visits = new List<Visits>(),
                PageViews = new Dictionary<string, int>(),
                PageTitles = new Dictionary<string, string>(),
                TopReferrers = new Dictionary<string, int>(),
                TopSearches = new Dictionary<string, int>(),
                BrowserViews = new List<BrowserView>(),
                Geolocation = new List<Geolocation>(),
                Languages = new List<Languages>()
            };

            model.Visits = await GetVisits(from, to);
            await GetSiteUsage(from, to, model);
            await GetTopPages(from, to, model);
            await GetTopReferrers(from, to, model);
            await GetTopSearches(from, to, model);
            await GetBrowserViews(from, to, model);
            await GetGeolocation(from, to, model);
            await GetLanguages(from, to, model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<List<Visits>> GetVisits(DateTime from, DateTime to)
        {
            List<Visits> visitList = new List<Visits>();
            var visits = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:visits",
                Dimensions = "ga:date",
                Sort = "ga:date"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(visits).Entries)
                {
                    var value = entry.Metrics.First().IntegerValue;
                    var date = entry.Dimensions.First().Value.Substring(6, 2) + "." + entry.Dimensions.First().Value.Substring(4, 2) + "." + entry.Dimensions.First().Value.Substring(0, 4);
                    Visits visit = new Visits
                    {
                        Date = date,
                        Value = value
                    };
                    visitList.Add(visit);
                }
            });
            return visitList;
        }

        public async Task GetSiteUsage(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var siteUsage = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:visits,ga:pageviews,ga:percentNewVisits,ga:avgTimeOnSite,ga:entranceBounceRate,ga:exitRate,ga:pageviewsPerVisit,ga:avgPageLoadTime"
            };
            var siteUsageResult = (DataEntry)analytics.Query(siteUsage).Entries.FirstOrDefault();
            if (siteUsageResult != null)
            {
                await Task.Run(() =>
                {
                    foreach (var metric in siteUsageResult.Metrics)
                    {
                        switch (metric.Name)
                        {
                            case "ga:visits":
                                model.TotalVisits = metric.IntegerValue;
                                break;
                            case "ga:pageviews":
                                model.TotalPageViews = metric.IntegerValue;
                                break;
                            case "ga:percentNewVisits":
                                model.PercentNewVisits = metric.FloatValue;
                                break;
                            case "ga:avgTimeOnSite":
                                model.AverageTimeOnSite = TimeSpan.FromSeconds(metric.FloatValue);
                                break;
                            case "ga:entranceBounceRate":
                                model.EntranceBounceRate = metric.FloatValue;
                                break;
                            case "ga:exitRate":
                                model.PercentExitRate = metric.FloatValue;
                                break;
                            case "ga:pageviewsPerVisit":
                                model.PageviewsPerVisit = metric.FloatValue;
                                break;
                            case "ga:avgPageLoadTime":
                                model.AveragePageLoadTime = TimeSpan.FromSeconds(metric.FloatValue);
                                break;
                        }
                    }
                });
            }

        }

        public async Task GetTopPages(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var topPages = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:pageviews",
                Dimensions = "ga:pagePath,ga:pageTitle",
                Sort = "-ga:pageviews",
                NumberToRetrieve = 20
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(topPages).Entries)
                {
                    var value = entry.Metrics.First().IntegerValue;
                    var url = entry.Dimensions.Single(x => x.Name == "ga:pagePath").Value.ToLowerInvariant();
                    var title = entry.Dimensions.Single(x => x.Name == "ga:pageTitle").Value;

                    if (!model.PageViews.ContainsKey(url))
                        model.PageViews.Add(url, 0);
                    model.PageViews[url] += value;

                    if (!model.PageTitles.ContainsKey(url))
                        model.PageTitles.Add(url, title);
                }
            });
        }

        public async Task GetTopReferrers(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var topReferrers = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:visits",
                Dimensions = "ga:source,ga:medium",
                Sort = "-ga:visits",
                Filters = "ga:medium==referral",
                NumberToRetrieve = 5
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(topReferrers).Entries)
                {
                    var visitCount = entry.Metrics.First().IntegerValue;
                    var source = entry.Dimensions.Single(x => x.Name == "ga:source").Value.ToLowerInvariant();

                    model.TopReferrers.Add(source, visitCount);
                }
            });
        }

        public async Task GetTopSearches(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var topSearches = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:visits",
                Dimensions = "ga:keyword",
                Sort = "-ga:visits",
                Filters = "ga:keyword!=(not set);ga:keyword!=(not provided)",
                NumberToRetrieve = 5
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(topSearches).Entries)
                {
                    var visitCount = entry.Metrics.First().IntegerValue;
                    var source = entry.Dimensions.Single(x => x.Name == "ga:keyword").Value.ToLowerInvariant();

                    model.TopSearches.Add(source, visitCount);
                }
            });
        }

        public async Task GetBrowserViews(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var browserViews = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:users",
                Dimensions = "ga:browser"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(browserViews).Entries)
                {
                    BrowserView browser = new BrowserView();
                    var value = entry.Metrics.First().IntegerValue;
                    var source = entry.Dimensions.Single(x => x.Name == "ga:browser").Value.ToString();
                    browser.Key = source;
                    browser.Value = value;
                    model.BrowserViews.Add(browser);
                }
            });
        }

        public async Task GetGeolocation(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var geoLocation = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:country"
            };
            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(geoLocation).Entries)
                {
                    Geolocation geo = new Geolocation();
                    var session = entry.Metrics.First().IntegerValue;
                    var country = entry.Dimensions.First().Value.ToString();
                    geo.Session = session;
                    geo.Country = country;
                    model.Geolocation.Add(geo);
                }
            });
        }

        public async Task GetLanguages(DateTime from, DateTime to, AnalyticsSummary model)
        {
            var languages = new DataQuery(siteID, from, to)
            {
                Metrics = "ga:sessions",
                Dimensions = "ga:language,ga:country",
                Sort = "-ga:sessions"
            };

            await Task.Run(() =>
            {
                foreach (DataEntry entry in analytics.Query(languages).Entries)
                {
                    Languages language = new Languages();
                    var value = entry.Metrics.First().IntegerValue;
                    var source = entry.Dimensions.Single(x => x.Name == "ga:language").Value;
                    Languages containsLanguage = null;

                    if (!String.IsNullOrEmpty(source))
                    {
                        if (source != "(not set)")
                        {
                            if (source.Length == 1)
                            {
                                source = source.Substring(0, 1);
                            }
                            else
                            {
                                source = source.Substring(0, 2);
                            }
                            if (model.Languages.Any())
                            {
                                containsLanguage = model.Languages.Where(x => x.Language.Contains(source)).FirstOrDefault();
                            }

                            if (containsLanguage == null)
                            {
                                language.Session = value;
                                language.Language = source;
                                model.Languages.Add(language);
                            }
                            else
                            {
                                containsLanguage.Session += value;
                            }
                        }
                    }


                }
            });
        }
    }
}