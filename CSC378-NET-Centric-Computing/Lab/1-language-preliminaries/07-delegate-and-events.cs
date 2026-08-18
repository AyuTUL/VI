// Lab 1.7 : Demonstrate the use of delegates and events in C# by creating a simple event that notifies subscribers when a temperature value crosses a threshold.
using System;
delegate void Notify();
class Alarm
{
    public event Notify Ring;

    public void CheckTemperature(double temperature)
    {
        if (temperature > 40)
        {
            Console.WriteLine("Warning: Temperature has crossed above 40 C!");
            Ring?.Invoke();
        }
    }
}
class _07_DelegateAndEvents
{
    static void WakeUp()
    {
        Console.WriteLine("TRING TRING!");
    }

    static void Main()
    {
        Alarm alarm = new Alarm();

        // Subscribe to the event
        alarm.Ring += WakeUp;

        // Check temperature
        alarm.CheckTemperature(45);
    }
}