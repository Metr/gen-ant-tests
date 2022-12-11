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

namespace gen4
{

    public partial class MainWindow : Window
    {
        private List<Bot> bots;
        private Label[] labels;
        private Ellipse[] Ellipses;
        private List<Brush> Colors;

        public static int[,] Graph = new int[11,11];
        public static Random random;

        private Bot lastSoution;

        private int botnumb = 10;
        private double chanceCrossingover = 0.8, chanceMutation = 0.4;
        private int currentIteration = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitGUI();
            random = new Random();

            initBots(botnumb);
            InitGUI();
        }

        private void MainProc()
        {
            int minIndex = 0, count = 0;
            double min, latestMin = double.MinValue;
            bool isSolutionExist = false;


            do {

                bots = Reproduce(ref bots);
                bots = Crossingover(ref bots);
                bots = Mutation(ref bots);

                currentIteration++;

                for (int i = 0; i < bots.Count; i++)
                {
                    if (bots[i].getFunction() == 0)
                    {
                        isSolutionExist = true;
                        lastSoution = bots[i];
                        UpdateGUI(lastSoution);
                    }

                }
            } while (currentIteration <= 5000 && !isSolutionExist);

            if (isSolutionExist)
            {
                labelIterarions.Content = "Итераций: " + currentIteration + ", решение найдено";
                Bot.RemovableColors++;
                initBots(botnumb);
            }
            else
                labelIterarions.Content = "Итераций: " + currentIteration + ", решение нe найдено";


        }

        public List<Bot> Reproduce(ref List<Bot> botList)
        {
            List<Bot> tempList = botList;
            List<Bot> bestList = new List<Bot>();
            int counter = 0;

            int index1, index2;
            Bot bot1 = null, bot2 = null;

            do
            {
                index1 = random.Next(tempList.Count);
                index2 = random.Next(tempList.Count);
                if (index1 != index2)
                {
                    if (tempList[index1].Value <= tempList[index2].Value)
                    {
                        bestList.Add(new Bot(tempList[index1].Genome));
                        bot1 = tempList[index1];
                        bot2 = tempList[index2];
                    }
                    else
                    {
                        bestList.Add(new Bot(tempList[index2].Genome));
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
                bestList.Add(new Bot(bestList[i].Genome));
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
            int index1, index2, border, buffer;

            for (int i = 0; i < botlist.Count / 2; i++)
            {
                do
                {
                    index1 = random.Next(botlist.Count);
                    index2 = random.Next(botlist.Count);
                } while (index1 == index2);
                //string str = "";
                //for (int j = 0; j < botlist[index1].Genome.Count; j++)
                //    str += botlist[index1].Genome[j] + " ";
                //Debug.Print("1 - " + str);

                //str = "";
                //for (int j = 0; j < botlist[index2].Genome.Count; j++)
                //    str += botlist[index2].Genome[j] + " ";
                //Debug.Print("2 - " + str);

                if (random.NextDouble() >= (1 - chanceCrossingover))
                {
                    border = random.Next(1,botlist[index1].Genome.Count-1);
                    Debug.Print("" + border);
                    for (int j = 0; j < botlist[index1].Genome.Count; j++)
                    {
                        if (j <= border)
                        {
                            buffer = botlist[index1].Genome[j];
                            botlist[index1].Genome[j] = botlist[index2].Genome[j];
                            botlist[index2].Genome[j] = buffer;
                        }
                    }
                }
                //str = "";
                //for (int j = 0; j < botlist[index1].Genome.Count; j++)
                //    str += botlist[index1].Genome[j] + " ";
                //Debug.Print("1 - " + str);

                //str = "";
                //for (int j = 0; j < botlist[index2].Genome.Count; j++)
                //    str += botlist[index2].Genome[j] + " ";
                //Debug.Print("2 - " + str);
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

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>


        private void initBots(int botnumb)
        {
            using (StreamReader reader = new StreamReader("Graph.txt"))
            {
                string line;
                string[] coll;
                int counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    coll = line.Split(' ');
                    Debug.Print(line);
                    for (int i = 0; i < coll.Length; i++)
                        if (int.Parse(coll[i]) > 0)
                            Graph[counter, i] = 1;
                        else
                            Graph[counter, i] = 0;
                    counter++;
                }
            }

            bots = new List<Bot>();
            for (int i = 0; i < botnumb; i++)
            {
                bots.Add(new Bot());
                //Debug.Print(""+ bots.Last().GetInfo());
            }
        }

        private void InitGUI()
        {
            labels = new Label[5];
            labels[0] = topValue1;

            Ellipses = new Ellipse[11];
            Ellipses[0] = ellipse_1;
            Ellipses[1] = ellipse_2;
            Ellipses[2] = ellipse_3;
            Ellipses[3] = ellipse_4;
            Ellipses[4] = ellipse_5;

            Ellipses[5] = ellipse_6;
            Ellipses[6] = ellipse_7;
            Ellipses[7] = ellipse_8;
            Ellipses[8] = ellipse_9;
            Ellipses[9] = ellipse_10;
            Ellipses[10] = ellipse_11;

            Colors = new List<Brush>();
            Colors.Add(Brushes.Red);
            Colors.Add(Brushes.Green);
            Colors.Add(Brushes.YellowGreen);
            Colors.Add(Brushes.Orange);
            Colors.Add(Brushes.LightGreen);
            Colors.Add(Brushes.LightBlue);
            Colors.Add(Brushes.DarkRed);
            Colors.Add(Brushes.DarkMagenta);
            Colors.Add(Brushes.DarkBlue);
            Colors.Add(Brushes.Gray);
            Colors.Add(Brushes.White);

            Line buffer = new Line();
            for (int i = 0; i < Ellipses.Length; i++)
            {
                for (int j = 0; j < Ellipses.Length; j++)
                {
                    if (MainWindow.Graph[i, j] > 0)
                    {
                        buffer = new Line();
                        buffer.Stroke = Brushes.Black;
                        buffer.X1 = Ellipses[i].Margin.Left + 15;
                        buffer.Y1 = Ellipses[i].Margin.Top + 15;
                        buffer.X2 = Ellipses[j].Margin.Left + 15;
                        buffer.Y2 = Ellipses[j].Margin.Top + 15;

                        this.grid_1.Children.Add(buffer);
                    }


                }

            }

            //inputBots.Text = botCount.ToString();
            inputPCross.Text = chanceCrossingover.ToString();
            inputPMut.Text = chanceMutation.ToString();
        }

        private void UpdateGUI(Bot bot)
        {
            labels[0].Content = bot.GetInfo();
            for (int i = 0; i < bot.Genome.Count; i++)
            {
                Ellipses[i].Fill = Colors[bot.Genome[i]];
            }
            colorsInfo.Content = "Цветов: " + (Bot.GraphCounter - Bot.RemovableColors) + " из " + Bot.GraphCounter; 

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
            Bot bot1 = new Bot();
            bots.Add(bot1);
            Bot bot2 = new Bot();
            bots.Add(bot2);
            int v1 = bot1.getFunction();
            int v2 = bot2.getFunction();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LockChanges();
            MainProc();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LockChanges();
            MainProc();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
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
            initBots(botnumb);
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
            initBots(botnumb);
        }

    }
}

