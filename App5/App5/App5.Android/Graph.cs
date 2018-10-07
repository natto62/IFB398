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
using MiCareApp.Droid;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace App5.Droid
{
    public class BankBalanceGraph : Android.Support.V4.App.DialogFragment
    {
        private List<BankBalance> dataObjects;


        //import the finance data item based on the item clicked 
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

            dataObjects.Sort(delegate (BankBalance one, BankBalance two) {
                return one.GetBankBalance().CompareTo(two.GetBankBalance());
            });
            //bug: reset button after changing fragments
            var MaxBalance = dataObjects.First().GetBankBalance();
            var MinBalance = dataObjects.Last().GetBankBalance();

            var maxVal = LinearAxis.ToDouble(MinBalance);
            var minVal = LinearAxis.ToDouble(MaxBalance);

            string formatter(double item)
            {
                return String.Format("{0}", item);
            }

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, AbsoluteMinimum = minVal-10000, AbsoluteMaximum = maxVal+10000, Title = "Bank Balance ($)" , MinorStep = 10000 , MajorStep = 50000, LabelFormatter = formatter});


            dataObjects.Sort(delegate (BankBalance one, BankBalance two) {
                return DateTime.Compare(one.GetDate(), two.GetDate());
            });

            var startDate = dataObjects.First().GetDate();
            var endDate = dataObjects.Last().GetDate();

            var firstVal = DateTimeAxis.ToDouble(startDate);
            var lastVal = DateTimeAxis.ToDouble(endDate);

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, AbsoluteMinimum = firstVal-1, AbsoluteMaximum = lastVal+1, MinorStep = 1, StringFormat = "MM/dd/yyyy", Title = "Date (M/D/Y)" });

            var BankBalanceSeries = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerStroke = OxyColors.Aqua };

            foreach (BankBalance item in dataObjects) {
                var dateAsDouble = DateTimeAxis.ToDouble(item.GetDate());
                var balanceAsDouble = LinearAxis.ToDouble(item.GetBankBalance());
                BankBalanceSeries.Points.Add(new DataPoint(dateAsDouble, balanceAsDouble));
            }

            plotModel.Series.Add(BankBalanceSeries);

            plotView.Model = plotModel;

            return view;
        }

    }
}