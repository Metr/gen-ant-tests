using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gen6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PlotModel model;
        private List<Bot> bots;
        private Label[] labels;

        public Random random;
        private int sizeMap = 200;
        public double startPoint = -5, endPoint = 5;
        private int iterations = 1;
        private int currentIteration = 0;
        private int botCount = 16;

        public MainWindow()
        {
            InitializeComponent();
            random = new Random();
            InitGUI();

            initHeatMap();
            initBots(botCount);
        }

        private void MainProc()
        {
            double iter = double.MinValue, min = double.MaxValue, tmp;
            int index = 0;

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < bots.Count; j++)
                {
                    bots[j].doNextStep();
                }
                currentIteration++;
            }

            for (int i = 0; i < labels.Length; i++)
            {
                min = double.MaxValue;
                for (int j = 0; j < bots.Count; j++)
                {
                    tmp = bots[j].Value;
                    if (tmp < min && tmp > iter)
                    {
                        index = j;
                        min = tmp;
                    }
                }
                iter = min;
                labels[i].Content = bots[index].getInfo();
            }

            updateHeatMap();
            labelIterarions.Content = "Итераций: " + currentIteration;
        }


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>

        private void initHeatMap()
        {
            model = new PlotModel { Title = "Heatmap" };
            model.Axes.Add(new LinearColorAxis { Palette = OxyPalettes.Viridis(255) });

            var data = new double[sizeMap, sizeMap];
            double xValue = startPoint, yValue = startPoint, max = endPoint;
            double det = (Math.Abs(xValue) + max) / sizeMap;

            //for (int x = 0; x < sizeMap; ++x)
            //{
            //    yValue = startPoint;
            //    for (int y = 0; y < sizeMap; ++y)
            //    {
            //        data[y, x] = getFunction(xValue, yValue);
            //        yValue += det;
            //    }
            //    xValue += det;
            //}

            for (int y = 0; y < sizeMap; ++y)
            {
                xValue = startPoint;
                for (int x = 0; x < sizeMap; ++x)
                {
                    data[x, y] = getFunction(xValue, yValue);
                    xValue += det;
                }
                yValue += det;
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = startPoint,
                X1 = endPoint,
                Y0 = startPoint,
                Y1 = endPoint,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = data
            };

            var axis1 = new LinearColorAxis();
            axis1.Key = "ColorAxis";
            axis1.Maximum = 1;
            axis1.Minimum = 0;
            axis1.IsAxisVisible = false;
            model.Axes.Add(axis1);

            axis1.Palette.Colors.Clear();
            axis1.Palette.Colors.Add(OxyColor.FromArgb((byte)255, 255, 0, 0));

            model.Series.Add(heatMapSeries);

        }

        private void initBots(int count)
        {
            bots = new List<Bot>();

            for (int i = 0; i < count; i++)
            {
                bots.Add(new Bot(i, startPoint + random.NextDouble() * Math.Abs(startPoint - endPoint), startPoint + random.NextDouble() * Math.Abs(startPoint - endPoint), this));
                model.Series.Add(bots[i].Point);
            }

            testPlotView.Model = model;
        }

        public double getFunction(double x, double y)
        {
            double sum = 0;
            for (int i = 0; i < sizeMap; i++)
            {
                sum += 5 * i;
            }
            return sum * (x * x);

        }

        public void updateHeatMap()
        {
            while (model.Series.Count > 1)
            {
                model.Series.Remove(model.Series.Last());
            }

            for (int i = 0; i < bots.Count; i++)
            {
                model.Series.Add(bots[i].Point);
            }

            testPlotView.Model.InvalidatePlot(true);
        }

        private void InitGUI()
        {
            labels = new Label[5];
            labels[0] = topValue1;
            labels[1] = topValue2;
            labels[2] = topValue3;
            labels[3] = topValue4;
            labels[4] = topValue5;

            inputBots.Text = botCount.ToString();
            inputSteps.Text = sizeMap.ToString();
        }

        private void LockChanges()
        {
            if (acceptButton.IsEnabled)
            {
                inputBots.IsEnabled = false;
                inputSteps.IsEnabled = false;
                acceptButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////
        /// </summary>

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            //Bot bot = new Bot(0, 2, 4, this);
            //bots.Add(bot);
            //Bot bot2 = new Bot(0, 4, 3, this);
            //bots.Add(bot2);
            //updateHeatMap();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iterations = 1;
            LockChanges();
            MainProc();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            iterations = 10;
            LockChanges();
            MainProc();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            iterations = 100;
            LockChanges();
            MainProc();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //step = Math.Abs(startPoint - endPoint) / steps;

            inputBots.IsEnabled = true;
            inputSteps.IsEnabled = true;
            acceptButton.IsEnabled = true;
            labelIterarions.Content = "Итераций: ";
            currentIteration = 0;

            InitGUI();
            initHeatMap();
            initBots(botCount);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            double Dbuff;
            int Ibuff;

            if (int.TryParse(inputBots.Text, out Ibuff))
            {
                if (Ibuff > 0)
                {
                    botCount = Ibuff;
                    Ibuff = 0;
                }
            }

            if (int.TryParse(inputSteps.Text, out Ibuff))
            {
                if (Ibuff > 0)
                {
                    sizeMap = Ibuff;
                    Ibuff = 0;
                }
            }

            InitGUI();
            initHeatMap();
            initBots(botCount);
        }



    }

}



