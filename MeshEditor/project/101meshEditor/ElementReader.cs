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
    interface IElementReader
    {
        /// <summary>
        /// Reads one element from stream
        /// If none, returns null
        /// </summary>
        Element ReadElement ();

        /// <summary>
        /// Closes the stream     
        /// </summary>
        void Close ();

        /// <summary>
        /// Number of already read elements.
        /// </summary>
        int ReadElementCount { get; set; }
        

    }
}
