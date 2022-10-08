<Query Kind="Program">
  <Reference Relative="..\src\MAB.RegexUtils\bin\Debug\netstandard2.0\MAB.RegexUtils.dll">D:\Src\Personal\MAB.RegexUtils\src\MAB.RegexUtils\bin\Debug\netstandard2.0\MAB.RegexUtils.dll</Reference>
  <Namespace>MAB.RegexUtils</Namespace>
  <Namespace>static MAB.RegexUtils.Functions</Namespace>
</Query>

void Main()
{
    string.Join("|", SplitToPatterns(1, 1000).Select(p => p.OptimisedPattern)).Dump();
    string.Join("|", SplitToPatterns(10, 1234).Select(p => p.OptimisedPattern)).Dump();
    
    // ReplaceLastNDigitsWithNines(1, 10).Dump();
}
