using Claunia.PropertyList;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XTensions;

namespace XTensions
{
	class CommonMethods
	{
        public static int ShouldStop()
        {
            if (HelperMethods.ShouldStop())
            {
                HelperMethods.OutputMessage("Stopping X-Tension");
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Prepare temp file with sam/system contents to use for processing.
        /// </summary>
        public static string Prep_temp(IntPtr hItem, string extension)
        {
            // create temp file 
            string filename = Path.GetTempFileName();
            System.IO.File.Move(filename, Path.ChangeExtension(filename, extension));
            filename = Path.ChangeExtension(filename, extension);
            FileInfo fi = new FileInfo(filename);
            fi.Attributes = FileAttributes.Temporary;

            // add data to temp file
            byte[] item_file = HelperMethods.ReadItem(hItem);
            System.IO.File.WriteAllBytes(filename, item_file);

            return filename;
        }

        public static XDocument Prep_plist(IntPtr hItem, string type)
        {
            var bytearray = HelperMethods.ReadItem(hItem);
            var xmlstring = "";

            try
            {
                if (type == "bplist")
                {
                    var parsed = BinaryPropertyListParser.Parse(bytearray);
                    xmlstring = parsed.ToXmlPropertyList();
                }
                else
                {
                    xmlstring = Encoding.UTF8.GetString(bytearray);
                }
                return XDocument.Parse(new string(xmlstring.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray()));
            }
            catch (Exception e)
            {
                HelperMethods.OutputMessage($"[ERROR] Failed to parse plist to XDocument: {e}");
                return new XDocument();
            }
        }

        /// <summary>
        /// Create a new process to execute a powershell command.
        /// </summary>
        /// <param name="exe">Powershell exe</param>
        /// <param name="cmd">Command to be executed by 'exe' parameter.</param>
        /// <returns>Returns output and error output from command.</returns>
        public static string[] StartProcess(string exe, string cmd)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = cmd;
            p.StartInfo.CreateNoWindow = true;
            if (cmd.Contains("RegRipper"))
            {
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            }
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            return new string[] { output, error };
        }

        public static string[] Get_split(string el, string del)
        {
            return el.Split(new string[] { del }, StringSplitOptions.None);
        }

    }
    
}

