// Lab 1.1 : Write a C# console program that defines a class Student with properties Name, Roll, and Marks using auto-implemented properties, and demonstrate object creation along with a constructor that initializes all three fields.
using System;
class Student
{
    public string Name { get; set; }
    public int Roll { get; set; }
    public float Marks { get; set; }

    public Student(string name, int roll, float marks)
    {
        Name = name;
        Roll = roll;
        Marks = marks;
    }
    public void Display()
    {
        Console.WriteLine("\nStudent Details :\nName : {0}\nRoll : {1}\nMarks : {2}", Name, Roll, Marks);
    }
}

class _01_ClassObjectCreation
{
    static void Main()
    {
        Console.Write("Enter student details :\nEnter name : ");
        string name = Console.ReadLine();

        Console.Write("Enter roll : ");
        int roll = int.Parse(Console.ReadLine());

        Console.Write("Enter marks : ");
        float marks = float.Parse(Console.ReadLine());

        Student s = new Student(name, roll, marks);

        s.Display();
    }
}