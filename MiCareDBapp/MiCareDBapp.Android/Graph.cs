using System;
using OxyPlot;
using OxyPlot.Xamarin.Android;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MiCareDBapp.Droid;
using OxyPlot.Axes;
using OxyPlot.Series;
using Android;

namespace MiCareDBapp.Droid
{
    //all of these graphs are setup with oxyplot, which is a nuGet package library which can be imported to allow the construction of various types of graphs for more info: "http:/docs.oxyplot.org/en/latest/"
    //each kpi is given it's own graph dialog fragment class

    public class BankBalanceGraph : Android.Support.V4.App.DialogFragment
    {
        private List<BankBalance> dataObjects;


        //import the bank balance items
        public BankBalanceGraph(List<BankBalance> data) {
            dataObjects = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Bank Balance";


            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            //get the max and min range for the left axis
            dataObjects.Sort(delegate (BankBalance one, BankBalance two) {
                return one.GetBankBalance().CompareTo(two.GetBankBalance());
            });

            var MinBalance = dataObjects.First().GetBankBalance();
            var MaxBalance = dataObjects.Last().GetBankBalance();

            var maxVal = LinearAxis.ToDouble(MaxBalance);
            var minVal = LinearAxis.ToDouble(MinBalance);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis { Key = "currency", Position = AxisPosition.Left, AbsoluteMinimum = minVal-10000, AbsoluteMaximum = maxVal+10000, Title = "Bank Balance ($)" , MinorStep = 50000 , MajorStep = 100000, LabelFormatter = formatter});

            //get the max and min range for the date axis
            dataObjects.Sort(delegate (BankBalance one, BankBalance two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal-1, AbsoluteMaximum = lastVal+1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            //add line series
            var BankBalanceSeries = new LineSeries { Title = "Bank Balance", MarkerStroke = OxyColors.Aqua };

            foreach (BankBalance item in dataObjects) {
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var balanceAsDouble = LinearAxis.ToDouble(item.GetBankBalance());
                BankBalanceSeries.Points.Add(new DataPoint(dateAsDouble, balanceAsDouble));
            }

            plotModel.Series.Add(BankBalanceSeries);

            plotView.Model = plotModel;

            //this button when pressed will perform a calculation which will output the moving average, upper channel and lower channel onto the graph
            //info on keltner channels (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:keltner_channels"
            //info on moving averages (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:moving_averages"
            //info on average true ranges (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:average_true_range_atr"
            Button KeltnerBtn = view.FindViewById<Button>(Resource.Id.KeltnerBtn);
            KeltnerBtn.Visibility = ViewStates.Visible;
            KeltnerBtn.Enabled = true;
            KeltnerBtn.Click += delegate {
                var MiddleLineSeries = new LineSeries { Title = "Moving Average", MarkerStroke = OxyColors.Black };
                var UpperChannelSeries = new LineSeries { Title = "Upper Channel", MarkerStroke = OxyColors.ForestGreen };
                var LowerChannelSeries = new LineSeries { Title = "Lower Channel", MarkerStroke = OxyColors.DarkRed };
                
                int fiveDayPeriod = 0; // the period used to calculated true ranges
                int twentyFiveDayPeriod = 0; //the period used to calculate average true range
                decimal currentHigh = dataObjects.First().GetBankBalance();
                decimal currentLow = dataObjects.First().GetBankBalance();

                decimal[] incomeOverFiveDays = new decimal[5] { 0, 0, 0, 0, 0 };
                decimal[] SMAOverTwentyFiveDays = new decimal[5] { 0, 0, 0, 0, 0 };
                decimal[] trueRangePeriod = new decimal[5] { 0, 0, 0, 0, 0 };
                decimal averageTrueRange = 0;
                decimal MovingAverage = 0;
                decimal multiplier = (decimal) 2 / 6;
                bool postData = false;

                int firstATR = 0;

                var highestVal = LinearAxis.ToDouble(0);
                var lowestVal = LinearAxis.ToDouble(Int32.MaxValue);

                foreach (BankBalance item in dataObjects) {
                    decimal itemBalance = item.GetBankBalance();
                    postData = false;
                    //before 5 days have passed find highest and lowest income values
                    if (fiveDayPeriod < 5) {
                        if (itemBalance > currentHigh) {
                            currentHigh = itemBalance;
                        } else if (itemBalance < currentLow) {
                            currentLow = itemBalance;
                        }
                        incomeOverFiveDays[fiveDayPeriod] = itemBalance;
                        //after 5 days have passed find average true range 
                    } else {
                        fiveDayPeriod = 0;
                        if (firstATR > 0) {//after 25 days
                            averageTrueRange = ((averageTrueRange * 4) + (currentHigh - currentLow)) / 5;
                            MovingAverage = ((itemBalance - MovingAverage) * multiplier) + MovingAverage;
                            postData = true;
                        } else {//for the first 25 days
                            trueRangePeriod[twentyFiveDayPeriod] = currentHigh - currentLow;
                            SMAOverTwentyFiveDays[twentyFiveDayPeriod] = (incomeOverFiveDays.Sum()) / 5;
                            twentyFiveDayPeriod++;
                        }
                        currentHigh = itemBalance;
                        currentLow = itemBalance;
                    }
                    fiveDayPeriod++;
                    if (twentyFiveDayPeriod == 5) {
                        twentyFiveDayPeriod++;
                        firstATR++;
                        averageTrueRange = (trueRangePeriod.Sum()) / 5;
                        MovingAverage = (SMAOverTwentyFiveDays.Sum()) / 5;
                        postData = true;
                    }
                    if (postData) {
                        var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                        var middleLineAsDouble = LinearAxis.ToDouble(MovingAverage);
                        MiddleLineSeries.Points.Add(new DataPoint(dateAsDouble, middleLineAsDouble));


                        var upperChannelAsDouble = LinearAxis.ToDouble(MovingAverage + (2 * averageTrueRange));
                        UpperChannelSeries.Points.Add(new DataPoint(dateAsDouble, upperChannelAsDouble));

                        if (upperChannelAsDouble > highestVal) {
                            highestVal = upperChannelAsDouble;
                        }

                        var lowerChannelsAsDouble = LinearAxis.ToDouble(MovingAverage - (2 * averageTrueRange));
                        LowerChannelSeries.Points.Add(new DataPoint(dateAsDouble, lowerChannelsAsDouble));


                        if (lowerChannelsAsDouble < lowestVal) {
                            lowestVal = lowerChannelsAsDouble;
                        }
                    }
                }
                plotModel.Series.Add(MiddleLineSeries);
                plotModel.Series.Add(UpperChannelSeries);
                plotModel.Series.Add(LowerChannelSeries);
                if (lowestVal < minVal) {//reset the max and/or min axis values to accomodate for upper and lower channels
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMinimum = lowestVal - 10000;
                }
                if (highestVal > maxVal) {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMaximum = highestVal + 10000;
                }
                plotModel.LegendPlacement = LegendPlacement.Inside;
                plotModel.LegendPosition = LegendPosition.BottomRight;
                //refresh plot
                plotView.InvalidatePlot(true);
                KeltnerBtn.Enabled = false;
            };

            return view;
        }

    }

