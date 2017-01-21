using System;
using System.Text;
using System.Collections.Generic;

namespace KnapsackProblem {
    public class GeneticSolve {
        
        class Solution {
            public bool[] Set;
            public int    Generation = 0;
            public int    Mutations = 0;

            public Solution(bool[] solutionSet) {
                Set = solutionSet;
            }
        }

        int[] _values    = null;
        int[] _weights   = null;
        int   _maxWeight = 0;
        
        int    _populationSize = 0;
        int    _maxIters       = 0;
        double _killRatio      = 0.0;
        double _mutateRatio    = 0.0;

        List<Solution> _solutions = new List<Solution>();
        Random _random = null;

        public GeneticSolve(
            Item[] items, 
            Knapsack knapsack,
            int populationSize = 10,
            int maxIters = 10000, 
            double killRatio = 0.75, 
            double mutateRatio = 0.25) {
            
            _values = new int[items.Length];
            _weights = new int[items.Length];
            for( int i = 0; i < items.Length; i++ ) {
                _values[i] = items[i].Value;
                _weights[i] = items[i].Weight;
            }
            _maxWeight = knapsack.MaxWeight;

            _populationSize = populationSize;
            _maxIters = maxIters;
            _killRatio = killRatio;
            _mutateRatio = mutateRatio;

            _random = new Random(DateTime.Now.Millisecond);
        }

        public void Solve() {
            for( int i = 0; i < _populationSize; i++ ) {
                _solutions.Add(CreateRandomSolution());
            }
            Console.WriteLine("Initital solutions ({0}):", _populationSize);
            for( int i = 0; i < _solutions.Count; i++ ) {
                Console.WriteLine(OutputSolution(_solutions[i]));
            }
            Process(_maxIters);
            OutputResults();
        }

        Solution CreateRandomSolution() {
            var solutionSet = new bool[_values.Length];
            for( int i = 0; i < solutionSet.Length; i++ ) {
                solutionSet[i] = _random.Next(2) > 0;    
            }
            return new Solution(solutionSet);
        }

        void Process(int iters) {
            Console.WriteLine();
            for( int i = 0; i < iters; i++ ) {
                var kill = Kill();
                var cross = Cross();
                var mutate = Mutate();
                Console.WriteLine(OutputIter(i, kill, cross, mutate));
                if( IsStable() ) {
                    break;
                }
            }
            Console.WriteLine();
        }

        bool IsStable() {
            var firstSolution = _solutions[0];
            for( int i = 1; i < _solutions.Count; i++ ) {
                for( int j = 0; j < firstSolution.Set.Length; j++ ) {
                    if( firstSolution.Set[j] != _solutions[i].Set[j] ) {
                        return false;
                    }
                }
            }
            return true;
        }

        string OutputIter(int iter, int kill, int cross, int mutate) {
            return string.Format(
                "{0}: kill: {1}, cross: {2}, mutate: {3}",
                iter, kill, cross, mutate);
        }

        int Kill() {
            var count = 0;
            _solutions.Sort(FitnessComparer);
            var itemsToRemove = (int)Math.Floor(_killRatio * _populationSize);
            for( int i = 0; i < itemsToRemove; i++ ) {
                if( _solutions.Count > (_populationSize - itemsToRemove) ) {
                    _solutions.RemoveAt(0);
                    count++;
                }
            }
            return count;
        }

        int FitnessComparer(Solution first, Solution second) {
            return Fitness(first).CompareTo(Fitness(second));
        }

        int Mutate() {
            var count = 0;
            var itemsToMutate = (int)Math.Floor(_mutateRatio * _populationSize);
            for( int i = 0; i < itemsToMutate; i++ ) {
                var randIdx = _random.Next(_solutions.Count);
                MutateSolution(_solutions[randIdx]);
                count++;
            }
            return count;
        }

        void MutateSolution(Solution solution) {
            var randIdx = _random.Next(solution.Set.Length);
            solution.Set[randIdx] = !solution.Set[randIdx];
            solution.Mutations++;
        }

        int Cross() {
            var leftBest = FindBestSolution();
            var rightBest = FindBestSolution(leftBest);
            if( (leftBest != null) && (rightBest != null) ) {
                CrossSolutions(leftBest, rightBest);
                return 2;
            } else {
                return 0;
            }
        }

        void CrossSolutions(Solution a, Solution b) {
            var ab = new Solution(new bool[a.Set.Length]);
            ab.Generation = Math.Max(a.Generation, b.Generation) + 1;
            var ba = new Solution(new bool[a.Set.Length]);
            ba.Generation = Math.Max(a.Generation, b.Generation) + 1;
            for( int i = 0; i < a.Set.Length; i++ ) {
                var isEven = (i % 2 == 0);
                ab.Set[i] = isEven ? a.Set[i] : b.Set[i];
                ba.Set[i] = isEven ? b.Set[i] : a.Set[i];
            }
            _solutions.Add(ab);
            _solutions.Add(ba);
        }

        void OutputResults() {
            Console.WriteLine();
            Console.WriteLine("Last solutions ({0}):", _solutions.Count);
            for( int i = 0; i < _solutions.Count; i++ ) {
                Console.WriteLine(OutputSolution(_solutions[i]));
            }
            Console.WriteLine();
            var bestSolution = FindBestSolution();
            Console.WriteLine("Best solution: {0}", OutputSolution(bestSolution));
        }

        string OutputSolution(Solution solution) {
            if( solution == null ) {
                return "null";
            }
            var value = 0;
            var weight = 0;
            var sb = new StringBuilder();
            sb = sb.Append("Items: ");
            for( int i = 0; i < solution.Set.Length; i++ ) {
                if( solution.Set[i] ) {
                    sb = sb.AppendFormat("{0}; ", i);
                    value += _values[i];
                    weight += _weights[i];
                }
            }
            var fitness = Fitness(solution);
            sb = sb.AppendFormat(
                "(value: {0}, weight: {1}, fitness: {2}, gen: {3}, muts: {4})", 
                value, weight, fitness, solution.Generation, solution.Mutations);
            return sb.ToString();
        }

        Solution FindBestSolution(Solution except = null) {
            Solution bestSolution = null;
            int bestFitness = 0;
            foreach( var solution in _solutions ) {
                if ( solution == except ) {
                    continue;
                }
                var curFitness = Fitness(solution);
                if( curFitness > bestFitness ) {
                    bestFitness = curFitness;
                    bestSolution = solution;
                }
            }
            return bestSolution;
        }

        int Fitness(Solution solution) {
            var priceAccum = 0;
            var weightAccum = 0;
            for( int i = 0; i < solution.Set.Length; i++ ) {
                if( solution.Set[i] ) {
                    priceAccum += _values[i];
                    weightAccum += _weights[i];
                }
            }
            return weightAccum > _maxWeight ? 0 : priceAccum;
        }
    }
}