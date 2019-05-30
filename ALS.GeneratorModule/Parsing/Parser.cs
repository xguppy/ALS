using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Generator.Parsing
{
    class Parser
    {
        /*названия блоков*/
        private const string _storage = "ХРАНИЛИЩЕ_ОБЪЕКТОВ";
        private const string _template = "ШАБЛОННЫЙ_ВИД";
        private const string _solution = "РЕШЕНИЕ";
        private const string _service  = "СЛУЖЕБНОЕ";
        private const string _tests    = "ТЕСТОВЫЕ_ДАННЫЕ";
        /*настройки в блоке СЛУЖЕБНОЕ*/
        private const string _arithmSigns = "знаки_арифм";
        private const string _arithmFuncs = "функции_арифм";

        
        
        private List<DataContainer> _storageData;
        private List<DataContainer> _testData;
        private string _templateStr, _code;
        public  GenData GenData;
        
        public Parser()
        {
            _storageData = new List<DataContainer>();
        }
        
        public GenData Read(string fileName)
        {
            _code = "";
            _templateStr = "";
            try
            {
                string text;
                using (StreamReader s = new StreamReader(fileName))
                {
                    text = s.ReadToEnd();
                }
                Regex r = new Regex(@"----*\n");
                text = r.Replace(text, "---");
                var splitedText = text.Split("---");
                foreach (string str in splitedText)
                {
                    var s = str.Trim(' ', '\n', '\r');
                    if (s.Length == 0) continue;
                    var pos = s.IndexOf('\n'); 
                    var head = s.Substring(0, pos);
                    var body = s.Remove(0, pos);
                    Parse(head, body);
                }
    
                if (_storageData.Count > 0 && _templateStr.Length > 0 && _code.Length > 0)
                {
                    GenData = new GenData(_storageData, _templateStr, _code, _testData);   
                }
            }
            catch (Exception)
            {
                throw new Exception ("Error during parsing!");
            }

            return GenData;
        }
        
        private void Parse(string head, string body)
        {
            switch (head.Trim(' ', '\r').ToUpper())
            {
                case _storage :
                    GetStorageData(body);
                    break;
                case _template:
                    GetTemplateData(body);
                    break;
                case _service:
                    GetServiceData(body);
                    break;
                case _solution:
                    GetCode(body);
                    break;
                case _tests:
                    GetTestsD(body);
                    break;
                default:
                    //throw new Exception("Блок не имеет обозначения");
                    break;
                    
            }
        }

        private List<DataContainer> GetDCFromBody(string body)
        {
            List<DataContainer> sd = new List<DataContainer>();
            var lines = body.Split(';');
            
            foreach (string v in lines)
            {
                List<string> data = new List<string>();
                var parts = v.Split(':');
                if (parts.Length < 2) continue;
                var rightPart = parts[1].Split(',');
                foreach (var s in rightPart)
                {
                    if (s.Length == 0) continue;
                    data.Add(s.Trim(' ', '\n', '\r'));
                }

                if (data.Count > 0)
                {
                    sd.Add(new DataContainer(parts[0].Trim(' ', '\r', '\n'), data));
                }
            }

            return sd;
        }
        
        private void GetStorageData(string body)
        {
            _storageData = GetDCFromBody(body);
        }

        private void GetTemplateData(string body)
        {
            _templateStr = body.Trim(' ', '\r', '\n');
        }
        
        private void GetCode (string body)
        {
            _code = body.Trim(' ', '\r', '\n');
        }
        
        private void GetServiceData(string body)
        {
            var sd = GetDCFromBody(body);

            foreach (var d in sd)
            {
                switch (d.Name.ToLower())
                {
                    case _arithmFuncs:
                        Elems.SetFuncs(d.Data);
                        break;
                    case _arithmSigns:
                        Elems.SetSigns(d.Data);
                        break;
                    default:
                        break;
                }
            }
            //throw new NotImplementedException();
        }
        
        private void GetTestsD(string body)
        {
            _testData = GetDCFromBody(body);
        }
        
    }
}
