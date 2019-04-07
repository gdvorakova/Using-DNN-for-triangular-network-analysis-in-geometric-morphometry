/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Scene3D
{
    /// <summary>
    /// Data structure representing a vertex of a triangle
    /// </summary>
    class Vertex
    {
        /// <summary>
        /// 3D coordinate 
        /// </summary>
        public Vector3 Coord { get; set; }

        /// <summary>
        /// Number of vertex in CCW order
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Vertices incident to this vertex
        /// </summary>
        public List<Vertex> IncidentEdges { get; set; }
        
        /// <summary>
        /// Subsequent vertex in a polygon
        /// </summary>
        public Vertex Next;

        /// <summary>
        /// Preceding vertex in a polygon
        /// </summary>
        public Vertex Previous;

        public Vertex ( Vector3 coord )
        {
            this.Coord = coord;
        }

    }
}
