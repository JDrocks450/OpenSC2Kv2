using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API.Prim
{
    public static class MathExtensions
    {
        public static int gcf(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static int lcm(int a, int b)
        {
            return (a / gcf(a, b)) * b;
        }

        public static int lcm(params int[] numbers)
        {
            if (numbers.Length == 0) return 0;
            return numbers.Aggregate((S, val) => S * val / gcf(S, val));
        }
    }
}
