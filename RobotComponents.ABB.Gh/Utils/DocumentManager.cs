// This file is part of Robot Components. Robot Components is licensed 
// under the terms of GNU General Public License version 3.0 (GPL v3.0)
// as published by the Free Software Foundation. For more information and 
// the LICENSE file, see <https://github.com/RobotComponents/RobotComponents>.

// System Libs
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel;

namespace RobotComponents.ABB.Gh.Utils
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
                ObjectManagers.Add(documentID, new ObjectManager(documentID));

                // Subcribe to events
                document.ContextChanged += OnContextChanged;
                document.FilePathChanged += OnFilePathChanged;
            }

            // Gets ObjectManager of this document
            ObjectManager objectManager = DocumentManager.ObjectManagers[documentID];

            // Return the object manager of this document
            return objectManager;
        }

        /// <summary>
        /// Method that is called when then document context changed. This will remove the object mananger 
        /// of the document from the document manager if the document is closed. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private static void OnContextChanged(object sender, GH_DocContextEventArgs e)
        {
            string documentID = DocumentManager.GetRobotComponentsDocumentID(e.Document);

            if (e.Context == GH_DocumentContext.Close && ObjectManagers.ContainsKey(documentID))
            {
                ObjectManagers.Remove(documentID);
            }
        }

        /// <summary>
        /// Method that is called when the file path of the document changed. This will remove the 
        /// object manager of the old document /file since the object manangers are created based on the 
        /// GH document ID and the file path. 
        /// </summary>
        /// <param name="sender"> The object that raises the event. </param>
        /// <param name="e"> The event data. </param>
        private static void OnFilePathChanged(object sender, GH_DocFilePathEventArgs e)
        {
            // Remove the old object manager (id is changed)
            string oldId = DocumentManager.GetRobotComponentsDocumentID(e.Document, e.OldFilePath);

            if (ObjectManagers.ContainsKey(oldId))
            {
                ObjectManagers.Remove(oldId);
            }
        }

        /// <summary>
        /// Method to get the document identifier for RobotComponents.
        /// The identifier is created based on the Grasshopper document ID and the file path. 
        /// This method is typically used to get the ID if the file path of the document is changed. 
        /// Since the GH_Document object doesn't contain the old file path which is needed to get the RC document ID. 
        /// </summary>
        /// <param name="document"> The Grasshopper document as GH_Document. </param>
        /// <param name="filePath"> The Grasshopper document file path. </param>
        /// <returns>The RobotComponents document identifier</returns>
        private static string GetRobotComponentsDocumentID(GH_Document document, string filePath)
        {
            // Get the Grasshopper document ID to create the first part of the ID
            string grasshopperDocID = document.DocumentID.ToString();

            // Set a custom string if the file is was not saved yet
            if (filePath == null)
            {
                filePath = "NoFilePathFound";
            }

            // Make the additional part of the ID by hashing the file path string
            string filePathHash = filePath.GetHashCode().ToString();

            // Replace - since the hashcode can be a negative integer
            filePathHash = filePathHash.Replace("-", "");

            // Return
            return grasshopperDocID + "-" + filePathHash;
        }

        /// <summary>
        /// Method to get the document identifier for RobotComponents.
        /// The identifier is created based on the Grasshopper document ID and the file path. 
        /// </summary>
        /// <param name="document"> The Grasshopper document as GH_Document. </param>
        /// <returns>The RobotComponents document identifier</returns>
        public static string GetRobotComponentsDocumentID(GH_Document document)
        {
            return GetRobotComponentsDocumentID(document, document.FilePath);
        }
    }

}