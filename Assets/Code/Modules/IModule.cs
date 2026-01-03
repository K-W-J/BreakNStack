using Code.Agents;

namespace Code.Modules
{
    public interface IModule
    {
        void InitializeComponent(ModuleOwner owner);
    }
}