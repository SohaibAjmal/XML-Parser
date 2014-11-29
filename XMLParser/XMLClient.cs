using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XMLParser
{
    class XMLClient
    {
        
        static void Main(string[] args)
        {
            string line;
            string xml_string = "";

            string path = args[0];

    
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                xml_string += line.Trim();

            }
            file.Close();

            


            XMLParser xml_parser = new XMLParser();


            xml_parser.BuildTreeFromXML(xml_string);

            if (!xml_parser.isEmpty)
            {
                Console.WriteLine("XML Tree Constructed Successfully!");
            }
            else
            {
                Console.WriteLine("\nXML Tree Construction Failed!");
            }
            

            Console.ReadLine();
        }
    }
}
