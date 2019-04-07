/* MeshEditor
 * Copyright (C) 2017 
 * Written by Josef Pelikan, extended by Gabriela Dvorakova
 * MeshEditor is a user friendly editor, that can 
 * import 3D meshes, enables to choose from several 
 * view options, select features and export selected features 
 * in PLY and OBJ file formats.
 * 
 * Available for use with the authors' permission
 * <pepca@cgg.mff.cuni.cz>
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Windows.Forms;

namespace _101meshEditor
{
  static class Program
  {
    /// <summary>
    /// Optional data initialization.
    /// </summary>
    public static void InitParams ( out string param, out string branch )
    {
      param   = "";
      branch  = "main";
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main ()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault( false );
      Application.Run( new Form1() );
    }
  }
}
