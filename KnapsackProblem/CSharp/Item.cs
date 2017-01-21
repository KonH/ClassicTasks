namespace KnapsackProblem {
    public class Item {
        public int Value  { get; private set; }
        public int Weight { get; private set; }

        public Item(int value, int weight) {
            Value = value;
            Weight = weight;
        }
    }
}