using System;
class _09_FileIOAndExceptionHandling
{
    static void Main()
    {
        try
        {
            File.WriteAllText("09-file-io-and-exception-handling", "Ram, Age : 20");
            Console.WriteLine("Data written successfully");
            string data = File.ReadAllText("student.txt");
            Console.WriteLine(data);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
    }
}
