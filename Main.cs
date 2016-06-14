using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ProfileViewer
{
    class Main
    {
        public static ObjectId SelectPolyline(out string entType)
        {
            entType = "";            
            ObjectId nullObjId = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            if (ed != null)
            {
                PromptEntityResult prmtEnt = ed.GetEntity("\nSELECT PROFILE LINE ");
                if (prmtEnt.Status == PromptStatus.OK)
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        DBObject obj = tr.GetObject(prmtEnt.ObjectId, OpenMode.ForRead);
                        Polyline lPline = obj as Polyline;
                        Polyline2d pline2d = obj as Polyline2d;
                        Entity ent = obj as Entity;
                        Line line = obj as Line;
                        if(lPline != null || pline2d !=null || line != null )
                        {
                            entType = ent.GetType().ToString();
                            entType = entType.Substring(34, entType.Count() - 34);
                            return obj.ObjectId;
                        }
                        else
                        {
                            ed.WriteMessage("\n Unsupported entity type");
                        }
                    }
                }
            }

            return nullObjId;
        }

        public static List<string> GetLayerNames()
        {
            List<string> layerNames = new List<string>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForWrite);
                foreach(ObjectId id in layerTable)
                {
                    LayerTableRecord layer = tr.GetObject(id, OpenMode.ForRead) as LayerTableRecord;
                    layerNames.Add(layer.Name);
                }
            }
            return layerNames;
        }

        public static void GetProfile(ObjectId profileLine, string majorContLayer, string minorContLayer)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                List<ObjectId> contoursId = new List<ObjectId>();
                ObjectIdCollection majorEnts = SelectByLayer(majorContLayer);
                ObjectIdCollection minorEnts = SelectByLayer(minorContLayer);
                foreach(ObjectId id in majorEnts)
                {
                    
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    Entity ent = tr.GetObject(profileLine, OpenMode.ForRead) as Entity;
                    Entity ent2 = tr.GetObject(id, OpenMode.ForRead) as Entity;
                    Polyline entPoly = tr.GetObject(profileLine, OpenMode.ForRead) as Polyline;
                    Point3d point = entPoly.GetPointAtDist(2);
                    
                    Vector3d vec = entPoly.GetFirstDerivative(point);
                    vec = vec.TransformBy(Matrix3d.Rotation(Math.PI / 2.0, entPoly.Normal, Point3d.Origin));
                    Plane plane = ent2.GetPlane();
                    Curve curv = tr.GetObject(id, OpenMode.ForRead) as Curve;
                    Curve projCurv = curv.GetProjectedCurve(plane, plane.Normal);

                    /// SET ELEVATION TO ZERO TEMPORARY THEN PUT IT BACk
                    Point3dCollection intersectionPoints = new Point3dCollection();
                    //ent.IntersectWith(ent2, Intersect.ExtendBoth, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
                    ent.BoundingBoxIntersectWith(ent2, Intersect.ExtendBoth, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
                    foreach (Point3d pt in intersectionPoints )
                    {
                        ed.WriteMessage(string.Format("\n Intersection Pts : {0}, {1}, {2}", pt.X.ToString(), pt.Y.ToString(), pt.Z.ToString()));
                        Circle circ = new Circle(pt, Vector3d.ZAxis, 10 * db.Dimtxt);
                        Polyline pl = new Polyline();
                       
                        circ.ColorIndex = 1;
                        using (DocumentLock doclock = doc.LockDocument())
                        {
                            btr.UpgradeOpen();
                            btr.AppendEntity(circ);
                            btr.DowngradeOpen();
                            tr.AddNewlyCreatedDBObject(circ, true);
                        }
                    }
                    ent.BoundingBoxIntersectWith(ent2, Intersect.ExtendBoth, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
                    foreach (Point3d pt in intersectionPoints)
                    {
                        ed.WriteMessage(string.Format("\n Bounding box Intersection Pts : {0}, {1}, {2}", pt.X.ToString(), pt.Y.ToString(), pt.Z.ToString()));
                    }
                }
                tr.Commit();
            }
         }
        private static List<ObjectId> ObjectIdColl2List(ObjectIdCollection objCol)
        {
            List<ObjectId> list = new List<ObjectId>();
            foreach(ObjectId id in objCol)
            {
                list.Add(id);
            }
            return list;
        }

        public static ObjectIdCollection SelectByLayer(string layerName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            ObjectIdCollection selectedObjects = new ObjectIdCollection();
            TypedValue[] typedVals = new TypedValue[]
            {
                //Selection using dxf codes
               new TypedValue((int)DxfCode.Operator, "<or"),
               new TypedValue((int)DxfCode.Operator, "<and"),
               new TypedValue((int)DxfCode.LayerName,layerName),
               new TypedValue((int)DxfCode.Start, "LINE"),
               new TypedValue((int)DxfCode.Operator, "and>"),
               new TypedValue((int)DxfCode.Operator, "<and"),
               new TypedValue((int)DxfCode.LayerName,layerName),
               new TypedValue((int)DxfCode.Start, "POLYLINE"),
               new TypedValue((int)DxfCode.Operator, "and>"),
               new TypedValue((int)DxfCode.Operator, "<and"),
               new TypedValue((int)DxfCode.LayerName,layerName),
               new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
               new TypedValue((int)DxfCode.Operator, "and>"),
               new TypedValue((int)DxfCode.Operator, "or>")

            };
            SelectionFilter selectFilter = new SelectionFilter(typedVals);
            PromptSelectionResult selection = ed.SelectAll(selectFilter);
            if (selection.Status == PromptStatus.OK)
            {
                selectedObjects = new ObjectIdCollection(selection.Value.GetObjectIds());
            }

            return selectedObjects;
        }
    }
    
    class GlobalVars
    {
        public static ObjectId profileLineId = ObjectId.Null;
    }
}
