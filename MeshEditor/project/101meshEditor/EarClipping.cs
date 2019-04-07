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

namespace Scene3D
{
    /// <summary>
    /// EarClipping class implements document by David Eberly
    /// https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
    /// It triangulates a polygon using ear clipping algorithm 
    /// </summary>
    class EarClipping
    {
        Polygon polygon;

        Polygon copy;

        SceneBrep Scene;

        /// <summary>
        /// List of vertex coordinates of the polygon
        /// </summary>
        public List<Vector3> PolygonVertices { get; set; }

        /// <summary>
        /// Result of triangulation of a polygon.
        /// </summary>
        public List<Vector3> Triangles { get; set; }

        /// <summary>
        /// Normal of the polygon
        /// </summary>
        public Vector3 Normal { get; set; }

        public EarClipping ( List<int> pointers, Scene3D.SceneBrep scene )
        {
            // Polygon has at least 3 vertices
            if ( pointers.Count < 3 )
            {
                return;
            }

            this.Scene = scene;

            // Get coordinates from vertex pointers
            PolygonVertices = new List<Vector3>();
            pointers.ForEach( x => PolygonVertices.Add( Scene.GetVertex( x ) ) );

            PrepareNormal( PolygonVertices );
            polygon = new Polygon();
            copy = new Polygon();
            CreatePolygon( polygon, PolygonVertices );
            CreatePolygon( copy, PolygonVertices );

        }
        private void PrepareNormal ( List<Vector3> coordinates )
        {

            Vector3 norm = Vector3.Zero;

            for ( int i = 0; i < coordinates.Count; i++ )
            {
                int idx = (i + 1) % (coordinates.Count);
                norm.X += (coordinates[ i ].Y - coordinates[ idx ].Y) * (coordinates[ i ].Z + coordinates[ idx ].Z);
                norm.Y += (coordinates[ i ].Z - coordinates[ idx ].Z) * (coordinates[ i ].X + coordinates[ idx ].X);
                norm.Z += (coordinates[ i ].X - coordinates[ idx ].X) * (coordinates[ i ].Y + coordinates[ idx ].Y);
            }

            Normal = norm;

        }

        /// <summary>
        /// Creates cyclic data structure that represents polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="nodes"></param>
        private void CreatePolygon ( Polygon polygon, List<Vector3> nodes )
        {

            Vertex previous = null;
            Vertex first = null;

            for ( int i = 0; i < nodes.Count; i++ )
            {
                var vertex = new Vertex( nodes[ i ] );
                polygon.Add( vertex );
                // set first vertex
                if ( i == 0 )
                    first = vertex;

                // set next vertex for previous vertex
                if ( previous != null )
                    previous.Next = vertex;
                // set previous vertex for cuttenr vertex
                vertex.Previous = previous;

                // new previous vertex
                previous = vertex;
            }

            // last vertex is previous to the first vertex 
            first.Previous = previous;
            // first vertex is next to the last vertex
            previous.Next = first;

            polygon.First = first;
        }

        public void Triangulate ()
        {
            if ( polygon == null ) return;
            FindConcaveVertices( polygon );

            Triangles = new List<Vector3>();

            // check progress
            bool check = false;

            // while we can triangulate polygon
            while ( polygon.Count > 2 )
            {
                check = false;
                foreach ( var vertex in polygon.Vertices )
                {
                    // find a convex vertex (ear)
                    if ( !IsConvex( vertex ) ) continue;

                    // test if the point is in triangle
                    if ( !IsInTriangle( vertex.Previous.Coord, vertex.Coord, vertex.Next.Coord ) )
                    {
                        check = true;
                        Triangles.Add( vertex.Previous.Coord );
                        Triangles.Add( vertex.Coord );
                        Triangles.Add( vertex.Next.Coord );

                        // ear vertex will be removed
                        // check if remaining vertices (previous & next) are concave
                        // if not, remove them from concave list

                        if ( IsConvex( vertex.Previous ) )
                            polygon.ConcaveVertices.Remove( vertex.Previous );

                        if ( IsConvex( vertex.Next ) )
                            polygon.ConcaveVertices.Remove( vertex.Next );

                        // remove ear from polygon
                        polygon.Remove( vertex );
                        break;
                    }                    
                }


                // check if any triangles can be found
                if ( AllOnLine() ) break;
                if (!check)
                {
                    return;
                }
            }
        }


