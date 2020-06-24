using System.Collections.Generic;
using System.IO;
using ALS.CheckModule.Compare.Preparer;

namespace ALS.CheckModule.Component.Preparer
{
    public class FilePreparer: IPreparer
    {
        public void Prepare(string pathToUserProgram, string pathToModelProgram, List<string> input, ref bool isStdInput)
        {
            //Скажем системе чтобы она не использовала стандартный ввод
            isStdInput = false;
            //Создадим пути до входных файлов
            var userFile = GetFilePath(pathToUserProgram);
            var modelFile = GetFilePath(pathToModelProgram);
            //Заполним файл тестовыми данными
            FillFile(userFile, input);
            FillFile(modelFile, input);
        }
        
        //Функция записи в файл
        private void FillFile(string pathToFile, List<string> input)
        {
            //Используем поток на запись в стиле C# 8
            using var sw = new StreamWriter(pathToFile);
            foreach (var item in input)
            {
                //На каждой строке запишем файл
                sw.WriteLine(item);
            }
        }

        //Функция для получения входного файла
        private string GetFilePath(string pathToProgram)
        {
            return Path.Combine(Directory.GetParent(pathToProgram).ToString(), "input.txt");
        }
    }
}