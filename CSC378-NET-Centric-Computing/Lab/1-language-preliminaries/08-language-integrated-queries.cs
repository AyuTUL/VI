// Lab 1.8 : Write a program using LINQ and lambda expressions to filter a list of integers and return only even numbers greater than 10, sorted in descending order.
using System;
using System.Collections.Generic;
using System.Linq;
class _08_LanguageIntegratedQueries
{
    static void Main()
    {
        List<int> numbers = new List<int>();

        Console.WriteLine("Enter 10 integers:");

        for (int i = 0; i < 10; i++)
        {
            numbers.Add(int.Parse(Console.ReadLine()));
        }

        var sortedDesc = numbers
            .Where(n => n > 10 && n % 2 == 0)
            .OrderByDescending(n => n);

        Console.WriteLine("Even numbers greater than 10 in descending order:");

        foreach (var n in sortedDesc)
        {
            Console.WriteLine(n);
        }
    }
}