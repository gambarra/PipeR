﻿using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PipelineR
{
    public class ContextConverter : JsonObjectCreationConverter<BaseContext>
    {
        public ContextConverter()
        {
            var contexts = this.GetAssemblies().SelectMany(p => p.GetTypes())
                .Where(p => p.GetTypeInfo().BaseType == typeof(BaseContext));

            this.Property = "Id";
            this.TypesMapping = new Dictionary<string, Type>();

            foreach (var context in contexts)
            {
                this.TypesMapping.Add(context.FullName, context.UnderlyingSystemType);
            }
        }

        private Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries.Where(p => p.Type.Equals("Project", StringComparison.CurrentCultureIgnoreCase));
            foreach (var library in dependencies)
            {
                var name = new AssemblyName(library.Name);
                var assembly = Assembly.Load(name);
                assemblies.Add(assembly);
            }
            return assemblies.ToArray();
        }
    }
}
