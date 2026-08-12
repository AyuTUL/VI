using System;
delegate void Notify();
class Alarm
{
    public event Notify Ring;
    public void Start()
    {
        Console.WriteLine("Warning : Temperature has crossed above 40 C!");
        Ring?.Invoke();
    }
}
class _07_DelegateAndEvents
{
    static void WakeUp()
    {
        Console.WriteLine("TRING");
    }
    static void Main()
    {
        Alarm alarm = new Alarm();
        alarm.Ring += WakeUp;
        alarm.Start();
    }
}