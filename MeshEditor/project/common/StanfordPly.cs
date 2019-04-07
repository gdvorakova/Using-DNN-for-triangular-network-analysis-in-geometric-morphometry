/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using OpenTK;
using System.Threading;

namespace Scene3D
{
    class StanfordPly
    {
        #region Constants

        /// <summary>
        /// The first line (including the ending character CR) is used as a 4-byte magic number.
        /// </summary>
        const string HEADER = "ply";

        /// <summary>
        /// Comments are introduced by this string.
        /// </summary>
        const string COMMENT = "comment";

        const string FORMAT = "format";

        const string FORMAT_TEXT = "format ascii 1.0";

        const string FORMAT_BINARY_LE = "format binary_little_endian 1.0";

        const string FORMAT_BINARY_BE = "format binary_big_endian 1.0";

        const string ELEMENT = "element";

        const string VERTEX = "vertex";

        const string FACE = "face";

        const string PROPERTY = "property";

        const string NORMAL_X = "nx";

        const string NORMAL_Y = "ny";

        const string NORMAL_Z = "nz";

        const string TEXTURE_S = "s";

        const string TEXTURE_T = "t";

        const string COLOR_R = "red";

        const string COLOR_G = "green";

        const string COLOR_B = "blue";

        const string END_HEADER = "end_header";

        public static char[] DELIMITERS = { ' ', '\t' };

        #endregion

        #region Instance data
        /// <summary>
        /// Element vertex defines vertex properties   
        /// </summary>
        private List<ElementType> elements;

        /// <summary>
        /// Format of the file can be
        /// Ascii 1.0, format binary_little_endian 1.0, format binary_big_endian 1.0
        /// </summary>
        private Format format;

        /// <summary>
        /// Reads header
        /// </summary>
        private StreamReader headerReader;

        /// <summary>
        /// Reads data in binary file format
        /// </summary>
        private BinaryReader binaryReader;

        /// <summary>
        /// Reads data in ascii file format
        /// </summary>
        private StreamReader textReader;

        private SceneBrep scene;

        public bool DoNormals { get; set; }

        public bool DoTxtCoords { get; set; }

        public bool DoColors { get; set; }

        public Matrix4 matrix = Matrix4.Identity;

        /// <summary>
        /// Change axis orientation in import/export.
        /// </summary>
        public bool Orientation { get; set; }

        public bool TextFormat { get; set; }

        public bool NativeNewLine { get; set; }


        #endregion

        #region Enums
        public enum Format
        {
            ASCII, BINARY_LE, BINARY_BE
        }
        public enum PropertyType
        {
            // 1 byte signer integer
            CHAR,
            // 1 byte unsigned integer
            UCHAR,
            // 2 byte signed integer
            SHORT,
            // Two byte unsigned integer
            USHORT,
            // Four byte signed integer
            INT,
            // Four byte unsigned integer
            UINT,
            // four byte floating point number
            FLOAT,
            // Eight byte byte floating point number
            DOUBLE
        }

        #endregion

        public StanfordPly ( )
        {            

            Orientation = false;
            TextFormat = true;
            NativeNewLine = true;
            DoNormals = false;
            DoTxtCoords = false;
            DoColors = false;
        }

