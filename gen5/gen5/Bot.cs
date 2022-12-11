using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen5
{
    public class Bot
    {
        private string name;
        private int number;
        private double xTrue;
        private double yTrue;
        private double value;
        private ScatterSeries point;

        private MainWindow mainWindow;

        public Bot(int number, double xTrue, double yTrue, MainWindow mainWindow, bool isChild = false)
        {
            this.name = "bot_" + number.ToString();
            this.number = number;
            this.xTrue = xTrue;
            this.yTrue = yTrue;
            value = mainWindow.getFunction(xTrue, yTrue);
            this.mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));

            this.point = new ScatterSeries();
            this.point.ColorAxisKey = "ColorAxis";
            if(isChild)
            {
                this.point.MarkerSize = 6;
                this.point.MarkerType = MarkerType.Triangle;
            }
                else
            {
                this.point.MarkerSize = 10;
                this.point.MarkerType = MarkerType.Plus;
            }

            point.Points.Add(new ScatterPoint(XTrue, YTrue, double.NaN, 1));
        }


        public Bot getChild()
        {
            double detX = -0.5 + mainWindow.random.NextDouble();
            double detY = -0.5 + mainWindow.random.NextDouble();
            double buffX = 0, buffY = 0;

            if (XTrue + detX <= -5)
                buffX = -5;
            if (XTrue + detX >= 5)
                buffX = 5;
            if (YTrue + detY <= -5)
                buffY = -5;
            if (YTrue + detY >= 5)
                buffY = 5;

            if (buffX == 0)
                detX = XTrue + detX;
            else
                detX = buffX;

            if (buffY == 0)
                detY = YTrue + detY;
            else
                detY = buffY;

            Bot child = new Bot(number + 1, detX, detY , mainWindow, true);

            return child;

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
