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

namespace Scene3D
{
    static class SelectionHelper
    {
        /// <summary>
        /// Returns index of vertex of a triangle, that is closest to given point
        /// using its barycentric coordinates.    
        /// </summary>
        /// <param name="uv"> Barycentric coordinate of point </param>
        /// <returns>3D coordinates of the closest vertex.</returns>
        public static int GetClosestVertex ( Vector2d uv, Vector3 A, Vector3 B, Vector3 C, SceneBrep scene )
        {
            double u, v, w;
            u = uv.X;
            v = uv.Y;
            w = 1 - u - v;

            Vector3 intersectionPoint = Geometry.MultiplyByScalar( A, w ) + Geometry.MultiplyByScalar( B, u ) + Geometry.MultiplyByScalar( C, v );

            float PA, PB, PC;
            PA = Geometry.Distance( intersectionPoint, A );
            PB = Geometry.Distance( intersectionPoint, B );
            PC = Geometry.Distance( intersectionPoint, C );

            // Compare distances
            float smallest = PA;
            Vector3 nearest = A;

            if ( smallest > PB )
            {
                smallest = PB;
                nearest = B;
            }

            if ( smallest > PC )
            {
                smallest = PC;
                nearest = C;
            }

            return scene.geometry.IndexOf( nearest );
        }

        /// <summary>
        /// Returns true if vertex lies on the edge of triangle mesh
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static bool IsOnEdge ( Vector3 vertex, SceneBrep scene )
        {
            bool onEdge = false;

            var handle = scene.GetVertex( vertex );
            List<int> trianglePtr = new List<int>();

            // get triangles
            for ( int i = 0; i < scene.vertexPtr.Count; i++ )
            {
                if ( scene.vertexPtr[ i ] == handle )
                {
                    if ( i % 3 == 0 )
                        trianglePtr.Add( i / 3 );
                    else if ( i % 3 == 1 )
                        trianglePtr.Add( (i - 1) / 3 );
                    else if ( i % 3 == 2 )
                        trianglePtr.Add( (i - 2) / 3 );
                }
            }

            // is triangle on edge?

            foreach ( var ptr in trianglePtr )
            {
                // get handle of corners of triangle
                var c1 = Scene3D.SceneBrep.tCorner( ptr );
                var c2 = c1 + 1;
                var c3 = c1 + 2;



                if ( scene.cOpposite( c1 ) == -1 )
                {
                    onEdge = true;

                }
                else if ( scene.cOpposite( c2 ) == -1 )
                {
                    onEdge = true;
                }
                else if ( scene.cOpposite( c3 ) == -1 )
                {
                    onEdge = true;
                }
            }

            return onEdge;
        }

        /// <summary>
        /// For given vertex handler return points that are incident to it
        /// Sets boundary as true if given vertex lies on the edge of the mesh
        /// </summary>
        public static List<int> GetPolygonVerticesAndDelete ( int vertexIndex, ref bool boundary, SceneBrep scene )
        {
            List<int> polygon = new List<int>();

            List<int> trianglePointers = new List<int>();

            int add1 = 0;
            int add2 = 0;

            for ( int i = 0; i < scene.vertexPtr.Count; i++ )
            {
                // found triangle such that one corner is selected vertex
                if ( scene.vertexPtr[ i ] == vertexIndex )
                {
                    // if it's main triangle handle
                    if ( i % 3 == 0 )
                    {
                        // get the remaining triangle corners
                        add1 = i + 1;
                        add2 = i + 2;

                        // get triangle handle
                        trianglePointers.Add( i );

                    }
                    else if ( i % 3 == 1 )
                    {
                        add1 = i - 1;
                        add2 = i + 1;
                        trianglePointers.Add( i - 1 );
                    }
                    else if ( i % 3 == 2 )
                    {
                        add2 = i - 1;
                        add1 = i - 2;

                        trianglePointers.Add( i - 2 );
                    }

                    if ( !polygon.Contains( scene.vertexPtr[ add1 ] ) ) polygon.Add( scene.vertexPtr[ add1 ] );
                    if ( !polygon.Contains( scene.vertexPtr[ add2 ] ) ) polygon.Add( scene.vertexPtr[ add2 ] );
                }
            }

            // CCW order of points in polygon

            ReorderPoints( ref polygon, trianglePointers, vertexIndex, scene.vertexPtr, ref boundary );
            trianglePointers.ForEach( x => scene.FreeTriangleSpace( x ) );

            return polygon;
        }





