using System;
using System.Collections.Generic;
using System.Text;

namespace Networks
{
    public interface IFunction
    {
        double Function(double arg);
        double Derivative(double arg);
    }

    public class LinearFunction : IFunction
    {
        public double Derivative(double arg)
        {
            return 1;
        }

        public double Function(double arg)
        {
            return arg;
        }
    }

    public class HyperbolicTangentFunction : IFunction
    {
        public double Coef = 1;

        public HyperbolicTangentFunction(double coef = 1)
        {
            Coef = coef;
        }
        public double Derivative(double arg)
        {
            return Coef / Math.Pow(Math.Cosh(arg * Coef) , 2);
        }

        public double Function(double arg)
        {
            return Math.Tanh(arg * Coef);
        }
    }

    public class SigmoidFunction : IFunction
    {
        public double Coef = 1;

        public SigmoidFunction(double coef = 1)
        {
            Coef = coef;
        }
        public double Derivative(double arg)
        {
            return Coef * Function(arg) * (1 - Function(arg));
        }

        public double Function(double arg)
        {
            return 1 / (1 + Math.Exp(-Coef * arg)); 
        }
    }

    public class RELUFunction : IFunction
    {
        Random rand = new Random();
        public double Derivative(double arg)
        {
            if (arg > 0)
            {
                return 1;
            }
            else
            {
                return rand.NextDouble() * 0.04 + 0.01;
            }
        }

        public double Function(double arg)
        {
            return Math.Max(0 ,arg);
        }
    }

}
