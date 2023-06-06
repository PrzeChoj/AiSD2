using System;
using System.Linq;
using System.Text;
using ASD;
using System.Collections.Generic;
using Lab15;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.InteropServices;

namespace Lab15
{
    public abstract class ReservationOperation
    {
        public abstract bool Perform(ReservationManager rm);

        public virtual string ExplainLastError()
        {
            return "";
        }
    }

    public class AddReservation : ReservationOperation
    {
        int start, end;
        public AddReservation(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
        public override bool Perform(ReservationManager rm)
        {
            rm.AddReservation(start, end);
            return true;
        }
    }

    public class RemoveReservation : ReservationOperation
    {
        int start, end;
        public RemoveReservation(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
        public override bool Perform(ReservationManager rm)
        {
            rm.RemoveReservation(start, end);
            return true;
        }
    }

    public class FilledSeats : ReservationOperation
    {
        int station, expected;
        string lastError = "";

        public FilledSeats(int station, int expected)
        {
            this.station = station;
            this.expected = expected;
        }

        public override bool Perform(ReservationManager rm)
        {
            int res = rm.FilledSeats(station);
            if (res != expected)
                lastError = $"FilledSeats({station}) zwróciło {res}, a powinno zwrócić {expected}";
            return res == expected;
        }

        public override string ExplainLastError()
        {
            return lastError;
        }
    }

    public class FirstOverlappingReservation : ReservationOperation
    {
        int start, end, station;
        string lastError = "";

        public FirstOverlappingReservation(int station, int expectedS, int expectedE)
        {
            this.start = expectedS;
            this.end = expectedE;
            this.station = station;
        }

        public override bool Perform(ReservationManager rm)
        {
            (int resS, int resE) = rm.FirstOverlappingReservation(station);
            if (resS != start || resE != end)
                lastError = $"FirstOverlappingReservation({station}) zwróciło ({resS}, {resE}), a powinno zwrócić ({start}, {end})";
            return (resS == start && resE == end);
        }

        public override string ExplainLastError()
        {
            return lastError;
        }
    }

    public class Lab15TestCase : TestCase
    {
        List<ReservationOperation> operations;
        bool success = true;
        string ReturnMessage = "OK";
        int N;

        public Lab15TestCase(List<ReservationOperation> operations, int N, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.operations = operations;
            this.N = N;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (success)
                return OkResult("OK");
            return (Result.WrongResult, ReturnMessage);
        }

        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");

        protected override void PerformTestCase(object prototypeObject)
        {
            ReservationManager rm = (ReservationManager)prototypeObject;
            rm.Initialize(N);
            for (int i = 0; i < operations.Count; i++)
                if (!operations[i].Perform(rm))
                {
                    success = false;
                    ReturnMessage = $"Operacja nr. {i}: {operations[i].ExplainLastError()}";
                    return;
                }
        }
    }




    public class Lab15Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new ReservationManager(), description: "Etap 1", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new ReservationManager(), description: "Etap 2", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;

            prepareStage1();
            prepareStage2();

        }

        private void addStage1(Lab15TestCase s1TestCase)
        {
            Stage1.TestCases.Add(s1TestCase);
        }

        private void addStage2(Lab15TestCase s2TestCase)
        {
            Stage2.TestCases.Add(s2TestCase);
        }


