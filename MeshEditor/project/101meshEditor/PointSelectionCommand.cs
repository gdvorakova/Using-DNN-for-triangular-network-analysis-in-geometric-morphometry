using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    /// <summary>
    /// The command that executes point select operation
    /// </summary>
    class PointSelectionCommand : UndoableCommand
    {
        private Selection selection;
        private Vector2d uvSpot;
        private int triangleIndex;

        // Snapshot of previous state before calling execute
        private List<int> pastSelectedPoints;

        public PointSelectionCommand(Selection selection, Vector2d uvSpot, int triangleIndex)
        {
            this.selection = selection;
            this.uvSpot = uvSpot;
            this.triangleIndex = triangleIndex;
        }

        // Call method on selection instance object
        public override void Execute()
        {
            pastSelectedPoints = new List<int>(selection.SelectionPointers);
            selection.SelectPoint(uvSpot, triangleIndex);
        }

        // Return previous state
        public override void Undo()
        {
            selection.SelectionPointers = new List<int>(pastSelectedPoints);
        }
    }
}
