
namespace AkkuCalc.Services
{
    public class NumberPartitioningService
    { 
        public IList<float> Numbers { get; set; } = new List<float>();

        public ushort NumberOfSubsets { get; set; } = 1;

        public IList<List<float>> Subsets { get; set; } = new List<List<float>>();

        public bool IsCalculated => Subsets.Any();

        public async void Add(float number)
        {
            this.Numbers.Add(number);
            this.OnNumberAdded();
        }

        public event EventHandler CalculatedChanged;

        public void OnCalculatedChanged()
        {
            this.CalculatedChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler NumberAdded;

        public void OnNumberAdded()
        {
            this.NumberAdded?.Invoke(this, EventArgs.Empty);
        }

        public void DoPartition()
        {
            if (NumberOfSubsets < Numbers.Count())
            {
                try
                {
                    var partition = new effPartition.effPartition((List<float>)Numbers, NumberOfSubsets);
                    Subsets = partition.Subsets.Select(subset => subset.NumbIDs.Select(id => Numbers[(int)id-1]).ToList()).ToList();
                }
                catch (Exception e)
                {
                    //log
                }
            }
            this.OnCalculatedChanged();
        }
    }
}
