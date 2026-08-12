using System;
using System.Linq;
class _08_LanguageIntegratedQueries
{
    static void Main()
    {
        List<int> numbers = new List<int>();
        Console.WriteLine("Enter 10 integers : \n");
        for(int i = 0; i < 10; i++)
        {
            numbers[i] = int.Parse(Console.ReadLine());
            
        }
        var greater = numbers.Where(m => (m > 10 && m % 2 == 0));
        var sortedDesc = greater.OrderBy(greater => greater);
        foreach (var n in sortedDesc)
            Console.WriteLine(n);
    }
}