using System.Collections.Generic;

namespace MAB.RegexUtils
{
    public class PatternInfo
    {
        public string Pattern { get; set; }

        public string OptimisedPattern { get; set; }

        public Stack<int> Count { get; set; }
            = new Stack<int>();

        public int Digits { get; set; }
    }
}