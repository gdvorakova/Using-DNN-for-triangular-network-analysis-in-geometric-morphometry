/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;

namespace _101meshEditor
{
    class ExportBatch
    {
        #region Constants

        const string PLY = ".ply";

        const string OBJ = ".obj";

        const string FORMAT_TEXT = "format ascii 1.0";

        #endregion

        /// <summary>
        /// Directory, where the batch will be saved
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// Extension to export this batch of files
        /// </summary>
        public string Extension { get; set; }

        List<int> _vertexPointers { get; set; }
        
        List<int> _trianglePointers { get; set; }        

        /// <summary>
        /// Method handler depending on target extension
        /// </summary>
        delegate void extensionHandler ( StreamReader reader, StreamWriter writer, CancellationToken cancelSource );

        public ExportBatch ( List<int> _vertexPointers, List<int> _trianglePointers, string path, string ext )
        {
            this._vertexPointers = _vertexPointers;
            this._trianglePointers = _trianglePointers;
            this.TargetPath = path;
            this.Extension = ext;
        }

        /// <summary>
        /// Searches for .obj and .ply files recursively
        /// and for each file executes a task that exports 
        /// the selection from them
        /// Meshes from thse files need to be topologically unified,
        /// otherwise exporting the selection will not work.
        /// </summary>
        public async void ProcessDirectory ( IProgress<string> progress, CancellationToken cancelSource, Action report )
        {
            // call method depending on selected extension
            extensionHandler handler = null;

            var tasks = new List<Task>(); 

            switch ( Extension )
            {
                case PLY:
                    handler = new extensionHandler( WriteSelectionPly );
                    break;
                case OBJ:
                    handler = new extensionHandler( WriteSelectionObj );
                    break;
                default:
                    MessageBox.Show( "{0} extension is not supported", Extension );
                    break;
            }

            var fbd = new FolderBrowserDialog();


            DialogResult result = fbd.ShowDialog();    


            string path = string.Empty;
            if ( result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
            {
                path = fbd.SelectedPath;
            }

            Stack<string> dirs = new Stack<string>();
            dirs.Push( path );

            while ( dirs.Count > 0 )
            {
                string currentDir = dirs.Pop();
                string[] subDirs = null;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories( currentDir );
                }

                catch ( UnauthorizedAccessException )
                {
                    continue;
                }
                catch ( System.IO.DirectoryNotFoundException )
                {
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles( currentDir );
                }

                catch ( UnauthorizedAccessException )
                {
                    continue;
                }

                catch ( System.IO.DirectoryNotFoundException )
                {
                    continue;
                }                              
                  
                foreach ( string file in files )
                {
                    if ( file.EndsWith( ".obj" ) || file.EndsWith( ".ply" ) )
                    {
                        // Export selected points
                        var fs = new FileStream( file, FileMode.Open );
                        var name = Path.GetFileName( currentDir );
                        StreamReader reader = new StreamReader( fs );
                        StreamWriter writer = new StreamWriter( new FileStream( TargetPath + "\\" + name + Extension, FileMode.Create ) );

                        // Report name of currently processed file
                        if ( progress != null )
                        {
                            progress.Report( "Processing file " + name + ".obj..." );
                        }

                        // Execute a separate task for each file in directory
                        Task t = Task.Run( () => handler( reader, writer, cancelSource ) );
                        tasks.Add( t );
                        if ( !cancelSource.IsCancellationRequested )
                            await t;
                        else
                            report.Invoke();
                    }
                }

                foreach ( string str in subDirs )
                    dirs.Push( str );
            }

            // When all tasks are completed, report the result
            var final = Task.Factory.ContinueWhenAll( tasks.ToArray(), _ => report() );
        }

