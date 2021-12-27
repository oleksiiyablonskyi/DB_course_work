using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using ScottPlot;
namespace progbase3
{
    public static class ExportAndImport
    {
        public static void Export(string filepath, List<Movie> list)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Movie>));
            StreamWriter writer = new StreamWriter(filepath);
            ser.Serialize(writer, list);
            writer.Close();
        }
        public static List<Movie> Import(string filepath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Movie>));
            StreamReader reader = new StreamReader(filepath);
            List<Movie> value = (List<Movie>)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
        
    }
    
}