    public class ACFIGraph : Android.Support.V4.App.DialogFragment
    {
        private decimal AvondrustLodgeTotal = 0;
        private decimal MargrietManorTotal = 0;
        private decimal OverbeekLodgeTotal = 0;
        private decimal PrinsWillemAlexanderLodgeTotal = 0;


        //import the acfi data and get total income per facility 
        public ACFIGraph(List<ACFIFunding> data) {
            foreach (ACFIFunding item in data) {
                int itemFacility = item.GetFacilityID();
                switch (itemFacility) {
                    case 1:
                        AvondrustLodgeTotal = AvondrustLodgeTotal + item.GetIncome();
                        break;
                    case 2:
                        MargrietManorTotal = MargrietManorTotal + item.GetIncome();
                        break;
                    case 3:
                        OverbeekLodgeTotal = OverbeekLodgeTotal + item.GetIncome();
                        break;
                    case 4:
                        PrinsWillemAlexanderLodgeTotal = PrinsWillemAlexanderLodgeTotal + item.GetIncome();
                        break;
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Total ACFI Funding across MiCare";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            //setup vertical bar graph
            var barSeries = new ColumnSeries {
                ItemsSource = new List<ColumnItem>(new[]
                {
                    new ColumnItem{ Value = (double) AvondrustLodgeTotal},
                    new ColumnItem{ Value = (double) MargrietManorTotal},
                    new ColumnItem{ Value = (double) OverbeekLodgeTotal},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeTotal}
                }),
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0}",
                FillColor = OxyColors.LightSeaGreen
            };

            plotModel.Series.Add(barSeries);

            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = new[] {
                    "",
                    "",
                    "",
                    ""
                }
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                Title = "Amount ($)"
            });

            plotView.Model = plotModel;

            //put names of facilities visible 
            LinearLayout BarGraphLables = view.FindViewById<LinearLayout>(Resource.Id.BarGraphLabels);
            BarGraphLables.Visibility = ViewStates.Visible;

