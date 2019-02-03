using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncfusionWpfSample3
{
    class ViewModel
    {
        public List<DataPoint> Data { get; set; }

        public ViewModel()
        {
            Data = new List<DataPoint>()
            {
                new DataPoint { Time = 0.0, Value = 0.0 },
                new DataPoint { Time = 1.0, Value = 20.0 },
                new DataPoint { Time = 2.0, Value = 40.0 },
                new DataPoint { Time = 3.0, Value = 60.0 },
            };
        }
    }
}
