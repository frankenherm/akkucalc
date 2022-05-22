
namespace AkkuCalc.Services
{
    public class NumberPartitioningService
    { 
        public IList<float> Numbers { get; set; } = new List<float>();

        public ushort NumberOfSubsets { get; set; } = 1;

        public IList<IEnumerable<float>> Subsets { get; set; } = new List<IEnumerable<float>>();

        public bool IsCalculated => Subsets.Any();

        public void DoPartition()
        {
            if (NumberOfSubsets < Numbers.Count())
            {
                var partition = new effPartition.effPartition((List<float>)Numbers, NumberOfSubsets); ;
                Subsets = partition.Subsets.Select(subset => subset.NumbIDs.Select(id => Numbers[(int)id])).ToList();
            }
        }
    }
}
