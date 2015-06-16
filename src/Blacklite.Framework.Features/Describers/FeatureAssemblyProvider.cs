using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if DNX451 || DNXCORE50
using Microsoft.Framework.Runtime;
#endif

namespace Blacklite.Framework.Features.Describers
{
    public class FeatureAssemblyProvider : IFeatureAssemblyProvider
    {
        public FeatureAssemblyProvider(
#if DNX451 || DNXCORE50
            ILibraryManager libraryManager,
#endif
            IFeatureDescriberFactory factory)
        {
#if DNX451 || DNXCORE50
            var assemblies = GetAssemblies(libraryManager);
#else
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif
            Assemblies = assemblies.ToArray();
        }

        public IEnumerable<Assembly> Assemblies { get; }

#if DNX451 || DNXCORE50
        protected virtual HashSet<string> ReferenceAssemblies
        { get; }
        = new HashSet<string>(StringComparer.Ordinal) {
            "Blacklite.Framework.Features",
            "Blacklite.Framework.Multitenancy.Features",
        };
        private IEnumerable<Assembly> GetAssemblies(ILibraryManager libraryManager)
        {
            return ReferenceAssemblies.SelectMany(libraryManager.GetReferencingLibraries)
                    .Distinct()
                    .Where(IsCandidateLibrary)
                    .SelectMany(l => l.LoadableAssemblies)
                    .Select(Load);
        }

        private static Assembly Load(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        private bool IsCandidateLibrary(ILibraryInformation library)
        {
            return !ReferenceAssemblies.Contains(library.Name);
        }
#endif
    }


}
