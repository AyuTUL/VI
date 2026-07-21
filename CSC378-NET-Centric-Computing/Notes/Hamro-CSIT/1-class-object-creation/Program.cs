using System;
using System.Runtime.InteropServices;
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
        Console.WriteLine("\nStudent Details :");
        Console.WriteLine("Name  : " + Name);
        Console.WriteLine("Roll  : " + Roll);
        Console.WriteLine("Marks : " + Marks);
    }
}

class ClassObjectCreation
{
    static void Main()
    {
        Console.Write("Enter student details:\nEnter name : ");
        string name = Console.ReadLine();

        Console.Write("Enter roll : ");
        int roll = int.Parse(Console.ReadLine());

        Console.Write("Enter marks : ");
        float marks = float.Parse(Console.ReadLine());

        Student s = new Student(name, roll, marks);

        s.Display();
    }
}