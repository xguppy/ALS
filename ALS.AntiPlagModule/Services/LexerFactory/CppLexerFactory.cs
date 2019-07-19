using Antlr4.Runtime;
using CppGrammar;

namespace ALS.AntiPlagModule.Services.LexerFactory
{
    public class CppLexerFactory: ILexerFactory
    {
        public Lexer Create(AntlrInputStream inputStream) => new CPP14Lexer(inputStream);
    }
}
