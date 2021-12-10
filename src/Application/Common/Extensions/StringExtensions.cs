namespace Application.Common.Extensions;

public static class StringExtensions
{
   public static bool IsNullOrEmpty(this string s)
   {
       return s is not null && string.IsNullOrWhiteSpace(s);
   }
}