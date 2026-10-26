using System;

// Integral of marginal gains. Thresholds are not hard limits; the final rate continues.
[Serializable]
public sealed class AttributeSoftCapCurve
{
    public CharacterAttribute attribute;
    public int[] thresholds = Array.Empty<int>();
    public double[] gains = new[] { 1d };

    public double Evaluate(int value)
    {
        if (value <= 0) return 0;
        double result = 0; int previous = 0;
        for (int i = 0; i < (thresholds?.Length ?? 0); i++)
        {
            int end = Math.Max(previous, thresholds[i]);
            result += Math.Max(0, Math.Min(value, end) - previous) * Rate(i);
            if (value <= end) return result;
            previous = end;
        }
        return result + Math.Max(0, (long)value - previous) * Rate(thresholds?.Length ?? 0);
    }
    private double Rate(int index)
    {
        double value = gains == null || gains.Length == 0 ? 0 : gains[Math.Min(index, gains.Length - 1)];
        return double.IsNaN(value) || double.IsInfinity(value) ? 0 : Math.Max(0, value);
    }
    public int NextCap(int value)
    {
        foreach (int cap in thresholds ?? Array.Empty<int>()) if (cap > value) return cap;
        return 99;
    }
    public static AttributeSoftCapCurve Default(CharacterAttribute attribute)
    {
        var curve = new AttributeSoftCapCurve { attribute = attribute };
        switch (attribute)
        {
            case CharacterAttribute.Strength: case CharacterAttribute.Dexterity: case CharacterAttribute.MagicPower:
                curve.thresholds = new[] { 20, 55, 80 }; curve.gains = new[] { 1d, .5d, .25d, .1d }; break;
            case CharacterAttribute.Endurance:
                curve.thresholds = new[] { 30, 50, 60 }; curve.gains = new[] { 1d, .5d, .25d, .1d }; break;
            case CharacterAttribute.Vitality:
                curve.thresholds = new[] { 40, 60 }; curve.gains = new[] { 8d, 4d, 1d }; break;
            case CharacterAttribute.Intelligence:
                curve.thresholds = new[] { 40, 60 }; curve.gains = new[] { 4d, 2d, .5d }; break;
            default:
                curve.thresholds = new[] { 30, 60 }; curve.gains = new[] { 1d, .5d, .2d }; break;
        }
        return curve;
    }
}
