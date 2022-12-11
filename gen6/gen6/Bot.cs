using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gen6
{
    public class Bot
    {

        public static double measure = 0.01;
        public static Point globalBestPoint;
        public static double globalBestValue;

        private Point botBestPoint;
        private Point inertionPoint;
        private Point directionPoint;

        private string name;
        private int number;
        private double xTrue;
        private double yTrue;
        private double value;
        private double bestValue;

        private ScatterSeries point;


        private MainWindow mainWindow;

        static Bot()
        {
            globalBestValue = double.MaxValue;
        }

        public Bot(int number, double xTrue, double yTrue, MainWindow mainWindow)
        {
            this.name = "bot_" + number.ToString();
            this.number = number;
            this.xTrue = xTrue;
            this.yTrue = yTrue;
            value = mainWindow.getFunction(xTrue, yTrue);
            this.mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));

            point = new ScatterSeries();
            point.ColorAxisKey = "ColorAxis";
            point.MarkerSize = 10;
            point.MarkerType = MarkerType.Plus;
            point.Points.Add(new ScatterPoint(XTrue, YTrue, double.NaN, 1));

            directionPoint.X = -0.5 + mainWindow.random.NextDouble();
            directionPoint.Y = -0.5 + mainWindow.random.NextDouble();

            bestValue = double.MaxValue;
        }

        public void doNextStep()
        {
            inertionPoint.X = directionPoint.X / 2;
            inertionPoint.Y = directionPoint.Y / 2;

            directionPoint.X = (5 * measure) * (-0.5 + mainWindow.random.NextDouble());
            directionPoint.Y = (5 * measure) * (-0.5 + mainWindow.random.NextDouble());

            botBestPoint.X = (1.0 * measure) * (botBestPoint.X - xTrue);
            botBestPoint.Y = (1.0 * measure) * (botBestPoint.Y - yTrue);

            globalBestPoint.X = (1.5 * measure) * (botBestPoint.X - xTrue);
            globalBestPoint.Y = (1.5 * measure) * (botBestPoint.Y - yTrue);

            XTrue += directionPoint.X + inertionPoint.X + botBestPoint.X + globalBestPoint.X;
            YTrue += directionPoint.Y + inertionPoint.Y + botBestPoint.Y + globalBestPoint.Y;

            if (value <= bestValue)
            {
                bestValue = value;
                botBestPoint.X = xTrue;
                botBestPoint.Y = yTrue;
            }
            if (value <= globalBestValue)
            {
                globalBestValue = value;
                globalBestPoint.X = xTrue;
                globalBestPoint.Y = yTrue;
            }
        }


        public string Name { get => name; set => name = value; }
        public double XTrue
        {
            get => xTrue;
            set
            {
                if (value <= mainWindow.endPoint && value >= mainWindow.startPoint)
                    xTrue = value;
                else if (value < mainWindow.startPoint)
                    xTrue = mainWindow.startPoint;
                else if (value > mainWindow.endPoint)
                    xTrue = mainWindow.endPoint;
                this.value = mainWindow.getFunction(xTrue, yTrue);
            }
        }
        public double YTrue
        {
            get => yTrue;
            set
            {
                if (value <= mainWindow.endPoint && value >= mainWindow.startPoint)
                    yTrue = value;
                else if (value < mainWindow.startPoint)
                    yTrue = mainWindow.startPoint;
                else if (value > mainWindow.endPoint)
                    yTrue = mainWindow.endPoint;
                this.value = mainWindow.getFunction(xTrue, yTrue);
            }
        }

        public double Value
        {
            get
            {
                value = mainWindow.getFunction(xTrue, yTrue);
                return value;
            }

        }

        public ScatterSeries Point
        {
            get
            {
                this.point = new ScatterSeries();
                this.point.ColorAxisKey = "ColorAxis";
                this.point.MarkerSize = 8;
                this.point.MarkerType = MarkerType.Plus;

                point.Points.Add(new ScatterPoint(XTrue, YTrue, double.NaN, 1));
                return point;
            }
            set => point = value;
        }
        public int Number { get => number; set => number = value; }

        public string getInfo()
        {
            return string.Format("name = {0}, xTrue = {1:f4}, yTrue = {2:f4}, value = {3:f4}", name, xTrue, yTrue, value); //name + " xTrue = " + xTrue + "\t yTrue = " + yTrue + "\t value = " + value;
        }
    }
}
