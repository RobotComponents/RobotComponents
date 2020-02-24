// This file is part of RobotComponents. RobotComponents is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/EDEK-UniKassel/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace RobotComponentsABB.Utils
{
    /// <summary>
    /// Document Manager class
    /// </summary>
    public static class DocumentManager
    {
        /// <summary>
        /// Dictionary that stores the objects managers of the unique documents.
        /// </summary>
        public static Dictionary<string, ObjectManager> ObjectManagers = new Dictionary<string, ObjectManager>();

        /// <summary>
        /// Gets the document object manager
        /// </summary>
        /// <param name="document"> The Grasshopper docuement as GH_Document. </param>
        /// <returns> Returns the object manager of the passed document. </returns>
        public static ObjectManager GetDocumentObjectManager(GH_Document document)
        {
            // Gets Document ID
            string documentID = DocumentManager.GetRobotComponentsDocumentID(document);

            // Checks if ObjectManager for this document already exists. If not it creates a new one. 
            if (!ObjectManagers.ContainsKey(documentID))
            {
                ObjectManagers.Add(documentID, new ObjectManager());
            }

            // Gets ObjectManager of this document
            ObjectManager objectManager = DocumentManager.ObjectManagers[documentID];

            // Return the object manager of this document
            return objectManager;
        }

        /// <summary>
        /// Method to get the document identifier for RobotComponents.
        /// The identifier is created based on the Grasshopper document ID and the file path. 
        /// </summary>
        /// <param name="document"> The Grasshopper docuement as GH_Document. </param>
        /// <returns>The RobotComponents document identifier</returns>
        public static string GetRobotComponentsDocumentID(GH_Document document)
        {
            // Get the Grasshopper document ID to create the first part of the ID
            string GrasshopperDocID = document.DocumentID.ToString();
 
            // Get the file path
            string FilePath = document.FilePath;

            // Set a custom string if the file is was not saved yet
            if (FilePath == null)
            {
                FilePath = "NoFilePathFound";
            }

            // Make the additional part of the ID by hashing the file path string
            string FilePathHash = FilePath.GetHashCode().ToString();

            // Replace - since the hashcode can be a negative integer
            FilePathHash.Replace("-", "");

            // Return
            return GrasshopperDocID + "-" + FilePathHash;
        }
    }

}