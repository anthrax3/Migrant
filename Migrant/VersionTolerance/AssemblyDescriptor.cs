﻿// *******************************************************************
//
//  Copyright (c) 2012-2015, Antmicro Ltd <antmicro.com>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// *******************************************************************
using System;
using System.Reflection;
using System.Linq;
using Antmicro.Migrant.Customization;

namespace Antmicro.Migrant.VersionTolerance
{
    internal class AssemblyDescriptor
    {
        public static AssemblyDescriptor ReadFromStream(ObjectReader reader)
        {
            var descriptor = new AssemblyDescriptor();
            descriptor.ReadAssemblyStamp(reader);
            var assemblyName = new AssemblyName(descriptor.FullName);
            descriptor.UnderlyingAssembly = Assembly.Load(assemblyName);
            return descriptor;
        }

        public int? AssemblyId { get; set; }

        public static AssemblyDescriptor CreateFromAssembly(Assembly assembly)
        {
            return new AssemblyDescriptor(assembly);
        }

        public void WriteTo(ObjectWriter writer)
        {
            WriteAssemblyStamp(writer);
        }

        public override bool Equals(object obj)
        {
            var objAsAssemblyDescriptor = obj as AssemblyDescriptor;
            if (objAsAssemblyDescriptor != null)
            {
                return FullName == objAsAssemblyDescriptor.FullName;
            }
            return obj != null && obj.Equals(this);
        }

        public bool Equals(AssemblyDescriptor obj, VersionToleranceLevel versionToleranceLevel)
        {
            if(versionToleranceLevel.HasFlag(VersionToleranceLevel.AllowAssemblyVersionChange))
            {
                return obj.Name == Name && obj.CultureName == CultureName && obj.Token.SequenceEqual(Token);
            }

            return Equals(obj);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        private AssemblyDescriptor()
        {
        }

        private AssemblyDescriptor(Assembly assembly)
        {
            if(assembly.Modules.Count() != 1)
            {
                throw new ArgumentException("Multimoduled assemblies are not supported yet.");
            }

            this.UnderlyingAssembly = assembly;

            Name = assembly.GetName().Name;
            Version = assembly.GetName().Version;
            CultureName = assembly.GetName().CultureName;
            if (CultureName == string.Empty)
            {
                CultureName = "neutral";
            }
            Token = assembly.GetName().GetPublicKeyToken();

            ModuleGUID = assembly.Modules.First().ModuleVersionId;
        }

        private void WriteAssemblyStamp(ObjectWriter writer)
        {
            writer.PrimitiveWriter.Write(Name);
            writer.PrimitiveWriter.Write(Version);
            writer.PrimitiveWriter.Write(CultureName);
            writer.PrimitiveWriter.Write((byte)Token.Length);
            writer.PrimitiveWriter.Write(Token);

            writer.PrimitiveWriter.Write(ModuleGUID);
        }

        private void ReadAssemblyStamp(ObjectReader reader)
        {
            Name = reader.PrimitiveReader.ReadString();
            Version = reader.PrimitiveReader.ReadVersion();
            CultureName = reader.PrimitiveReader.ReadString();
            var tokenLength = reader.PrimitiveReader.ReadByte();
            switch(tokenLength)
            {
            case 0:
                Token = new byte[0];
                break;
            case 8:
                Token = reader.PrimitiveReader.ReadBytes(8);
                break;
            default:
                throw new ArgumentException("Wrong token length!");
            }

            ModuleGUID = reader.PrimitiveReader.ReadGuid();
        }

        private string _fullName;
        public string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    _fullName = string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", Name, Version, CultureName,
                   Token.Length == 0
                       ? "null"
                       : String.Join(string.Empty, Token.Select(x => string.Format("{0:x2}", x))));
                }

                return _fullName;
            }
        }

        public Guid ModuleGUID { get; private set; } 
        public Assembly UnderlyingAssembly { get; private set; }
        public string Name { get; private set; }
        public Version Version { get; private set; }
        public string CultureName { get; private set; }
        public byte[] Token { get; private set; }
    }

    internal static class PrimitiveWriterReaderExtensions
    {
        public static void Write(this PrimitiveWriter @this, Version version)
        {
            @this.Write(version.Major);
            @this.Write(version.Minor);
            @this.Write(version.Build);
            @this.Write(version.Revision);
        }
       
        public static Version ReadVersion(this PrimitiveReader @this)
        {
            var major = @this.ReadInt32();
            var minor = @this.ReadInt32();
            var build = @this.ReadInt32();
            var revision = @this.ReadInt32();

            if(revision != -1)
            {
                return new Version(major, minor, build, revision);
            }
            else if(build != -1)
            {
                return new Version(major, minor, build);
            }
            return new Version(major, minor);
        }
    }
}

