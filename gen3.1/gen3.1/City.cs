using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen3._1
{
    public class City
    {
        private static int counter = 0;
        public ScatterSeries Point { get; }
        public double X { get; set; }
        public double Y { get; set; }
        public int number { get; }

        public City() { }

        public City(City c)
        {
            this.X = c.X;
            this.Y = c.Y;
            this.number = c.number;

            this.Point = new ScatterSeries();
            this.Point.ColorAxisKey = "ColorAxis";
            this.Point.MarkerSize = 8;
            this.Point.MarkerType = MarkerType.Plus;

            Point.Points.Add(new ScatterPoint(this.X, this.Y, double.NaN, 1));
        }

        public City (double X, double Y, int number)
        {
            this.X = X;
            this.Y = Y;
            this.number = number;

            this.Point = new ScatterSeries();
            this.Point.ColorAxisKey = "ColorAxis";
            this.Point.MarkerSize = 8;
            this.Point.MarkerType = MarkerType.Plus;

            Point.Points.Add(new ScatterPoint(this.X, this.Y, double.NaN, 1));

        }

        public double GetRangeAt(City c)
        {
            return Math.Sqrt(Math.Pow((c.X - X), 2) + Math.Pow((c.Y - Y), 2));
        }

        static double getRange(City c1, City c2)
        {
            return Math.Sqrt(Math.Pow((c1.X - c2.X), 2) + Math.Pow((c1.Y - c2.Y), 2) );
        }
    }
}
