using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonParser
{
    internal static class Program
    {
        static void Main()
        {

            Console.WriteLine("Paste your json content below and end with an empty line:");

            using (var input = new MemoryStream(Encoding.ASCII.GetBytes(ReadLines())))
            {
                using (var output = AttributeFirstJsonTransformer.Transform(input))
                {
                    Console.WriteLine("Output:");
                    using (var reader = new StreamReader(output))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
            }
        }

        static string ReadLines()
        {
            IEnumerable<string> InfiniteReadLines()
            {
                while (true) yield return Console.ReadLine();
            }

            return string.Join(Environment.NewLine, InfiniteReadLines().TakeWhile(line => !string.IsNullOrEmpty(line)));
        }
    }
}