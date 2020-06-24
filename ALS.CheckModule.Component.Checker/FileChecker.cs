using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ALS.CheckModule.Compare.Checker;
using ALS.CheckModule.Compare.DataStructures;

namespace ALS.CheckModule.Component.Checker
{
    public class FileChecker: IChecker
    {

            // Реализуемый метод из интерфейса
        public void Check (List<string> modeOutput, string pathToUserProgram, string pathToModelProgram, ref ResultRun result)
        {
            //Получим потоки чтения для пользовательской программы и эталонной
            using var srUser = new StreamReader(GetFilePath(pathToUserProgram));
            using var srModel = new StreamReader(GetFilePath(pathToModelProgram));


            //Сравним пользовательский вывод в файл и эталонный
            if (srUser.ReadToEnd() != srModel.ReadToEnd())
            {
                result.Comment = $"Выводы в файлах не совпадают";
                return;
            }
            // Если все проверки до этого были успешны запишем комментарий об успешности 
            // выполненного теста и пометим флаг IsCorret в значение true, если этого не  
            // сделать тестовый прогон будет считаться ошибочным и в следствии вся задача 
            // будет   оценена в 0 из N, где N – количество тестовых наборов 
            result.Comment = "Всё хоккей";
            result.IsCorrect = true;
        }
        //Функция для получения выходного файла
        private string GetFilePath(string pathToProgram)
        {
            return Path.Combine(Directory.GetParent(pathToProgram).ToString(), "output.txt");
        }
    }
}