        public void WriteSelectionObj ( StreamReader reader, StreamWriter writer, CancellationToken cancelSource )
        {
            string line;
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> newVertices = new List<Vector3>();

            int vCount = 0;
            int tCount = 0;

            while ( (line = reader.ReadLine()) != null )
            {
                if ( cancelSource.IsCancellationRequested )
                    return;

                if ( line.StartsWith( "v " ) )
                {
                    Vector3 v;
                    vertices.Add( v = GetVectorCoordinates( line ) );

                    if ( _vertexPointers.Contains( vCount ) )
                    {
                        writer.WriteLine( line );
                        newVertices.Add( v );
                    }

                    vCount++;
                }
                else if ( line.StartsWith( "f" ) )
                {
                    if ( _trianglePointers.Contains( tCount ) )
                    {
                        int A, B, C;
                        GetTriangleIndices( vertices, newVertices, line, out A, out B, out C );
                        // vertex coordinates are indexed from 1 on .obj
                        writer.WriteLine( "f {0} {1} {2}", A + 1, B + 1, C + 1 );
                    }

                    tCount++;
                }

            }

            writer.Close();
            reader.Close();

        }

        private void WriteSelectionPly( StreamReader reader, StreamWriter writer, CancellationToken cancelSource )
        {
            string line;
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> newVertices = new List<Vector3>();

            int vCount = 0;
            int tCount = 0;

            // print header
            writer.WriteLine( "ply" );
            writer.WriteLine( FORMAT_TEXT );
            // vertex header
            writer.WriteLine( "element vertex {0}", _vertexPointers.Count );
            writer.WriteLine( "property float x" );
            writer.WriteLine( "property float y" );
            writer.WriteLine( "property float z" );
            // face header
            writer.WriteLine( "element face {0}", _trianglePointers.Count );
            writer.WriteLine( "property list uchar int vertex_indices");
            writer.WriteLine( "end_header" );


            while ( (line = reader.ReadLine()) != null )
            {
                if ( cancelSource.IsCancellationRequested )
                    return;

                if ( line.StartsWith( "v " ) )
                {
                    Vector3 v;
                    vertices.Add( v = GetVectorCoordinates( line ) );

                    if ( _vertexPointers.Contains( vCount ) )
                    {
                        writer.WriteLine( "{0} {1} {2}", v.X.ToString( CultureInfo.InvariantCulture ),
                            v.Y.ToString( CultureInfo.InvariantCulture ), v.Z.ToString( CultureInfo.InvariantCulture ) );
                        newVertices.Add( v );
                    }

                    vCount++;
                }
                else if ( line.StartsWith( "f" ) )
                {
                    if ( _trianglePointers.Contains( tCount ) )
                    {
                        int A, B, C;
                        GetTriangleIndices( vertices, newVertices, line, out A, out B, out C );
                        writer.WriteLine( "3 {0} {1} {2}", A, B, C );
                    }

                    tCount++;
                }

            }

            writer.Close();
            reader.Close();

        }

        /// <summary>
        /// Gets a line from ply file and parses it into a vector coordinate
        /// </summary>
        private Vector3 GetVectorCoordinates ( string line )
        {
            var cancelSources = line.Split();

            var a = float.Parse( cancelSources[ 1 ], CultureInfo.InvariantCulture );
            var b = float.Parse( cancelSources[ 2 ], CultureInfo.InvariantCulture );
            var c = float.Parse( cancelSources[ 3 ], CultureInfo.InvariantCulture );


            return new Vector3( a, b, c );
        }

        private void GetTriangleIndices ( List<Vector3> vertices, List<Vector3> newVertices, string line, out int A, out int B, out int C )
        {
            var token = line.Split();

            int a = int.Parse( token[ 1 ] ) - 1;
            int b = int.Parse( token[ 2 ] ) - 1;
            int c = int.Parse( token[ 3 ] ) - 1;

            var aa = vertices[ a ];
            var bb = vertices[ b ];
            var cc = vertices[ c ];

            A = newVertices.IndexOf( aa );
            B = newVertices.IndexOf( bb );
            C = newVertices.IndexOf( cc );
        }
    }

}
