// Lab 1.3 : Write a program using an indexer in a class Week that allows accessing day names using an integer index(e.g., week[0] returns "Sunday").
using System;

class Week
{
    private string[] days =
    {
        "Sunday", "Monday", "Tuesday",
        "Wednesday", "Thursday", "Friday", "Saturday"
    };

    public string this[int index]
    {
        get
        {
            if (index >= 0 && index < days.Length)
                return days[index];
            else
                return "Invalid Index";
        }
    }
}

class _03_Indexer
{
    static void Main()
    {
        Week week = new Week();

        Console.Write("Enter an integer (0-6): ");
        int index = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Day: " + week[index]);
    }
}