        /// <summary>
        /// Returns true if all points lie on one line
        /// </summary>
        /// <returns></returns>
        private bool AllOnLine ()
        {
            foreach ( var item in polygon.Vertices )
            {
                if ( !IsOnLine( item ) ) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if there is a point 
        /// in ConcaveVertices that lies inside of triangle
        /// </summary>
        private bool IsInTriangle ( Vector3 A, Vector3 B, Vector3 C )
        {
            foreach ( var point in polygon.ConcaveVertices )
            {
                if ( point.Coord == A || point.Coord == B || point.Coord == C )
                    continue;
                if ( IsNotOutsideOfTriangle( A, B, C, point.Coord ) )
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the point lies insife of the triangle
        /// </summary>
        private bool IsNotOutsideOfTriangle ( Vector3 A, Vector3 B, Vector3 C, Vector3 concavePoint )
        {
            var pos1 = FindPosition( A, concavePoint, B );
            var pos2 = FindPosition( B, concavePoint, C );
            var pos3 = FindPosition( C, concavePoint, A );

            return pos1 != 1 && pos2 != 1 && pos3 != 1;
        }

        private void FindConcaveVertices ( Polygon polygon )
        {
            if ( polygon == null ) return;
            foreach ( var vertex in polygon.Vertices )
            {
                if ( IsConcave( vertex ) )
                {
                    polygon.ConcaveVertices.Add( vertex );
                }
            }
        }

        private int FindPosition ( Vector3 A, Vector3 B, Vector3 C )
        {
            Vector3 v1 = new Vector3( A.X - B.X, A.Y - B.Y, A.Z - B.Z );
            Vector3 v2 = new Vector3( C.X - B.X, C.Y - B.Y, C.Z - B.Z );
            Vector3 cross = Vector3.Cross( v1, v2 );

            // return zero if zero angle was found (points lie on one line)
            if ( cross.LengthSquared == 0 ) return 0;

            // return one if interior angle was found
            if ( cross.X.GetSign() != Normal.X.GetSign() ||
                 cross.Y.GetSign() != Normal.Y.GetSign() ||
                 cross.Z.GetSign() != Normal.Z.GetSign() ) return 1;

            // return -1 if exterior angle was found
            return -1;
        }


        /// <summary>
        /// A convex vertex is one for which the interior angle is smaller than 180 degrees.
        /// </summary>
        private bool IsConvex ( Vertex vertex )
        {
            if ( FindPosition( vertex.Previous.Coord, vertex.Coord, vertex.Next.Coord ) == 1 ) return true;

            return false;
        }
        /// <summary>
        /// A concave vertex is one for which the interior angle is greater than 180 degrees.
        /// </summary>
        private bool IsConcave ( Vertex vertex )
        {
            int pos = FindPosition( vertex.Previous.Coord, vertex.Coord, vertex.Next.Coord );
            if ( pos == -1 || pos == 0 ) return true;

            return false;
        }

        /// <summary>
        /// Vertices are on the same line if there is zero angle between their vectors.
        /// </summary>
        private bool IsOnLine ( Vertex vertex )
        {
            if ( FindPosition( vertex.Previous.Coord, vertex.Coord, vertex.Next.Coord ) == 0 ) return true;

            return false;
        }

        public List<int> GetTrianglePointers ()
        {
            var list = new List<int>();

            if ( Triangles == null ) return list;
            foreach ( var v in Triangles )
            {
                list.Add( Scene.geometry.IndexOf( v ) );
            }

            return list;
        }

    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// return true of positive, false if negative
        /// </summary>
        public static int GetSign ( this float number )
        {
            return Math.Sign( number );
        }
    }



}
