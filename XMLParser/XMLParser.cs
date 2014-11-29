using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XMLParser
{
    class XMLTag
    {
        public String tag_name;
        public Dictionary<String, String> attributes = new Dictionary<String, String>();
        public List<XMLTag> children = new List<XMLTag>();

        public String TagName
        {
            get { return (this.tag_name); }
            set { this.tag_name = value; }
        }

        public Dictionary<String, String> Attributes
        {
            get { return (this.attributes); }
            set { this.attributes = value; }
        }

        public List<XMLTag> Chilren
        {
            get { return (this.children); }
            set { this.children = value; }
        }

    }
    class XMLParser
    {
        XMLTag root;

        public XMLParser()
        {
            root = null;
        }

        public bool isEmpty
        {
            get
            {
                if (this.root == null)
                {
                    return true;
                }
                return false;
            }

            
        }

        bool isValidTag(String xml_string)
        {
            bool valid_tag = false;


            string opening_tag_pattern = @"\<[\w]+(\s+[\w]+\s*=\s*[""][\d\w\s]*[""])*\s*\>";
            Regex opening_tag_rgx = new Regex(opening_tag_pattern);


            string tag_closing_pattern = @"\</[\w]+>";
            Regex closing_tag_rgx = new Regex(tag_closing_pattern);

            if (opening_tag_rgx.Match(xml_string).Success && opening_tag_rgx.Match(xml_string).Index == 0)
            {
                valid_tag = true;
            }
            else if (closing_tag_rgx.Match(xml_string).Success && closing_tag_rgx.Match(xml_string).Index == 0)
            {
                valid_tag = true;
            }

            return valid_tag;
        }

        public List<String> GetChildren(String xml_string)
        {
            List<String> list_children = new List<string>();
            String local_xml_string = xml_string;

            string opening_tag_pattern = @"\<[\w]+(\s+[\w]+\s*=\s*[""][\d\w\s]*[""])*\s*\>";
            Regex opening_tag_rgx = new Regex(opening_tag_pattern);


            string tag_closing_pattern = @"\</[\w]+>";
            Regex closing_tag_rgx = new Regex(tag_closing_pattern);


            /*Make sure the first tag in string is opening tag, before starting the processing*/
            if (opening_tag_rgx.Match(local_xml_string).Success && opening_tag_rgx.Match(local_xml_string).Index == 0)
            {
                try
                {

                    local_xml_string = local_xml_string.Substring((local_xml_string.IndexOf('<', 1)));

                    /*Loop on the xml string as long as there are opening tags*/
                    while (opening_tag_rgx.Match(local_xml_string).Success && opening_tag_rgx.Match(local_xml_string).Index == 0
                        && isValidTag(local_xml_string))
                    {

                        String child_string = "";
                        Stack<String> tags_stack = new Stack<string>();
                        do
                        {
                            if (opening_tag_rgx.Match(local_xml_string).Success && opening_tag_rgx.Match(local_xml_string).Index == 0)
                            {
                                /*Extract Tag Name from starting tag*/
                                String opening_tag_name_pattern = @"[\w]+";
                                Regex opening_tag_name_rgx = new Regex(opening_tag_name_pattern);
                                String opening_tag_value = opening_tag_rgx.Match(local_xml_string).Value;
                                String opening_tag_name = opening_tag_name_rgx.Match(opening_tag_rgx.Match(local_xml_string).Value).Value;

                                /*Found a opening tag so increment count*/
                                tags_stack.Push(opening_tag_name);

                                /*Concatenate the tag to child string*/
                                child_string += opening_tag_rgx.Match(local_xml_string).Value;

                                /*take the opening tag out of string to search for rest of tags*/
                                local_xml_string = local_xml_string.Substring(local_xml_string.IndexOf('<', 1));


                            }
                            else if (closing_tag_rgx.Match(local_xml_string).Success && closing_tag_rgx.Match(local_xml_string).Index == 0)
                            {
                                /*Found a closing tag so decrement count*/
                                String tag_name = tags_stack.Pop();

                                /*Extract closing tag name*/
                                String closing_tag_name_pattern = @"\</[\w]+";
                                Regex closing_tag_name_rgx = new Regex(closing_tag_name_pattern);


                                String opening_tag_value = opening_tag_rgx.Match(local_xml_string).Value;
                                String closing_tag_name = closing_tag_name_rgx.Match(closing_tag_rgx.Match(local_xml_string).Value).Value;
                                closing_tag_name = closing_tag_name.Remove(0, 2);

                                if (tag_name != closing_tag_name)
                                {
                                    list_children = null;

                                    Console.WriteLine("Closing Tag Doesn't Match!");
                                    break;
                                }

                                /*Concatenate the tag to child string*/
                                child_string += closing_tag_rgx.Match(local_xml_string).Value;

                                /*take the closing tag out of string to search for rest of tags*/
                                local_xml_string = local_xml_string.Substring(local_xml_string.IndexOf('<', 1));


                            }

                        } while (tags_stack.Count > 0 && isValidTag(local_xml_string));

                        if (list_children != null)
                        {
                            list_children.Add(child_string);
                        }
                        else
                        { break; }

                    }
                }

                catch
                {
                    list_children = null;
                    Console.WriteLine("One or More Tag Missing. Incorrect XML!");
                }
            }
            if (!isValidTag(local_xml_string))
            {
                list_children = null;
                Console.WriteLine("Invalid Tag Encountered. Incorrect Error!");
            }
            else if (closing_tag_rgx.Matches(local_xml_string).Count > 1)
            {
                list_children = null;
                Console.WriteLine("One or More Extra Tags. Incorrect XML!");
            }


            return list_children;
        }


        public XMLTag GetValue(String xml_string)
        {

            String local_xml_string = xml_string;

            XMLTag tag = new XMLTag();
            Dictionary<String, String> attributes = new Dictionary<String, String>();

            string opening_tag_pattern = @"\<[\w]+(\s+[\w]+\s*=\s*[""][\d\w\s]*[""]\s*)*\s*\>";
            Regex opening_tag_rgx = new Regex(opening_tag_pattern);

            int index = opening_tag_rgx.Match(local_xml_string).Index;
            bool success = opening_tag_rgx.Match(local_xml_string).Success;

            /*The first tag in string is opening tag*/
            if (opening_tag_rgx.Match(local_xml_string).Index == 0 && opening_tag_rgx.Match(local_xml_string).Success)
            {
                String tag_string = local_xml_string.Substring(0, local_xml_string.IndexOf('>') + 1);

                /*Tag name*/
                String tag_name_pattern = @"\<[\w]+";
                Regex tag_name_rgx = new Regex(tag_name_pattern);

                tag.tag_name = tag_name_rgx.Match(tag_string).Value.Remove(0, 1);

                String value_pattern = @"\s+[\w]+\s*=\s*[""][\d\w\s]*[""]";
                Regex value_rgx = new Regex(value_pattern);

                while (value_rgx.Match(tag_string).Success)
                {
                    Match match = value_rgx.Match(tag_string);

                    String[] attribute = match.Value.Split('=');

                    tag.attributes.Add(attribute[0], attribute[1]);

                    /*Operate on rest of string to extract remaining attributes*/
                    tag_string = tag_string.Substring(match.Index + match.Value.Count());
                }

            }


            return tag;
        }

        public String CleanXMLString(String xml_string)
        {
            string local_xml_string = xml_string;

            /*Remove top most version tag*/
            if (local_xml_string.Substring(1, 4) == "?xml")
            {
                local_xml_string = local_xml_string.Substring(local_xml_string.IndexOf('>') + 1);
            }

            /*Remove comments*/
            String comment_tag_name_pattern = @"\<!--.*--\>";
            Regex comment_tag_name_rgx = new Regex(comment_tag_name_pattern);

            local_xml_string = comment_tag_name_rgx.Replace(local_xml_string, "");

            /*Replace any closing tag with opening tag followed by closing tag*/
            String self_closing_tag_pattern = @"\<[\w]+(\s+[\w]+\s*=\s*[""][\d\w\s]*[""])*\s*/\>";
            Regex self_closing_tag_rgx = new Regex(self_closing_tag_pattern);

            while (self_closing_tag_rgx.Match(local_xml_string).Success)
            {
                String self_closing_tag = self_closing_tag_rgx.Match(local_xml_string).Value;

                /*Extract tag name*/
                String tag_name_pattern = @"[\w]+";
                Regex tag_name_rgx = new Regex(tag_name_pattern);
                String tag_name = tag_name_rgx.Match(self_closing_tag).Value;

                /*Construct pair of opening and closing tags that would replace self closing tag*/
                String open_closing_tag_pair = self_closing_tag.Substring(0, self_closing_tag.Length - 2);
                open_closing_tag_pair += "></" + tag_name + ">";


                local_xml_string = self_closing_tag_rgx.Replace(local_xml_string, open_closing_tag_pair, 1);
                //local_xml_string = local_xml_string.Replace(self_closing_tag, open_closing_tag_pair);
            }

            return local_xml_string;

        }

        public void BuildTreeFromXML(string xml_string)
        {
            /*Preprocess XML String to remove extra stuff and make it suitable for processing*/
            CleanXMLString(xml_string);

            root = BuildTree(xml_string);
        }
        private XMLTag BuildTree(string xml_string)
        {
            List<String> children = GetChildren(xml_string);
            XMLTag tag_node = GetValue(xml_string);

            if (children != null && tag_node != null)
            {
                foreach (String child in children)
                {
                    tag_node.children.Add(BuildTree(child));
                }
            }
            else
            {
                tag_node = null;
            }


            return tag_node;
        }

    }
   
}
