using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networks
{
    public class Neuron
    {
        static Random rand = new Random();
        /// <summary>
        /// Функция активации нейрона
        /// </summary>
        public IFunction Function { get; private set; }
        /// <summary>
        /// Выходное значение нейрона. Определяется как F(S) где F - функция активации, S - взвешенная сумма входящих нейронов
        /// </summary>
        public double Output { get; private set; }
        /// <summary>
        /// Входящие нейроны предыдущего слоя
        /// </summary>
        public Neuron[] ConnectedNeurons { get; private set; }
        /// <summary>
        /// Веса входящих нейронов
        /// </summary>
        public double[] Weights { get; private set; }
        /// <summary>
        /// Пороговое значение нейрона
        /// </summary>
        public double Treshold { get; set; }
        /// <summary>
        /// Взвешенная сумма входящих нейронов предыдущего слоя
        /// </summary>
        public double WeightedSum { get; private set; }
        /// <summary>
        /// Инициализирует объект класса Neuron
        /// </summary>
        /// <param name="function">Функция активации</param>
        /// <param name="connected">Входящие нейроны</param>
        public int InputsCount { get; private set; }
        public Neuron(IFunction function , Neuron[] connected , int inputsCount)
        {
            Function = function;
            ConnectedNeurons = connected;
            InputsCount = inputsCount;
            Weights = new double[inputsCount];
        }
        /// <summary>
        /// Вычислить значение нейрона
        /// </summary>
        public double Compute()
        {
            double sum = 0;
            for (int i = 0; i < InputsCount; i++)
            {
                sum += ConnectedNeurons[i].Output * Weights[i];
            }
            sum += Treshold;
            WeightedSum = sum;
            Output = Function.Function(sum);
            return Output;
        }
        /// <summary>
        /// Вычислить значение по входящим значениям
        /// </summary>
        /// <param name="inputs">Входящие значения</param>
        /// <returns></returns>
        public double Compute(double[] inputs)
        {
            if (inputs.Length != InputsCount) throw new ArgumentException("Invalid inputs size");
            double sum = 0;
            for (int i = 0; i < InputsCount; i++)
            {
                sum += inputs[i] * Weights[i];
            }
            sum += Treshold;
            WeightedSum = sum;
            Output = Function.Function(sum);
            return Output;
        }
        /// <summary>
        /// Рандомизировать веса нейрона
        /// </summary>
        public void Randomize()
        {
            for (int i = 0; i < InputsCount; i++)
            {
                Weights[i] = rand.NextDouble();
            }
            Treshold = rand.NextDouble();
        }
    }
}
