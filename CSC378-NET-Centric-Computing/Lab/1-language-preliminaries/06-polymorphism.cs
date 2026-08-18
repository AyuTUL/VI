// Lab 1.6 : Write a program that defines an interface IPayable with a method Pay(), implement it in two classes Invoice and Salary, and call the method polymorphically using a List<IPayable>.
using System;
using System.Collections.Generic;
interface IPayable
{
    void Pay();
}
class Invoice : IPayable
{
    public void Pay()
    {
        Console.WriteLine("Invoice paid");
    }
}
class Salary : IPayable
{
    public void Pay()
    {
        Console.WriteLine("Salary paid");
    }
}
class _06_Polymorphism
{
    static void Main()
    {
        List<IPayable> payables = new List<IPayable>();

        payables.Add(new Invoice());
        payables.Add(new Salary());

        foreach (IPayable payable in payables)
        {
            payable.Pay();
        }
    }
}