using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gen_1
{
    class Subject
    {
        private bool[] binaryArray;
        private double absoluteX;
        private double steps;
        private ObservablePoint point;
        private string name;

        public Subject(double value, string name)
        {
            absoluteX = value;
            Point = new ObservablePoint();
            this.binaryArray = new bool[10];
            this.Name = name;

            valueToBinary();
        }

        public bool[] BinaryArray { get => binaryArray; set => binaryArray = value; }
        public double AbsoluteX { get => absoluteX; 
            set 
            { 
                this.absoluteX = value;
                valueToBinary();
            } 
        }
        public ObservablePoint Point { get => point; set => point = value; }
        public string Name { get => name; set => name = value; }
        public double Steps { get => steps; set => steps = value; }

        public void addToChart(ref CartesianChart cartesianChart)
        {

            bool f = false;
            for (int i = 0; i < cartesianChart.Series.Count; i++)
            {
                if (cartesianChart.Series[i].Title.Equals(this.name))
                {
                    cartesianChart.Series[i].Values.Clear();
                    cartesianChart.Series[i].Values.Add(this.point);
                    f = !f;
                    break;
                }
            }

            if (!f)
            {
                LineSeries line = new LineSeries();
                line.Title = name;
                line.Values = new ChartValues<ObservablePoint>();
                line.Values.Add(point);
                cartesianChart.Series.Add(line);
            }
            
        }

        //////////////////////////////////////////////////////////////////////////////////

        public static void Crossingover(Subject sub1, Subject sub2)
        {
            Random random = new Random();
            int lenght = sub1.binaryArray.Length;
            int borderline = (lenght/4) + random.Next(lenght/2);
            //Debug.Print("borderline = " + borderline);

            for (int i = 0; i < sub1.binaryArray.Length; i++)
            {
                if(i <= borderline)
                    sub2.binaryArray[i] = sub1.binaryArray[i];
                else
                    sub1.binaryArray[i] = sub2.binaryArray[i];
            }

            sub1.binaryToValue();
            sub2.binaryToValue();
            
        }



        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// 

        private void valueToBinary()
        {
            double counter = 0, tmp = -20;
            do
            {
                counter++;
                tmp += MainWindow.step;
            } while (tmp < absoluteX && counter < MainWindow.steps);
            steps = counter;
            //AbsoluteX = MainWindow.step * steps;

            counter++;
            for (int i = 0; i < binaryArray.Length; i++)
            {
                binaryArray[i] = !Convert.ToBoolean(counter % 2);
                counter = Math.Truncate(0.5 + (counter / 2));
            }


            updatePoint();
        }

        public void binaryToValue()
        {
            double tmp = 0;

            for (int i = binaryArray.Length-1; i >= 0; i--)
            {
                if (binaryArray[i])
                    tmp += Math.Pow(2, i);
            }
            //Debug.Print("tmp = " + tmp);
            steps = tmp;
            AbsoluteX = -20 + (MainWindow.step * steps);

            updatePoint();
        }

        private void updatePoint()
        {
            Point.X = steps;
            Point.Y = MainWindow.getValueOf(-20 + MainWindow.step*steps);
        }


        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////
        /// </summary>
        
        public string getBinaryArrayString()
        {
            string str = "";
            for (int i = binaryArray.Length-1; i >= 0; i--)
            {
                if (binaryArray[i]) str += "1";
                else str += "0";
                
            }
            return str + "";
        }
    
        public string getInfo()
        {
            return name + " bin = " + getBinaryArrayString() + " point.x = " + point.X + " point.y = " + point.Y + " absoluteX = " + absoluteX + " steps = " + steps;
        }
    }
}
