using System;
using System.Collections.Generic;
using System.Text;

namespace Networks
{
    public interface ILearning
    {
        Network Network { get; }
        double LearningRate { get; set; }
        void Run(double[] inputs, double[] outputs);
    }
}
