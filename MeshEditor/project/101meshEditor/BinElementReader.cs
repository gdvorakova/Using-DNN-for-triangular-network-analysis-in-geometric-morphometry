/* Copyright (C) 2017 Gabriela Dvorakova
 * Available for use with the author's permission
 * <gabdvorakova@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scene3D
{
    class BinElementReader : IElementReader
    {
        public int ReadElementCount { get; set; }

        private BinaryReader reader;

        private ElementType element;

        public StanfordPly.Format format;


        /// <summary>
        /// Returns true if reader is closed
        /// </summary>
        private bool IsClosed { get; set; }

        public BinElementReader ( BinaryReader binaryReader, ElementType element, StanfordPly.Format format )
        {
            this.reader = binaryReader;
            this.element = element;
            this.format = format;
        }

        /// <summary>
        /// Reads one data element with its properties
        /// </summary>
        /// <returns></returns>
        public Element ReadElement ()
        {
            if ( IsClosed ) throw new ObjectDisposedException( "Reader closed" );

            // if number of read elements equals to the number 
            // of elements defined in header
            if ( ReadElementCount == element.Count )
                return null;            

            // increase nuber of read elements
            ReadElementCount++;

            Element newElement = new Element();

            var elementProperties = new List<KeyValuePair<string, float>>();
            // read each property of given element
            for ( int i = 0; i < element.Properties.Count; i++ )
            {
                var property = element.Properties[ i ].Name;

                // if the data is face definition
                if ( property == "list" )
                {
                    float corners = ReadType( element.Properties[ 0 ].DataType );

                    // read polygon corners
                    for ( int j = 1; j <= corners; j++ )
                    {
                        newElement.vertexIndices.Add( (int) ReadType( element.Properties[1].DataType ) );
                    }

                    break;
                }

                // otherwise
                float value;
                value = ReadType( element.Properties[ i ].DataType );
                elementProperties.Add( new KeyValuePair<string, float>( property, value ) );
            }

            newElement.values = elementProperties;

            return newElement;
        }

        /// <summary>
        /// Reads bytes depending on gines parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private float ReadType ( StanfordPly.PropertyType type )
        {
            byte[] bytes;
            switch ( type )
            {
                case StanfordPly.PropertyType.CHAR:
                    bytes = reader.ReadBytes( 1 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return BitConverter.ToChar( bytes, 0 );
                case StanfordPly.PropertyType.UCHAR:
                    bytes = reader.ReadBytes( 1 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return (float) bytes[0];
                case StanfordPly.PropertyType.SHORT:
                    bytes = reader.ReadBytes( 2 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return BitConverter.ToInt16( bytes, 0 );
                case StanfordPly.PropertyType.USHORT:
                    bytes = reader.ReadBytes( 2 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return BitConverter.ToUInt16( bytes, 0 );
                case StanfordPly.PropertyType.INT:
                    bytes = reader.ReadBytes( 4 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return BitConverter.ToInt32( bytes, 0 );
                case StanfordPly.PropertyType.UINT:
                    bytes = reader.ReadBytes( 4 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return BitConverter.ToUInt32( bytes, 0 );
                case StanfordPly.PropertyType.FLOAT:
                    bytes = reader.ReadBytes( 4 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return (float)BitConverter.ToSingle( bytes, 0 );
                case StanfordPly.PropertyType.DOUBLE:
                    bytes = reader.ReadBytes( 8 );
                    if ( format == StanfordPly.Format.BINARY_BE )
                    {
                        Array.Reverse( bytes );
                    }
                    return (float)BitConverter.ToDouble( bytes, 0 );
                default: throw new IOException( "Invalid data type" );
            }

        }

        /// <summary>
        /// Close the reader
        /// </summary>
        public void Close ()
        {
            this.IsClosed = true;
        }


    }
}
