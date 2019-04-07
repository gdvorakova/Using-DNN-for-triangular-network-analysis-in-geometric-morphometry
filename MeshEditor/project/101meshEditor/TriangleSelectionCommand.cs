using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    /// <summary>
    /// The command that executes triangle select operation
    /// </summary>
    class TriangleSelectionCommand : UndoableCommand
    {
        private Selection selection;
        private int triangleIndex;

        // Snapshot of previous state before calling execute
        private List<int> pastSelectedPoints;

        public TriangleSelectionCommand(Selection selection, int triangleIndex)
        {
            this.selection = selection;
            this.triangleIndex = triangleIndex;
        }

        // Call method on selection instance object
        public override void Execute()
        {
            pastSelectedPoints = new List<int>(selection.SelectionPointers);
            selection.SelectTriangle(triangleIndex);
        }

        // Return previous state
        public override void Undo()
        {
            selection.SelectionPointers = new List<int>(pastSelectedPoints);
        }
    }
}
