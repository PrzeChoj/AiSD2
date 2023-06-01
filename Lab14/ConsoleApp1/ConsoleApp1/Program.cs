using System;
using ASD;
using System.Linq;

namespace Lab14
{
    public abstract class Lab14TestCase : TestCase
    {
        protected Lab14TestCase(double timeLimit, string description) : base(timeLimit, null, description)
        {
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution();
            return (code, $"{msg} [{this.Description}]");
        }

        protected abstract (Result resultCode, string message) checkSolution();

        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");
    }

    public class Stage1TestCase : Lab14TestCase
    {
        protected readonly string text;
        protected readonly string expected;
        protected string result;

        public Stage1TestCase(string text, string expected, double timeLimit, string description) : base(timeLimit, description)
        {
            this.text = text;
            this.expected = expected;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((Lab14)prototypeObject).Lab14Stage1(text);
        }

        protected override (Result resultCode, string message) checkSolution()
        {
            if (this.expected != this.result)
            {
                return (Result.WrongResult, $"Zwrócono ({result}), powinno być ({expected})");
            }

            return OkResult("OK");
        }
    }

    public class Stage2TestCase : Lab14TestCase
    {
        protected readonly int[] numbers;
        protected readonly (int[], int) expected;
        protected (int[], int) result;
        protected int result_length;

        public Stage2TestCase(int[] numbers, (int[], int) expected, double timeLimit, string description) : base(timeLimit, description)
        {
            this.numbers = numbers;
            this.expected = expected;
        }

        protected override void PerformTestCase(object prototypeObject)
        {

            result = ((Lab14)prototypeObject).Lab14Stage2(numbers);
        }

        protected override (Result resultCode, string message) checkSolution()
        {
            bool error = false;
            if (this.result.Item2 != -1 && this.expected.Item2 != this.result.Item2) { 
                return (Result.WrongResult, $"Zwrócono złą długość: ({result.Item2}), powinno być ({expected.Item2})");
            }
            if (result.Item1.Length != expected.Item1.Length)
            {
                error = true;
            }
            else
            {
                for(int i = 0; i<expected.Item1.Length; i++)
                {
                    if (result.Item1[i] != expected.Item1[i]) { error = true; break; }
                }
            }
            if (error)
            {
                return (Result.WrongResult, $"Zwrócono ({string.Join(", ", result.Item1)}), powinno być ({string.Join(", ", expected.Item1)})");
            }

            return OkResult("OK");
        }
    }



    public class Lab14Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new Lab14(), description: "Etap 1", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new Lab14(), description: "Etap 2", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;

