using System;

abstract class Employee
{
    public string Name { get; set; }

    public Employee(string name)
    {
        Name = name;
    }

    public abstract double CalculateSalary();
}

class Manager : Employee
{
    private double basicSalary;
    private double bonus;

    public Manager(string name, double basicSalary, double bonus)
        : base(name)
    {
        this.basicSalary = basicSalary;
        this.bonus = bonus;
    }

    public override double CalculateSalary()
    {
        return basicSalary + bonus;
    }
}

class Clerk : Employee
{
    private double hourlyRate;
    private int hoursWorked;

    public Clerk(string name, double hourlyRate, int hoursWorked)
        : base(name)
    {
        this.hourlyRate = hourlyRate;
        this.hoursWorked = hoursWorked;
    }

    public override double CalculateSalary()
    {
        return hourlyRate * hoursWorked;
    }
}

class _05_AbstractClass
{
    static void Main()
    {
        Employee manager = new Manager("Haaland", 50000, 10000);
        Employee clerk = new Clerk("Bellingham", 500, 160);

        Console.WriteLine("Manager");
        Console.WriteLine("Name : " + manager.Name);
        Console.WriteLine("Salary : " + manager.CalculateSalary());

        Console.WriteLine();

        Console.WriteLine("Clerk");
        Console.WriteLine("Name : " + clerk.Name);
        Console.WriteLine("Salary : " + clerk.CalculateSalary());
    }
}