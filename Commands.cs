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
        [CommandMethod("PROFILEVIEWER", "PVWR", CommandFlags.Modal)]
        public void ProfileViewer()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed;
            if (doc != null)
            {
                ed = doc.Editor;
                ProfileViewer_Form pvwr = new ProfileViewer_Form();
                pvwr.Show();
            }
        }

        [CommandMethod("PlaneCurveIntersection")]
        public static void PlaneCurveIntersection()
        {
            Document activeDoc
                = Application.DocumentManager.MdiActiveDocument;
            Database db = activeDoc.Database;
            Editor ed = activeDoc.Editor;

            // Create the planar region and the ellipse
            ObjectId regionId = ObjectId.Null;
            ObjectId ellipseId = ObjectId.Null;
            using (Transaction tr
                        = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(
                                                db.BlockTableId,
                                                OpenMode.ForRead
                                            ) as BlockTable;

                BlockTableRecord modelSpace = tr.GetObject
                                (
                                    bt[BlockTableRecord.ModelSpace],
                                    OpenMode.ForWrite
                                ) as BlockTableRecord;

                Autodesk.AutoCAD.DatabaseServices.Polyline polyline
                    = new Autodesk.AutoCAD.DatabaseServices.Polyline();

                polyline.Closed = true;
                polyline.AddVertexAt
                            (
                                0,
                                new Point2d(10.0, -10.0),
                                0.0,
                                0.0,
                                0.0
                            );
                polyline.AddVertexAt
                            (1,
                                new Point2d(10.0, 10.0),
                                0.0,
                                0.0,
                                0.0
                            );
                polyline.AddVertexAt
                            (2,
                                new Point2d(-10.0, 10.0),
                                0.0,
                                0.0,
                                0.0
                            );
                polyline.AddVertexAt
                            (
                                3,
                                new Point2d(-10.0, -10.0),
                                0.0,
                                0.0,
                                0.0
                            );
                DBObjectCollection coll = new DBObjectCollection();
                coll.Add(polyline);
                DBObjectCollection rgnColl
                                        = Region.CreateFromCurves(coll);
                if (rgnColl.Count > 0)
                {
                    Region rgn = rgnColl[0] as Region;
                    regionId = modelSpace.AppendEntity(rgn);
                    tr.AddNewlyCreatedDBObject(rgn, true);
                }
                Vector3d ellipseNormal
                    = Vector3d.ZAxis.RotateBy(
                                                45.0 * Math.PI / 180.0,
                                                Vector3d.XAxis
                                             );
                Vector3d ellipseMajorAxis = new Vector3d(0, 5, 0);
                ellipseMajorAxis
                    = ellipseMajorAxis.RotateBy
                                            (
                                                45.0 * Math.PI / 180.0,
                                                Vector3d.XAxis
                                            );
                Ellipse ellipse = new Ellipse
                                            (
                                                Point3d.Origin,
                                                ellipseNormal,
                                                ellipseMajorAxis,
                                                0.5,
                                                0,
                                                0
                                            );
                ellipseId = modelSpace.AppendEntity(ellipse);
                tr.AddNewlyCreatedDBObject(ellipse, true);
                tr.Commit();
            }

            // Find the intersection of the curve and the plane
            using (Transaction tr
                    = db.TransactionManager.StartTransaction())
            {
                Region rgn = tr.GetObject
                                        (
                                            regionId,
                                            OpenMode.ForRead
                                        ) as Region;
                DBObjectCollection exploded = new DBObjectCollection();
                rgn.Explode(exploded);
                Entity firstEntity = exploded[0] as Entity;
                Plane intersectionPlane = firstEntity.GetPlane();
                Curve curve = tr.GetObject
                                        (
                                            ellipseId,
                                            OpenMode.ForRead
                                        ) as Curve;
                Plane curvePlane = curve.GetPlane();
                if (curvePlane.IsCoplanarTo(intersectionPlane))
                {
                    ed.WriteMessage("\nInfinite intersections !");
                }
                else if (curvePlane.IsParallelTo(intersectionPlane))
                {
                    ed.WriteMessage("\nNo intersections !");
                }
                else
                {
                    Curve projectedCurve
                        = curve.GetProjectedCurve
                                            (
                                                intersectionPlane,
                                                intersectionPlane.Normal
                                            );
                    Point3dCollection intPts = new Point3dCollection();
                    projectedCurve.IntersectWith
                                            (
                                                curve,
                                                Intersect.ExtendBoth,
                                                intPts,
                                                IntPtr.Zero,
                                                IntPtr.Zero
                                            );
                    foreach (Point3d pt in intPts)
                    {
                        if (curvePlane.IsOn(pt))
                        {
                            ed.WriteMessage
                                        (
                                string.Format(
                                                "{0}{1}",
                                                Environment.NewLine,
                                                pt.ToString()
                                             )
                                        );
                        }
                    }
                }
                tr.Commit();
            }
        }
    }

}
