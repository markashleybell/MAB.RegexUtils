using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAB.RegexUtils
{
    public static class Functions
    {
        public static string GenerateNumericRangeRegex(int min, int max, Func<string, string> postProcess = null)
        {
            if (max <= min)
            {
                throw new ArgumentOutOfRangeException(nameof(max), $"{nameof(max)} must be greater than {nameof(min)}");
            }

            var _postProcess = postProcess ?? (s => s);

            if (max - min == 1)
            {
                return _postProcess($"{min}|{max}");
            }

            if (max < 10)
            {
                return _postProcess($"[{min}-{max}]");
            }

            return null;
        }

        public static PatternInfo[] SplitToPatterns(int min, int max)
        {
            var ranges = SplitToRanges(min, max);
            var tokens = new List<PatternInfo>();
            var start = min;
            var previous = default(PatternInfo);

            for (var i = 0; i < ranges.Length; i++)
            {
                var end = ranges[i];
                var current = RangeToPattern(start, end);

                if (previous?.Pattern == current.Pattern)
                {
                    if (previous.Count.Count > 1)
                    {
                        previous.Count.Pop();
                    }

                    previous.Count.Push(current.Count.ToArray()[0]);

                    previous.OptimisedPattern = previous.Pattern + ToQuantifier(previous.Count);

                    start = end + 1;

                    continue;
                }

                current.OptimisedPattern = current.Pattern + ToQuantifier(current.Count);

                tokens.Add(current);

                start = end + 1;

                previous = current;
            }

            return tokens.ToArray();
        }

        public static string ToQuantifier(Stack<int> digits)
        {
            if (digits.Count == 0)
            {
                return string.Empty;
            }

            var d = digits.ToArray();

            Array.Reverse(d);

            var start = d[0];
            var stop = d.Length > 1 ? d[1] : default(int?);

            if (stop.HasValue || start > 1)
            {
                return $"{{{start + (stop.HasValue ? "," + stop : string.Empty)}}}";
            }

            return string.Empty;
        }

        public static PatternInfo RangeToPattern(int min, int max)
        {
            if (min == max)
            {
                return new PatternInfo
                {
                    Pattern = min.ToString()
                };
            }

            var zip = GetDigits(min).Zip(GetDigits(max), (f, s) => (f, s)).ToArray();
            var digits = zip.Length;
            var pattern = new StringBuilder();
            var count = 0;

            for (var i = 0; i < digits; i++)
            {
                var (startDigit, stopDigit) = zip[i];

                if (startDigit == stopDigit)
                {
                    pattern.Append(startDigit);
                }
                else if (startDigit != 0 || stopDigit != 9)
                {
                    pattern.AppendFormat("[{0}{1}{2}]", startDigit, stopDigit - startDigit == 1 ? string.Empty : "-", stopDigit);
                }
                else
                {
                    count++;
                }
            }

            if (count > 0)
            {
                pattern.Append("[0-9]");
            }

            return new PatternInfo
            {
                Pattern = pattern.ToString(),
                Count = new Stack<int>(new[] { count }),
                Digits = digits
            };
        }

        public static int[] SplitToRanges(int min, int max)
        {
            var nines = 1;
            var zeros = 1;

            var stop = ReplaceLastNDigitsWithNines(min, nines);
            var stops = new HashSet<int>() { max };

            while (min <= stop && stop <= max)
            {
                stops.Add(stop);
                nines++;
                stop = ReplaceLastNDigitsWithNines(min, nines);
            }

            stop = CountZeros(max + 1, zeros) - 1;

            while (min < stop && stop <= max)
            {
                stops.Add(stop);
                zeros++;
                stop = CountZeros(max + 1, zeros) - 1;
            }

            var arr = stops.ToArray();

            Array.Sort(arr, (a, b) => a > b ? 1 : b > a ? -1 : 0);

            return arr;
        }

        public static int CountZeros(int n, int zeros) =>
            n - (int)(n % Math.Pow(10, zeros));

        public static int ReplaceLastNDigitsWithNines(int n, int digitsToReplace)
        {
            var digits = GetDigits(n);

            int[] newDigits;

            if (digitsToReplace > digits.Length)
            {
                newDigits = new int[digitsToReplace];

                Fill(newDigits, 9);
            }
            else
            {
                newDigits = new int[digits.Length];

                Array.Copy(digits, newDigits, digits.Length - digitsToReplace);

                var replacementDigits = new int[digitsToReplace];

                Fill(replacementDigits, 9);

                Array.Copy(replacementDigits, 0, newDigits, digits.Length - digitsToReplace, replacementDigits.Length);
            }

            long newValue = 0;

            for (var i = newDigits.Length - 1; i >= 0; i--)
            {
                newValue += ((long)Math.Pow(10, newDigits.Length - i - 1)) * newDigits[i];
            }

            if (newValue > int.MaxValue)
            {
                throw new OverflowException($"Value {newValue} overflows Int32");
            }

            return (int)newValue;
        }

        public static int[] GetDigits(int n)
        {
            if (n == 0)
            {
                return new[] { 0 };
            }

            var (digits, abs) = NumberOfDigitsAndAbs(n);

            var res = new int[digits];
            var count = 0;

            while (abs > 0)
            {
                res[count++] = abs % 10;
                abs /= 10;
            }

            Array.Reverse(res);

            return res;
        }

        public static (int Digits, int Absolute) NumberOfDigitsAndAbs(int n)
        {
            var abs = Math.Abs(n);

            var digits = NumberOfDigits(abs);

            return (digits, abs);
        }

        public static int NumberOfDigits(long n)
        {
            if (n >= 0)
            {
                if (n < 10L) return 1;
                if (n < 100L) return 2;
                if (n < 1000L) return 3;
                if (n < 10000L) return 4;
                if (n < 100000L) return 5;
                if (n < 1000000L) return 6;
                if (n < 10000000L) return 7;
                if (n < 100000000L) return 8;
                if (n < 1000000000L) return 9;
                if (n < 10000000000L) return 10;
                if (n < 100000000000L) return 11;
                if (n < 1000000000000L) return 12;
                if (n < 10000000000000L) return 13;
                if (n < 100000000000000L) return 14;
                if (n < 1000000000000000L) return 15;
                if (n < 10000000000000000L) return 16;
                if (n < 100000000000000000L) return 17;
                if (n < 1000000000000000000L) return 18;
                return 19;
            }
            else
            {
                if (n > -10L) return 2;
                if (n > -100L) return 3;
                if (n > -1000L) return 4;
                if (n > -10000L) return 5;
                if (n > -100000L) return 6;
                if (n > -1000000L) return 7;
                if (n > -10000000L) return 8;
                if (n > -100000000L) return 9;
                if (n > -1000000000L) return 10;
                if (n > -10000000000L) return 11;
                if (n > -100000000000L) return 12;
                if (n > -1000000000000L) return 13;
                if (n > -10000000000000L) return 14;
                if (n > -100000000000000L) return 15;
                if (n > -1000000000000000L) return 16;
                if (n > -10000000000000000L) return 17;
                if (n > -100000000000000000L) return 18;
                if (n > -1000000000000000000L) return 19;
                return 20;
            }
        }

        public static void Fill<T>(T[] a, T v)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = v;
            }
        }
    }
}