using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class RandomUtils
{
    private static readonly System.Random _rnd = new System.Random();

    public static Color GetRandomColor()
    {
        var c = _rnd.Next();
        float Ch(int channel) => (float)(((c >> (channel << 3)) & 0xFF) / 255.0);

        return new Color(Ch(0), Ch(1), Ch(2));
    }
}