            return view;
        }

    }

    public class AgencyUsageGraph : Android.Support.V4.App.DialogFragment
    {
        private List<AgencyUsageData> dataObjects;
        private int facilityNum;


        //import agency usage data
        public AgencyUsageGraph(List<AgencyUsageData> data, int facility)
        {
            dataObjects = data;
            facilityNum = facility;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Agency Usage";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            dataObjects.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                return one.GetAgencyUsageAmount().CompareTo(two.GetAgencyUsageAmount());
            });
            //setup maximum and minium values for left axis
            var MinUsageAmount = dataObjects.First().GetAgencyUsageAmount();
            var MaxUsageAmount = dataObjects.Last().GetAgencyUsageAmount();

            var maxVal = LinearAxis.ToDouble(MaxUsageAmount);
            var minVal = LinearAxis.ToDouble(MinUsageAmount);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis {Key="currency", Position = AxisPosition.Left, AbsoluteMinimum = minVal - 10000, AbsoluteMaximum = maxVal + 10000, Title = "Amount ($)", MinorStep = 50000, MajorStep = 100000, LabelFormatter = formatter });

            //setup maximum and minium values for date axis
            dataObjects.Sort(delegate (AgencyUsageData one, AgencyUsageData two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal - 1, AbsoluteMaximum = lastVal + 1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            //setup the line series
            var AvondrustVillageSeries = new LineSeries { MarkerStroke = OxyColors.Aqua, Title = "Avondrust Village" };
            var MargrietManorSeries = new LineSeries { MarkerStroke = OxyColors.Crimson, Title = "Margriet Manor" };
            var OverbeekLodgeSeries = new LineSeries { MarkerStroke = OxyColors.Orange, Title = "Overbeek Lodge" };
            var PrinsWillemAlexanderLodgeSeries = new LineSeries { MarkerStroke = OxyColors.DimGray, Title = "Prins Willem Alexander Lodge" };

            foreach (AgencyUsageData item in dataObjects) {
                int itemFacility = item.GetFacilityID();
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var amountAsDouble = LinearAxis.ToDouble(item.GetAgencyUsageAmount());
                switch (itemFacility) {
                    case 1:
                        AvondrustVillageSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 2:
                        MargrietManorSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 3:
                        OverbeekLodgeSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 4:
                        PrinsWillemAlexanderLodgeSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                }
            }

            switch (facilityNum) {
                case 1:
                    plotModel.Series.Add(AvondrustVillageSeries);
                    break;
                case 2:
                    plotModel.Series.Add(MargrietManorSeries);
                    break;
                case 3:
                    plotModel.Series.Add(OverbeekLodgeSeries);
                    break;
                case 4:
                    plotModel.Series.Add(PrinsWillemAlexanderLodgeSeries);
                    break;
            }
            
            plotView.Model = plotModel;

            //this button when pressed will perform a calculation which will output the moving average, upper channel and lower channel onto the graph
            //info on keltner channels (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:keltner_channels"
            //info on moving averages (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:moving_averages"
            //info on average true ranges (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:average_true_range_atr"
            Button KeltnerBtn = view.FindViewById<Button>(Resource.Id.KeltnerBtn);
            KeltnerBtn.Visibility = ViewStates.Visible;
            KeltnerBtn.Enabled = true;
            KeltnerBtn.Click += delegate {
                var MiddleLineSeries = new LineSeries { Title = "Moving Average", MarkerStroke = OxyColors.Black };
                var UpperChannelSeries = new LineSeries { Title = "Upper Channel", MarkerStroke = OxyColors.ForestGreen };
                var LowerChannelSeries = new LineSeries { Title = "Lower Channel", MarkerStroke = OxyColors.DarkRed };

                int threeMonthPeriod = 0; // the period used to calculated true ranges
                int nineMonthPeriod = 0; //the period used to calculate average true range
                decimal currentHigh = dataObjects.First().GetAgencyUsageAmount();
                decimal currentLow = dataObjects.First().GetAgencyUsageAmount();

                decimal[] incomeOverThreeMonths = new decimal[3] { 0, 0, 0 };
                decimal[] SMAOverNineMonths = new decimal[3] { 0, 0, 0 };
                decimal[] trueRangePeriod = new decimal[3] { 0, 0, 0 };
                decimal averageTrueRange = 0;
                decimal MovingAverage = 0;
                decimal multiplier = (decimal)2 / 4;
                bool postData = false;

                int firstATR = 0;

                var highestVal = LinearAxis.ToDouble(0);
                var lowestVal = LinearAxis.ToDouble(Int32.MaxValue);

                foreach (AgencyUsageData item in dataObjects)
                {
                    decimal itemAgencyUsage = item.GetAgencyUsageAmount();

                    postData = false;
                    //before 3 months have passed find highest and lowest hours values
                    if (threeMonthPeriod < 3)
                    {
                        if (itemAgencyUsage > currentHigh)
                        {
                            currentHigh = itemAgencyUsage;
                        }
                        else if (itemAgencyUsage < currentLow)
                        {
                            currentLow = itemAgencyUsage;
                        }
                        incomeOverThreeMonths[threeMonthPeriod] = itemAgencyUsage;
                        //after 3 months have passed find average true range 
                    }
                    else
                    {
                        threeMonthPeriod = 0;
                        if (firstATR > 0)//after nine months
                        {
                            averageTrueRange = ((averageTrueRange * 2) + (currentHigh - currentLow)) / 3;
                            MovingAverage = ((itemAgencyUsage - MovingAverage) * multiplier) + MovingAverage;
                            postData = true;
                        }
                        else
                        {//for the first nine months
                            trueRangePeriod[nineMonthPeriod] = currentHigh - currentLow;
                            SMAOverNineMonths[nineMonthPeriod] = (incomeOverThreeMonths.Sum()) / 3;
                            nineMonthPeriod++;
                        }
                        currentHigh = itemAgencyUsage;
                        currentLow = itemAgencyUsage;
                    }
                    threeMonthPeriod++;
                    if (nineMonthPeriod == 3)
                    {
                        nineMonthPeriod++;
                        firstATR++;
                        averageTrueRange = (trueRangePeriod.Sum()) / 3;
                        MovingAverage = (SMAOverNineMonths.Sum()) / 3;
                        postData = true;
                    }
                    if (postData)
                    {
                        var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                        var middleLineAsDouble = LinearAxis.ToDouble(MovingAverage);
                        MiddleLineSeries.Points.Add(new DataPoint(dateAsDouble, middleLineAsDouble));

                        var upperChannelAsDouble = LinearAxis.ToDouble(MovingAverage + (2 * averageTrueRange));
                        UpperChannelSeries.Points.Add(new DataPoint(dateAsDouble, upperChannelAsDouble));

                        if (upperChannelAsDouble > highestVal)
                        {
                            highestVal = upperChannelAsDouble;
                        }

                        var lowerChannelsAsDouble = LinearAxis.ToDouble(MovingAverage - (2 * averageTrueRange));
                        LowerChannelSeries.Points.Add(new DataPoint(dateAsDouble, lowerChannelsAsDouble));

                        if (lowerChannelsAsDouble < lowestVal)
                        {
                            lowestVal = lowerChannelsAsDouble;
                        }
                    }
                }
                plotModel.Series.Add(MiddleLineSeries);
                plotModel.Series.Add(UpperChannelSeries);
                plotModel.Series.Add(LowerChannelSeries);
                if (lowestVal < minVal)//reset the max and/or min axis values to accomodate for upper and lower channels
                {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMinimum = lowestVal - 10000;
                }
                if (highestVal > maxVal)
                {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMaximum = highestVal + 10000;
                }
                plotModel.LegendPlacement = LegendPlacement.Inside;
                plotModel.LegendPosition = LegendPosition.BottomRight;
                //refresh plot
                plotView.InvalidatePlot(true);
                KeltnerBtn.Enabled = false;
            };

            return view;
        }

    }

    public class BrokerageHoursGraph : Android.Support.V4.App.DialogFragment {
        private List<BrokerageHoursData> dataObjects;


        //import the brokerage hours data
        public BrokerageHoursGraph(List<BrokerageHoursData> data)
        {
            dataObjects = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Brokerage Hours";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            //setup maximum and minium values for left axis
            dataObjects.Sort(delegate (BrokerageHoursData one, BrokerageHoursData two) {
                return one.GetBrokerageHours().CompareTo(two.GetBrokerageHours());
            });

            var MinHours = dataObjects.First().GetBrokerageHours();
            var MaxHours = dataObjects.Last().GetBrokerageHours();

            var maxVal = LinearAxis.ToDouble(MaxHours);
            var minVal = LinearAxis.ToDouble(MinHours);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis { Key = "currency", Position = AxisPosition.Left, AbsoluteMinimum = minVal - 100, AbsoluteMaximum = maxVal + 100, Title = "Hours", MinorStep = 10, MajorStep = 20, LabelFormatter = formatter });

            //setup maximum and minium values for date axis
            dataObjects.Sort(delegate (BrokerageHoursData one, BrokerageHoursData two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal - 1, AbsoluteMaximum = lastVal + 1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            //setup line series
            var QLDSeries = new LineSeries { MarkerStroke = OxyColors.Aqua, Title = "QLD" };
            var VICSeries = new LineSeries { MarkerStroke = OxyColors.Crimson, Title = "VIC" };
            

            foreach (BrokerageHoursData item in dataObjects)
            {
                string itemLocation = item.GetLocation();
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var amountAsDouble = LinearAxis.ToDouble(item.GetBrokerageHours());
                switch (itemLocation)
                {
                    case "QLD":
                        QLDSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case "VIC":
                        VICSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                }
            }

            plotModel.Series.Add(QLDSeries);
            plotModel.Series.Add(VICSeries);


            plotView.Model = plotModel;

            //this button when pressed will perform a calculation which will output the moving average, upper channel and lower channel onto the graph
            //info on keltner channels (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:keltner_channels"
            //info on moving averages (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:moving_averages"
            //info on average true ranges (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:average_true_range_atr"
            Button KeltnerBtn = view.FindViewById<Button>(Resource.Id.KeltnerBtn);
            KeltnerBtn.Visibility = ViewStates.Visible;
            KeltnerBtn.Enabled = true;
            KeltnerBtn.Click += delegate {
                var MiddleLineSeries = new LineSeries { Title = "Moving Average", MarkerStroke = OxyColors.Black };
                var UpperChannelSeries = new LineSeries { Title = "Upper Channel", MarkerStroke = OxyColors.ForestGreen };
                var LowerChannelSeries = new LineSeries { Title = "Lower Channel", MarkerStroke = OxyColors.DarkRed };

                int threeMonthPeriod = 0; // the period used to calculated true ranges
                int nineMonthPeriod = 0; //the period used to calculate average true range
                decimal currentHigh = dataObjects.First().GetBrokerageHours();
                decimal currentLow = dataObjects.First().GetBrokerageHours();

                decimal[] incomeOverThreeMonths = new decimal[3] { 0, 0, 0 };
                decimal[] SMAOverNineMonths = new decimal[3] { 0, 0, 0 };
                decimal[] trueRangePeriod = new decimal[3] { 0, 0, 0 };
                decimal averageTrueRange = 0;
                decimal MovingAverage = 0;
                decimal multiplier = (decimal)2 / 4;
                bool postData = false;

                int firstATR = 0;

                var highestVal = LinearAxis.ToDouble(0);
                var lowestVal = LinearAxis.ToDouble(Int32.MaxValue);

                foreach (BrokerageHoursData item in dataObjects)
                {
                    decimal itemHours = item.GetBrokerageHours();

                    postData = false;
                    //before 3 months have passed find highest and lowest hours values
                    if (threeMonthPeriod < 3)
                    {
                        if (itemHours > currentHigh)
                        {
                            currentHigh = itemHours;
                        }
                        else if (itemHours < currentLow)
                        {
                            currentLow = itemHours;
                        }
                        incomeOverThreeMonths[threeMonthPeriod] = itemHours;
                        //after 3 months have passed find average true range 
                    }
                    else
                    {
                        threeMonthPeriod = 0;
                        if (firstATR > 0)//after nine months
                        {
                            averageTrueRange = ((averageTrueRange * 2) + (currentHigh - currentLow)) / 3;
                            MovingAverage = ((itemHours - MovingAverage) * multiplier) + MovingAverage;
                            postData = true;
                        }
                        else
                        {//for the first nine months
                            trueRangePeriod[nineMonthPeriod] = currentHigh - currentLow;
                            SMAOverNineMonths[nineMonthPeriod] = (incomeOverThreeMonths.Sum()) / 3;
                            nineMonthPeriod++;
                        }
                        currentHigh = itemHours;
                        currentLow = itemHours;
                    }
                    threeMonthPeriod++;
                    if (nineMonthPeriod == 3)
                    {
                        nineMonthPeriod++;
                        firstATR++;
                        averageTrueRange = (trueRangePeriod.Sum()) / 3;
                        MovingAverage = (SMAOverNineMonths.Sum()) / 3;
                        postData = true;
                    }
                    if (postData)
                    {
                        var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                        var middleLineAsDouble = LinearAxis.ToDouble(MovingAverage);
                        MiddleLineSeries.Points.Add(new DataPoint(dateAsDouble, middleLineAsDouble));

                        var upperChannelAsDouble = LinearAxis.ToDouble(MovingAverage + (2 * averageTrueRange));
                        UpperChannelSeries.Points.Add(new DataPoint(dateAsDouble, upperChannelAsDouble));

                        if (upperChannelAsDouble > highestVal)
                        {
                            highestVal = upperChannelAsDouble;
                        }

                        var lowerChannelsAsDouble = LinearAxis.ToDouble(MovingAverage - (2 * averageTrueRange));
                        LowerChannelSeries.Points.Add(new DataPoint(dateAsDouble, lowerChannelsAsDouble));

                        if (lowerChannelsAsDouble < lowestVal)
                        {
                            lowestVal = lowerChannelsAsDouble;
                        }
                    }
                }
                plotModel.Series.Add(MiddleLineSeries);
                plotModel.Series.Add(UpperChannelSeries);
                plotModel.Series.Add(LowerChannelSeries);
                if (lowestVal < minVal)//reset the max and/or min axis values to accomodate for upper and lower channels
                {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMinimum = lowestVal - 10000;
                }
                if (highestVal > maxVal)
                {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMaximum = highestVal + 10000;
                }
                plotModel.LegendPlacement = LegendPlacement.Inside;
                plotModel.LegendPosition = LegendPosition.BottomRight;
                //refresh plot
                plotView.InvalidatePlot(true);
                KeltnerBtn.Enabled = false;
            };

            return view;
        }

    }

    public class HomeCarePackagesGraph : Android.Support.V4.App.DialogFragment {


        private decimal AvondrustVillageTotal = 0;
        private decimal MargrietManorTotal = 0;
        private decimal OverbeekLodgeTotal = 0;
        private decimal PrinsWillemAlexanderLodgeTotal = 0;


        //import the home care package data and get the total income per facility
        public HomeCarePackagesGraph(List<HomeCarePackageData> data) {
            foreach (HomeCarePackageData item in data) {
                int itemFacility = item.GetFacilityID();
                switch (itemFacility)
                {
                    case 1:
                        AvondrustVillageTotal = AvondrustVillageTotal + item.GetPackageIncome();
                        break;
                    case 2:
                        MargrietManorTotal = MargrietManorTotal + item.GetPackageIncome();
                        break;
                    case 3:
                        OverbeekLodgeTotal = OverbeekLodgeTotal + item.GetPackageIncome();
                        break;
                    case 4:
                        PrinsWillemAlexanderLodgeTotal = PrinsWillemAlexanderLodgeTotal + item.GetPackageIncome();
                        break;
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Home Care Packages Total Across MiCare";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            //setup vertical bar graph
            var barSeries = new ColumnSeries
            {
                ItemsSource = new List<ColumnItem>(new[]
    {
                    new ColumnItem{ Value = (double) AvondrustVillageTotal},
                    new ColumnItem{ Value = (double) MargrietManorTotal},
                    new ColumnItem{ Value = (double) OverbeekLodgeTotal},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeTotal}
                }),
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0}",
                FillColor = OxyColors.LightSeaGreen
            };

            plotModel.Series.Add(barSeries);

            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = new[] {
                    "",
                    "",
                    "",
                    ""
                }
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                Title = "Amount ($)"
            });

            plotView.Model = plotModel;

            //put names of facilities visible 
            LinearLayout BarGraphLables = view.FindViewById<LinearLayout>(Resource.Id.BarGraphLabels);
            BarGraphLables.Visibility = ViewStates.Visible;

            return view;
        }

    }

    public class IncomeGraph : Android.Support.V4.App.DialogFragment
    {
        private List<IncomeData> dataObjects;


        //import the income data
        public IncomeGraph(List<IncomeData> data)
        {
            dataObjects = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Income";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            dataObjects.Sort(delegate (IncomeData one, IncomeData two) {
                return one.GetIncome().CompareTo(two.GetIncome());
            });
            //setup maximum and minium values for left axis
            var MinIncome = dataObjects.First().GetIncome();
            var MaxIncome = dataObjects.Last().GetIncome();

            var maxVal = LinearAxis.ToDouble(MaxIncome);
            var minVal = LinearAxis.ToDouble(MinIncome);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis { Key = "currency", Position = AxisPosition.Left, AbsoluteMinimum = minVal - 10000, AbsoluteMaximum = maxVal + 10000, Title = "Income ($)", MinorStep = 50000, MajorStep = 100000, LabelFormatter = formatter });

            //setup maximum and minium values for date axis
            dataObjects.Sort(delegate (IncomeData one, IncomeData two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal - 1, AbsoluteMaximum = lastVal + 1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            //setup line series
            var incomeSeries = new LineSeries { Title = "Income", MarkerStroke = OxyColors.Aqua };

            foreach (IncomeData item in dataObjects) {
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var incomeAsDouble = LinearAxis.ToDouble(item.GetIncome());
                incomeSeries.Points.Add(new DataPoint(dateAsDouble, incomeAsDouble));
            }

            plotModel.Series.Add(incomeSeries);

            plotView.Model = plotModel;

            //this button when pressed will perform a calculation which will output the moving average, upper channel and lower channel onto the graph
            //info on keltner channels (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:keltner_channels"
            //info on moving averages (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:moving_averages"
            //info on average true ranges (including the formula): "https:/stockcharts.com/school/doku.php?id=chart_school:technical_indicators:average_true_range_atr"
            Button KeltnerBtn = view.FindViewById<Button>(Resource.Id.KeltnerBtn);
            KeltnerBtn.Visibility = ViewStates.Visible;
            KeltnerBtn.Enabled = true;
            KeltnerBtn.Click += delegate {
                var MiddleLineSeries = new LineSeries { Title = "Moving Average", MarkerStroke = OxyColors.Black };
                var UpperChannelSeries = new LineSeries { Title = "Upper Channel", MarkerStroke = OxyColors.ForestGreen };
                var LowerChannelSeries = new LineSeries { Title = "Lower Channel", MarkerStroke = OxyColors.DarkRed };

                int threeMonthPeriod = 0; // the period used to calculated true ranges
                int nineMonthPeriod = 0; //the period used to calculate average true range
                decimal currentHigh = dataObjects.First().GetIncome();
                decimal currentLow = dataObjects.First().GetIncome();

                decimal[] incomeOverThreeMonths = new decimal[3] { 0, 0, 0 };
                decimal[] SMAOverNineMonths = new decimal[3] { 0, 0, 0 };
                decimal[] trueRangePeriod = new decimal[3] { 0, 0, 0 };
                decimal averageTrueRange = 0;
                decimal MovingAverage = 0;
                decimal multiplier = (decimal) 2 / 4;
                bool postData = false;

                int firstATR = 0;

                var highestVal = LinearAxis.ToDouble(0); 
                var lowestVal = LinearAxis.ToDouble(Int32.MaxValue);

                foreach (IncomeData item in dataObjects) {
                    decimal itemIncome = item.GetIncome();
                    
                    postData = false;
                    //before 3 months have passed find highest and lowest income values
                    if (threeMonthPeriod < 3) {
                        if (itemIncome > currentHigh) {
                            currentHigh = itemIncome;
                        } else if (itemIncome < currentLow) {
                            currentLow = itemIncome;
                        }
                        incomeOverThreeMonths[threeMonthPeriod] = itemIncome;
                        //after 3 months have passed find average true range 
                    } else {
                        threeMonthPeriod = 0;
                        if (firstATR > 0) {//after nine months
                            averageTrueRange = ((averageTrueRange*2) + (currentHigh - currentLow)) / 3;
                            MovingAverage = ((itemIncome - MovingAverage) * multiplier) + MovingAverage;
                            postData = true;
                        } else {//for the first nine months
                            trueRangePeriod[nineMonthPeriod] = currentHigh - currentLow;
                            SMAOverNineMonths[nineMonthPeriod] = (incomeOverThreeMonths.Sum()) / 3;
                            nineMonthPeriod++;
                        }
                        currentHigh = itemIncome;
                        currentLow = itemIncome;
                    }
                    threeMonthPeriod++;
                    if (nineMonthPeriod == 3) {
                        nineMonthPeriod++;
                        firstATR++;
                        averageTrueRange = (trueRangePeriod.Sum()) / 3;
                        MovingAverage = (SMAOverNineMonths.Sum()) / 3;
                        postData = true;
                    }
                    if (postData) {
                        var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                        var middleLineAsDouble = LinearAxis.ToDouble(MovingAverage);
                        MiddleLineSeries.Points.Add(new DataPoint(dateAsDouble, middleLineAsDouble));

                        var upperChannelAsDouble = LinearAxis.ToDouble(MovingAverage + (2 * averageTrueRange));
                        UpperChannelSeries.Points.Add(new DataPoint(dateAsDouble, upperChannelAsDouble));

                        if (upperChannelAsDouble > highestVal) {
                            highestVal = upperChannelAsDouble;
                        }

                        var lowerChannelsAsDouble = LinearAxis.ToDouble(MovingAverage - (2 * averageTrueRange));
                        LowerChannelSeries.Points.Add(new DataPoint(dateAsDouble, lowerChannelsAsDouble));

                        if (lowerChannelsAsDouble < lowestVal) {
                            lowestVal = lowerChannelsAsDouble;
                        }
                    }
                }
                plotModel.Series.Add(MiddleLineSeries);
                plotModel.Series.Add(UpperChannelSeries);
                plotModel.Series.Add(LowerChannelSeries);
                if (lowestVal < minVal)
                {//reset the max and/or min axis values to accomodate for upper and lower channels
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMinimum = lowestVal - 10000;
                }
                if (highestVal > maxVal) {
                    plotModel.GetAxisOrDefault("currency", null).AbsoluteMaximum = highestVal + 10000;
                }
                plotModel.LegendPlacement = LegendPlacement.Inside;
                plotModel.LegendPosition = LegendPosition.BottomRight;
                //refresh plot
                plotView.InvalidatePlot(true);
                KeltnerBtn.Enabled = false;
            };

            return view;
        }

    }

    public class OccupancyGraph : Android.Support.V4.App.DialogFragment
    {
        private List<OccupancyData> dataObjects;
        private int facilityNum;

        //import the finance data item based on the item clicked 
        public OccupancyGraph(List<OccupancyData> data, int facility) {
            dataObjects = data;
            facilityNum = facility;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Occupancy";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel();

            //setup maximum and minium values for left axis
            dataObjects.Sort(delegate (OccupancyData one,OccupancyData two) {
                return one.GetOccupancyRate().CompareTo(two.GetOccupancyRate());
            });
            
            var MinIncome = dataObjects.First().GetOccupancyRate();
            var MaxIncome = dataObjects.Last().GetOccupancyRate();

            var maxVal = LinearAxis.ToDouble(MaxIncome);
            var minVal = LinearAxis.ToDouble(MinIncome);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis { Key = "currency", Position = AxisPosition.Left, AbsoluteMinimum = minVal - 0.05, AbsoluteMaximum = maxVal + 0.05, Title = "Occupancy Rate (%)", MinorStep = 0.005, MajorStep = 0.01, LabelFormatter = formatter });


            //setup maximum and minium values for date axis
            dataObjects.Sort(delegate (OccupancyData one, OccupancyData two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal - 1, AbsoluteMaximum = lastVal + 1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            //set up the different line series
            var AvondrustVillageSeries = new LineSeries { MarkerStroke = OxyColors.Aqua, Title = "Avondrust Village" };
            var MargrietManorSeries = new LineSeries { MarkerStroke = OxyColors.Crimson, Title = "Margriet Manor" };
            var OverbeekLodgeSeries = new LineSeries { MarkerStroke = OxyColors.Orange, Title = "Overbeek Lodge" };
            var PrinsWillemAlexanderLodgeSeries = new LineSeries { MarkerStroke = OxyColors.DimGray, Title = "Prins Willem Alexander Lodge" };

            foreach (OccupancyData item in dataObjects)
            {
                int itemFacility = item.GetFacilityID();
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var amountAsDouble = LinearAxis.ToDouble(item.GetOccupancyRate());
                switch (itemFacility)
                {
                    case 1:
                        AvondrustVillageSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 2:
                        MargrietManorSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 3:
                        OverbeekLodgeSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                    case 4:
                        PrinsWillemAlexanderLodgeSeries.Points.Add(new DataPoint(dateAsDouble, amountAsDouble));
                        break;
                }
            }

            switch (facilityNum)
            {
                case 1:
                    plotModel.Series.Add(AvondrustVillageSeries);
                    break;
                case 2:
                    plotModel.Series.Add(MargrietManorSeries);
                    break;
                case 3:
                    plotModel.Series.Add(OverbeekLodgeSeries);
                    break;
                case 4:
                    plotModel.Series.Add(PrinsWillemAlexanderLodgeSeries);
                    break;
            }


            plotView.Model = plotModel;

            //Originally the occupancy graph was going to have a keltner channel button as well, however occupancy rate generally changes at such a slow rate, and with our lack of data
            //it didn't make much sense to implement

            return view;
        }

    }

    public class SalariesWagesGraph : Android.Support.V4.App.DialogFragment
    {
        private decimal AvondrustLodgeActualCost = 0;
        private decimal AvondrustLodgeBudget = 0;

        private decimal MargrietManorActualCost = 0;
        private decimal MargrietManorBudget = 0;

        private decimal OverbeekLodgeActualCost = 0;
        private decimal OverbeekLodgeBudget = 0;

        private decimal PrinsWillemAlexanderLodgeActualCost = 0;
        private decimal PrinsWillemAlexanderLodgeBudget = 0;


        //import the salaries and wages data and get the actual cost and budget for each facility
        public SalariesWagesGraph(List<SalariesWagesData> data) {
            List<SalariesWagesData> dataObjects = data;
            dataObjects.Sort(delegate (SalariesWagesData one, SalariesWagesData two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            List<SalariesWagesData> dataObjectsAvondrust = new List<SalariesWagesData>();
            List<SalariesWagesData> dataObjectsMargriet = new List<SalariesWagesData>();
            List<SalariesWagesData> dataObjectsOverbeek = new List<SalariesWagesData>();
            List<SalariesWagesData> dataObjectsPrins = new List<SalariesWagesData>();

            foreach (SalariesWagesData item in dataObjects) {
                int itemFacility = item.GetFacilityID();
                switch (itemFacility)
                {
                    case 1:
                        dataObjectsAvondrust.Add(item);
                        break;
                    case 2:
                        dataObjectsMargriet.Add(item);
                        break;
                    case 3:
                        dataObjectsOverbeek.Add(item);
                        break;
                    case 4:
                        dataObjectsPrins.Add(item);
                        break;
                }
            }
            
            AvondrustLodgeActualCost = dataObjectsAvondrust.Last().GetActualCost();
            AvondrustLodgeBudget = dataObjectsAvondrust.Last().GetBudget();

            MargrietManorActualCost = dataObjectsMargriet.Last().GetActualCost();
            MargrietManorBudget = dataObjectsMargriet.Last().GetBudget();

            OverbeekLodgeActualCost = dataObjectsOverbeek.Last().GetActualCost();
            OverbeekLodgeBudget = dataObjectsOverbeek.Last().GetBudget();

            PrinsWillemAlexanderLodgeActualCost = dataObjectsPrins.Last().GetActualCost();
            PrinsWillemAlexanderLodgeBudget = dataObjectsPrins.Last().GetBudget();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Current Salaries and Wages - Budget vs Cost";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel() { LegendPlacement = LegendPlacement.Outside, LegendPosition = LegendPosition.TopCenter };

            //setup vertical bar graphs
            var ActualCostBarSeries = new ColumnSeries
            {
                Title = "Actual Cost",
                ItemsSource = new List<ColumnItem>(new[]
{
                    new ColumnItem{ Value = (double) AvondrustLodgeActualCost},
                    new ColumnItem{ Value = (double) MargrietManorActualCost},
                    new ColumnItem{ Value = (double) OverbeekLodgeActualCost},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeActualCost}
                }),
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N2}",
                FillColor = OxyColors.DarkBlue
            };

            var BudgetBarSeries = new ColumnSeries
            {
                Title = "Budget",
                ItemsSource = new List<ColumnItem>(new[]
{
                    new ColumnItem{ Value = (double) AvondrustLodgeBudget},
                    new ColumnItem{ Value = (double) MargrietManorBudget},
                    new ColumnItem{ Value = (double) OverbeekLodgeBudget},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeBudget}
                }),
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N2}",
                FillColor = OxyColors.OrangeRed
            };

            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = new[] {
                    "",
                    "",
                    "",
                    ""
                }
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                Title = "Amount ($)"
            });

            plotModel.Series.Add(ActualCostBarSeries);
            plotModel.Series.Add(BudgetBarSeries);

            plotView.Model = plotModel;

            //make facility names visible
            LinearLayout BarGraphLables = view.FindViewById<LinearLayout>(Resource.Id.BarGraphLabels);
            BarGraphLables.Visibility = ViewStates.Visible;

            return view;
        }

    }

    public class StaffGraph : Android.Support.V4.App.DialogFragment {

        private decimal AvondrustLodgeALAverage = 0;
        private decimal AvondrustLodgeLSLAverage = 0;
        private decimal AvondrustLodgeSLAverage = 0;

        private decimal MargrietManorALAverage = 0;
        private decimal MargrietManorLSLAverage = 0;
        private decimal MargrietManorSLAverage = 0;

        private decimal OverbeekLodgeALAverage = 0;
        private decimal OverbeekLodgeLSLAverage = 0;
        private decimal OverbeekLodgeSLAverage = 0;

        private decimal PrinsWillemAlexanderLodgeALAverage = 0;
        private decimal PrinsWillemAlexanderLodgeLSLAverage = 0;
        private decimal PrinsWillemAlexanderLodgeSLAverage = 0;

        //import the staff data and retrieve the average annual leave, long serve leave, and sick leave accrued for each facility
        public StaffGraph(List<StaffData> data)
        {
            int AvondrustCount = 0;
            int MargrietCount = 0;
            int OverbeekCount = 0;
            int PrinsCount = 0;
            foreach (StaffData item in data)
            {
                int itemFacility = item.GetFacilityID();
                switch (itemFacility)
                {
                    case 1:
                        AvondrustLodgeALAverage = AvondrustLodgeALAverage + item.GetAnnualLeaveAcrewed();
                        AvondrustLodgeLSLAverage = AvondrustLodgeLSLAverage + item.GetLongServiceLeaveAcrewed();
                        AvondrustLodgeSLAverage = AvondrustLodgeSLAverage + item.GetSickLeaveAcrewed();
                        AvondrustCount++;
                        break;
                    case 2:
                        MargrietManorALAverage = MargrietManorALAverage + item.GetAnnualLeaveAcrewed();
                        MargrietManorLSLAverage = MargrietManorLSLAverage + item.GetLongServiceLeaveAcrewed();
                        MargrietManorSLAverage = MargrietManorSLAverage + item.GetSickLeaveAcrewed();
                        MargrietCount++;
                        break;
                    case 3:
                        OverbeekLodgeALAverage = OverbeekLodgeALAverage + item.GetAnnualLeaveAcrewed();
                        OverbeekLodgeLSLAverage = OverbeekLodgeLSLAverage + item.GetLongServiceLeaveAcrewed();
                        OverbeekLodgeSLAverage = OverbeekLodgeSLAverage + item.GetSickLeaveAcrewed();
                        OverbeekCount++;
                        break;
                    case 4:
                        PrinsWillemAlexanderLodgeALAverage = PrinsWillemAlexanderLodgeALAverage + item.GetAnnualLeaveAcrewed();
                        PrinsWillemAlexanderLodgeLSLAverage = PrinsWillemAlexanderLodgeLSLAverage + item.GetLongServiceLeaveAcrewed();
                        PrinsWillemAlexanderLodgeSLAverage = PrinsWillemAlexanderLodgeSLAverage + item.GetSickLeaveAcrewed();
                        PrinsCount++;
                        break;
                }
            }
            AvondrustLodgeALAverage = AvondrustLodgeALAverage / AvondrustCount;
            AvondrustLodgeLSLAverage = AvondrustLodgeLSLAverage / AvondrustCount;
            AvondrustLodgeSLAverage = AvondrustLodgeSLAverage / AvondrustCount;

            MargrietManorALAverage = MargrietManorALAverage / MargrietCount;
            MargrietManorLSLAverage = MargrietManorLSLAverage / MargrietCount;
            MargrietManorSLAverage = MargrietManorSLAverage / MargrietCount;

            OverbeekLodgeALAverage = OverbeekLodgeALAverage / OverbeekCount;
            OverbeekLodgeLSLAverage = OverbeekLodgeLSLAverage / OverbeekCount;
            OverbeekLodgeSLAverage = OverbeekLodgeSLAverage / OverbeekCount;

            PrinsWillemAlexanderLodgeALAverage = PrinsWillemAlexanderLodgeALAverage / PrinsCount;
            PrinsWillemAlexanderLodgeLSLAverage = PrinsWillemAlexanderLodgeLSLAverage / PrinsCount;
            PrinsWillemAlexanderLodgeSLAverage = PrinsWillemAlexanderLodgeSLAverage / PrinsCount;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            int width = (Resources.DisplayMetrics.WidthPixels) + 150;
            this.Dialog.Window.Attributes.Width = width;

            var view = inflater.Inflate(Resource.Layout.Graph, container, false);

            global::Xamarin.Forms.Forms.Init(this.Context, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            TextView titleText = view.FindViewById<TextView>(Resource.Id.GraphTitle);
            titleText.Text = "Staff Attendance";

            PlotView plotView = view.FindViewById<PlotView>(Resource.Id.GraphPlotView);

            var plotModel = new PlotModel() { LegendPlacement = LegendPlacement.Outside, LegendPosition = LegendPosition.TopCenter};

            //setup vertical bar graphs
            var ALBarSeries = new ColumnSeries {
                Title = "Average Annual Leave Accrued",
                ItemsSource = new List<ColumnItem>(new[]
{
                    new ColumnItem{ Value = (double) AvondrustLodgeALAverage},
                    new ColumnItem{ Value = (double) MargrietManorALAverage},
                    new ColumnItem{ Value = (double) OverbeekLodgeALAverage},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeALAverage}
                }),
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N2}",
                FillColor = OxyColors.LightSeaGreen
            };

            var LSLBarSeries = new ColumnSeries {
                Title = "Average Long Service Leave Accrued",
                ItemsSource = new List<ColumnItem>(new[]
{
                    new ColumnItem{ Value = (double) AvondrustLodgeLSLAverage},
                    new ColumnItem{ Value = (double) MargrietManorLSLAverage},
                    new ColumnItem{ Value = (double) OverbeekLodgeLSLAverage},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeLSLAverage}
                }),
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N2}",
                FillColor = OxyColors.DarkBlue
            };

            var SLBarSeries = new ColumnSeries {
                Title = "Average Sick Leave Accrued",
                ItemsSource = new List<ColumnItem>(new[]
{
                    new ColumnItem{ Value = (double) AvondrustLodgeSLAverage},
                    new ColumnItem{ Value = (double) MargrietManorSLAverage},
                    new ColumnItem{ Value = (double) OverbeekLodgeSLAverage},
                    new ColumnItem{ Value = (double) PrinsWillemAlexanderLodgeSLAverage}
                }),
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0:N2}",
                FillColor = OxyColors.OrangeRed
            };

            plotModel.Axes.Add(new CategoryAxis {
                Position = AxisPosition.Bottom,
                ItemsSource = new[] {
                    "",
                    "",
                    "",
                    ""
                }
            });

            plotModel.Axes.Add(new LinearAxis {
                Position = AxisPosition.Left,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                Title = "Leave"
            });

            plotModel.Series.Add(ALBarSeries);
            plotModel.Series.Add(LSLBarSeries);
            plotModel.Series.Add(SLBarSeries);

            plotView.Model = plotModel;

            //make facility names visible
            LinearLayout BarGraphLables = view.FindViewById<LinearLayout>(Resource.Id.BarGraphLabels);
            BarGraphLables.Visibility = ViewStates.Visible;

            return view;
        }

    }
}