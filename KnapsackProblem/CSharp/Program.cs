namespace KnapsackProblem {
    public class Program {
        public static void Main(string[] args) {
            var items = new Item[] {
                // known: best = 7
                new Item(3, 5),
                new Item(5, 10),
                new Item(4, 6),
                new Item(2, 5),
                // new elements
                new Item(4, 4),
                new Item(8, 5),
                new Item(3, 3)
            };
            var knapsack = new Knapsack(14);
            var geneticSolve = new GeneticSolve(items, knapsack);
            geneticSolve.Solve();
        }
    }
}
