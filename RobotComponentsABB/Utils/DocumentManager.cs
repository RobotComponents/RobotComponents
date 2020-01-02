using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace RobotComponentsABB
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
        /// Method to get the document identifier for RobotComponents.
        /// The identifier is created based on the Grasshopper document ID and the file path. 
        /// </summary>
        /// <param name="document"></param>
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