using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    /// <summary>
    /// Interface that is implemented by 'Concrete Commands'
    /// </summary>
    abstract class Command
    {
        public abstract void Execute();
    }
}
