using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gen4
{
    public class Bot
    {
        public static int GraphCounter = 0;
        public static int RemovableColors = 0;
        public static List<int> Colors;

        public List<int> Genome;
        public int Value { get; set; }

        static Bot()
        {
            GraphCounter = (int)Math.Sqrt( MainWindow.Graph.Length);
            Colors = new List<int>();
            
            for (int i = 0; i < GraphCounter; i++)
                Colors.Add(Colors.Count);
        }

        public Bot()
        {
            Genome = new List<int>();
            for(int i = 0; i < GraphCounter; i++)
                Genome.Add(Colors[MainWindow.random.Next(GraphCounter-RemovableColors)]);
            Value = getFunction();
        }

        public Bot(List<int> gen)
        {
            Genome = new List<int>();
            Genome.AddRange(gen);
            Value = getFunction();
        }

        public void Mutation()
        {
            int tmp = MainWindow.random.Next(Genome.Count);
            Genome[tmp] = MainWindow.random.Next(GraphCounter - RemovableColors);
        }

        public int getFunction()
        {
            int counter = 0;
            List<int> colors = new List<int>();

            for (int i = 0; i < GraphCounter; i++)
            {
                colors.Clear();
                for (int j = i; j < GraphCounter; j++)
                {
                    if (MainWindow.Graph[i, j] > 0)
                    {
                        colors.Add(Genome[j]);
                    }

                }

                for (int j = 0; j < colors.Count; j++)
                {
                    if (colors[j] == Genome[i])
                    {
                        //Debug.Print("output node=" + (i + 1));
                        counter++;
                    }
                }

            }
            Value = counter;
            return counter;
        }

        public string GetInfo()
        {
            string str = "";
            int b = 0;
            for (int i = 0; i < Genome.Count; i++)
            {
                b = i + 1;
                str += b + "-" + Genome[i] + "  ";
            }
            return str+"";
        }

    }
}
