// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RobotComponents.Utils
{
    /// <summary>
    /// Represents serialization methods 
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Serializes a common object to a byte array. 
        /// Typically used for serializing robot meshes and data inside Goo classes.
        /// </summary>
        /// <param name="obj"> The common object. </param>
        /// <returns> The byte array. </returns>
        [Obsolete("This method is OBSOLETE and will be removed after it has been replaced with a modern standard. " +
            "The current approach is not recommended and is deprecated in .NET Core/.NET 8.0.", false)]
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) { return null; }

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a byte array to a common object. 
        /// Typically used for deserializing robot meshes and data inside Goo classes.
        /// </summary>
        /// <param name="data"> The byte array. </param>
        /// <returns> The common object. </returns>
        [Obsolete("This method is OBSOLETE and will be removed after it has been replaced with a modern standard. " +
            "The current approach is not recommended and is deprecated in .NET Core/.NET 8.0.", false)]
        public static object ByteArrayToObject(byte[] data)
        {
            if (data == null) return null;

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(data))
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}
