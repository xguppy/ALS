namespace ALS.CheckModule.Processes
{
    public interface IExecutable
    {
        /// <summary>
        /// Запуск процесса
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        bool Execute(int timeMilliseconds);
    }
}