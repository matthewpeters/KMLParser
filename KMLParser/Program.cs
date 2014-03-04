using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KMLParser
{
    class Program
    {

        public class KmlObject
        {
            public string Name { get; set; }
            public int CoordSetCount { get; set; }
        }

        static void Main()
        {
            Console.Write("Running ...");

            StringBuilder output = new StringBuilder();
            Boolean insideNameNode = false;
            int pointSetCount = 1;
            string prevKmlObjName = string.Empty;
            
            output.AppendLine("function AddParsedGeoRegions(){ ");
            
            // Create an XML reader for this file.
            using (XmlReader reader = XmlReader.Create("app2-test5.kml"))
            {
                List<string> kmlObjectNamesToGet = new List<string>();
                List<string> kmlObjectNames = new List<string>();
                string kmlObjectName = string.Empty;
                string kmlObjectCoords = string.Empty;
                var kmlObject = new List<KmlObject>();
                bool ignoreThisGrouping = false;
                bool insidePlacemark = false;
                
                kmlObjectNamesToGet.Add("mexico");

                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "Placemark":
                                //insidePlacemark = true;
                                break;
                            case "name":
                                    if (reader.Read()&&insidePlacemark)
                                    {
                                        kmlObjectName = reader.Value.Trim().ToLower();

                                        kmlObjectName = kmlObjectName.Replace(" ", "_");
                                        kmlObjectName = kmlObjectName.Replace("&", "");
                                        kmlObjectName = kmlObjectName.Replace("__", "_");
                                        kmlObjectName = kmlObjectName.Replace("(", "");
                                        kmlObjectName = kmlObjectName.Replace(")", "");
                                        kmlObjectName = kmlObjectName.Replace(".", "");
                                        kmlObjectName = kmlObjectName.Replace(",", "");
                                        kmlObjectName = kmlObjectName.Replace("'", "");
                                        kmlObjectName = kmlObjectName.Replace(">", "");
                                        kmlObjectName = kmlObjectName.Replace("<", "");

                                        kmlObjectNames.Add(kmlObjectName);
                                    }
                                break;
                            case "Data":
                                // Search for the attribute name on this current node.
                                string attribute = reader["name"];
                                if (attribute != null)
                                {
                                    insideNameNode = attribute == "NAME" || attribute == "Country";
                                }
                                break;
                            case "value":
                                if (insideNameNode)
                                {
                                    if (reader.Read())
                                    {
                                        kmlObjectName = reader.Value.Trim().ToLower();
                                        //Console.WriteLine("name: " + kmlObjectName);
                                        //output.AppendLine("  name: " + reader.Value.Trim());
                                        
                                        kmlObjectName = kmlObjectName.Replace(" ", "_");
                                        kmlObjectName = kmlObjectName.Replace("&", "");
                                        kmlObjectName = kmlObjectName.Replace("__", "_");
                                        kmlObjectName = kmlObjectName.Replace("(", "");
                                        kmlObjectName = kmlObjectName.Replace(")", "");
                                        kmlObjectName = kmlObjectName.Replace(".", "");
                                        kmlObjectName = kmlObjectName.Replace(",", "");
                                        kmlObjectName = kmlObjectName.Replace("'", "");
                                        kmlObjectName = kmlObjectName.Replace(">", "");
                                        kmlObjectName = kmlObjectName.Replace("<", "");
                                        
                                        kmlObjectNames.Add(kmlObjectName);
                                        //kmlObject.Add(new KmlObject { CoordSetCount = 0, Name = kmlObjectName });
                                        
                                        //output.AppendLine("  //" + kmlObjectName);
                                        //output.AppendLine("  var " + kmlObjectName + "_points = []; ");
                                        insideNameNode = false;

                                        //if (kmlObjectNamesToGet.Contains(kmlObjectName))
                                        //{
                                        //    //proceed
                                        //    ignoreThisGrouping = false;
                                        //}
                                        //else
                                        //{
                                        //    //do not do anything with it
                                        //    ignoreThisGrouping = true;
                                        //}
                                        
                                    }
                                }
                                break;
                            case "coordinates":

                                //if (ignoreThisGrouping)
                                //{
                                //    break;
                                //}

                                //Console.WriteLine("Start <coordinates> element.");
                                //output.AppendLine("Start <coordinates> element.");
                                if (reader.Read())
                                {
                                    kmlObjectCoords = reader.Value.Trim();
                                    //Console.WriteLine("coordinates for "+kmlObjectName+" : " + kmlObjectCoords);
                                    //output.AppendLine("coordinates for " + kmlObjectName + " : " + kmlObjectCoords);

                                    string[] split = kmlObjectCoords.Split(new Char[] { ' ' });
                                    string[] realCoords = new string[split.Length];

                                    if (kmlObjectName == prevKmlObjName)
                                    {
                                        //iterate point set count
                                        pointSetCount++;
                                    }
                                    else
                                    {
                                        //kmlObjectPointSets.Add(pointSetCount);
                                        Console.WriteLine(" [DEBUG]: pointsetcount=" + pointSetCount + " for " + kmlObjectName);
                                        //Console.ReadLine();
                                        kmlObject.Add(new KmlObject { CoordSetCount = pointSetCount, Name = kmlObjectName });

                                        //reset point set count
                                        pointSetCount = 1;
                                        prevKmlObjName = kmlObjectName;
                                    }

                                    output.AppendLine("  //" + kmlObjectName);
                                    output.AppendLine("  var " + kmlObjectName + "_pointset_" + pointSetCount + " = [ ");

                                    for (int j = 0; j < split.Length; j++)
                                    {
                                        string[] split2 = split[j].Split(new Char[] { ',' });
                                        //Console.WriteLine(split2[1] + "," + split2[0]);
                                        realCoords[j] = split2[1] + "," + split2[0];
                                        //Console.WriteLine(kmlObjectName + ": " + realCoords[j]);
                                        output.AppendLine("    new google.maps.LatLng( " + realCoords[j] + " ), ");
                                    }
                                    
                                    output.AppendLine("  ]; ");
                                    output.AppendLine("");



                                    //Console.Write(".");
                                }
                                break;
                        }
                    }
                }

                //for (int i = 0; i < kmlObject.Count; i++)
                //{
                //    int containsCount = 0;
                //    if (kmlObject[i].Name.Contains(kmlObjectNames[i]))
                //    {
                //        containsCount++;
                //        if (containsCount > 1)
                //        {
                //            Console.WriteLine("duplicate: " + kmlObjectNames[i]);
                //            Console.ReadLine();
                //        }
                //    }
                //}

                //for (int i = 0; i < kmlObjectNames.Count; i++)
                //{
                //    //see if each name is in the kmlobj and how many times is it in there
                //}

                //when we are done
                if ((kmlObject.Count == kmlObjectNames.Count) && kmlObject.Count > 0) 
                {
                    for (int i = 0; i < kmlObject.Count; i++)
                    {
                        //kmlObject[i].Name = kmlObject[(i + 1)].Name;
                        if (i==(kmlObject.Count-1))
                        {
                            //kmlObject[i].CoordSetCount = kmlObject[(i + 1)].CoordSetCount;
                        }
                        else
                        {
                            kmlObject[i].CoordSetCount = kmlObject[(i + 1)].CoordSetCount;
                        }
                        
                    }
                    kmlObject.RemoveAt((kmlObjectNames.Count - 1));
                    kmlObject.Add(new KmlObject { CoordSetCount = pointSetCount, Name = kmlObjectName });
                }

                //determine if there are any duplicates
                for (int i = 0; i < kmlObject.Count; i++)
                {
                    int nameIsThereCount = 0;
                    for (int j = 0; j < kmlObjectNames.Count; j++)
                    {

                        if (kmlObject[i].Name == kmlObjectNames[j])
                        {
                            nameIsThereCount++;
                        }

                    }
                    if (nameIsThereCount > 1)
                    {
                        //change the duplicate

                        Console.WriteLine("duplicate: " + kmlObject[i].Name);
                        //Console.ReadLine();
                    }
                }
                
                //for (int l = 0; l < kmlObjectPointSets.Count; l++)
                //{
                //    kmlObject.Add(new KmlObject { CoordSetCount = kmlObjectPointSets[l], Name = kmlObjectNames[l] });
                //}

                //rearrage
                

                //kmlObject.RemoveAt(0);
                string[] tempA = kmlObjectNames.ToArray();
                for (int j = 0; j < tempA.Length; j++)
                {
                    //Console.Write(".");
                    //Console.WriteLine(tempA[j]);
                    
                    output.AppendLine("  //contruct the polygon ");
                    output.AppendLine("  var " + tempA[j] + "_polygon =  new google.maps.Polygon({ ");
                    if (kmlObject[j].Name==tempA[j] && kmlObject[j].CoordSetCount>1)
                    {
                        //string[] tempB = kmlObjectPointSet.ToArray();
                        output.AppendLine("    paths: [");
                        for (int k = 0; k < kmlObject[j].CoordSetCount; k++)
                        {
                            output.AppendLine("            " + tempA[j] + "_pointset_" + (k + 1) + ", ");
                            //output.AppendLine("            " + tempA[j] + "_pointset_" + (k + 1) + ", ");
                        }
                        output.AppendLine("           ],");
                    }
                    else
                    {
                        output.AppendLine("    paths: " + tempA[j] + "_pointset_1, ");
                    }
                    
                    output.AppendLine("    strokeColor: defaultOutlineColor, ");
                    output.AppendLine("    strokeOpacity: defaultStrokeOpacity, ");
                    output.AppendLine("    strokeWeight: defaultStrokeWeight, ");
                    output.AppendLine("    fillColor: defaultFillColor, ");
                    output.AppendLine("    fillOpacity: defaultFillOpacity ");
                    output.AppendLine("  }); ");
                    output.AppendLine("");
                    output.AppendLine("  //add event listeners to polygon, then add polygon to map ");
                    output.AppendLine("  google.maps.event.addListener(" + tempA[j] + "_polygon, 'mouseover',  function() { " + tempA[j] + "_polygon.setOptions({strokeOpacity: 1, fillColor: defaultHighlightColor}); });");
                    output.AppendLine("  google.maps.event.addListener(" + tempA[j] + "_polygon, 'mouseout', function() { " + tempA[j] + "_polygon.setOptions({strokeOpacity: 0, fillColor: defaultFillColor}); });");
                    output.AppendLine("  google.maps.event.addListener(" + tempA[j] + "_polygon, 'click', function() { polygonClickedActionHandler(\"" + tempA[j] + "\"); });");
                    output.AppendLine("  " + tempA[j] + "_polygon.setMap(map); ");
                    output.AppendLine("");
                }

                output.AppendLine("} ");
                
                //output 
                System.IO.StreamWriter outputFile = new System.IO.StreamWriter("C:\\Users\\cadetpeters89\\Documents\\CUSTOM\\projects\\git\\SobekCM-Web-Application\\SobekCM\\dev\\mapedit\\sandbox\\output.js");
                outputFile.WriteLine(output);
                
                outputFile.Close(); //close output

                Console.Write(" Completed!");
                //Console.ReadLine(); //pause
            }
        }
    }
}
