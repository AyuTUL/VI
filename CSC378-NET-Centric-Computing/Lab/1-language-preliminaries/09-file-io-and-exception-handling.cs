// Lab 1.9 : Write a C# program that reads data from a text file and writes processed output to another file using File I/O classes (StreamReader/StreamWriter), handling exceptions using try-catch-finally.
using System;
using System.IO;
class _09_FileIOAndExceptionHandling
{
    static void Main()
    {
        string inputFile = "student.txt";
        string outputFile = "processed-student.txt";

        try
        {
            // Write data to the input file
            using (StreamWriter writer = new StreamWriter(inputFile))
            {
                writer.WriteLine("Ram, Age : 20");
            }

            Console.WriteLine("Data written successfully.");

            // Read data from the input file
            string data;

            using (StreamReader reader = new StreamReader(inputFile))
            {
                data = reader.ReadToEnd();
            }

            Console.WriteLine("Data read from file:");
            Console.WriteLine(data);

            // Write processed data to another file
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Processed Data:");
                writer.WriteLine(data.ToUpper());
            }

            Console.WriteLine("Processed data written successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            Console.WriteLine("File operation completed.");
        }
    }
}