/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scene3D
{
    /// <summary>
    /// Data structure representing a set of vertices
    /// that create polygon
    /// </summary>
    class Polygon
    {
        public int Count
        {
            get { return Vertices.Count; }
        }

        /// <summary>
        /// First vertex of a polygon
        /// </summary>
        public Vertex First { get; set; }

        public List<Vertex> Vertices { get; set; }

        public List<Vertex> ConcaveVertices { get; set; }
        public List<Vertex> ConvexVertices { get; set; }

        public Polygon ()
        {
            ConcaveVertices = new List<Vertex>();
            Vertices = new List<Vertex>();
        }


        public void Add ( Vertex vertex )
        {
            Vertices.Add( vertex );
        }

        public void Remove ( Vertex vertex )
        {
            Vertex found = null;

            for ( int i = 0; i < Count; i++ )
            {
                if ( Vertices[ i ] == vertex )
                {
                    found = Vertices[ i ];
                    found.Previous.Next = found.Next;
                    found.Next.Previous = found.Previous;

                    if ( i == 0 )
                        First = found.Next;
                    break;
                }
            }

            Vertices.Remove( found );
        }



    }
}
