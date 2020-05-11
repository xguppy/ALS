using System.Collections.Generic;

namespace Generator
{
    public static class Elems
    {
        public static List<AEElem> Funcs = new List<AEElem>();
        public static List<AEElem> Signs = new List<AEElem>();
       
        public static void SetFuncs(List<string> funcs)
        {
            AEElem sin = new AEElem("sin", "sin(?L)", "\\sin{(?L)}", default);
            AEElem cos = new AEElem("cos", "cos(?L)", "\\cos{(?L)}", default);
            AEElem tan = new AEElem("tan", "tan(?L)", "\\tan{(?L)}", default);
            AEElem atan = new AEElem("atan", "atan(?L)", "\\arctan{(?L)}", default);
            AEElem scobs = new AEElem("scobs", "(?L)", "(?L)", default);

            AEElem log10 = new AEElem("log10", "log10(?L)", "\\log_{10}{(?L)}", "((?L) > 1.0e-6 && fabs((?L) - 1.0) > 1.0e-6)");
            AEElem ln = new AEElem("ln", "ln(?L)", "\\ln{(?L)}", "((?L) > 1.0e-6 && fabs((?L) - 1.0) > 1.0e-6)");
            AEElem sqrt = new AEElem("sqrt", "sqrt(?L)", "\\sqrt{?L}", "((?L) > 1.0e-6)");

            if (funcs.Contains("sin"))   Funcs.Add(sin);
            if (funcs.Contains("cos"))   Funcs.Add(cos);
            if (funcs.Contains("tan"))   Funcs.Add(tan);
            if (funcs.Contains("atan"))  Funcs.Add(atan);
            if (funcs.Contains("log10")) Funcs.Add(log10);
            if (funcs.Contains("ln"))    Funcs.Add(ln);
            if (funcs.Contains("sqrt"))  Funcs.Add(sqrt);
                                         Funcs.Add(scobs);
        }
        
        public static void SetSigns(List<string> signs)
        {// знаки
            AEElem add = new AEElem("+", "?L + ?R", "?L + ?R", default);
            AEElem sub = new AEElem("-", "?L - ?R", "?L - ?R", default);
            AEElem mul = new AEElem("*", "?L * ?R", "?L * ?R", default);
            AEElem div = new AEElem("/", "?L / (?R)", "\\frac{?L}{?R}", "(fabs((?R)) > 1.0e-6)");
            AEElem pow = new AEElem("^", "pow(?L, ?R)", "(?L)^{?R}", default);

            if (signs.Contains("+")) Signs.Add(add);
            if (signs.Contains("-")) Signs.Add(sub);
            if (signs.Contains("*")) Signs.Add(mul);
            if (signs.Contains("/")) Signs.Add(div);
            if (signs.Contains("^")) Signs.Add(pow);
        }
        
    }
}