using System;
class Shape
{
    public double area;
    public virtual void Area()
    {
        Console.WriteLine("Area of Shape :");
    }
}
class Circle : Shape
{
    public double radius;
    public Circle (double r)
    {
        radius = r;
    }
    public override void Area() 
    {
        area = 3.14 * radius * radius;
        Console.WriteLine("Circle = "+area);
    }
}
class Rectangle : Shape
{
    public int length, breadth;
    public Rectangle(int l,int b)
    {
        length = l;
        breadth = b;
    }
    public override void Area()
    {
        area = length*breadth;
        Console.WriteLine("Rectangle = " + area);
    }
}
class Program
{
    static void Main()
    {
        Circle c =new Circle(3.44);
        c.Area();
        Rectangle r = new Rectangle(7, 5);
        r.Area();
    }
}