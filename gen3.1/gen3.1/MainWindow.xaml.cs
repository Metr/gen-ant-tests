using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

namespace gen3._1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlotModel model;
        public static List<City> GlobalCityList;
        public static City startCity;
        private List<Bot> bots;
        private Label[] labels;
        private int botnumb = 12;

        private double chanceCrossingover = 0.2, chanceMutation = 0.5;
        private int iterations = 1;
        private int currentIteration = 500;

        public MainWindow()
        {
            InitializeComponent();
            InitGUI();

            initHeatMap();
            initBotsAsync(botnumb);
        }

        private void MainProc()
        {
            int minIndex = 0, count = 0;
            double min, latestMin = double.MinValue;


            for (int i = 0; i < iterations; i++)
            {
                bots = Reproduce(ref bots);
                //bots = Crossingover(ref bots);
                bots = Mutation(ref bots);

                
                

                //Debug.Print(" ");
                currentIteration++;

                updateHeatMap();
            }

            for (int i = 0; i < 3; i++)
            {
                min = double.MaxValue;
                for (int j = 0; j < bots.Count; j++)
                    if (bots[j].Distance < min && bots[j].Distance > latestMin)
                    {
                        minIndex = j;
                        min = bots[j].Distance;
                    }
                labels[i].Content = bots[minIndex].getInfo();
                latestMin = bots[minIndex].Distance;
                minIndex = 0;
            }
            labelIterarions.Content = "Итераций: " + currentIteration;
        }

        public List<Bot> Reproduce(ref List<Bot> botList)
        {
            int powerIndex = 3; //устойчивость популяции, сколько потомков дадут самые лучшие, при условии ненарушения баланса
            List<Bot> bestList = new List<Bot>();
            double minLenght = double.MaxValue;
            double border = double.MinValue;
            double tmp;
            int minIndex = 0;

            for (int i = 0; i < botList.Count/ powerIndex; i++)
            {
                for (int j = 0; j < botList.Count; j++)
                {
                    tmp = botList[j].getLenght();
                    if (tmp < minLenght && tmp > border )
                    {
                        minIndex = j;
                        minLenght = botList[j].getLenght();
                    }
                }
                for(int j = 0; j < powerIndex; j++)
                    bestList.Add(new Bot(botList[minIndex]));

                border = minLenght;
                minLenght = double.MaxValue;
                //botList.RemoveAt(minIndex);
            }
            Debug.Print("" + bestList[0].Distance);
            return bestList;
        }

        public List<Bot> Crossingover(ref List<Bot> botlist)
        {
            Random random = new Random();
            int index1, index2;

            for (int i = 0; i < botlist.Count / 2; i++)
            {
                index1 = random.Next(botlist.Count);
                index2 = random.Next(botlist.Count);

                if (index1 != index2 && random.NextDouble() >= (1 - chanceCrossingover))
                {
                    //Debug.Print("" + botlist[index1].getInfo() + "\n" + botlist[index2].getInfo());
                    //Debug.Print(botlist[index1].getInfo());
                    //Debug.Print(botlist[index2].getInfo());
                    //Debug.Print("index " + index1);/
                    City buffer = new City();
                    int p = random.Next(botlist.Count);

                    buffer = bots[index1].cities[p];
                    bots[index1].cities[p] = bots[index2].cities[p];
                    bots[index2].cities[p] = buffer;

                    bots[i].getLenght();
                    bots[i+1].getLenght();
                    //Debug.Print(botlist[index1].getInfo());
                    //Debug.Print(botlist[index2].getInfo());
                    //Debug.Print("" + botlist[index1].getInfo() + "\n" + botlist[index2].getInfo() + "\n");
                }
            }


            return botlist;
        }

        public List<Bot> Mutation(ref List<Bot> botlist)
        {
            Random random = new Random();
            for (int i = 0; i < botlist.Count; i++)
            {
                if (random.NextDouble() <= chanceMutation)
                {
                    botlist[i].Mutation();
                }
            }
            return botlist;

        }

        public List<City> Shuffle()
        {
            Random random = new Random();
            City buffer = new City();
            List<City> list = new List<City>();

            int n = GlobalCityList.Count;

            for (int i = n-1; i >= 1; i--)
            { 
                int j = random.Next(i + 1);
                buffer = GlobalCityList[j];
                GlobalCityList[j] = GlobalCityList[i];
                GlobalCityList[i] = buffer;
            }

            //string str = "";
            for (int i = 0; i < n; i++)
            {
                //str += GlobalCityList[i].number+" ";
                list.Add(new City(GlobalCityList[i]));
            }
            //Debug.Print("shifle        " + str);
            return list;
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>

        private void initHeatMap()
        {
            model = new PlotModel { Title = "areamap" };
            model.Axes.Add(new LinearColorAxis { Palette = OxyPalettes.Viridis(255) });

            var AreaSeries = new AreaSeries
            {            };

            var axis1 = new LinearColorAxis();
            axis1.Key = "ColorAxis";
            axis1.Maximum = 1;
            axis1.Minimum = 0;
            axis1.IsAxisVisible = false;
            model.Axes.Add(axis1);
            

            axis1.Palette.Colors.Clear();
            axis1.Palette.Colors.Add(OxyColor.FromArgb((byte)255, 255, 0, 0));

            model.Series.Add(AreaSeries);

        }

        private void initBotsAsync(int botnumb)
        {
            GlobalCityList = new List<City>();
            string[] coll;
            int counter = 0;
            bots = new List<Bot>();

            using (StreamReader reader = new StreamReader("berlin52.txt"))
            {
                string line;
                IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
                while ((line = reader.ReadLine()) != null)
                {
                    coll = line.Split(' ');
                    double x = double.Parse(coll[1], formatter);
                    double y = double.Parse(coll[2], formatter);
                    GlobalCityList.Add(new City(x , y, int.Parse(coll[0])));
                    if (counter == 0)
                        startCity = GlobalCityList[0];
                    model.Series.Add(GlobalCityList[counter].Point);
                    counter++;
                }
            }

            bots.Clear();
            for (int i = 0; i < botnumb; i++)
            {
                bots.Add(new Bot(this));
                model.Series.Add(bots.Last().getWay());
            }
    
            MainPlotView.Model = model;
        }


        public void updateHeatMap()
        {
            //while (model.Series.Count >= GlobalCityList.Count)
            //{
            //    model.Series.Remove(model.Series.Last());
            //}

            int count = model.Series.Count - GlobalCityList.Count;

            for (int i = 0; i < count; i++)
            {
                model.Series.RemoveAt(model.Series.Count - 1);
            }

            for (int i = 0; i < bots.Count; i++)
            {
                model.Series.Add(bots[i].getWay());
            }

            MainPlotView.Model.InvalidatePlot(true);
        }

        private void InitGUI()
        {
            labels = new Label[5];
            labels[0] = topValue1;
            labels[1] = topValue2;
            labels[2] = topValue3;
            labels[3] = topValue4;
            labels[4] = topValue5;

            //inputBots.Text = botCount.ToString();
            inputPCross.Text = chanceCrossingover.ToString();
            inputPMut.Text = chanceMutation.ToString();
        }

        private void LockChanges()
        {
            if (acceptButton.IsEnabled)
            {
                inputBots.IsEnabled = false;
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
            Bot bot = new Bot(this);
            bot.getLenght();
            model.Series.Add(bot.getWay());
            model.InvalidatePlot(true);
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
            inputPCross.IsEnabled = true;
            inputPMut.IsEnabled = true;
            acceptButton.IsEnabled = true;
            labelIterarions.Content = "Итераций: ";
            currentIteration = 0;

            InitGUI();
            initHeatMap();
            initBotsAsync(botnumb);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            double Dbuff;
            int Ibuff;


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
            initBotsAsync(botnumb);
        }

    }
}
