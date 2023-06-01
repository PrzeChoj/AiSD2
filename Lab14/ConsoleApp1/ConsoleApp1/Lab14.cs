using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab14
{
    public class Lab14 : MarshalByRefObject
    {
        private static string ReverseString(string text)
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static int[] BuildPrefixTable(string text)
        {
            int[] prefixTable = new int[text.Length];
            int j = 0;

            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == text[j])
                {
                    prefixTable[i] = j + 1;
                    j++;
                }
                else
                {
                    if (j != 0)
                    {
                        j = prefixTable[j - 1];
                        i--;
                    }
                    else
                    {
                        prefixTable[i] = 0;
                    }
                }
            }

            return prefixTable;
        }
        
        /// <summary>
        /// Etap 1 - znaleźć najkrótszy palindrom, który można uzyskać poprzez rozszerzanie słowa
        /// </summary>
        /// <param name="text">ciąg znaków do rozszerzenia w palindrom  </param>
        /// <returns>string - najkrótszy palindrom uzyskany z wejścia</returns>
        public string Lab14Stage1(string text)
        {
            string reversedText = ReverseString(text);
            string modifiedText = text + "#" + reversedText;

            int[] prefixTable = BuildPrefixTable(modifiedText);
            

            int palindromeLength = prefixTable[prefixTable.Length - 1];
            string palindromeSuffix = text.Substring(palindromeLength);

            string shortestPalindrome = ReverseString(palindromeSuffix) + text;

            return shortestPalindrome;
        }

        /// <summary>
        /// Etap 2 - znaleźć najdłuższy podciąg bez powtórzeń
        /// </summary>
        /// <param name="numbers"> ciąg liczb, w których należy znaleźć najkrótszy podciag bez powtórzeń </param>
        /// <returns>(int[], int) najdłuższy podciąg bez powtarzających się znaków i jego długość
        public (int[], int) Lab14Stage2(int[] numbers)
        {
            Dictionary<int, int> lastIndexMap = new Dictionary<int, int>();
            int maxLength = 0;
            int maxLengthIndex = 0;
            int currentLength = 0;
            int currentStartIndex = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                if (lastIndexMap.ContainsKey(numbers[i]))
                {
                    int lastIndex = lastIndexMap[numbers[i]];
                    if (lastIndex >= currentStartIndex)
                    {
                        currentStartIndex = lastIndex + 1;
                        currentLength = i - currentStartIndex;
                    }
                }

                lastIndexMap[numbers[i]] = i;
                currentLength++;

                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    maxLengthIndex = currentStartIndex;
                }
            }

            int[] longestSubsequence = new int[maxLength];
            for (int i = maxLengthIndex; i < maxLengthIndex + maxLength; i++)
            {
                longestSubsequence[i - maxLengthIndex] = numbers[i];
            }

            return (longestSubsequence, maxLength);
        }
    }
}
