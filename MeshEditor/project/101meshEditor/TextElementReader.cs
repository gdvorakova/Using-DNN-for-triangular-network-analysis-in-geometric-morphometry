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
    class TextElementReader : IElementReader
    {
        private StreamReader reader;

        private ElementType element;

        public int ReadElementCount { get; set; }

        /// <summary>
        /// Returns true if reader is closed
        /// </summary>
        private bool IsClosed { get; set; }

        public TextElementReader( StreamReader reader, ElementType element )
        {
            this.reader = reader;
            this.element = element;
            this.IsClosed = false;
        }

        /// <summary>
        /// Reads one data element with its properties
        /// </summary>
        /// <returns></returns>
        public Element ReadElement ()
        {
            if ( IsClosed ) throw new ObjectDisposedException("Reader closed");

            // if number of read elements equals to the number 
            // of elements defined in header
            if ( ReadElementCount == element.Count )
                return null;

            string line = reader.ReadLine();
            
            if ( line == null ) return null;

            // increase nuber of read elements
            ReadElementCount++;

            Element newElement = new Element();

            string[] tokens = line.Split( StanfordPly.DELIMITERS, StringSplitOptions.RemoveEmptyEntries );

            var elementProperties = new List<KeyValuePair<string, float>>();
            // read each property of given element
            for ( int i = 0; i < element.Properties.Count; i++ )
            {
                var property = element.Properties[ i ].Name;
                
                // if the data is face definition
                if ( property == "list" )
                {
                    int corners;
                    if ( !int.TryParse( tokens[ 0 ], NumberStyles.Float, CultureInfo.InvariantCulture, out corners ) ) continue;

                    // read polygon corners
                    for ( int j = 1; j <= corners; j++ )
                    {
                        newElement.vertexIndices.Add( int.Parse( tokens[ j ] ) );
                    }

                    break;
                }

                // otherwise
                float value;
                if ( !float.TryParse( tokens[ i ], NumberStyles.Float, CultureInfo.InvariantCulture, out value ) ) continue;
                elementProperties.Add( new KeyValuePair<string, float>( property, value ) );
            }

            newElement.values = elementProperties;           

            return newElement;
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
