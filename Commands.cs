using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

[assembly: CommandClass(typeof(ProfileViewer.MyCommands))]

namespace ProfileViewer
{
    public class MyCommands
    {
        [CommandMethod("PROFILEVIEWER", CommandFlags.Modal)]
        public void ProfileViewer()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed;
            if (doc != null)
            {
                ed = doc.Editor;
                ProfileViewer_Form pvwr = new ProfileViewer_Form();
                pvwr.ShowDialog();
            }
        }
    }

}
