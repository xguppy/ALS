using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ALS.CheckModule.Compare
{
    public abstract class ComponentList<T>
    {
        protected static readonly Dictionary<string, T> Actions = new Dictionary<string, T>();
        private static CheckModuleLoadContext _loadContext = new CheckModuleLoadContext();
        static ComponentList()
        {
            FillActions();
        }
        
        protected abstract string GetPathToSource();
        public abstract T Get(string name);
        public async Task<bool> ReloadActions()
        {
            if (Actions.Count != 0)
            {
                Actions.Clear();
                _loadContext.Unload();
            }
            var result = await ModuleGovernor.BuildCheckModule();
            if(result)
            {
                FillActions();
            }
            return result;
        }

        private static void FillActions()
        {
            //Получим сборку
            var checkModulePath = Path.Combine(ModuleGovernor.GetPathToModule(), "bin", "Debug", "netcoreapp3.1", "ALS.CheckModule.dll");
            var checkModuleAssembly = _loadContext.LoadFromAssemblyPath(checkModulePath);
            //Получим все действия пользователя
            var checkersAvailable = checkModuleAssembly.GetTypes().Where(t => t.IsClass && typeof(T).IsAssignableFrom(t));
            //Соберем(или обновим) словарь действий
            foreach (var action in checkersAvailable)
            {
                Actions.Add(action.Name, (T)Activator.CreateInstance(action));
            }
        }
        
        public List<string> GetList()
        {
            var listAction = new List<string>(Actions.Count);
            listAction.AddRange(Actions.Select(elem => elem.Key));
            return listAction;
        }
        
        public async Task<string> Delete(string fileName)
        {
            var pathDeleteFile = Path.Combine(GetPathToSource(), fileName);
            var sourceCode = await GetText(fileName);
            File.Delete(pathDeleteFile);
            return sourceCode;
        }
        
        public async Task<string> GetText(string fileName)
        {
            var filePath = Path.Combine(GetPathToSource(), fileName);
            string sourceCode;
            using (var fileStreamReader = new StreamReader(filePath))
            {
                sourceCode = await fileStreamReader.ReadToEndAsync();
            }
            return sourceCode;
        }
        
        public async Task Add(string code, string fileName)
        {
            await using var fileStreamWriter = new StreamWriter(Path.Combine(GetPathToSource(), fileName));
            await fileStreamWriter.WriteAsync(code);
        }

        
    }
}