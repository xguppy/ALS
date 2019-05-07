using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace ALS.AntiPlagModule.Services
{
    public interface ILexer
    {
        /// <summary>
        ///  Операторы языка -- для анализа
        /// </summary>
        string[] Operators { get; }
        /// <summary>
        /// // Ключевые слова -- для анализа
        /// </summary>
        string[] KeyWords { get; }
        /// <summary>
        /// Обработанное представление программы в виде числовых лексем
        /// </summary>
        int[] Tokens { get; }
        /// <summary>
        /// Основная логика обработки
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="code"></param>
        void Parse(ILexerFactory lexer, string code);
    }
}
