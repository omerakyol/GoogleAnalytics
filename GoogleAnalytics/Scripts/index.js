var Index = function () {

    String.prototype.capitalize = function () {
        return this.charAt(0).toUpperCase() + this.slice(1);
    }

    if (typeof String.prototype.startsWith != 'function') {
        // see below for better implementation!
        String.prototype.startsWith = function (str) {
            return this.indexOf(str) == 0;
        };
    }

    /**   ANALYTICS   **/

    function getAnalytics(startdate, enddate) {
        $.getJSON("/Home/AnalyticsSummary", { "startDate": startdate, "endDate": enddate }, function (data) {
            var browserViews = data.BrowserViews;
            var geoLocation = data.Geolocation;
            var pageViews = data.PageViews;
            var languages = data.Languages;

            var arrVisits = [];
            var arrGeo = [];
            var arrViews = [];
            var arrSiteUsage = [];

            arrSiteUsage.push({ "text": "Total Visits", "value": data.TotalVisits });
            arrSiteUsage.push({ "text": "Total Page Views", "value": data.TotalPageViews });
            arrSiteUsage.push({ "text": "New Visitor Percentage", "value": Math.round(data.PercentNewVisits.toFixed(2)) + "%" });
            arrSiteUsage.push({ "text": "Average Time on Site", "value": data.AverageTimeOnSite.Seconds + " sec." });
            arrSiteUsage.push({ "text": "Entrance Bounce Rate", "value": Math.round(data.EntranceBounceRate.toFixed(2)) + "%" });
            arrSiteUsage.push({ "text": "Exit Rate Percentage", "value": Math.round(data.PercentExitRate.toFixed(2)) + "%" });
            arrSiteUsage.push({ "text": "Page Views Per Visit", "value": data.PageviewsPerVisit.toFixed(2) });
            arrSiteUsage.push({ "text": "Average Page Load Time", "value": data.AveragePageLoadTime.Milliseconds + " ms." });


            $.each(geoLocation, function (index, value) {
                arrGeo.push([value.Country, value.Session]);
            });

            $.each(pageViews, function (index, value) {
                arrViews.push({ "url": index, "view": value });
            });

            initializeBrowserPie(browserViews);
            initializeGeolocationMap(arrGeo);
            initializeVisitPages(arrViews);
            initializeSiteUsage(arrSiteUsage);
            initializeLanguage(languages);
        })
            .error(function () {
                alert("Please sure your credentials are valid!");
            });
    }

    function initializeDailyChart() {
        $("#jqChartVisitDaily").jqChart("destroy");
        var arrVisits = [];
        $.getJSON("/Home/BindDailyStatistic", function (data) {
            var visits = data;
            $.each(visits, function (index, value) {
                arrVisits.push({ "datetime": value.Value, "value": value.Session });
            });
        }).done(function () {
            initializeVisitChart(arrVisits, "#jqChartVisitDaily", "DAILY VISITS", "Time", 0);
        }).error(function () {
            alert("Please sure your credentials are valid!");
        });
    }

    function initializeWeeklyChart() {
        $("#jqChartVisitWeekly").jqChart("destroy");
        var arrVisits = [];
        $.getJSON("/Home/BindWeeklyStatistic", function (data) {
            var visits = data;
            $.each(visits, function (index, value) {
                arrVisits.push({ "datetime": value.Value, "value": value.Session });
            });
        }).done(function () {
            initializeVisitChart(arrVisits, "#jqChartVisitWeekly", "WEEKLY VISITS", "Date", 0);
        }).error(function () {
            alert("Please sure your credentials are valid!");
        });
    }

    function initializeMonthlyChart() {
        $("#jqChartVisitMonthly").jqChart("destroy");
        var arrVisits = [];
        $.getJSON("/Home/BindMonthlyStatistic", function (data) {
            var visits = data;
            $.each(visits, function (index, value) {
                arrVisits.push({ "datetime": value.Value, "value": value.Session });
            });
        }).done(function () {
            initializeVisitChart(arrVisits, "#jqChartVisitMonthly", "MONTHLY VISITS", "Date", -90);
        }).error(function () {
            alert("Please sure your credentials are valid!");
        });
    }

    function initializeYearlyChart() {
        $("#jqChartVisitYearly").jqChart("destroy");
        var arrVisits = [];
        $.getJSON("/Home/BindYearlyStatistic", function (data) {
            var visits = data;
            $.each(visits, function (index, value) {
                arrVisits.push({ "datetime": value.Value, "value": value.Session });
            });
        }).done(function () {
            initializeVisitChart(arrVisits, "#jqChartVisitYearly", "YEARLY VISITS", "Month", 0);
        }).error(function () {
            alert("Please sure your credentials are valid!");
        });
    }

    function initializeVisitChart(model, id, text, datetime, angle) {
        $(id).jqChart({
            title: { text: text.toString() },
            legend: { visible: false },
            animation: { duration: 2 },
            background: '#EEEEEE',
            border: {
                strokeStyle: 'white',
                cornerRadius: 0,
                padding: 20
            },
            shadows: {
                enabled: true
            },
            dataSource: model,
            axes: [
                {
                    location: 'bottom',
                    labels: {
                        angle: angle
                    }
                }
            ],
            series: [
                {
                    type: 'line',
                    xValuesField: {
                        name: 'datetime',
                        type: 'string' // string, numeric, dateTime
                    },
                    yValuesField: 'value'
                }
            ]
        });

        $(id).bind('tooltipFormat', function (e, data) {
            return "<div style='padding:10px'><strong>" + datetime + ":</strong> " + data.dataItem[0] + "<br />" +
                   "<strong>Session:</strong> " + data.dataItem[1] + "</div>";
        });
    }

    function initializeBrowserPie(data) {

        var browsers = [];
        for (var i = 0; i < data.length; i++) {
            browsers.push([data[i].Key, data[i].Value]);
        }

        $('#jqChart').jqChart({
            title: { text: 'BROWSER ANALYTICS' },
            legend: { title: 'BROWSERS' },
            background: '#EEEEEE',
            border: {
                strokeStyle: 'white',
                cornerRadius: 0,
                padding: 20
            },
            animation: { duration: 2 },
            shadows: {
                enabled: true
            },
            series: [
                {
                    type: 'pie',
                    fillStyles: ['#418CF0', '#FCB441', '#E0400A', '#056492', '#BFBFBF', '#1A3B69', '#FFE382'],
                    labels: {
                        stringFormat: '%.1f%%',
                        valueType: 'percentage',
                        font: '15px sans-serif',
                        fillStyle: 'white'
                    },
                    explodedRadius: 10,
                    explodedSlices: [0],
                    data: browsers
                }
            ]
        });

        $('#jqChart').bind('tooltipFormat', function (e, data) {
            var percentage = data.series.getPercentage(data.value);
            percentage = data.chart.stringFormat(percentage, '%.2f%%');

            return '<b>' + data.dataItem[0] + '</b><br />' +
                   data.value + ' (' + percentage + ')';
        });

    }

    function initializeGeolocationMap(data) {

        $('#jqGeoMap').jqGeoMap({
            border: {
                strokeStyle: 'white',
                cornerRadius: 0,
                padding: 20
            },
            background: '#EEEEEE',
            layers: [
                       {
                           source: "/Scripts/countries.geo.json",
                           valueOption: 'name',
                           values: data
                       }
            ]
        });

        $('#jqGeoMap').bind('tooltipFormat', function (e, data) {

            var props = data.properties;
            var countryName = props.name;

            var tooltip = "<div style='padding:10px'><strong>" + countryName + "</strong>";
            if (data.value) {
                tooltip += "</br><strong>Session:</strong> " + data.value;
            }
            tooltip += "</div>";
            return tooltip;
        });
    }

    function initializeVisitPages(data) {
        var html = "<table class='table table-striped table-bordered table-hover table-condensed'><thead><td><strong>#</strong></td><td><strong>URL</strong></td><td><strong>Visits</strong></td></thead><tbody>";
        for (var i = 0; i < data.length; i++) {
            html += "<tr><td>" + (i + 1) + "</td><td>" + data[i].url + "</td><td>" + data[i].view + "</td><tr>";
        }
        html += "</tbody></table>";
        $("#visitPages").html(html);
    }

    function initializeSiteUsage(data) {
        var html = "<table class='table table-striped table-bordered table-hover table-condensed'><thead><td><strong>#</strong></td><td><strong>Description</strong></td><td><strong>Value</strong></td></thead><tbody>";
        for (var i = 0; i < data.length; i++) {
            html += "<tr><td>" + (i + 1) + "</td><td>" + data[i].text + "</td><td>" + data[i].value + "</td><tr>";
        }
        html += "</tbody></table>";
        $("#visitSite").html(html);
    }

    function initializeLanguage(data) {
        var html = "<table class='table table-striped table-bordered table-hover table-condensed'><thead><td><strong>#</strong></td><td><strong>Language</strong></td><td><strong>Session</strong></td></thead><tbody>";
        for (var i = 0; i < data.length; i++) {
            var language = data[i].Language == "en" ? "uk" : data[i].Language;
            var session = data[i].Session;
            var flag = data[i].Language == "uk" ? "en" : data[i].Language;
            var name = "";
            $.ajax({
                type: "GET",
                url: "http://api.geonames.org/countryInfoJSON",
                data: { "country": language, "username": "omerakyol" },
                async: false,
                dataType: 'json',
                success: function (data) {
                    if (!!data.geonames[0]) {
                        name = data.geonames[0].countryName;
                    }
                    else {
                        name = "";
                    }
                }
            });
            if (name != "") {
                html += "<tr><td><img src='' class='flag flag-" + flag + "' alt='' /></td><td>" + name + " (" + data[i].Language + ")</td><td>" + session + "</td><tr>";
            }

        }
        html += "</tbody></table>";
        $("#sessionLanguage").html(html);
    }

    /***/

    return {

        initHomeDaterange: function () {

            $('#dashboard-report-range').daterangepicker({
                opens: 'left',
                startDate: moment().subtract('days', 30),
                endDate: moment(),
                minDate: '01/01/2012',
                maxDate: moment(),
                dateLimit: {
                    days: 60
                },
                showDropdowns: false,
                showWeekNumbers: true,
                timePicker: false,
                timePickerIncrement: 1,
                timePicker12Hour: true,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)],
                    'Last 7 Days': [moment().subtract('days', 6), moment()],
                    'Last 30 Days': [moment().subtract('days', 29), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
                },
                buttonClasses: ['btn'],
                applyClass: 'blue',
                cancelClass: 'default',
                format: 'DD.MM.YYYY',
                separator: ' - '
            },
            function (start, end) {
                if (!!start && !!end) {
                    $('#dashboard-report-range span').html(start.format('DD.MM.YYYY') + ' - ' + end.format('DD.MM.YYYY'));
                    getAnalytics(start.format('YYYY-MM-DD'), end.format('YYYY-MM-DD'));
                }
            }
            );


            $('#dashboard-report-range span').html(moment().subtract('days', 30).format('DD.MM.YYYY') + ' - ' + moment().format('DD.MM.YYYY'));
            $('#dashboard-report-range').show();
            initializeDailyChart();

            getAnalytics(moment().subtract('days', 30).format('YYYY-MM-DD'), moment().format('YYYY-MM-DD'));

            $("#daily").click(initializeDailyChart);
            $("#weekly").click(initializeWeeklyChart);
            $("#monthly").click(initializeMonthlyChart);
            $("#yearly").click(initializeYearlyChart);

        }
    };

}();