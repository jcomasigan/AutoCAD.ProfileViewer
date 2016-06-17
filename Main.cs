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
using Autodesk.AutoCAD.Colors;

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
                        if(lPline != null || pline2d !=null)
                        {
                            entType = ent.GetType().ToString();
                            entType = entType.Substring(34, entType.Count() - 34);
                            return obj.ObjectId;
                        }
                        else
                        {
                            ed.WriteMessage("\n Unsupported entity type \n\n Please select a 2d Polyline");
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
                List<Tuple<double, double>> intersectDist = new List<Tuple<double, double>>();
                double maxElevation = 0;
                double profileLength = 0;
                #region Combining two selection collection together into one single list
                if (majorEnts != null)
                {
                    foreach(ObjectId id in majorEnts)
                    {
                        contoursId.Add(id);
                    }
                    if(GlobalVars.majorOnly == false)
                    {
                        foreach(ObjectId id in minorEnts)
                        {
                            contoursId.Add(id);
                        }
                    }
                    #endregion 
                    foreach (ObjectId id in contoursId)
                    {
                        using (DocumentLock doclock = doc.LockDocument())
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                            Polyline ent = tr.GetObject(profileLine, OpenMode.ForRead) as Polyline;
                            //Label start and end of section
                            CreateLayer("PROFILEVIEWER.PROFILE LABEL", 10);
                            DrawText(new Point2d(ent.StartPoint.X, ent.StartPoint.Y), GlobalVars.textHeight * 2, 0, "A", "PROFILEVIEWER.PROFILE LABEL", 10);
                            DrawText(new Point2d(ent.EndPoint.X, ent.EndPoint.Y), GlobalVars.textHeight * 2, 0, "A'", "PROFILEVIEWER.PROFILE LABEL", 10);
                            Polyline ent2 = tr.GetObject(id, OpenMode.ForWrite) as Polyline;
                            double elav = ent2.Elevation; //store elevation temporary
                            ent2.Elevation = 0;

                            if (maxElevation < elav)
                            {
                                maxElevation = elav;
                            }
                            profileLength = ent.Length;

                            /// SET ELEVATION TO ZERO TEMPORARY THEN PUT IT BACk
                            Point3dCollection intersectionPoints = new Point3dCollection();
                            ent.IntersectWith(ent2, Intersect.OnBothOperands, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
                            foreach (Point3d pt in intersectionPoints)
                            {
                                Point3d ptOnProfile = ent.GetClosestPointTo(pt, true);
                                double ptParameter = ent.GetParameterAtPoint(ptOnProfile);
                                double a = ent.GetDistanceAtParameter(ptParameter);
                                double b = ent.GetDistanceAtParameter(ent.StartParam);
                                double distance = a - b;
                                //double distance = ent.GetDistAtPoint(pt);
                                Tuple<double, double> xsectPt = new Tuple<double, double>(distance, elav);
                                intersectDist.Add(xsectPt);
                            }
                            ent2.Elevation = elav; //set it back
                        }
                    }
                    if (intersectDist.Count > 0)
                    {
                        intersectDist.Sort(); //sort to get the start and end points
                        double profileStartY = intersectDist[0].Item2;
                        double profileEndY = intersectDist[intersectDist.Count - 1].Item2;
                        Tuple<double, double> xsectStart = new Tuple<double, double>(0, profileStartY); //start of profile
                        Tuple<double, double> xsectEnd = new Tuple<double, double>(profileLength, profileEndY); //end of profile
                        intersectDist.Add(xsectStart);
                        intersectDist.Add(xsectEnd);
                        intersectDist.Sort(); //have to resort after adding new points
                        Polyline profile = new Polyline();
                        for (int i = 0; i < intersectDist.Count; i++)
                        {
                            profile.AddVertexAt(i, new Point2d(intersectDist[i].Item1 + GlobalVars.insertionPt.X, intersectDist[i].Item2 + GlobalVars.insertionPt.Y), 0, 0, 0);
                            double elevationGrid = DrawLine(new Point3d(intersectDist[i].Item1 + GlobalVars.insertionPt.X, intersectDist[i].Item2 + GlobalVars.insertionPt.Y, 0), new Point3d(intersectDist[i].Item1 + GlobalVars.insertionPt.X, GlobalVars.insertionPt.Y, 0), "PROFILEVIEWER.GRIDLINES", 252);
                            decimal elevLabel = Convert.ToDecimal(intersectDist[i].Item2);
                            DrawText(new Point2d(intersectDist[i].Item1 + GlobalVars.insertionPt.X, GlobalVars.insertionPt.Y), GlobalVars.textHeight, 1.5708, elevLabel.ToString("#.##"), "PROFILEVIEWER.LABELS", 10);
                            if (i == intersectDist.Count - 1)
                            {
                                using (DocumentLock docLock = doc.LockDocument())
                                {
                                    CreateLayer("PROFILEVIEWER.PROFILE", 255);
                                    profile.Layer = "PROFILEVIEWER.PROFILE";
                                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                    btr.AppendEntity(profile);
                                    tr.AddNewlyCreatedDBObject(profile, true);
                                }
                            }
                        }

                        double datumY = profileLength;
                        //Datum line
                        Polyline guides = new Polyline();
                        guides.AddVertexAt(0, new Point2d(GlobalVars.insertionPt.X, GlobalVars.insertionPt.Y + profileStartY), 0, 0, 0);
                        guides.AddVertexAt(1, new Point2d(GlobalVars.insertionPt.X, GlobalVars.insertionPt.Y), 0, 0, 0);
                        DrawText(new Point2d(GlobalVars.insertionPt.X, GlobalVars.insertionPt.Y), GlobalVars.textHeight * 2, 0, "A", "PROFILEVIEWER.DATUMLINE");
                        guides.AddVertexAt(2, new Point2d(GlobalVars.insertionPt.X + profileLength, GlobalVars.insertionPt.Y), 0, 0, 0);
                        DrawText(new Point2d(GlobalVars.insertionPt.X + profileLength, GlobalVars.insertionPt.Y), GlobalVars.textHeight * 2, 0, "A'", "PROFILEVIEWER.DATUMLINE");
                        guides.AddVertexAt(3, new Point2d(GlobalVars.insertionPt.X + profileLength, GlobalVars.insertionPt.Y + profileEndY), 0, 0, 0);
                        CreateLayer("PROFILEVIEWER.DATUMLINE", 251);
                        guides.Layer = "PROFILEVIEWER.DATUMLINE";

                        using (DocumentLock docLock = doc.LockDocument())
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            btr.AppendEntity(guides);
                            tr.AddNewlyCreatedDBObject(guides, true);
                        }
                    }
                    tr.Commit();
                    ed.WriteMessage("Successfully created profile.");
                }
                else
                {
                    ed.WriteMessage("No Polyline in selected layer");
                }
            }
         }

        private static void CreateLayer(string layerName, short colour)
        {

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database currentDB = doc.Database;
            using (Transaction tr = currentDB.TransactionManager.StartTransaction())
            {
                LayerTable layerTable = (LayerTable)tr.GetObject(currentDB.LayerTableId, OpenMode.ForWrite);
                if (!layerTable.Has(layerName))
                {
                    LayerTableRecord layer = new LayerTableRecord();
                    layer.Name = layerName;
                    layer.Color = Color.FromColorIndex(ColorMethod.ByLayer, colour);
                    ObjectId layerID = layerTable.Add(layer);
                    tr.AddNewlyCreatedDBObject(layer, true);
                    tr.Commit();
                }
            }
        }

        public static double DrawLine(Point3d pt1, Point3d pt2, string layerName, short colour = 255)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database currentDB = doc.Database;
            CreateLayer(layerName, colour);
            using (Transaction tr = currentDB.TransactionManager.StartTransaction())
            {
                BlockTable blckTable = tr.GetObject(currentDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRec = tr.GetObject(blckTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                Line line = new Line();
                line.StartPoint = pt1;
                line.EndPoint = pt2;
                line.Layer = layerName;      
                blockTableRec.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                tr.Commit();
                return line.Length;
            }
        }

        public static void DrawText(Point2d pt1, double textHeight, double rotation, string textVal, string layerName, short colour = 255)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database currentDB = doc.Database;
            CreateLayer(layerName, colour);
            Entity entCreated;
            double actualHeight = 0;
            using (Transaction tr = currentDB.TransactionManager.StartTransaction())
            {
                BlockTable blckTable = tr.GetObject(currentDB.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRec = tr.GetObject(blckTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                MText text = new MText();
                text.Contents = textVal;
                text.Rotation = rotation;
                text.TextHeight = textHeight;
                text.Location = new Point3d(pt1.X, pt1.Y, 0);
                text.Layer = layerName;
                text.Color = Color.FromColorIndex(ColorMethod.ByLayer, colour);
                blockTableRec.AppendEntity(text);
                tr.AddNewlyCreatedDBObject(text, true);
                actualHeight = text.ActualHeight;
                tr.Commit();
                entCreated = text;
            }
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

        public static string GetInsertionPoint()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            PromptPointOptions prmptPtOptions = new PromptPointOptions("\n\nPick insertion point....");
            PromptPointResult result = ed.GetPoint(prmptPtOptions);
            GlobalVars.insertionPt = result.Value;
            Decimal x = Convert.ToDecimal(result.Value.X);
            Decimal y = Convert.ToDecimal(result.Value.Y);
            string res = string.Format("{0}, {1}", x.ToString("#.##"), y.ToString("#.##"));
            return res;
        }
    }

    
    class GlobalVars
    {
        public static ObjectId profileLineId = ObjectId.Null;
        public static Point3d insertionPt = new Point3d();
        public static double textHeight = 1;
        public static bool majorOnly = false;
    }
}
