/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using MathSupport;
using OpenglSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.IO;

namespace Scene3D
{
    /// <summary>
    /// Implementation of class that operates 
    /// on vertices of triangle mesh selected by an user
    /// </summary>
    public class Selection : IDisposable, IEnumerable<int>
    {
        #region Class data
        /// <summary>
        /// This list's contents are integer pointers 
        /// in geometry array of selected vertices
        /// </summary>
        public List<int> SelectionPointers;

        /// <summary>
        /// Current representation of loaded triangle mesh
        /// </summary>
        SceneBrep scene;

        /// <summary>
        /// Should points be selected or deselected on mouse click?
        /// </summary>
        public bool Deselect { get; set; }

        #endregion

        public Selection ( SceneBrep scene )
        {
            SelectionPointers = new List<int>();
            Deselect = false;
            this.scene = scene;
        }

        /// <summary>
        /// Returns the number of currectly selected points
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return this.SelectionPointers.Count();
        }

        /// <summary>
        /// Returns triangle pointers of selection
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedTriangles ()
        {
            List<int> list = new List<int>();

            foreach ( var p in SelectionPointers )
            {
                var tr = scene.GetTriangleFromPtr( p );
                foreach ( var item in tr )
                {
                    if ( !list.Contains( item ) )
                        list.Add( item );
                }
            }

            return list;
        }

        # region Selection handlers

        /// <summary>
        /// Marks all vertices of the triangle as selected
        /// </summary>
        /// <param name="index"> Index of triangle </param>
        public void SelectTriangle ( int index )
        {
            int a, b, c;
            scene.GetTriangleVertices( index, out a, out b, out c );         

            int[] triangle = new int[] { a, b, c };


            // when user holds CTRL, points will be deselected
            if ( Deselect )
            {
                List<int> deselectedPtr = new List<int>();

                foreach ( var vertex in triangle )
                {
                    if ( SelectionPointers.Contains( vertex ) )
                        deselectedPtr.Add( vertex );
                }

                // every deselected point can be removed from selected list
                foreach ( var point in deselectedPtr )
                {
                    SelectionPointers.Remove( point );
                }

            }
            // otherwise select points
            else
            {
                foreach ( var vertex in triangle )
                {
                    if ( !SelectionPointers.Contains( vertex ) )
                        SelectionPointers.Add( vertex );
                }
            }

        }

        /// <summary>
        /// Adds a single point to the selection given barycentric coordinate 
        /// and index of a triangle.
        /// </summary> 
        /// <param name="uv">Barycentric coordinate</param>
        /// <param name="index">Triangle pointer</param>
        public void SelectPoint ( Vector2d uv, int index )
        {
            int a, b, c;
            scene.GetTriangleVertices( index, out a, out b, out c );

            int[] triangle = new int[] { a, b, c };

            Vector3 coordA = scene.GetVertex( a );
            Vector3 coordB = scene.GetVertex( b );
            Vector3 coordC = scene.GetVertex( c );

            // Find closest triangle vertex
            var closestVertex = SelectionHelper.GetClosestVertex( uv, coordA, coordB, coordC, scene );

            // Add it to selection
            if (Deselect)
            {
                if (SelectionPointers.Contains(closestVertex)) SelectionPointers.Remove(closestVertex);
            }
            else
            {
                SelectionPointers.Add(closestVertex);
            }
        }

        /// <summary>
        /// Adds a single selected point to the selection
        /// </summary>
        /// <param name="index">Triangle pointer</param>
        public void SelectPoint ( int index )
        {
            if ( Deselect )
            {
                if ( SelectionPointers.Contains( index ) ) SelectionPointers.Remove( index );
            }
            else SelectionPointers.Add( index );

        }

        #endregion

        #region Brush selection 
        /// <summary>
        /// Selects points which belong to circle with a diameter of "distance" 
        /// </summary>
        /// <param name="uv">Barymetric coordinate of point selected by an user</param>
        /// <param name="pointer">Triangle pointer</param>
        /// <param name="distance">Diameter of circle</param>
        public void SelectBrush ( Vector2d uv, int pointer, float distance )
        {
            Vector3 A, B, C;
            scene.GetTriangleVertices( pointer, out A, out B, out C );

            // Get closest vertex to the point on the screen selected by user
            int closestVertex = SelectionHelper.GetClosestVertex( uv, A, B, C, scene );
            var coord = scene.GetVertex( closestVertex );

            List<int> inCircle = new List<int>();

            SelectBrush( coord, closestVertex, distance, inCircle );

            if ( !inCircle.Contains( closestVertex ) )
                inCircle.Add( closestVertex );

            // Add each point that lies within ring area of clicked point
            // to selection
            foreach ( var item in inCircle )
            {
                SelectPoint( item );
            }

        }

        /// <summary>
        /// Recursively processes points that are close to centre
        /// and continues until the points are not within given distance fro the centre
        /// </summary>
        /// <param name="centre">Vector coordinate of point selected by user</param>
        /// <param name="point">Pointer of point to be processed</param>
        /// <param name="distance">Max allowed distance from the centre</param>
        /// <param name="inCircle">List of points belonging to circle area</param>
        private void SelectBrush ( Vector3 centre, int point, float distance, List<int> inCircle )
        {
            bool boundary = false;

            // Get vertices of all points that are incident to point
            var incident = SelectionHelper.GetPolygonVertices( point, ref boundary, scene );

            var found = new List<int>();
           
            foreach ( var p in incident )
            {
                // get vector coordinate of the point
                var pC = scene.GetVertex( p );
                // calculate distance from the centre
                var dist = Geometry.Distance( pC, centre );

                // if it belongs to area bounded 
                // by distance from centre, add to selection
                if ( dist <= distance )
                {
                    if ( !inCircle.Contains( p ) )
                    {
                        inCircle.Add( p );
                        found.Add( p );
                    }

                }
            }

            if ( found.Count == 0 )
                return;

            // repeat for each found point until its distance is out of circle
            foreach ( var p in found )
            {
                SelectBrush( centre, p, distance, inCircle );
            }

        }

        #endregion

        #region Enumerator
        public IEnumerator<int> GetEnumerator ()
        {
            foreach ( var point in SelectionPointers )
            {
                yield return point;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Colours each of selected points red
        /// </summary>
        public void Redraw ()
        {
            // fill each selected point with color red
            foreach ( var point in SelectionPointers )
            {
                GL.PointSize( 6.0f );
                GL.Begin( PrimitiveType.Points );

                GL.Color3( Color.Red );
                GL.Vertex3( scene.GetVertex( point ) );

                GL.End();
            }

        }

        private void TriangulatePolygon ( List<int> polygon )
        {
            // insert list of polygon vertices in CCW order
            EarClipping triangulator = new EarClipping( polygon, scene );
            triangulator.Triangulate();
            var t = triangulator.GetTrianglePointers();

            for ( int i = 0; i < t.Count; i += 3 )
            {
                scene.AddTriangle( t[ i ], t[ i + 1 ], t[ i + 2 ] );
            }

        }       

        public void Dispose ()
        {
            SelectionPointers = new List<int>();
        }
    }
}
