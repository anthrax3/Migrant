/*
  Copyright (c) 2015 Antmicro <www.antmicro.com>

  Authors:
   * Konrad Kruczynski (kkruczynski@antmicro.com)
   * Mateusz Holenko (mholenko@antmicro.com)

  Permission is hereby granted, free of charge, to any person obtaining
  a copy of this software and associated documentation files (the
  "Software"), to deal in the Software without restriction, including
  without limitation the rights to use, copy, modify, merge, publish,
  distribute, sublicense, and/or sell copies of the Software, and to
  permit persons to whom the Software is furnished to do so, subject to
  the following conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;

namespace Antmicro.Migrant.Customization
{
    /// <summary>
    /// Level of the version tolerance, that is how the deserialized type can differ
    /// from the serialized (original) type.
    /// </summary>
    [Flags]
    public enum VersionToleranceLevel
    {
        /// <summary>
        /// Modules GUIDs can vary between new and old streams.
        /// </summary>
        AllowGuidChange = 0x1,

        /// <summary>
        /// The new layout can have more fields that the old one. New fields are initialized to their default values.
        /// </summary>
        AllowFieldAddition = 0x2,

        /// <summary>
        /// The new layout can have less fields that the old one. Values of the missing ones are ignored.
        /// </summary>
        AllowFieldRemoval = 0x4,

        /// <summary>
        /// In the new layout fields can be moved to another classes in the inheritance hierarchy. 
        /// Fields will be matched by names, therefore this flag cannot be used in classes with multiple fields with the same name.
        /// </summary>
        AllowFieldMove = 0x8,

        /// <summary>
        /// Classes inheritance hierarchy can vary between new and old streams, e.g., base class can be removed.
        /// </summary>
        AllowInheritanceChainChange = 0x10,

        /// <summary>
        /// Classes names can vary between new and old streams.
        /// Fields will be matched by names, therefore this flag cannot be used in classes with multiple fields with the same name.
        /// </summary>
        AllowTypeNameChange = 0x20,

        /// <summary>
        /// Assemblies versions can vary between new and old streams.
        /// </summary>
        AllowAssemblyVersionChange = 0x40
    }
}

