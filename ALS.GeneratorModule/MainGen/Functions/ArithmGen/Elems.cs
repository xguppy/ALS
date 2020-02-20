using System.Collections.Generic;

namespace Generator
{
    class Elems
    {
        public static string[] Funcs = {"sin", "cos", "sqrt", "tan", "log", "ln", "log10" };
        public static string[] Signs = {"-", "+", "/", "*", "^" };

        public static void SetFuncs(List<string> funcs)
        {
            int length = funcs.Count;
            if (funcs.Count > 0 && funcs[0].Trim(' ').Length == 0) length = 0;
            Funcs = new string[length]; 
            for (int i = 0 ; i < length; i++)
            {
                Funcs[i] = funcs[i];
            }
        }
        
        public static void SetSigns(List<string> signs)
        {
            int length = signs.Count;
            if (signs.Count > 0 && signs[0].Trim(' ').Length == 0) length = 0;
            Signs = new string[length]; 
            for (int i = 0 ; i < length; i++)
            {
                Signs[i] = signs[i];
            }
        }
        
    }
}