        /// <summary>
        /// For given vertex handler return points of polygon that are incident to it
        /// </summary>
        public static List<int> GetPolygonVertices ( int vertexIndex, ref bool boundary, SceneBrep scene )
        {
            List<int> polygon = new List<int>();

            List<int> trianglePointers = new List<int>();

            int add1 = 0;
            int add2 = 0;

            for ( int i = 0; i < scene.vertexPtr.Count; i++ )
            {
                // found triangle such that one corner is selected vertex
                if ( scene.vertexPtr[ i ] == vertexIndex )
                {
                    // if it's main triangle handle
                    if ( i % 3 == 0 )
                    {
                        // get the remaining triangle corners
                        add1 = i + 1;
                        add2 = i + 2;

                    }
                    else if ( i % 3 == 1 )
                    {
                        add1 = i - 1;
                        add2 = i + 1;
                    }
                    else if ( i % 3 == 2 )
                    {
                        add2 = i - 1;
                        add1 = i - 2;
                    }

                    if ( !polygon.Contains( scene.vertexPtr[ add1 ] ) ) polygon.Add( scene.vertexPtr[ add1 ] );
                    if ( !polygon.Contains( scene.vertexPtr[ add2 ] ) ) polygon.Add( scene.vertexPtr[ add2 ] );
                }
            }

            return polygon;
        }

        /// <summary>
        /// Reorder points so that they are in counter clockwise order
        /// Return false if vertex is a boundary point
        /// </summary>
        public static void ReorderPoints ( ref List<int> polygon, List<int> trPointers, int middleVertex, List<int> vertexPtr, ref bool boundary )
        {
            var newPolygon = new List<int>();
            var count = trPointers.Count();

            List<int> list = new List<int>( trPointers );

            // Choose first triangle
            int firstTr = 0;
            if ( list.Count < 1 )
            {
                polygon = newPolygon;
                return;
            }
            firstTr = list[ 0 ];
            list.RemoveAt( 0 );

            var A = vertexPtr[ firstTr ];
            var B = vertexPtr[ firstTr + 1 ];
            var C = vertexPtr[ firstTr + 2 ];

            if ( A != middleVertex ) newPolygon.Add( A );
            if ( B != middleVertex ) newPolygon.Add( B );
            if ( C != middleVertex ) newPolygon.Add( C );

            for ( int j = 0; j < count - 2; j++ )
            {
                for ( int i = 0; i < list.Count; i++ )
                {
                    var lastAdded = newPolygon[ newPolygon.Count - 1 ];

                    firstTr = list[ i ];

                    A = vertexPtr[ firstTr ];
                    B = vertexPtr[ firstTr + 1 ];
                    C = vertexPtr[ firstTr + 2 ];

                    if ( (A != middleVertex && A == lastAdded) )
                    {
                        if ( B != middleVertex && !newPolygon.Contains( B ) ) newPolygon.Add( B );
                        else newPolygon.Add( C );
                    }
                    else if ( (B != middleVertex && B == lastAdded) )
                    {
                        if ( A != middleVertex && !newPolygon.Contains( A ) ) newPolygon.Add( A );
                        else newPolygon.Add( C );
                    }
                    else if ( (C != middleVertex && C == lastAdded) )
                    {
                        if ( A != middleVertex && !newPolygon.Contains( A ) ) newPolygon.Add( A );
                        else newPolygon.Add( B );
                    }
                    else
                    {
                        continue;
                    }

                    list.RemoveAt( i );
                    break;
                }
            }

            // if found vertices don't contain a loop, then the selected vertex is a boundary vertex
            if ( newPolygon.Count != count )
            {
                boundary = true;
                return;
            }

            // get CCW order of vertices
            newPolygon.Reverse();
            polygon = newPolygon;
            boundary = false;

        }
    }
}
