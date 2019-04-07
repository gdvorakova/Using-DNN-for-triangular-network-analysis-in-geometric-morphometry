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

namespace Scene3D
{
    class Property
    {
        /// <summary>
        /// Name of property
        /// </summary>
        public string Name { get; set; }

        public StanfordPly.PropertyType DataType { get; set; }

        public Property ( string name, string type )
        {
            this.Name = name;
            this.DataType = GetType( type );
        }

        private StanfordPly.PropertyType GetType ( string type )
        {
            if ( "char" == type )
            {
                return StanfordPly.PropertyType.CHAR;
            }
            else if ( "uchar" == type )
            {
                return StanfordPly.PropertyType.UCHAR;
            }
            else if ( "short" == type )
            {
                return StanfordPly.PropertyType.SHORT;
            }
            else if ( "ushort" == type )
            {
                return StanfordPly.PropertyType.USHORT;
            }
            else if ( "int" == type )
            {
                return StanfordPly.PropertyType.INT;
            }
            else if ( "uint" == type )
            {
                return StanfordPly.PropertyType.UINT;
            }
            else if ( "float" == type )
            {
                return StanfordPly.PropertyType.FLOAT;
            }
            else if ( "double" == type )
            {
                return StanfordPly.PropertyType.DOUBLE;
            }
            else if ( "int8" == type )
            {
                return StanfordPly.PropertyType.CHAR;
            }
            else if ( "uint8" == type )
            {
                return StanfordPly.PropertyType.UCHAR;
            }
            else if ( "int16" == type )
            {
                return StanfordPly.PropertyType.SHORT;
            }
            else if ( "uint16" == type )
            {
                return StanfordPly.PropertyType.USHORT;
            }
            else if ( "int32" == type )
            {
                return StanfordPly.PropertyType.INT;
            }
            else if ( "uint32" == type )
            {
                return StanfordPly.PropertyType.UINT;
            }
            else if ( "float32" == type )
            {
                return StanfordPly.PropertyType.FLOAT;
            }
            else if ( "float64" == type )
            {
                return StanfordPly.PropertyType.DOUBLE;
            }
            else
            {
                throw new IOException( "Type of element is not valid in PLY file." );
            }
        }
    }
}
