using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Parsing
{
    public interface IParser
    {
        GenData Read(string fileName);
        string[] GetSeparatedValuesOfObjParam(string str, char separator, char super);
        string[] GetSeparatedArgs(string str);
    }
}
