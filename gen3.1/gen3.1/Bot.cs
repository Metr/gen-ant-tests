using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen3._1
{
    public class Bot
    {
        public List<City> cities { get; }
        List<int> way { get; }
        private City startCity;
        public double Distance;

        private MainWindow main;

        public Bot(MainWindow main)
        {
            cities = new List<City>();

            List<int> deleteIndexes = new List<int>();
            startCity = MainWindow.startCity;

            this.main = main;
            cities = main.Shuffle();

            way = new List<int>();
            getLenght();
        }

        public Bot(Bot example)
        {
            this.cities = new List<City>();

            List<int> deleteIndexes = new List<int>();
            this.startCity = example.startCity;

            this.main = example.main;
            cities.AddRange(example.cities);

            way = new List<int>();
            getLenght();
        }

        public void Mutation()
        {
            Random random = new Random();
            int p1=-1, p2=-1;

            do
            {
                p1 = random.Next(cities.Count);
                p2 = random.Next(cities.Count);
            } while (p1 == p2);

            City buffer = cities[p1];
            cities[p1] = cities[p2];
            cities[p2] = buffer;
        }

        public double getLenght()
        {
            double dist = 0;
            this.way.Clear();

            string str = "";
            for (int i = 0; i < cities.Count; i++)
                str += cities[i].number + ",";

            ///Debug.Print(str);

            int pointer = 1;
            List<int> deleted = new List<int>();
            deleted.Add(startCity.number);

            bool isbeeing = false;
            int tryfind = -1;
            int newVar;
            
            List<City> way = new List<City>();
            way.Add(startCity);
            this.way.Add(startCity.number);

            for (int i = 1; i < cities.Count; i++)
            {
                isbeeing = false;

                for (int j = 0; j < deleted.Count; j++)
                {
                    if(deleted[j] == cities[pointer-1].number)
                    {
                        isbeeing = true;
                        break;
                    }
                }

                if(isbeeing)
                {
                    tryfind = deleted.Min();
                    do
                    {
                        tryfind++;
                        newVar = deleted.IndexOf(tryfind);
                    } while (newVar >= 0);

                    for (int j = 0; j < cities.Count; j++)
                    {
                        if (cities[j].number == tryfind)
                            pointer = cities[j].number;
                    }
                }
                else
                    pointer = cities[pointer - 1].number;

                deleted.Add(pointer);
                //Debug.Print("" + pointer);

                for (int j = 0; j < cities.Count; j++)
                {
                    if (cities[j].number == pointer)
                        way.Add(cities[j]);
                }
               
            }

            for (int i = 1; i < way.Count; i++)
            {
                //dist += way[i].GetRangeAt(way[i - 1]);
                this.way.Add(way[i].number);
            }

            this.way.Add(startCity.number);

            for (int i = 1; i < way.Count; i++)
                dist += way[i].GetRangeAt(way[i - 1]);


            this.Distance = dist;
            return dist;
        }

        public LineSeries getWay()
        {
            LineSeries lineSeries = new LineSeries();
            //scatterSeries.ColorAxisKey = "ColorAxis";

            lineSeries.MarkerSize = 10;
            lineSeries.MarkerType = MarkerType.Plus;
            for (int i = 0; i < way.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(cities[way[i] - 1].X, cities[way[i] - 1].Y));
            }
            //lineSeries.Points.Add
            return lineSeries;
        }

        public string getInfo()
        {
            string str = "value = "+Distance+", way: ";
            for (int i = 0; i < cities.Count; i++)
                str += cities[i].number + " ";
            return str;
        }

        private bool isAlreadyExist()
        {
            return false;
        }
    }
}
