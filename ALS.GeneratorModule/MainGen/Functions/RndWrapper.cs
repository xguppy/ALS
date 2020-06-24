using System;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
    public class RndWrapper
    {
        public static int NextI(int a, int b, Random r)
        {
            return r.Next(a, b);
        }

        public static string NextIMass(int a, int b, int count, Random r)
        {
            StringBuilder str = new StringBuilder("");
            for (int i = 0; i < count; i++)
            {
                str.Append($"{NextI(a, b, r)}, ");
            }
            return str.ToString().Remove(str.Length - 2);
        }
        public static List<(string,string)> NextIntArray(int a, int b, int count, Random r)
        {
            List<(string, string)> result = new List<(string, string)>();
            for (int i = 0; i < count; i++)
            {
                result.Add((i.ToString(), NextI(a, b, r).ToString()));
            }
            return result;
        }

        public static double NextD(double a, double b, Random r)
        {
            double res = r.NextDouble();
            return res * (b - a) + a;
        }

        public static List<(string, string)> NextDoubleArray(double a, double b, int count, Random r)
        {
            List<(string, string)> result = new List<(string, string)>();
            for (int i = 0; i < count; i++)
            {
                result.Add((i.ToString(), NextDStr(a,b,r)));
            }
            return result;
        }
        public static string NextDMass(double a, double b, int count, Random r)
        {
            StringBuilder str = new StringBuilder("");
            for (int i = 0; i < count; i++)
            {
                str.Append($"{NextDStr(a, b, r)}, ");
            }

            return str.ToString().Remove(str.Length - 2).ToString();
        }

        public static string NextDStr(double a, double b, Random r, int finesse = 3)
        {
            var res = NextD(a, b, r).ToString();
            return res.Substring(0, res.IndexOf(',') + finesse).Replace(',', '.');
        }

    }
}