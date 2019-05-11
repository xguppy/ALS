using System;

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
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str += $"{NextI(a, b, r)}, ";
            }
            
            return str.Remove(str.Length - 2);
        }
        
        public static double NextD(double a, double b, Random r)
        {
            double res = r.NextDouble();
            return res * (b - a) + a;
        }
        
        public static string NextDMass(double a, double b, int count, Random r)
        {
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str += $"{NextDStr(a, b, r)}, ";
            }
            
            return str.Remove(str.Length - 2);
        }    
        
        public static string NextDStr(double a, double b, Random r, int finesse = 3)
        {
            var res = RndWrapper.NextD(a,b, r).ToString();
            return res.Substring(0, res.IndexOf(',') + finesse).Replace(',', '.');
        }
        
    }
}