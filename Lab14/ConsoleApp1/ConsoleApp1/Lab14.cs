using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab14
{
    public class KMPAlgorithm
    {
        public static int[] ComputePrefixFunction(string pattern)
        {
            int[] prefixFunction = new int[pattern.Length];
            int i = 1, j = 0;

            while (i < pattern.Length)
            {
                if (pattern[i] == pattern[j])
                {
                    prefixFunction[i] = j + 1;
                    i++;
                    j++;
                }
                else
                {
                    if (j != 0)
                    {
                        j = prefixFunction[j - 1];
                    }
                    else
                    {
                        prefixFunction[i] = 0;
                        i++;
                    }
                }
            }

            return prefixFunction;
        }

        public static int KMPSearch(string text, string pattern)
        {
            int[] prefixFunction = ComputePrefixFunction(pattern);
            int i = 0, j = 0;

            while (i < text.Length && j < pattern.Length)
            {
                if (text[i] == pattern[j])
                {
                    i++;
                    j++;
                }
                else
                {
                    if (j != 0)
                    {
                        j = prefixFunction[j - 1];
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            if (j == pattern.Length)
            {
                return i - j; // Pattern found, return the starting index
            }

            return -1; // Pattern not found
        }
    }

    public class Lab14 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - znaleźć najkrótszy palindrom, który można uzyskać poprzez rozszerzanie słowa
        /// </summary>
        /// <param name="text">ciąg znaków do rozszerzenia w palindrom  </param>
        /// <returns>string - najkrótszy palindrom uzyskany z wejścia</returns>
        public string Lab14Stage1(string text)
        {
            text = "ABABDABACDABABCABAB";
            string pattern = "ABABCABAB";

            int index = KMPAlgorithm.KMPSearch(text, pattern);

            
            return "";
        }

        /// <summary>
        /// Etap 2 - znaleźć najdłuższy podciąg bez powtórzeń
        /// </summary>
        /// <param name="numbers"> ciąg liczb, w których należy znaleźć najkrótszy podciag bez powtórzeń </param>
        /// <returns>(int[], int) najdłuższy podciąg bez powtarzających się znaków i jego długość
        public (int[], int) Lab14Stage2(int[] numbers)
        {
            return (new int[0], 0);
        }
    }
}
