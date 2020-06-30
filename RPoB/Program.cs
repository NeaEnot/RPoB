using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace RPoB
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "";

            DirectoryInfo directory = new DirectoryInfo(ConfigurationManager.AppSettings["pathToLibrary"]);
            var files = directory.GetFiles();
            
            Random rnd = new Random();

            while (true)
            {
                text = "";

                var file = files[rnd.Next(0, files.Length)];

                int linesCount = getLinesCount(file);
                int startLine = rnd.Next(0, linesCount + 1);
                text = getText(file, startLine);

                if (text.Length >= int.Parse(ConfigurationManager.AppSettings["minLength"]))
                {
                    log(file.Name, startLine);
                    break;
                }

                log(file.Name, startLine, "Текст слишком короткий: " + text.Length);
            }

            using (StreamWriter writer = new StreamWriter(ConfigurationManager.AppSettings["pathToOut"]))
            {
                writer.Write(text);
            }
        }

        private static int getLinesCount(FileInfo file)
        {
            int count = 0;

            using (StreamReader reader = new StreamReader(file.FullName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    count++;
                }
            }

            return count;
        }

        private static string getText(FileInfo file, int startLine)
        {
            string text = "";
            Random rnd = new Random();

            using (StreamReader reader = new StreamReader(file.FullName))
            {
                for (int i = 0; i < startLine; i++)
                {
                    reader.ReadLine();
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    if (line == "***")
                    {
                        if(text.Length == 0)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    text += line + "\n";

                    if (rnd.Next(0, int.Parse(ConfigurationManager.AppSettings["randomSize"])) == 0 && text.Length >= int.Parse(ConfigurationManager.AppSettings["minLength"]))
                    {
                        break;
                    }
                }
            }

            return text;
        }

        private static void log(string fileName, int startLine, string message = "Замечаний нет")
        {
            using (StreamWriter writer = new StreamWriter(ConfigurationManager.AppSettings["pathToLog"], true))
            {
                writer.WriteLine("{0} - {1} ({2}) - {3}", DateTime.Now, fileName, startLine, message);
            }
        }
    }
}
