using Antlr4.Runtime;
using ALS.AntiPlagModule.Constants;

namespace ALS.AntiPlagModule.Services.LexerService
{
    public class CppLexer : ILexer
    {
        // TODO add read from file and Parse Logic
        public string[] KeyWords => null;
        public string[] Operators => null;
        private int[] _tokens;
        public int[] Tokens => _tokens;

        public void Parse(ILexerFactory lexer, string code)
        {
            // base initialize
            AntlrInputStream inputStream = new AntlrInputStream(code);
            var cppLexer = lexer.Create(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(cppLexer);
            
        }
    }
}