        private void prepareStage1()
        {
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new AddReservation(0, 9));
                ops.Add(new AddReservation(3, 9));
                ops.Add(new AddReservation(3, 6));
                ops.Add(new FilledSeats(5, 3));
                ops.Add(new FilledSeats(3, 1));
                ops.Add(new AddReservation(5, 9));
                ops.Add(new AddReservation(0, 5));
                ops.Add(new AddReservation(1, 6));
                ops.Add(new FilledSeats(8, 3));
                ops.Add(new FilledSeats(5, 4));
                addStage1(new Lab15TestCase(ops, N, 1, "Mały test 1, bez usuwania"));
            }
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new AddReservation(0, 9));
                ops.Add(new AddReservation(3, 9));
                ops.Add(new AddReservation(3, 6));
                ops.Add(new RemoveReservation(3, 9));
                ops.Add(new FilledSeats(5, 2));
                ops.Add(new FilledSeats(3, 1));
                ops.Add(new AddReservation(5, 9));
                ops.Add(new AddReservation(0, 5));
                ops.Add(new RemoveReservation(0, 9));
                ops.Add(new AddReservation(1, 6));
                ops.Add(new FilledSeats(8, 1));
                ops.Add(new FilledSeats(5, 2));
                ops.Add(new RemoveReservation(3, 6));
                ops.Add(new FilledSeats(5, 1));
                addStage1(new Lab15TestCase(ops, N, 1, "Mały test 2, z usuwaniem"));
            }
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(0, 9));
                    ops.Add(new AddReservation(3, 9));
                    ops.Add(new AddReservation(3, 6));
                }
                for (int i = 0; i < 3; i++)
                    ops.Add(new RemoveReservation(3, 9));
                ops.Add(new FilledSeats(5, 6));
                ops.Add(new FilledSeats(3, 3));
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(5, 9));
                    ops.Add(new AddReservation(0, 5));
                    ops.Add(new RemoveReservation(0, 9));
                    ops.Add(new AddReservation(1, 6));
                }
                ops.Add(new FilledSeats(8, 3));
                ops.Add(new FilledSeats(5, 6));
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new RemoveReservation(3, 6));
                }
                ops.Add(new FilledSeats(5, 3));
                addStage1(new Lab15TestCase(ops, N, 1, "Mały test 3, wielokrotne rezerwacje"));
            }
            {
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new FilledSeats(33, 0));
                ops.Add(new AddReservation(91, 97));
                ops.Add(new FilledSeats(58, 0));
                ops.Add(new AddReservation(86, 95));
                ops.Add(new RemoveReservation(91, 97));
                ops.Add(new AddReservation(14, 42));
                ops.Add(new AddReservation(2, 36));
                ops.Add(new RemoveReservation(86, 95));
                ops.Add(new AddReservation(56, 98));
                ops.Add(new FilledSeats(11, 1));
                ops.Add(new RemoveReservation(2, 36));
                ops.Add(new RemoveReservation(14, 42));
                ops.Add(new FilledSeats(52, 0));
                ops.Add(new AddReservation(18, 45));
                ops.Add(new RemoveReservation(56, 98));
                ops.Add(new RemoveReservation(18, 45));
                ops.Add(new AddReservation(32, 70));
                ops.Add(new RemoveReservation(32, 70));
                ops.Add(new AddReservation(75, 91));
                ops.Add(new AddReservation(93, 99));
                ops.Add(new RemoveReservation(75, 91));
                ops.Add(new AddReservation(78, 89));
                ops.Add(new AddReservation(27, 53));
                ops.Add(new AddReservation(80, 99));
                ops.Add(new FilledSeats(4, 0));
                ops.Add(new AddReservation(98, 99));
                ops.Add(new AddReservation(5, 77));
                ops.Add(new AddReservation(8, 53));
                ops.Add(new RemoveReservation(8, 53));
                ops.Add(new AddReservation(47, 71));
                ops.Add(new FilledSeats(72, 1));
                ops.Add(new AddReservation(44, 62));
                ops.Add(new AddReservation(62, 78));
                ops.Add(new FilledSeats(39, 2));
                ops.Add(new RemoveReservation(47, 71));
                ops.Add(new FilledSeats(51, 3));
                ops.Add(new FilledSeats(38, 2));
                ops.Add(new RemoveReservation(80, 99));
                ops.Add(new RemoveReservation(44, 62));
                ops.Add(new RemoveReservation(98, 99));
                ops.Add(new AddReservation(49, 80));
                ops.Add(new RemoveReservation(62, 78));
                ops.Add(new FilledSeats(10, 1));
                ops.Add(new AddReservation(27, 69));
                ops.Add(new FilledSeats(98, 1));
                ops.Add(new AddReservation(62, 65));
                ops.Add(new AddReservation(65, 87));
                ops.Add(new AddReservation(66, 73));
                ops.Add(new RemoveReservation(62, 65));
                ops.Add(new AddReservation(86, 97));
                ops.Add(new AddReservation(10, 25));
                ops.Add(new AddReservation(70, 98));
                ops.Add(new RemoveReservation(93, 99));
                ops.Add(new RemoveReservation(66, 73));
                ops.Add(new FilledSeats(66, 4));
                ops.Add(new RemoveReservation(65, 87));
                ops.Add(new AddReservation(42, 49));
                ops.Add(new FilledSeats(69, 2));
                addStage1(new Lab15TestCase(ops, 100, 1, "Średni test"));
            }
            {
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new FilledSeats(33292, 0));
                ops.Add(new AddReservation(91954, 97833));
                ops.Add(new FilledSeats(58586, 0));
                ops.Add(new AddReservation(86939, 94992));
                ops.Add(new RemoveReservation(91954, 97833));
                ops.Add(new AddReservation(14913, 42740));
                ops.Add(new AddReservation(2909, 36722));
                ops.Add(new RemoveReservation(86939, 94992));
                ops.Add(new AddReservation(57411, 98893));
                ops.Add(new FilledSeats(11097, 1));
                ops.Add(new RemoveReservation(2909, 36722));
                ops.Add(new RemoveReservation(14913, 42740));
                ops.Add(new FilledSeats(52478, 0));
                ops.Add(new AddReservation(18201, 44897));
                ops.Add(new RemoveReservation(57411, 98893));
                ops.Add(new RemoveReservation(18201, 44897));
                ops.Add(new AddReservation(32767, 70207));
                ops.Add(new RemoveReservation(32767, 70207));
                ops.Add(new AddReservation(76003, 91862));
                ops.Add(new AddReservation(93950, 99518));
                ops.Add(new RemoveReservation(76003, 91862));
                ops.Add(new AddReservation(79789, 89497));
                ops.Add(new AddReservation(28074, 53889));
                ops.Add(new AddReservation(80858, 99774));
                ops.Add(new FilledSeats(3509, 0));
                ops.Add(new AddReservation(99177, 99253));
                ops.Add(new AddReservation(5081, 76785));
                ops.Add(new AddReservation(8654, 53041));
                ops.Add(new RemoveReservation(8654, 53041));
                ops.Add(new AddReservation(48055, 71139));
                ops.Add(new FilledSeats(73092, 1));
                ops.Add(new AddReservation(45009, 62919));
                ops.Add(new AddReservation(63037, 78140));
                ops.Add(new FilledSeats(38836, 2));
                ops.Add(new RemoveReservation(48055, 71139));
                ops.Add(new FilledSeats(51818, 3));
                ops.Add(new FilledSeats(38173, 2));
                ops.Add(new RemoveReservation(80858, 99774));
                ops.Add(new RemoveReservation(45009, 62919));
                ops.Add(new RemoveReservation(99177, 99253));
                ops.Add(new AddReservation(50497, 80696));
                ops.Add(new RemoveReservation(63037, 78140));
                ops.Add(new FilledSeats(9191, 1));
                ops.Add(new AddReservation(27333, 69578));
                ops.Add(new FilledSeats(99840, 0));
                ops.Add(new AddReservation(62796, 65031));
                ops.Add(new AddReservation(66255, 87461));
                ops.Add(new AddReservation(67242, 73199));
                ops.Add(new RemoveReservation(62796, 65031));
                ops.Add(new AddReservation(87186, 97847));
                ops.Add(new AddReservation(10418, 24742));
                ops.Add(new AddReservation(71534, 98826));
                ops.Add(new RemoveReservation(93950, 99518));
                ops.Add(new RemoveReservation(67242, 73199));
                ops.Add(new FilledSeats(66426, 4));
                ops.Add(new RemoveReservation(66255, 87461));
                ops.Add(new AddReservation(43245, 49774));
                ops.Add(new FilledSeats(70189, 2));
                addStage1(new Lab15TestCase(ops, 100000, 1, "Duży test, mało operacji"));
            }
            {
                int N = 2200;
                int d = 5;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                for (int i = 0; i < N; i++)
                    for (int j = i + d; j < N; j += d)
                    {
                        ops.Add(new AddReservation(i, j));
                        if (i >= 1)
                            ops.Add(new FilledSeats(i, i * (N - 1 - i) / d));
                    }
                addStage1(new Lab15TestCase(ops, 100000, 5, "Duży test, dużo operacji"));
            }
        }

        int ProgSum(int a1, int an, int n)
        {
            return (a1 + an) * n / 2;
        }

        private void prepareStage2()
        {
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new AddReservation(3, 9));
                ops.Add(new FirstOverlappingReservation(5, 3, 9));
                ops.Add(new AddReservation(3, 6));
                ops.Add(new FirstOverlappingReservation(5, 3, 9));
                ops.Add(new FirstOverlappingReservation(3, 3, 9));
                ops.Add(new AddReservation(5, 9));
                ops.Add(new AddReservation(2, 5));
                ops.Add(new FirstOverlappingReservation(3, 2, 5));
                ops.Add(new AddReservation(1, 6));
                ops.Add(new FirstOverlappingReservation(5, 1, 6));
                addStage2(new Lab15TestCase(ops, N, 1, "Mały test 1, bez usuwania"));
            }
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new AddReservation(0, 9));
                ops.Add(new AddReservation(3, 9));
                ops.Add(new FirstOverlappingReservation(2, 0, 9));
                ops.Add(new AddReservation(3, 6));
                ops.Add(new RemoveReservation(0, 9));
                ops.Add(new FirstOverlappingReservation(5, 3, 9));
                ops.Add(new AddReservation(5, 9));
                ops.Add(new RemoveReservation(3, 9));
                ops.Add(new FirstOverlappingReservation(5, 3, 6));
                ops.Add(new FirstOverlappingReservation(7, 5, 9));
                ops.Add(new AddReservation(0, 5));
                ops.Add(new AddReservation(1, 6));
                ops.Add(new RemoveReservation(3, 6));
                ops.Add(new FirstOverlappingReservation(3, 0, 5));
                addStage2(new Lab15TestCase(ops, N, 1, "Mały test 2, z usuwaniem"));
            }
            {
                int N = 10;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(0, 9));
                    ops.Add(new AddReservation(3, 9));
                }
                ops.Add(new FirstOverlappingReservation(2, 0, 9));
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(3, 6));
                    ops.Add(new RemoveReservation(0, 9));
                }
                ops.Add(new FirstOverlappingReservation(5, 3, 9));
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(5, 9));
                    ops.Add(new RemoveReservation(3, 9));
                }
                ops.Add(new FirstOverlappingReservation(5, 3, 6));
                ops.Add(new FirstOverlappingReservation(7, 5, 9));
                for (int i = 0; i < 3; i++)
                {
                    ops.Add(new AddReservation(0, 5));
                    ops.Add(new AddReservation(1, 6));
                    ops.Add(new RemoveReservation(3, 6));
                }
                ops.Add(new FirstOverlappingReservation(3, 0, 5));
                addStage2(new Lab15TestCase(ops, N, 1, "Mały test 3, wielokrotne rezerwacje"));
            }
            {
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new FirstOverlappingReservation(32, -1, -1));
                ops.Add(new AddReservation(91, 97));
                ops.Add(new FirstOverlappingReservation(58, -1, -1));
                ops.Add(new AddReservation(86, 95));
                ops.Add(new RemoveReservation(91, 97));
                ops.Add(new AddReservation(14, 42));
                ops.Add(new AddReservation(2, 36));
                ops.Add(new RemoveReservation(86, 95));
                ops.Add(new AddReservation(56, 98));
                ops.Add(new FirstOverlappingReservation(10, 2, 36));
                ops.Add(new RemoveReservation(2, 36));
                ops.Add(new RemoveReservation(14, 42));
                ops.Add(new FirstOverlappingReservation(51, -1, -1));
                ops.Add(new AddReservation(18, 45));
                ops.Add(new RemoveReservation(56, 98));
                ops.Add(new RemoveReservation(18, 45));
                ops.Add(new AddReservation(32, 70));
                ops.Add(new RemoveReservation(32, 70));
                ops.Add(new AddReservation(75, 91));
                ops.Add(new AddReservation(93, 99));
                ops.Add(new RemoveReservation(75, 91));
                ops.Add(new AddReservation(78, 89));
                ops.Add(new AddReservation(27, 53));
                ops.Add(new AddReservation(80, 99));
                ops.Add(new FirstOverlappingReservation(3, -1, -1));
                ops.Add(new AddReservation(98, 99));
                ops.Add(new AddReservation(5, 77));
                ops.Add(new AddReservation(8, 53));
                ops.Add(new RemoveReservation(8, 53));
                ops.Add(new AddReservation(47, 71));
                ops.Add(new FirstOverlappingReservation(72, 5, 77));
                ops.Add(new AddReservation(44, 62));
                ops.Add(new AddReservation(62, 78));
                ops.Add(new FirstOverlappingReservation(38, 5, 77));
                ops.Add(new RemoveReservation(47, 71));
                ops.Add(new FirstOverlappingReservation(51, 5, 77));
                ops.Add(new FirstOverlappingReservation(37, 5, 77));
                ops.Add(new RemoveReservation(80, 99));
                ops.Add(new RemoveReservation(44, 62));
                ops.Add(new RemoveReservation(98, 99));
                ops.Add(new AddReservation(49, 80));
                ops.Add(new RemoveReservation(62, 78));
                ops.Add(new FirstOverlappingReservation(9, 5, 77));
                ops.Add(new AddReservation(27, 69));
                ops.Add(new FirstOverlappingReservation(98, 93, 99));
                ops.Add(new AddReservation(62, 65));
                ops.Add(new AddReservation(65, 87));
                ops.Add(new AddReservation(66, 73));
                ops.Add(new RemoveReservation(62, 65));
                ops.Add(new AddReservation(86, 97));
                ops.Add(new AddReservation(10, 25));
                ops.Add(new AddReservation(70, 98));
                ops.Add(new RemoveReservation(93, 99));
                ops.Add(new RemoveReservation(66, 73));
                ops.Add(new FirstOverlappingReservation(65, 5, 77));
                ops.Add(new RemoveReservation(65, 87));
                ops.Add(new AddReservation(42, 49));
                ops.Add(new FirstOverlappingReservation(69, 5, 77));
                addStage2(new Lab15TestCase(ops, 100, 1, "Średni test"));
            }
            {
                List<ReservationOperation> ops = new List<ReservationOperation>();
                ops.Add(new FirstOverlappingReservation(33291, -1, -1));
                ops.Add(new AddReservation(91954, 97833));
                ops.Add(new FirstOverlappingReservation(58586, -1, -1));
                ops.Add(new AddReservation(86939, 94992));
                ops.Add(new RemoveReservation(91954, 97833));
                ops.Add(new AddReservation(14913, 42740));
                ops.Add(new AddReservation(2909, 36722));
                ops.Add(new RemoveReservation(86939, 94992));
                ops.Add(new AddReservation(57411, 98893));
                ops.Add(new FirstOverlappingReservation(11096, 2909, 36722));
                ops.Add(new RemoveReservation(2909, 36722));
                ops.Add(new RemoveReservation(14913, 42740));
                ops.Add(new FirstOverlappingReservation(52477, -1, -1));
                ops.Add(new AddReservation(18201, 44897));
                ops.Add(new RemoveReservation(57411, 98893));
                ops.Add(new RemoveReservation(18201, 44897));
                ops.Add(new AddReservation(32767, 70207));
                ops.Add(new RemoveReservation(32767, 70207));
                ops.Add(new AddReservation(76003, 91862));
                ops.Add(new AddReservation(93950, 99518));
                ops.Add(new RemoveReservation(76003, 91862));
                ops.Add(new AddReservation(79789, 89497));
                ops.Add(new AddReservation(28074, 53889));
                ops.Add(new AddReservation(80858, 99774));
                ops.Add(new FirstOverlappingReservation(3508, -1, -1));
                ops.Add(new AddReservation(99177, 99253));
                ops.Add(new AddReservation(5081, 76785));
                ops.Add(new AddReservation(8654, 53041));
                ops.Add(new RemoveReservation(8654, 53041));
                ops.Add(new AddReservation(48055, 71139));
                ops.Add(new FirstOverlappingReservation(73092, 5081, 76785));
                ops.Add(new AddReservation(45009, 62919));
                ops.Add(new AddReservation(63037, 78140));
                ops.Add(new FirstOverlappingReservation(38836, 5081, 76785));
                ops.Add(new RemoveReservation(48055, 71139));
                ops.Add(new FirstOverlappingReservation(51818, 5081, 76785));
                ops.Add(new FirstOverlappingReservation(38172, 5081, 76785));
                ops.Add(new RemoveReservation(80858, 99774));
                ops.Add(new RemoveReservation(45009, 62919));
                ops.Add(new RemoveReservation(99177, 99253));
                ops.Add(new AddReservation(50497, 80696));
                ops.Add(new RemoveReservation(63037, 78140));
                ops.Add(new FirstOverlappingReservation(9191, 5081, 76785));
                ops.Add(new AddReservation(27333, 69578));
                ops.Add(new FirstOverlappingReservation(99840, -1, -1));
                ops.Add(new AddReservation(62796, 65031));
                ops.Add(new AddReservation(66255, 87461));
                ops.Add(new AddReservation(67242, 73199));
                ops.Add(new RemoveReservation(62796, 65031));
                ops.Add(new AddReservation(87186, 97847));
                ops.Add(new AddReservation(10418, 24742));
                ops.Add(new AddReservation(71534, 98826));
                ops.Add(new RemoveReservation(93950, 99518));
                ops.Add(new RemoveReservation(67242, 73199));
                ops.Add(new FirstOverlappingReservation(66425, 5081, 76785));
                ops.Add(new RemoveReservation(66255, 87461));
                ops.Add(new AddReservation(43245, 49774));
                ops.Add(new FirstOverlappingReservation(70188, 5081, 76785));
                addStage2(new Lab15TestCase(ops, 100000, 1, "Duży test, mało operacji"));
            }
            {
                int N = 2200;
                int d = 5;
                List<ReservationOperation> ops = new List<ReservationOperation>();
                for (int i = 0; i < N; i++)
                    for (int j = i + d; j < N; j += d)
                    {
                        ops.Add(new AddReservation(i, j));
                        if (i >= 1)
                            ops.Add(new FirstOverlappingReservation(i, 0, (N - 1) / d * d));
                    }
                addStage2(new Lab15TestCase(ops, 100000, 5, "Duży test, dużo operacji"));
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab15Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}
