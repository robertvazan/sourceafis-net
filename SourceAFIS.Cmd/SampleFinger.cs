// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS.Cmd
{
    class SampleFinger
    {
        public readonly SampleDataset Dataset;
        public readonly int Id;
        public SampleFinger(SampleDataset dataset, int id)
        {
            Dataset = dataset;
            Id = id;
        }
    }
}
