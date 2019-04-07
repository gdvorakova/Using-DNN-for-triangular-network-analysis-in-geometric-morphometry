using System;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    /// <summary>
    /// The command that executes brush select operation
    /// </summary>
    class BrushSelectionCommand : UndoableCommand
    {
        private Selection selection;
        private Vector2d uvSpot;
        private int triangleIndex;
        private float distance;

        // Snapshot of previous state before calling execute
        private List<int> pastSelectedPoints;

        public BrushSelectionCommand(Selection selection, Vector2d uvSpot, int triangleIndex, float distance)
        {
            this.selection = selection;
            this.uvSpot = uvSpot;
            this.triangleIndex = triangleIndex;
            this.distance = distance;
        }

        // Call method on selection instance object
        public override void Execute()
        {
            pastSelectedPoints = new List<int>(selection.SelectionPointers);
            selection.SelectBrush(uvSpot, triangleIndex, distance);
        }

        // Return previous state
        public override void Undo()
        {
            selection.SelectionPointers = new List<int>(pastSelectedPoints);
        }
    }
}
