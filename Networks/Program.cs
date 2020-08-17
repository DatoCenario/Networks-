using System;

namespace Networks
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputs = new double[][]
                            {
                new double[]{0,0} ,
                new double[]{0,1} ,
                new double[]{1,0} ,
                new double[]{1,1} ,
                            };
            var outputs = new double[]
                {
                0,1,1,0
                };
            var net = new DenseNetwork(new SigmoidFunction(), 2 ,40 , 20 , 1);
            net.Randomize();
            var len = new BackPropagationLearning(net, 0.1, 0);
            len.Momentum = 0;
            net.Randomize();
            for (int i = 0; i < 10000000; i++)
            {
                for (int g = 0; g < inputs.Length; g++)
                {
                    net.Compute(inputs[g]);
                    Console.WriteLine("expected  {0}  was  {1}", outputs[g], net.Output[0]);
                    len.Run(inputs[g], new double[] { outputs[g] });
                }
            }

        }
    }
}
