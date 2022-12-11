using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace gen7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Bot> bots;
        //private Label[] labels;
        //private Ellipse[] Ellipses;
        //private List<Brush> Colors;

        public static int[,] Graph = new int[7, 7];
        public static Random random;

        private int botnumb = 10;
        private double chanceCrossingover = 0.8, chanceMutation = 0.4;
        private int currentIteration = 0;
        private int iterations = 1;

        public MainWindow()
        {
            InitializeComponent();
            random = new Random();

            initBots(botnumb);
        }

        private void MainProc()
        {
            int minIndex = 0, count = 0;
            double min, latestMin = double.MinValue;
            bool isSolutionExist = false;

            for(int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < bots.Count; j++)
                {
                    if (bots[j].tryFindWay())
                    {
                        //Debug.Print("true");
                        bots[j].dropFeromons();
                    }
                }
                currentIteration++;
            }


            labelIterarions.Content = "Итераций: " + currentIteration;
            wayLabel.Content = bots.First().GetMostProbabilityWay();
            updateGUI();

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
            updateGUI();


        }

        private void updateGUI()
        {
            string str = "";
            for (int i = 0; i < Bot.GraphCounter; i++)
            {
                for (int j = 0; j < Bot.GraphCounter; j++)
                {
                    str += string.Format(" {0:F3}  ", Bot.feromons[i, j]);
                }
                str += "\n";
            }
            FeromonsTextBox.Text = str;

            str = "";
            for (int i = 0; i < Bot.GraphCounter; i++)
            {
                for (int j = 0; j < Bot.GraphCounter; j++)
                {
                    str += "  " + Graph[i, j] + "  ";
                }
                str += "\n";
            }
            GraphTextBox.Text = str;
        }


        /// <summary>
        /// /////////////////////////////////////////////////////////////////////
        /// </summary>

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            Bot bot1 = new Bot();
            if (bot1.tryFindWay())
            {
                Debug.Print("true");
                bot1.dropFeromons();
            }
            updateGUI();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iterations = 1;
            MainProc();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            iterations = 5;
            MainProc();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            iterations = 10;
            MainProc();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            currentIteration = 0;
            initBots(botnumb);
            updateGUI();
        }


    }


}
