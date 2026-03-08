using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Modules
{
    public class ModuleOwner : MonoBehaviour
    {
        private Dictionary<Type, IModule> _moduleDict;

        protected virtual void Awake()
        {
            _moduleDict = GetComponentsInChildren<IModule>().ToDictionary(m => m.GetType());
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            foreach (var module in _moduleDict.Values) 
                module.InitializeComponent(this);
        }

        public T GetModule<T>()
        {
            if (_moduleDict.TryGetValue(typeof(T), out IModule module))
                return (T) module;
            
            IModule findModule = _moduleDict.Values.FirstOrDefault(moduleType => moduleType is T);
            
            if(findModule != null && findModule is T castModule)
                return castModule;

            return default;
        }
    }
}