        public int ReadBrep ( string fileName, SceneBrep scene )
        {
            Debug.Assert( scene != null );

            this.scene = scene;

            scene.Reset();

            if ( fileName == null || fileName.Length == 0 )
                throw new IOException( "Invalid file name" );

            if ( fileName.EndsWith( ".gz" ) )
                headerReader = new StreamReader( new GZipStream( new FileStream( fileName, FileMode.Open ), CompressionMode.Decompress ) );
            else
            {
                var fs = new FileStream( fileName, FileMode.Open );
                headerReader = new StreamReader( fs );
            }


            // prepare buffers for data filling
            if ( !ParseHeader() ) return -1;

            // read vertices
            var vertexReader = GetReader( "vertex" );
            var element = vertexReader.ReadElement();

            List<Vector2> txtCoords = new List<Vector2>( 256 );
            int[] f = new int[ 3 ];
            int v0 = scene.Vertices;
            int lastVertex = v0 - 1;

            while ( element != null )
            {
                lastVertex = scene.AddVertex( Vector3.TransformPosition( element.GetVertex(), matrix ) );

                if ( DoNormals )
                {
                    Vector3.TransformNormal( element.GetNormal(), matrix );

                    scene.SetNormal( lastVertex, element.GetNormal() );
                }

                if ( DoTxtCoords )
                {
                    scene.SetTxtCoord( lastVertex, element.GetTextureCoordinate() );
                }

                if ( DoColors )
                {
                    scene.SetColor( lastVertex, element.GetVertexColor() );
                }

                element = vertexReader.ReadElement();
            }            

            // read triangles
            var faceReader = GetReader( "face" );
            element = faceReader.ReadElement();

            while ( element != null )
            {
                int A, B, C;
                element.GetTriangleVertices( out A, out B, out C );

                scene.AddTriangle( A, B, C );

                element = faceReader.ReadElement();
            }

            headerReader.Close();
            vertexReader.Close();
            faceReader.Close();

            return scene.Triangles;

        }

        /// <summary>
        /// Reads the header of .ply file and prepares data structures
        /// for data filling.
        /// </summary>
        private bool ParseHeader ()
        {
            // .ply file header must start with "ply"
            string magic = headerReader.ReadLine();
            if ( magic != HEADER ) return false;


            elements = new List<ElementType>();

            // last defined element type is used to add property deifnitions
            ElementType element = null;
            
            string line;
            while ( (line = headerReader.ReadLine()) != null )
            {
                // ignore comments
                if ( line.StartsWith( COMMENT ) ) continue;
                if ( line.StartsWith( FORMAT ) )
                {
                    if ( line == "format ascii 1.0" ) format = Format.ASCII;
                    else if ( line == "format binary_little_endian 1.0" ) format = Format.BINARY_LE;
                    else if ( line == "format binary_big_endian 1.0" ) format = Format.BINARY_BE;
                }
                else if ( line.StartsWith( ELEMENT ) )
                {
                    string[] tokens = line.Split( DELIMITERS, StringSplitOptions.RemoveEmptyEntries );
                    string name = tokens[ 1 ];
                    int count = int.Parse( tokens[ 2 ] );

                    if ( name == VERTEX )
                    {
                        var vertex = new ElementType( count, VERTEX );
                        elements.Add( vertex );
                        element = vertex;
                    }
                    else if ( name == FACE )
                    {
                        var face = new ElementType( count, FACE );
                        elements.Add( face );
                        element = face;
                    }
                }
                else if ( line.StartsWith( PROPERTY ) )
                {

                    string[] tokens = line.Split( DELIMITERS, StringSplitOptions.RemoveEmptyEntries );

                    // list properties define a number of vertices in a face of an element                   
                    if ( tokens[ 1 ] == "list" )
                    {
                        Property property1 = new Property( tokens[ 1 ], tokens[ 2 ] );
                        Property property2 = new Property( tokens[ 4 ], tokens[ 3 ] );

                        // add properties to currently read element
                        element.Properties.Add( property1 );
                        element.Properties.Add( property2 );
                    }
                    // basic property defines element with a single value
                    else
                    {
                        if ( tokens[ 2 ] == NORMAL_X ) DoNormals = true;
                        else if ( tokens[ 2 ] == TEXTURE_S ) DoTxtCoords = true;
                        else if ( tokens[ 2 ] == COLOR_R ) DoColors = true;

                        Property property = new Property( tokens[ 2 ], tokens[ 1 ] );

                        element.Properties.Add( property );
                    }

                }
                else if ( line.StartsWith( END_HEADER ) )
                    break;
                else return false;                

            }
            
            // prepare reader
            PrepareReader();
            return true;

        }

        private void PrepareReader ()
        {
            switch ( format )
            {
                case Format.ASCII:
                    textReader = headerReader;
                    binaryReader = null;
                    break;
                case Format.BINARY_LE:
                    binaryReader = new BinaryReader( headerReader.BaseStream );
                    textReader = null;
                    break;
                case Format.BINARY_BE:
                    binaryReader = new BinaryReader( headerReader.BaseStream );
                    textReader = null;
                    break;
                default:
                    break;
            }
        }

