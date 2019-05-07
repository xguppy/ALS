using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace ALS.AntiPlagModule.Services
{
    public interface ILexerFactory
    {
        /// <summary>
        /// Abstract factory for ANTLR Lexer
        /// </summary>
        /// <param name="inputStream"> ANTLR input stream </param>
        /// <returns></returns>
        Lexer Create(AntlrInputStream inputStream);
    }
}
