using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class MParmorT_ModulesTracker(IMParmorInstance instance) : IExposable
{
    public IMParmorInstance instance = instance;

    private List<Module> loadedModules = [];

    private Dictionary<string, Module> modules;

    public bool AnyModules => loadedModules.Count > 0;

    public string NamesOfModules
    {
        get
        {
            var text = "";
            foreach (var loadedModule in loadedModules)
            {
                if (!text.NullOrEmpty())
                {
                    text += ",";
                }

                text += loadedModule.def.label;
            }

            return text;
        }
    }

    private Dictionary<string, Module> Modules
    {
        get
        {
            if (modules != null)
            {
                return modules;
            }

            modules = new Dictionary<string, Module>();
            foreach (var loadedModule in loadedModules)
            {
                modules.Add(loadedModule.def.defName, loadedModule);
            }

            return modules;
        }
    }

    public string TextOnGizmo
    {
        get
        {
            var text = "";
            foreach (var loadedModule in loadedModules)
            {
                var textOnGizmo = loadedModule.TextOnGizmo;
                if (textOnGizmo.NullOrEmpty())
                {
                    continue;
                }

                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }

                text += textOnGizmo;
            }

            return text;
        }
    }

    public void ExposeData()
    {
        Scribe_Collections.Look(ref loadedModules, "loadedModules", LookMode.Deep);
        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        foreach (var loadedModule in loadedModules)
        {
            loadedModule.tracker = this;
        }
    }

    public bool TryLoadNewModule(ModuleDef module)
    {
        if (TryGetModule<Module>(module.defName, out _))
        {
            return false;
        }

        if (Activator.CreateInstance(module.moduleClass) is not Module module3)
        {
            return true;
        }

        module3.tracker = this;
        module3.def = module;
        loadedModules.Add(module3);
        modules.Add(module.defName, module3);
        module3.Installed();

        return true;
    }

    public bool TryGetModule<T>(string moduleName, out T module) where T : Module
    {
        module = null;
        if (!Modules.TryGetValue(moduleName, out var value))
        {
            return false;
        }

        module = value as T;
        return true;
    }

    public void TickCore()
    {
        foreach (var loadedModule in loadedModules)
        {
            loadedModule.TickCore();
        }
    }

    public void TickBuilding()
    {
        foreach (var loadedModule in loadedModules)
        {
            loadedModule.TickBuilding();
        }
    }

    public void DrawCore(Vector3 vector3)
    {
        foreach (var loadedModule in loadedModules)
        {
            loadedModule.DrawCore(vector3);
        }
    }

    public void DrawBuilding(Vector3 vector3)
    {
        foreach (var loadedModule in loadedModules)
        {
            loadedModule.DrawBuilding(vector3);
        }
    }
}