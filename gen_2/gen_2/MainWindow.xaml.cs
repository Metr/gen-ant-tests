using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace gen_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PlotModel model;
        private List<Bot> bots;
        private Label[] labels;

        private int sizeMap = 200;
        private double chanceCrossingover = 0.1, chanceMutation = 0.2;
        public double startPoint = -5, endPoint = 5;
        private int iterations = 1;
        private int currentIteration = 0;
        private int botCount = 20;

        public MainWindow()
        {
            InitializeComponent();
            InitGUI();

            initHeatMap();
            initBots(botCount);
        }

        private void MainProc()
        {
            for (int i = 0; i < iterations; i++)
            {
                bots = Reproduce(ref bots);
                bots = Crossingover(ref bots);
                bots = Mutation(ref bots);
                currentIteration++;
            }
            updateHeatMap();
            findBest();
            labelIterarions.Content = "Итераций: " + currentIteration;
        }

        public List<Bot> Reproduce(ref List<Bot> botList)
        {
            List<Bot> tempList = botList;
            List<Bot> bestList = new List<Bot>();
            int counter = 0;

            Random random = new Random();
            int index1, index2;
            Bot bot1 = null, bot2 = null;

            do
            {
                index1 = random.Next(tempList.Count);
                index2 = random.Next(tempList.Count);
                Debug.Print("" + tempList[index1].getInfo() + "\n" + tempList[index2].getInfo());
                if (index1 != index2)
                {
                    if (tempList[index1].Value <= tempList[index2].Value)
                    {
                        bestList.Add(new Bot(counter, tempList[index1].XTrue, tempList[index1].YTrue, this));
                        bot1 = tempList[index1];
                        bot2 = tempList[index2];
                    }
                    else
                    {
                        bestList.Add(new Bot(counter, tempList[index2].XTrue, tempList[index2].YTrue, this));
                        bot1 = tempList[index1];
                        bot2 = tempList[index2];
                    }
                    tempList.Remove(bot1);
                    tempList.Remove(bot2);
                    counter++;
                }
            } while (tempList.Count > 0);

            int tmp = bestList.Count;
            for (int i = 0; i < tmp; i++)
            {
                bestList.Add(new Bot(counter, bestList[i].XTrue, bestList[i].YTrue, this));
                counter++;
            }

            //Debug.Print("\nBest:");
            //for (int i = 0; i < bestList.Count; i++)
            //{
            //    Debug.Print(bestList[i].getInfo());
            //}

            return bestList;
        }

        public List<Bot> Crossingover(ref List<Bot> botlist)
        {
            double modifier;
            Random random = new Random();
            int index1, index2;

            for(int i = 0; i < botlist.Count/2; i++)
            {
                index1 = random.Next(botlist.Count);
                index2 = random.Next(botlist.Count);
                
                if (index1 != index2 && random.NextDouble() >= (1 - chanceCrossingover))
                {
                    //Debug.Print("" + botlist[index1].getInfo() + "\n" + botlist[index2].getInfo());
                    //Debug.Print("cross true " + index1 + " " + index2);
                    modifier = -0.25 + random.NextDouble() + (0.5* random.NextDouble());
                    botlist[index1].XTrue = botlist[index1].XTrue + ( modifier*Math.Abs(botlist[index1].XTrue - botlist[index2].XTrue));
                    botlist[index2].XTrue = botlist[index2].XTrue + (modifier * Math.Abs(botlist[index1].XTrue - botlist[index2].XTrue));

                    modifier = -0.25 + random.NextDouble() + (0.5 * random.NextDouble());
                    botlist[index1].YTrue = botlist[index1].YTrue + (modifier * Math.Abs(botlist[index1].YTrue - botlist[index2].YTrue));
                    botlist[index2].YTrue = botlist[index2].YTrue + (modifier * Math.Abs(botlist[index1].YTrue - botlist[index2].YTrue));
                    Debug.Print("" + botlist[index1].getInfo() + "\n" + botlist[index2].getInfo() + "\n");
                }
            }

            return botlist;
        }

        public List<Bot> Mutation(ref List<Bot> botlist)
        {
            Random random = new Random();

            for (int i = 0; i < botlist.Count; i++)
            {

                if (random.NextDouble() >= (1 - chanceMutation))
                {
                    //if (random.NextDouble() >= 0.5)         // x or y
                    //{
                        botlist[i].XTrue = startPoint + random.NextDouble() * Math.Abs(startPoint - endPoint);//botlist[i].XTrue + ((-0.5 + random.NextDouble()) * Math.Abs(botlist[i].XTrue - Math.Min(Math.Abs(startPoint - botlist[i].XTrue), Math.Abs(botlist[i].XTrue - endPoint))));
                    //}
                    //else
                    //{
                        botlist[i].YTrue = startPoint + random.NextDouble() * Math.Abs(startPoint - endPoint);//botlist[i].YTrue + ((-0.5 + random.NextDouble()) * Math.Abs(botlist[i].YTrue - Math.Min(Math.Abs(startPoint - botlist[i].YTrue), Math.Abs(botlist[i].YTrue - endPoint))));
                    //}
                }
            }

            return botlist;

        }

        public void findBest()
        {
            int[] indexArray = new int[5];
            int index = 0, index2 = -1;
            double min = double.MaxValue, iter_min = double.MinValue;

            //for (int i = 0; i < bots.Count; i++)
            //{
            //    Debug.Print(bots[i].getInfo());
            //}

            for (int i = 0; i < indexArray.Length; i++)
            {
                for (int j = 0; j < bots.Count; j++)
                {
                    if (bots[j].Value <= min && bots[j].Value > iter_min && bots[j].Number != index2)
                    {
                        min = bots[j].Value;
                        index = j;
                        //indexArray[i] = j;
                    }
                }
                iter_min = min;
                min = double.MaxValue;
                index2 = bots[index].Number;
                labels[i].Content = bots[index].getInfo();
            }

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
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                bots.Add(new Bot( i, startPoint + random.NextDouble()*Math.Abs(startPoint - endPoint), startPoint + random.NextDouble() * Math.Abs(startPoint - endPoint), this));
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

            //return Math.Pow((x * x) + y - 10, 2) + Math.Pow(x + (y * y) - 7, 2);
            //return ((y + 47) * Math.Sin(Math.Sqrt(Math.Abs((x / 2) + (y + 47))))) - (x * Math.Sin(Math.Sqrt(Math.Abs(x - (y + 47)))));
        }

        public void updateHeatMap()
        {
            while(model.Series.Count > 1)
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
            inputPCross.Text = chanceCrossingover.ToString();
            inputPMut.Text = chanceMutation.ToString();
        }

        private void LockChanges()
        {
            if (acceptButton.IsEnabled)
            {
                inputBots.IsEnabled = false;
                inputSteps.IsEnabled = false;
                inputPCross.IsEnabled = false;
                inputPMut.IsEnabled = false;
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
            inputPCross.IsEnabled = true;
            inputPMut.IsEnabled = true;
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

            if (double.TryParse(inputPCross.Text, out Dbuff))
            {
                if (Dbuff >= 0 && Dbuff <= 1)
                {
                    chanceCrossingover = Dbuff;
                    Dbuff = 0;
                }
            }

            if (double.TryParse(inputPMut.Text, out Dbuff))
            {
                if (Dbuff >= 0 && Dbuff <= 1)
                {
                    chanceMutation = Dbuff;
                    Dbuff = 0;
                }
            }

            InitGUI();
            initHeatMap();
            initBots(botCount);
        }



    }

}
