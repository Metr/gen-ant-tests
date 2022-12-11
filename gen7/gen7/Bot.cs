using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen7
{
    public class Bot
    {
        public static int GraphCounter = 0;
        public static double[,] feromons;


        public List<int> Way;
        public int Value { get; set; }

        static Bot()
        {
            GraphCounter = (int)Math.Sqrt(MainWindow.Graph.Length);
            feromons = new double[7, 7];

            List<int> indexes = new List<int>();
            double feromon;

            for (int i = 0; i < GraphCounter; i++)
            {
                for (int j = 0; j < GraphCounter; j++)
                    if (MainWindow.Graph[i, j] > 0)
                        indexes.Add(j);

                if (indexes.Count > 0)
                {
                    feromon = (double)1 / indexes.Count;
                    for (int j = 0; j < indexes.Count; j++)
                        feromons[i, indexes[j]] = feromon;
                    indexes.Clear();
                }
            }
        }

        public Bot()
        {
            Way = new List<int>();
            Way.Add(0);
        }


        public bool tryFindWay()
        {
            bool result = true;
            int counter = 0, pointer = 0;
            double P, tmp = 0;
            List<double> probabilities = new List<double>();
            Dictionary<int, double> ProbabilityIndex = new Dictionary<int, double>();


            do
            {
                tmp = 0;
                for (int i = 0; i < GraphCounter; i++)
                    if (MainWindow.Graph[pointer, i] > 0)
                    {
                        tmp += feromons[pointer, i];
                        ProbabilityIndex.Add(i, tmp);
                    }

                if (ProbabilityIndex.Count > 0)
                {
                    P = MainWindow.random.NextDouble();
                    for (int i = 0; i < GraphCounter; i++)
                    {
                        if (ProbabilityIndex.ContainsKey(i))
                            if (P < ProbabilityIndex[i])
                            {
                                pointer = i;
                                Way.Add(pointer);

                                break;
                            }
                    }

                    string str = "";
                    foreach (var item in ProbabilityIndex)
                    {
                        str += "  {" + item.Key + ":" + item.Value + "}";
                    }
                    //Debug.Print("" + P + ", pointer = " + pointer + str);
                }
                else
                    break;

                ProbabilityIndex.Clear();
                counter++;
            } while (counter < feromons.Length);

            for (int i = 0; i < GraphCounter; i++)
            {
                if (!Way.Contains(i))
                    result = false;
            }

            return result;
        }

        public void dropFeromons()
        {
            double usefulness = (double)1 / Way.Count;
            int pointer = 0;
            List<int> numbers = new List<int>();

            for (int i = 1; i < Way.Count; i++)
            {
                if (!numbers.Contains(Way[i]) && feromons[pointer, Way[i]] > 0)
                {
                    feromons[pointer, Way[i]] += usefulness;
                    numbers.Add(Way[i]);
                    //Debug.Print("added " + usefulness + "  to " + pointer + " " + Way[i]);
                    pointer = Way[i];
                }
            }


            List<int> indexes = new List<int>();
            double normalizedValue = 0;
            double sum = 0;

            for (int i = 0; i < GraphCounter; i++)
            {
                for (int j = 0; j < GraphCounter; j++)
                    if (feromons[i, j] > 0)
                    {
                        indexes.Add(j);
                        sum += feromons[i, j];
                    }

                for (int j = 0; j < indexes.Count; j++)
                {
                    normalizedValue = (feromons[i, indexes[j]] * 100) / (sum*100);
                    feromons[i, indexes[j]] = normalizedValue;
                }

                sum = 0;
                indexes.Clear();
            }

            //string str = "";
            //for (int i = 0; i < 7; i++)
            //{
            //    for (int j = 0; j < 7; j++)
            //    {
            //        str += string.Format("{0:F3}\t", feromons[i, j]);
            //    }
            //    Debug.Print(str);
            //    str = "";
            //}

        }


        public string GetInfo()
        {
            string str = "";
            for (int i = 0; i < Way.Count; i++)
            {
                str += Way[i] + " -> ";
            }
            return str + "";
        }

        public string GetMostProbabilityWay()
        {
            string str = "0  ";
            int pointer = 0, maxIndex = 0;
            double max = double.MinValue;

            for (int i = 0; i < GraphCounter-1; i++)
            {
                for (int j = 0; j < GraphCounter; j++)
                {
                    if(feromons[pointer, j] > max)
                    {
                        maxIndex = j;
                        max = feromons[pointer, j];
                    }
                }

                str += maxIndex + "  ";
                pointer = maxIndex;
                max = double.MinValue;

            }

            return str;
        }

    }
}
