// Lab 1.2 : Create a base class Shape with a virtual method Area() and derive classes Circle and Rectangle that override Area(). Demonstrate runtime polymorphism by calling Area() through a base class reference.
using System;
class Shape
{
    public virtual double Area()
    {
        return 0.0;
    }
}
class Circle : Shape
{
    public double radius;
    public Circle(double r)
    {
        radius = r;
    }
    public override double Area()
    {
        return 3.14 * radius * radius;
    }
}
class Rectangle : Shape
{
    public int length, breadth;
    public Rectangle(int l, int b)
    {
        length = l;
        breadth = b;
    }
    public override double Area()
    {
        return length * breadth;
    }
}
class _02_RunTimePolymorphism
{
    static void Main()
    {
        Shape s;
        s = new Circle(3.44);
        Console.WriteLine("Area of Circle = " + s.Area());
        s = new Rectangle(7, 5);
        Console.WriteLine("Area of Rectangle = " + s.Area());
    }
}