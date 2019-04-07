using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _101meshEditor
{
    /// <summary>
    /// Invokes commands and also enables Undo and Redo functionality
    /// </summary>
    class CommandManager
    {
        // Stacks where commands are stored
        private Stack<Scene3D.Command> undoStack;
        private Stack<Scene3D.Command> redoStack;

        public CommandManager()
        {
            undoStack = new Stack<Scene3D.Command>();
            redoStack = new Stack<Scene3D.Command>();
        }

        // Calls execute on command that is passed as argument
        public void Execute(Scene3D.Command command)
        {
            command.Execute();

            if (command is Scene3D.UndoableCommand) {
                undoStack.Push(command);
            }
        }

        // Undos last called operation
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                Scene3D.UndoableCommand command = (Scene3D.UndoableCommand)undoStack.Pop();
                redoStack.Push(command);
                command.Undo();
            }
        }

        // Redos last undoed operation
        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                Scene3D.Command command = redoStack.Pop();
                command.Execute();

                undoStack.Push(command);
            }
        }

        // Returns the number of items in undoStack
        public int UndoCount
        {
            get
            {
               return undoStack.Count;
            }  
        }

        // Returns the number of items in redoStack
        public int RedoCount
        {
            get
            {
                return redoStack.Count;
            }
        }
    }
}
