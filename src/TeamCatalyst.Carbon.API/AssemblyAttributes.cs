using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCatalyst.Carbon.API
{
    /// <summary>
    /// Attribute that specifies that the assembly is a Carbon module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute() { }
    }
}
