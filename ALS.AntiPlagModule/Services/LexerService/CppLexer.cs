using System;
using System.Collections.Generic;
using System.Linq;
using ALS.AntiPlagModule.Constants;
using Antlr4.Runtime;

namespace ALS.AntiPlagModule.Services.LexerService
{
    public class CppLexer : ILexer
    {
        public string[] KeyWords => new[] { "alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand", "bitor", "bool", "break", "case", "catch", "char", "char16_t", "char32_t", "class", "compl", "const", "constexpr", "const_cast", "continue", "decltype", "default", "delete", "do", "double", "dynamic_cast", "else", "enum", "explicit", "export", "extern", "false", "float", "for", "friend", "goto", "if", "inline", "int", "long", "mutable", "namespace", "new", "noexcept", "not", "not_eq", "nullptr", "operator", "or", "or_eq", "private", "protected", "public", "register", "reinterpret_cast", "return", "short", "signed", "sizeof", "static", "static_assert", "static_cast", "struct", "switch", "template", "this", "thread_local", "throw", "true", "try", "typedef", "typeid", "typename", "union", "unsigned", "using", "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq", "override", "final" };
        public string[] Operators => new[] { "(", ")", "[", "]", "{", "}", "<", ">", "::", ".", "->", "++", "--", "...", ".*", "->", ";", "=", "?", ":", "#", ">=", "<=", "==", "!=", "+", "-", "*", "/", "%", "+=", "-=", "*=", "/=", "%=", "<<", "<<=", ">>", ">>=", "~", "~=", "^", "^=", "|", "|=", "&", "&=", "&&", "||", ",", "->*" };

        private ILexerFactory _lexerFactory;

        public CppLexer(ILexerFactory lexerFactory)
        {
            _lexerFactory = lexerFactory;
        }

        public ICollection<int> Parse(string code)
        {
            // base initialize
            ICollection<int> resTokens = new List<int>();
            AntlrInputStream inputStream = new AntlrInputStream(code);
            var cppLexer = _lexerFactory.Create(inputStream);

            var tokens = cppLexer.GetAllTokens();

            foreach (var token in tokens)
            {
                // trying to get type of the token
                // keyword?
                if (KeyWords.Contains(token.Text))
                {
                    resTokens.Add(LexerConstants.KeyWordsStart + Array.IndexOf(KeyWords, token.Text));
                }
                // operator?
                else if (Operators.Contains(token.Text))
                {
                    resTokens.Add(LexerConstants.OpStart + Array.IndexOf(Operators, token.Text));
                }
                // string/char?
                else if (token.Text.Last() == '\"' || token.Text.Last() == '\'')
                {
                    resTokens.Add(LexerConstants.String);
                }
                else if (char.IsDigit(token.Text.First()))
                {
                    // int?
                    if (token.Text.Contains('.'))
                    {
                        resTokens.Add(LexerConstants.Real);
                    }
                    // real?
                    else
                    {
                        resTokens.Add(LexerConstants.Number);
                    }
                }
                // id?
                else if (char.IsLetter(token.Text.First()) || token.Text.First() == '_')
                {
                    resTokens.Add(LexerConstants.Id);
                }
                // ?????
                else
                {
                    resTokens.Add(LexerConstants.Unknown);
                }
            }

            return resTokens;
        }
    }
}