            prepare();
        }

        private void addStage1(Stage1TestCase s1TestCase)
        {
            Stage1.TestCases.Add(s1TestCase);
        }

        private void addStage2(Stage2TestCase s2TestCase)
        {
            Stage2.TestCases.Add(s2TestCase);
        }


        private void prepare()
        {
            const double timeLimit = 1;
            string text;
            string palindrome;

            int[] numbers;
            int[] subsequence;
            int sub_length;
            string description;

            //TASK 1
            //// TEST 1
            text = "abbed";
            palindrome = "debbabbed";

            description = "Przykład z zadania";
            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            //// TEST 2
            text = "aacecaaa";
            palindrome = "aaacecaaa";

            description = "Krótki prosty przykład";
            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));


            //// TEST 3
            text = "abcd";
            palindrome = "dcbabcd";
            description = "Krótki przykład 2";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            //// TEST 4
            text = "";
            palindrome = "";

            description = "Pusty przykład";
            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));


            //// TEST 5
            text = "kajak";
            palindrome = "kajak";

            description = "Palindrom";
            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            /// TEST 6
            text = "aaaaa";
            palindrome = "aaaaa";
            description = "Powtarzający się znak";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            /// TEST 7
            text = "hello world";
            palindrome = "dlrow ollehello world";
            description = "Ze spacją";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            /// TEST 8
            text = "!@#$%adsf";
            palindrome = "fsda%$#@!@#$%adsf";
            description = "Dziwne znaki";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            /// TEST 9
            text = GetRandomStage1(20, 20);
            palindrome = "DNYQg82XFBocd1COzuqKquzOC1dcoBFX28gQYND";
            description = "Mały test losowy";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));

            /// TEST 10
            text = GetRandomStage1(100, 20);
            palindrome = "JcS1EZ501TMSwjzcM8WPPjzLMM4mQ5VP3uSEQZw40KtTaMiJtt1GvIZchiQ1TvAlnmeVsjLEEIYoBXudDNYQg82XFBocd1COzuqKquzOC1dcoBFX28gQYNDduXBoYIEELjsVemnlAvT1QihcZIvG1ttJiMaTtK04wZQESu3PV5Qm4MMLzjPPW8MczjwSMT105ZE1ScJ";
            description = "Duży test losowy";

            addStage1(new Stage1TestCase(text, palindrome, timeLimit, description));



            //TASK 2
            //// TEST 1
            numbers = new int[] { 2, 1, 2, 3, 4, 5, 2, 1 };
            subsequence = new int[] { 1, 2, 3, 4, 5 };
            sub_length = 5;

            description = "Przykład z zadania";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            //// TEST 2
            numbers = new int[] { 1,2,3,1,2,3,2,2 };
            subsequence = new int[] {1,2,3};
            sub_length = 3;

            description = "Krótki prosty przykład";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            //// TEST 3
            numbers = new int[] { 21, 43, 88, 43 };
            subsequence = new int[] { 21, 43, 88 };
            sub_length = 3;

            description = "Krótki prosty przykład 2";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            //// Test 4
            numbers = new int[] { };
            subsequence = new int[] { };
            sub_length = 0;

            description = "Pusty przykład";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            //// Test 5
            numbers = new int[] { -1, 10, -1, -2, 230, -1000, -2};
            subsequence = new int[] {10, -1, -2, 230, -1000};
            sub_length = 5;

            description = "Duży rozrzut liczb";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 6
            numbers = new int[] { 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            subsequence = new int[] { 2 };
            sub_length = 1;

            description = "Powtarzająca się liczba";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 7
            numbers = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            subsequence = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            sub_length = 10;

            description = "Ciąg rosnący";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 8
            numbers = new int[] {1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            subsequence = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            sub_length = 10;

            description = "Powtórzenie początku";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 9
            sub_length = 20;
            numbers = GetRandomStage2(20, 20, 1000);
            subsequence = new int[] { 809, 622, 688, 503, 866, 291, 105, 302, 988, 358, 488, 405, 95, 271, 397, 92, 560, 489, 37, 615 };

            description = "Mały test losowy";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 10
            sub_length = 58;
            numbers = GetRandomStage2(100, 20, 1000);
            subsequence = new int[] { 811, 10, 119, 150, 121, 161, 959, 694, 342, 386, 33, 464, 822, 675, 230, 39, 67, 799, 340, 241, 115, 697, 710, 826, 455, 939, 750, 168, 397, 853, 65, 370, 435, 589, 877, 493, 394, 788, 984, 141, 29, 664, 793, 666, 709, 807, 575, 64, 330, 97, 617, 463, 254, 780, 565, 761, 300, 293 };

            description = "Duży test losowy";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), timeLimit, description));

            /// Test 11
            sub_length = 10;
            numbers = GetRandomStage2(10000000, 20, 10);
            subsequence = new int[] { 0, 6, 3, 1, 9, 5, 2, 8, 7, 4 };
            description = "Bardzo Duży test losowy";
            addStage2(new Stage2TestCase(numbers, (subsequence, sub_length), 3, description));
        }

        private string GetRandomStage1(int length, int seed)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random(seed);
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        private int[] GetRandomStage2(int length, int seed, int mod = 0)
        {
            Random random = new Random(seed);
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = random.Next();
                if (mod != 0)
                {
                    array[i] = array[i] % mod;
                }
            }
            return array;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab14Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

            Console.ReadKey();
        }
    }
}
