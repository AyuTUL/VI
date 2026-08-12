using System;
interface IPayable
{
    void Pay();
}
class Invoice : IPayable
{
    public void Pay()
    {
        Console.WriteLine("PAY");
    }
}
class Salary : IPayable
{
    public void Pay()
    {
        Console.WriteLine("PAY");
    }
}
class _06_Polymorphism
{
    static void Main()
    {
        List<IPayable> wrong = new List<IPayable>();
    }
}