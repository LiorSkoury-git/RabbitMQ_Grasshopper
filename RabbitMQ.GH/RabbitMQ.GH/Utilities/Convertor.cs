﻿using Grasshopper.Kernel.Types;
using Newtonsoft.Json;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.GH.Utilities
{
    /// <summary>
    /// Represents a class to handle data conversion between different formats.
    /// </summary>
    public static class Convertor
    {
        #region Methods

        /// <summary>
        /// Serializes a list of grasshopper objects and sets them up in a Json object.
        /// </summary>
        /// <param name="objectWrappers"> Wrappers with the objects to convert.</param>
        /// <param errors="port">[Out] Errors raised during conversion.</param>
        public static List<string> ConvertToStrings(List<GH_ObjectWrapper> objectWrappers, out List<string> errors)
        {
            List<string> jsons = new List<string>();
            errors = new List<string>();
            var jsonOptions = new SerializationOptions();
            int counter = 0;

            foreach (var wrapper in objectWrappers)
            {
                var objectType = wrapper.Value.GetType();
                var t = wrapper.Value.GetType().ToString();
                string so = null;

                // Rhino.RhinoApp.WriteLine(wrapper.Value.GetType().ToString());

                // Converts base geometry objects.
                if (wrapper.Value is GeometryBase ghGeometry)
                {
                    GeometryBase geometry = ghGeometry;
                    t = geometry.GetType().ToString();
                    so = geometry.ToJSON(jsonOptions);
                }
                // Converts point objects.
                else if (objectType == typeof(GH_Point))
                {
                    GH_Point ghPoint = wrapper.Value as GH_Point;
                    Point3d point = ghPoint.Value;
                    t = point.GetType().ToString();
                    so = point.ToString();
                }
                // Converts vector objects.
                else if (objectType == typeof(GH_Vector))
                {
                    GH_Vector ghVector = wrapper.Value as GH_Vector;
                    Vector3d vector = ghVector.Value;
                    t = vector.GetType().ToString();
                    so = vector.ToString();
                }
                // Converts plane objects.
                else if (objectType == typeof(GH_Plane))
                {
                    GH_Plane ghPlane = wrapper.Value as GH_Plane;
                    Plane plane = ghPlane.Value;
                    t = plane.GetType().ToString();
                    so = plane.ToString();
                }
                // Converts transform objects.
                else if (objectType == typeof(GH_Transform))
                {
                    GH_Transform ghTrans = wrapper.Value as GH_Transform;
                    Transform trans = ghTrans.Value;
                    t = trans.GetType().ToString();
                    so = trans.ToString();
                }
                // Converts line objects.
                else if (wrapper.Value is GH_Line ghLine)
                {
                    Line line = ghLine.Value;
                    t = line.GetType().ToString();
                    so = line.ToString();
                }
                // Converts curve objects.
                else if (wrapper.Value is GH_Curve ghCurve)
                {
                    Curve curve = ghCurve.Value;
                    t = curve.GetType().ToString();
                    so = curve.ToJSON(jsonOptions);
                }
                // Converts surface objects.
                else if (wrapper.Value is GH_Surface ghSurface)
                {
                    Brep surface = ghSurface.Value;
                    t = surface.GetType().ToString();
                    so = surface.ToJSON(jsonOptions);
                }
                // Converts brep objects.
                else if (wrapper.Value is GH_Brep ghBrep)
                {
                    Brep brep = ghBrep.Value;
                    t = brep.GetType().ToString();
                    so = brep.ToJSON(jsonOptions);
                }
                // Converts mesh objects.
                else if (wrapper.Value is GH_Mesh ghMesh)
                {
                    Rhino.Geometry.Mesh mesh = ghMesh.Value;
                    t = mesh.GetType().ToString();
                    so = mesh.ToJSON(jsonOptions);
                }
                // Sets up the error message to use for invalid objects.
                else
                {
                    t = $"Input object number {counter} is not a rhino type or it is not supported";
                }

                // Serializes the converted objects and adds them to the jsons list.
                if (so != null)
                {
                    Dictionary<string, string> data = new Dictionary<string, string> { { t, so } };

                    var jsonObj = JsonConvert.SerializeObject(data);

                    jsons.Add(jsonObj);
                }
                // Adds the error mesasge to the errors list.
                else
                {
                    errors.Add(t);
                }
                counter++;
            }

            return jsons;
        }

        /// <summary>
        /// Converts a list of String objects to their corresponding Rhinoceros type.
        /// </summary>
        /// <param name="stringObjects"> Objects to convert.</param>
        /// <param errors="port">[Out] Errors raised during conversion.</param>
        public static List<object> ConvertToRhino(List<string> stringObjects, out List<string> errors)
        {
            List<object> objects = new List<object>();
            errors = new List<string>();

            for (int i = 0; i < stringObjects.Count; i++)
            {
                Dictionary<String, String> data = null;

                // Deserializes an object.
                try
                {
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, String>>(stringObjects[i]);
                }
                catch 
                {
                    errors.Add("Invalid format in input number - " + i.ToString());
                    continue;
                }
                string[] keys = data.Keys.ToArray();

                // Converts base geometry objects.
                try
                {
                    var o = GeometryBase.FromJSON(data[keys[0]]);
                    objects.Add(o);
                }
                catch
                {
                    // Converts point objects.
                    if (keys[0].Contains("Point"))
                    {
                        var o = Point3d.Unset;
                        Point3d.TryParse(data[keys[0]], out o);
                        objects.Add(o);
                    }
                    // Converts vector objects.
                    else if (keys[0].Contains("Vector"))
                    {
                        var temp = Point3d.Unset;
                        Point3d.TryParse(data[keys[0]], out temp);
                        var o = new Vector3d(temp);
                        objects.Add(o);
                    }
                    // Converts plane objects.
                    else if (keys[0].Contains("Plane"))
                    {
                        string[] parts = data[keys[0]].Split(' ');
                        string[] originValues = parts[0].Split('=')[1].Split(',');
                        var origin = new Point3d(double.Parse(originValues[0]), double.Parse(originValues[1]), double.Parse(originValues[2]));


                        var vectors = new Vector3d[3];
                        for (int j = 0; j < 3; j++)
                        {
                            string[] vectorValues = parts[j + 1].Split('=')[1].Split(',');
                            vectors[j] = new Vector3d(double.Parse(vectorValues[0]), double.Parse(vectorValues[1]), double.Parse(vectorValues[2]));
                        }
                        var o = new Plane(origin, vectors[0], vectors[1]);
                        objects.Add(o);
                    }
                    // Converts line objects.
                    else if (keys[0].Contains("Line"))
                    {
                        string[] parts = data[keys[0]].Split(',');
                        var p1 = new Point3d(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2]));
                        var p2 = new Point3d(double.Parse(parts[3]), double.Parse(parts[4]), double.Parse(parts[5]));
                        var o = new Line(p1, p2);
                        objects.Add(o);
                    }
                    // Converts transform objects.
                    else if (keys[0].Contains("Transform"))
                    {
                        var numbers = data[keys[0]].Split(new[] { '=', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        numbers = numbers.Where((item, index) => index != 0 && index != 5 && index != 10 && index != 15).ToList();
                        var transform = new Transform();
                        for (int j = 0; j < 4; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                transform[j, k] = double.Parse(numbers[j * 4 + k]);
                            }
                        }
                        objects.Add(transform);

                    }
                    // Handles invalid-formated data.
                    else
                    {
                        errors.Add("Invalid format in input number - " + i.ToString());
                    }
                }
            }

            return objects;
        }

        /// <summary>
        /// Serializes the list of objects to JSON.
        /// </summary>
        /// <param name="list"> List of objects to serialize.</param>
        public static string ConvertListToJson<T>(List<T> list)
        {
            string json = JsonConvert.SerializeObject(list);
            return json;
        }
        
        #endregion Methods

    }
}
