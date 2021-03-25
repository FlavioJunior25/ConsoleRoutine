using System;

namespace ConsoleRoutine
{
    class Program
    {
        static void Main(string[] args)
        {
            Routine routine = new Routine();

            routine.ValidationCoinQuotationAsync();
            Console.WriteLine("Pressione alguma tecla para sair ...");
            Console.ReadKey();
        }
    }
}
