using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public delegate void UpdateElements();
    public class UpdateElementsEvents : EventArgs
    {
        public bool Updating { get; set; }

        public event UpdateElements UpdateElementsEvent;
        public void EvokeUpdating()
        {
            if (!Updating)
                UpdateElementsEvent?.Invoke();
        }
    }
}
