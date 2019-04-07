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
    class ElementType
    {
        /// <summary>
        /// Name of this element type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of properties which belong to this element type
        /// </summary>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Number of element entries
        /// </summary>
        public int Count { get; set; }

        public ElementType ( int count, string name )
        {
            this.Properties = new List<Property>();
            this.Count = count;
            this.Name = name;
        }
    }
}
