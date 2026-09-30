using System;

public readonly struct Money
{
    private readonly decimal value;

    public Money(decimal value)
    {
        this.value = value;
    }

    public decimal Value => value;

    public Money Add(Money other)
    {
        return new Money(value + other.value);
    }

    public Money Subtract(Money other)
    {
        return new Money(value - other.value);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(value * multiplier);
    }

    public bool IsGreaterThanOrEqual(Money other)
    {
        return value >= other.value;
    }

    public bool IsLessThan(Money other)
    {
        return value < other.value;
    }

    public bool IsZero()
    {
        return value == 0;
    }

    public string ToFormattedString()
    {
        if (value >= 1_000_000_000m)
        {
            return $"{value / 1_000_000_000m:0.##}B";
        }

        if (value >= 1_000_000m)
        {
            return $"{value / 1_000_000m:0.##}M";
        }

        if (value >= 1_000m)
        {
            return $"{value / 1_000m:0.##}K";
        }

        return value.ToString("0.##");
    }

    public override string ToString()
    {
        return ToFormattedString();
    }
}