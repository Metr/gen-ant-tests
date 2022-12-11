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

using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using System.Runtime.Serialization;

namespace gen_1
{


    public partial class MainWindow : Window
    {
        public static double startPoint = -20, endPoint = -3.1;
        public static double step, steps = 1000;
        private double chanceCrossingover = 0.4, chanceMutation = 0.2;
        private int iterations = 1;
        private int currentIteration = 0;
        private int bots = 10;
        private int logOutLenght = 0;
        private List<Subject> subjects;
        
        public MainWindow()
        {
            InitializeComponent();
            step = Math.Abs(startPoint - endPoint) / steps;

            InitGUI();
            InitLiveCharts();
            InitSubjects(bots);
            isLogNeeded.IsChecked = false;
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Subject sub1 = new Subject(-9.71, "sub1");
            sub1.addToChart(ref testChart);

            Subject sub2 = new Subject(sub1.AbsoluteX, "sub2");
            sub2.addToChart(ref testChart);
            Debug.Print(sub1.getInfo() + "\n" + sub2.getInfo() + "\n");



            sub1.BinaryArray = new bool[] {true, true, true, true, true, true, false, false, true, true };
            sub1.binaryToValue();
            Debug.Print(sub1.getInfo() + "\n" + sub2.getInfo() + "\n");

            List<Subject> list = new List<Subject>();
            List<Subject> list2 = new List<Subject>();

            list.Add(sub1);
            list.Add(sub2);
            list2 = Reproduce(ref list);

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            step = Math.Abs(startPoint - endPoint) / steps;

            inputBots.IsEnabled = true;
            inputSteps.IsEnabled = true;
            inputPCross.IsEnabled = true;
            inputPMut.IsEnabled = true;
            acceptButton.IsEnabled = true;
            labelIterarions.Content = "Итераций: ";
            currentIteration = 0;
            logTextBlock.Text = "";

            InitGUI();
            InitLiveCharts();
            InitSubjects(bots);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            double Dbuff;
            int Ibuff;

            if (int.TryParse(inputBots.Text, out Ibuff))
            {
                if (Ibuff > 0)
                {
                    bots = Ibuff;
                    Ibuff = 0;
                }
            }

            if (int.TryParse(inputSteps.Text, out Ibuff))
            {
                if (Ibuff > 0)
                {
                    steps = Ibuff;
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
            InitLiveCharts();
            InitSubjects(bots);
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private void MainProc()
        {
            List<Subject> tempSubjects = new List<Subject>();

            for (int i = 0; i < iterations; i++)
            {
                LogOut("iteration: " + currentIteration);
               
                tempSubjects = Reproduce(ref subjects);
                Crossingover(ref tempSubjects);
                Mutation(ref tempSubjects);

                subjects = tempSubjects;
                currentIteration++;
            }
            labelIterarions.Content = "Итераций: " + currentIteration;
        }

        private List<Subject> Reproduce(ref List<Subject> subjectsList)
        {
            int count = subjectsList.Count;
            int counter = 0;

            List<Subject> tempList = new List<Subject>();
            tempList = subjectsList;
            List<Subject> bestList = new List<Subject>();

            double max = double.MinValue, Itermax = double.MaxValue;
            int index = 0;

            for (int i = 0; i < count/2; i++)
            {
                for (int j = 0; j < tempList.Count; j++)
                {
                    if(tempList[j].Point.Y > max && tempList[j].Point.Y < Itermax)
                    {
                        index = j;
                        max = tempList[j].Point.Y;
                    }
                }
                bestList.Add(tempList[index]);
                Itermax = max;
                max = double.MinValue;
                index = 0;
            }

            while (testChart.Series.Count > 1)
            {
                testChart.Series.Remove(testChart.Series.Last());
            }

            //for (int i = 0; i < count; i++)
            //{
            //    LogOut(tempList[i].getInfo());
            //}
            //LogOut("\n");

            //for (int i = 0; i < bestList.Count; i++)
            //{
            //    LogOut(bestList[i].getInfo());
            //}
            //LogOut("\n");

            tempList.Clear();
            for (int i = 0; i < count; i += 2)
            {
                tempList.Add(new Subject(bestList[counter].AbsoluteX, "subject" + ( i + 1)));
                tempList.Last().addToChart(ref testChart);

                tempList.Add(new Subject(bestList[counter].AbsoluteX, "subject" + (i + 2)));
                tempList.Last().addToChart(ref testChart);
                counter++;
            }

            for (int i = 0; i < count; i++)
            {
                LogOut(tempList[i].getInfo());
            }
            LogOut("\n");

            return tempList;
        }

        private void Crossingover(ref List<Subject> subjectsList)
        {
            List<int> indexs = new List<int>();
            Random random = new Random();
            int i1, i2;

            for (int j = 0; j < subjectsList.Count; j++)
                indexs.Add(j);

            for (int j = 0; j < subjectsList.Count / 4; j++)
            {
                if (random.NextDouble() >= (1-chanceCrossingover))
                {
                    i1 = indexs[random.Next(indexs.Count)];
                    indexs.Remove(i1);

                    i2 = indexs[random.Next(indexs.Count)];
                    indexs.Remove(i2);

                    Subject.Crossingover(subjectsList[i1], subjectsList[i2]);
                }
            }
        }

        private void Mutation(ref List<Subject> subjectsList)
        {
            Random random = new Random();
            int index;

            for (int i = 0; i < subjectsList.Count; i++)
            {
                if (random.NextDouble() >= (1 - chanceMutation))
                {
                    index = random.Next(1+subjectsList[i].BinaryArray.Length - 1);
                    if(index >= 0)
                    {
                        subjectsList[i].BinaryArray[index] = !subjectsList[i].BinaryArray[index];
                        subjectsList[i].binaryToValue();

                    }

                    //break;
                }
            }
            
        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////
        /// </summary>/

        private void InitSubjects(int count)
        {
            subjects = new List<Subject>(count);
            for (int i = 0; i < count; i++)
            {
                subjects.Add( new Subject((double)startPoint + ((i*(steps/count)) * step), "subject_"+(i+1)));
                //Debug.Print(""+subjects[i].Point.X + " " + subjects[i].Point.Y + " " );
                subjects[i].addToChart(ref testChart);
            }
        }

        private void InitLiveCharts()
        {
            LineSeries line = new LineSeries();
            line.Title = "testFunction";
            line.Values = new ChartValues<double>();
            line.PointGeometry = null;

            for (double i = startPoint; i < endPoint; i += step)
            {
                line.Values.Add(getValueOf(i));
            }
            testChart.Series = new SeriesCollection();
            
            testChart.Series.Add(line);


            //LineSeries line2 = new LineSeries();
            //line2.Title = "test2";
            //line2.Opacity = 0.5;

            //line2.Values = new ChartValues<ObservablePoint>();
            //line2.Values.Add( new ObservablePoint(1, 1));
            //line2.Values.Add( new ObservablePoint(100, 2));

            //testChart.Series.Add(line2);

        }

        private void InitGUI()
        {
            inputBots.Text = bots.ToString();
            inputSteps.Text = steps.ToString();
            inputPCross.Text = chanceCrossingover.ToString();
            inputPMut.Text = chanceMutation.ToString();
        }

        private void isLogNeeded_Checked_1(object sender, RoutedEventArgs e)
        {

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

        private void LogOut(string str)
        {
            if ((bool)isLogNeeded.IsChecked)
            {
                logOutLenght += str.Length;
                if (logTextBlock.Text.Length > 110000)
                    logTextBlock.Text = "символов >100000, была произведена очистка буфера\n";

                logTextBlock.Text += str + "\n";
            }
        }

        static public double getValueOf(double arg)
        {
            return 2 + Math.Sin(3 * arg) / (arg * arg);
            //return Math.Pow(arg, 2);
        }


    }
}
