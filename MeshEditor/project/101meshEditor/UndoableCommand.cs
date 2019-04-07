using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    abstract class UndoableCommand : Command
    {
        public abstract void Undo();
    }
}
