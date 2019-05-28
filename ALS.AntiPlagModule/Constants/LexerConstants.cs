using System;
using System.Collections.Generic;
using System.Text;

namespace ALS.AntiPlagModule.Constants
{
    public static class LexerConstants
    {
        public const int KeyWordsStart = 0;
        public const int OpStart = 0x00ff;
        public const int Id = 0xff00;
        public const int Number = 0xff01;
        public const int String = 0xff02;
        public const int Real = 0xff03;
        public const int Unknown = 0xffff;
    }
}
