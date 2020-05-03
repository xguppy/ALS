using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALS.CheckModule.Compare
{
    public abstract class ComponentList<T>
    {
        protected static readonly Dictionary<string, T> Actions = new Dictionary<string, T>();
        private static CheckModuleLoadContext _loadContext;
        private static WeakReference _weakReference;
        private static ComponentAssemblyManager _manager = new ComponentAssemblyManager();
        static ComponentList()
        {
            var result = ModuleGovernor.BuildModule(GetComponentName()).Result;
            if (!result)
            {
                throw new Exception($"Невозможно инициализировать компоненты {GetComponentName()}");
            }
            ModuleGovernor.AllowBuild();
            FillActions();
        }
        protected abstract string TemplateComponent { get; set; }
        
        protected abstract string GetPathToSource();

        public virtual T Get(string name)
        {
            //Если компонент не задан применим стандартный
            name ??= TemplateComponent.Split('.')[0];
            //Если компонента нет в словаре бросим исключение
            if (!Actions.ContainsKey(name))
            {
                throw new Exception("Выбранного компонента не существует");
            }
            return Actions[name];
        }
        protected static string GetComponentName() => typeof(T).Name.Remove(0, 1);
        public async Task<bool> ReloadActions()
        {
            if (Actions.Count != 0)
            {
                Actions.Clear();
                await _manager.Unload(_weakReference, _loadContext);
            }
            var result = await ModuleGovernor.BuildModule(GetComponentName());
            if(result)
            {
                _loadContext = new CheckModuleLoadContext();
                FillActions();
            }
            return result;
        }

        private static void FillActions()
        {
            //Получим сборку
            var checkModulePath = Path.Combine(ModuleGovernor.GetPathToModule(GetComponentName()), "bin", "Debug", "netcoreapp3.1", $"ALS.CheckModule.Component.{GetComponentName()}.dll");
            var checkModuleAssembly = _manager.Execute(out _weakReference, out _loadContext, checkModulePath);
            //Получим все действия пользователя
            var checkersAvailable = checkModuleAssembly.GetTypes().Where(t => t.IsClass && typeof(T).IsAssignableFrom(t));
            //Соберем(или обновим) словарь действий
            foreach (var action in checkersAvailable)
            {
                if (!Actions.ContainsKey(action.Name))
                {
                    Actions.Add(action.Name, (T)Activator.CreateInstance(action));
                }
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
            if (!IsDeletable(fileName))
            {
                throw new Exception("Компонент невозможно удалить");
            }
            var sourceCode = await GetText(fileName);
            File.Delete(GetPathToFile(fileName));
            return sourceCode;
        }
        public string GetPathToFile(string fileName)
            => Path.Combine(GetPathToSource(), fileName);

            private async Task<string> GetText(string fileName)
        {
            using var fileStreamReader = new StreamReader(GetPathToFile(fileName));
            var sourceCode = await fileStreamReader.ReadToEndAsync();
            return sourceCode;
        }
        
        public async Task Add(string code, string fileName)
        {
            await using var fileStreamWriter = new StreamWriter(GetPathToFile(fileName));
            await fileStreamWriter.WriteAsync(code);
        }
        
        private bool IsDeletable(string filename)
            => filename != TemplateComponent;
    }
}