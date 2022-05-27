
namespace AkkuCalc.Services
{
    public class NumberPartitioningService
    { 
        public IList<float> Numbers { get; set; } = new List<float>();

        public ushort NumberOfSubsets { get; set; } = 2;

        public IList<List<float>> Subsets { get; set; } = new List<List<float>>();

        public bool IsCalculated => Subsets.Any();

        public void RemoveAt(int index)
        {
            this.Numbers.RemoveAt(index);
            this.OnNumberChanged();
        }

        public void Add(float number)
        {
            this.Numbers.Add(number);
            this.OnNumberChanged();
        }

        public event EventHandler CalculatedChanged;

        public void OnCalculatedChanged()
        {
            this.CalculatedChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? NumberChanged;

        public void OnNumberChanged()
        {
            this.NumberChanged?.Invoke(this, EventArgs.Empty);
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
