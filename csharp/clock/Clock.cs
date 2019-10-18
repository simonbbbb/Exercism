using System;

public struct Clock
{
    private const int dayLength = 24 * 60;
    private int minutes;

    public int Hours => minutes / 60;
    public int Minutes => minutes % 60;

    private void Normalize()
    {
        if (minutes < 0)
        {
            while (minutes < 0)
            {
                minutes = dayLength + minutes;
            }
        }
        if (minutes >= dayLength)
        {
            minutes = minutes % dayLength;
        }
    }
    
    public Clock(int hours, int minutes)
    {
        this.minutes = minutes + (60 * hours);
        Normalize();
    }

    public Clock Add(int minutesToAdd)
    {
        this.minutes += minutesToAdd;
        Normalize();
        return this;
    }

    public Clock Subtract(int minutesToSubtract)
    {
        this.minutes -= minutesToSubtract;
        Normalize();
        return this;

    }

    public override string ToString()
    {
        return String.Format("{0,2:D2}:{1,2:D2}", Hours, Minutes);
        
    }
}