        private IElementReader GetReader ( string elementName )
        {
            ElementType element = null;
            foreach ( var item in elements )
            {
                if ( item.Name == elementName )
                {
                    element = item;
                    break;
                }

            }
            switch ( format )
            {
                case Format.ASCII:
                    return new TextElementReader( textReader, element );
                case Format.BINARY_LE:
                case Format.BINARY_BE:
                    return new BinElementReader( binaryReader, element, format );
            }

            return null;
        }

        /// <summary>
        /// Writes the whole B-rep scene to a given text stream (uses text variant of Stanford PLY format).
        /// </summary>
        /// <param name="writer">Already open text writer</param>
        /// <param name="scene">Scene to write</param>
        public void WriteBrep ( StreamWriter writer, SceneBrep scene )
        {
            DoNormals = true;
            DoTxtCoords = true;
            DoColors = true;

            if ( scene == null ||
                 scene.Triangles < 1 )
                return;

            Debug.Assert( TextFormat );
            if ( !NativeNewLine )
                writer.NewLine = "\r";     // CR only

            bool writeNormals = DoNormals && scene.HasNormals();
            bool writeTxtCoords = DoTxtCoords && scene.HasTxtCoords();
            bool writeColors = DoColors && scene.HasColors();

            writer.WriteLine( HEADER );
            writer.WriteLine( FORMAT_TEXT );

            // vertex-header:
            writer.WriteLine( "{0} {1} {2}", ELEMENT, VERTEX, scene.Vertices );
            writer.WriteLine( "{0} float x", PROPERTY );
            writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'z' : 'y' );
            writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'y' : 'z' );
            if ( writeNormals )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, NORMAL_X );
                writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Z : NORMAL_Y );
                writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Y : NORMAL_Z );
            }
            if ( writeTxtCoords )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_S );
                writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_T );
            }
            if ( writeColors )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_R );
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_G );
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_B );
            }

            // face-header:
            writer.WriteLine( "{0} {1} {2}", ELEMENT, FACE, scene.Triangles );
            writer.WriteLine( "{0} list uchar int vertex_indices", PROPERTY );

            writer.WriteLine( END_HEADER );

            // vertex-data:
            int i;
            Vector3 v3;
            Vector2 v2;
            StringBuilder sb = new StringBuilder();
            for ( i = 0; i < scene.Vertices; i++ )
            {
                v3 = scene.GetVertex( i );
                sb.Clear();
                sb.AppendFormat( CultureInfo.InvariantCulture, "{0} {1} {2}", v3.X, v3.Y, v3.Z );
                if ( writeNormals )
                {
                    v3 = scene.GetNormal( i );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
                }
                if ( writeTxtCoords )
                {
                    v2 = scene.GetTxtCoord( i );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1}", v2.X, v2.Y );
                }
                if ( writeColors )
                {
                    v3 = scene.GetColor( i );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
                }
                writer.WriteLine( sb.ToString() );
            }

            // face-data:
            int A, B, C;
            for ( i = 0; i < scene.Triangles; i++ )
            {
                scene.GetTriangleVertices( i, out A, out B, out C );
                writer.WriteLine( "3 {0} {1} {2}", A, Orientation ? C : B, Orientation ? B : C );
            }
        }

        /// <summary>
        /// Writes the whole B-rep scene to a given text stream (uses text variant of Stanford PLY format).
        /// </summary>
        /// <param name="writer">Already open text writer</param>
        /// <param name="scene">Scene to write</param>
        public void WriteBrepSelection ( StreamWriter writer, SceneBrep scene, Selection selection, 
            IProgress<string> progress, CancellationToken cancel, out List<int> trianglePointers, out List<int> vertexPointers )
        {
            DoNormals = true;
            DoTxtCoords = true;
            DoColors = true;

            vertexPointers = new List<int>();
            trianglePointers = new List<int>();

            if ( scene == null || selection.GetCount() < 3 ) return;

            vertexPointers = selection.SelectionPointers;

            Debug.Assert( TextFormat );
            if ( !NativeNewLine )
                writer.NewLine = "\r";     // CR only

            bool writeNormals = DoNormals && scene.HasNormals();
            bool writeTxtCoords = DoTxtCoords && scene.HasTxtCoords();
            bool writeColors = DoColors && scene.HasColors();

            writer.WriteLine( HEADER );
            writer.WriteLine( FORMAT_TEXT );

            // vertex-header:
            writer.WriteLine( "{0} {1} {2}", ELEMENT, VERTEX, selection.GetCount() );
            writer.WriteLine( "{0} float x", PROPERTY );
            writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'z' : 'y' );
            writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? 'y' : 'z' );
            if ( writeNormals )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, NORMAL_X );
                writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Z : NORMAL_Y );
                writer.WriteLine( "{0} float {1}", PROPERTY, Orientation ? NORMAL_Y : NORMAL_Z );
            }
            if ( writeTxtCoords )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_S );
                writer.WriteLine( "{0} float {1}", PROPERTY, TEXTURE_T );
            }
            if ( writeColors )
            {
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_R );
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_G );
                writer.WriteLine( "{0} float {1}", PROPERTY, COLOR_B );
            }

            var triangles = selection.GetSelectedTriangles();
            var list = new List<int>();

            int A, B, C;
            for ( int j = 0; j < triangles.Count; j++ )
            {
                scene.GetTriangleVertices( triangles[ j ], out A, out B, out C );
                int Anew = selection.SelectionPointers.IndexOf( A );
                int Bnew = selection.SelectionPointers.IndexOf( B );
                int Cnew = selection.SelectionPointers.IndexOf( C );

                // if this vertex was not selected, do not export the triangle containing it
                if ( Anew == -1 || Bnew == -1 || Cnew == -1 ) continue;

                // save vertex indices of triangles that were selected
                // this supports bach export
                trianglePointers.Add( triangles[ j ] );

                // otherwise add vertices with their new indices to list
                list.Add( Anew );
                list.Add( Bnew );
                list.Add( Cnew );

                cancel.ThrowIfCancellationRequested();
            }

            // face-header:
            writer.WriteLine( "{0} {1} {2}", ELEMENT, FACE, list.Count / 3 );
            writer.WriteLine( "{0} list uchar int vertex_indices", PROPERTY );

            writer.WriteLine( END_HEADER );

            // vertex-data:
            int i;
            Vector3 v3;
            Vector2 v2;
            StringBuilder sb = new StringBuilder();
            var selectionEnum = selection.GetEnumerator();

            int totalCount = selection.GetCount() + list.Count / 3;

            for ( i = 0; i < selection.GetCount(); i++ )
            {
                cancel.ThrowIfCancellationRequested();

                selectionEnum.MoveNext();
                v3 = scene.GetVertex( selectionEnum.Current );               

                sb.Clear();
                sb.AppendFormat( CultureInfo.InvariantCulture, "{0} {1} {2}", v3.X, v3.Y, v3.Z );
                if ( writeNormals )
                {
                    v3 = scene.GetNormal( selectionEnum.Current );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
                }
                if ( writeTxtCoords )
                {
                    v2 = scene.GetTxtCoord( selectionEnum.Current );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1}", v2.X, v2.Y );
                }
                if ( writeColors )
                {
                    v3 = scene.GetColor( selectionEnum.Current );
                    sb.AppendFormat( CultureInfo.InvariantCulture, " {0} {1} {2}", v3.X, v3.Y, v3.Z );
                }
                writer.WriteLine( sb.ToString() );                

                if ( progress != null )
                {                    
                    progress.Report( (i * 100 / totalCount).ToString() );
                }

            }

            // face-data:         
            for ( i = 0; i < list.Count; i+=3 )
            {  
                writer.WriteLine( "3 {0} {1} {2}", list[ i ], Orientation ? list[ i + 2 ] : list[ i + 1 ], Orientation ? list[ i + 1 ] : list[ i + 2 ] );

                if ( progress != null )
                {
                    progress.Report( ( (i/3 + selection.GetCount()) * 100 / totalCount).ToString() );
                }

                cancel.ThrowIfCancellationRequested();
            }
        }


    }
}
