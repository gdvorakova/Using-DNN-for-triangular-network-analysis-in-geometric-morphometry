/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace Scene3D
{
    class Element
    {
        // optional (vertex element)     
        public List<KeyValuePair<string, float>> values;

        // optional (face element)
        public List<int> vertexIndices;


        public Element ()
        {
            values = new List<KeyValuePair<string, float>>();
            vertexIndices = new List<int>();
        }

        public Vector3 GetVertex ()
        {
            Vector3 coord;
            coord.X = values.Find( kvp => kvp.Key == "x" ).Value;
            coord.Y = values.Find( kvp => kvp.Key == "y" ).Value;
            coord.Z = values.Find( kvp => kvp.Key == "z" ).Value;

            return coord;
        }

        public Vector3 GetNormal ()
        {
            Vector3 norm;
            norm.X = values.Find( kvp => kvp.Key == "nx" ).Value;
            norm.Y = values.Find( kvp => kvp.Key == "ny" ).Value;
            norm.Z = values.Find( kvp => kvp.Key == "nz" ).Value;

            return norm;
        }

        public Vector2 GetTextureCoordinate ()
        {
            Vector2 txtCoord;
            txtCoord.X = values.Find( kvp => kvp.Key == "s" ).Value;
            txtCoord.Y = values.Find( kvp => kvp.Key == "t" ).Value;

            return txtCoord;
        }
        public Vector3 GetVertexColor ()
        {
            Vector3 color;
            color.X = values.Find( kvp => kvp.Key == "red" ).Value;
            color.Y = values.Find( kvp => kvp.Key == "green" ).Value;
            color.Z = values.Find( kvp => kvp.Key == "blue" ).Value;

            return color;
        }

        public void GetTriangleVertices ( out int v1, out int v2, out int v3 )
        {
            v1 = vertexIndices[ 0 ];
            v2 = vertexIndices[ 1 ];
            v3 = vertexIndices[ 2 ];
        }
    }    
}
