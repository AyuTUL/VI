// Lab 1.4 : Implement a generic class Stack<T> with Push, Pop, and Peek methods, and demonstrate its use with both int and string data types.
using System;
using System.Collections.Generic;

class Stack<T>
{
    private List<T> items = new List<T>();
    public void Push(T item)
    {
        items.Add(item);
    }

    public T Pop()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        T item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        return item;
    }

    public T Peek()
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        return items[items.Count - 1];
    }
}

class _04_GenericClass
{
    static void Main()
    {
        Stack<int> intStack = new Stack<int>();

        intStack.Push(7);
        intStack.Push(10);
        intStack.Push(20);
        
        Console.WriteLine("Integer Stack :\nTop Element : {0}\nPopped Element : {1}\nTop Element after Pop : {2}", intStack.Peek(), intStack.Pop(), intStack.Peek());

        Stack<string> stringStack = new Stack<string>();

        stringStack.Push("Heung-Min Son");
        stringStack.Push("Harry Kane");
        stringStack.Push("Dele Alli");
        
        Console.WriteLine("\nString Stack :\nTop Element : {0}\nPopped Element : {1}\nTop Element after Pop : {2}", stringStack.Peek(), stringStack.Pop(), stringStack.Peek());
    }
}