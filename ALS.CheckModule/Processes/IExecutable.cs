using System.Threading.Tasks;

namespace ALS.CheckModule.Processes
{
    public interface IExecutable
    {
        /// <summary>
        /// Запуск процесса
        /// </summary>
        /// <param name="timeMilliseconds">Время исполнения</param>
        /// <returns>Если не удачно завершился или занял больше времени, чем было задано</returns>
        Task<bool> Execute(int timeMilliseconds);
    }
}