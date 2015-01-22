using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IFeatureDescriberProvider
    {
        IEnumerable<IFeatureDescriber> Describers { get; }
    }

    class FeatureDescriberProvider : IFeatureDescriberProvider
    {
        protected virtual HashSet<string> ReferenceAssemblies { get; } = new HashSet<string>(StringComparer.Ordinal) { typeof(IFeature).GetTypeInfo().Assembly.FullName };

        private readonly ILibraryManager _libraryManager;

        public FeatureDescriberProvider(ILibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        /// <inheritdoc />
        public IEnumerable<IFeatureDescriber> Describers
        {
            get
            {
                return FeatureDescriber.Fixup(
                    GetCandidateLibraries()
                        .SelectMany(l => l.LoadableAssemblies)
                        .Select(Load)
                        .SelectMany(x => x.DefinedTypes)
                        .Where(x => x.ImplementedInterfaces.Contains(typeof(IFeature)))
                        .Select(FeatureDescriber.Create));
            }
        }

        protected virtual IEnumerable<ILibraryInformation> GetCandidateLibraries()
        {
            if (ReferenceAssemblies == null)
            {
                return Enumerable.Empty<ILibraryInformation>();
            }

            return ReferenceAssemblies.SelectMany(_libraryManager.GetReferencingLibraries)
                                      .Distinct()
                                      .Where(IsCandidateLibrary);
        }

        private static Assembly Load(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        private bool IsCandidateLibrary(ILibraryInformation library)
        {
            Debug.Assert(ReferenceAssemblies != null);
            return !ReferenceAssemblies.Contains(library.Name);
        }
    }
}
