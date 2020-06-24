using System.Collections.Generic;
using System.IO;
using ALS.CheckModule.Compare.Finaliter;

namespace ALS.CheckModule.Component.Finaliter
{
    public class FileFinaliter: IFinaliter
    {
        public void Finalite(string pathToUserProgram, string pathToModelProgram)
        {
            //Получим входной и выходной файл модели и пользовательского решения
            var deletableFiles = new List<string> {GetInputTxtFile(pathToUserProgram), GetOutputTxtFile(pathToUserProgram), 
                GetInputTxtFile(pathToModelProgram), GetOutputTxtFile(pathToModelProgram)};
            //Удалим все файлы
            DeleteFiles(deletableFiles);
        }
        
        //Полулчения входного файла
        private string GetInputTxtFile(string pathToProgram)
        {
            return Path.Combine(Directory.GetParent(pathToProgram).ToString(), "input.txt");
        }
        
        //Полулчения выходного файла
        private string GetOutputTxtFile(string pathToProgram)
        {
            return Path.Combine(Directory.GetParent(pathToProgram).ToString(), "output.txt");
        }
        
        //Функция удаления файлов по их пути
        private void DeleteFiles(List<string> pathsToFiles)
        {
            foreach (var item in pathsToFiles)
            {
                File.Delete(item);
            }
        }
    }
}