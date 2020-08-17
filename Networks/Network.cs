using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace Networks
{
    public abstract class Network
    {
        /// <summary>
        /// Количество входов сети
        /// </summary>
        public int InputsCount { get; protected set; }
        /// <summary>
        /// Выходное знвачение сети. Формируется из значений нейронов предидущего слоя
        /// </summary>
        public double[] Output { get; protected set; }
        /// <summary>
        /// Количество скрытых слоев в сети
        /// </summary>
        public int LayersCount => Layers.Length;
        /// <summary>
        /// Массив скрытых слоев сети
        /// </summary>
        public Layer[] Layers { get; protected set; }
        ///<param name="layersCount">Колиество скрытых слоев в сети</param>
        public Network(int inputsCount, int layersCount)
        {
            InputsCount = inputsCount;
            Layers = new Layer[layersCount];
        }

        private int neuronsComputed; // эта переменная нужна для индикации количества паралельно обработанных нейронов
        /// <summary>
        /// Вычислить выходное значение нейронной сети
        /// </summary>
        /// <param name="inputs">Вектор входных значений</param>
        public double[] Compute(double[] inputs)
        {
            //Вычисляем первый слой сети
            for (int i = 0; i < InputsCount; i++)
            {
                Layers[0].Neurons[i].Compute(inputs);
            }
            //Последовательно вычисляем все слои сети
            for (int i = 1; i < LayersCount; i++)
            {
                var layer = Layers[i];
                neuronsComputed = 0;
                for (int g = 0; g < layer.NeuronsCount; g++)
                {
                    //запускаем асинхронную задачу вычисления нейрона
                    Parallel.Invoke(() => ComputeNeuronAsync(layer.Neurons[g]));
                }
                //Ждем пока не вычислятся все нейроны слоя
                while (neuronsComputed != layer.NeuronsCount) ;
            }
            var output = Layers.Last().Neurons.Select(n => n.Output).ToArray();
            Output = output;
            return output;
        }
        void ComputeNeuronAsync(Neuron neuron)
        {
            neuron.Compute();
            neuronsComputed++;
        }
        /// <summary>
        /// Рандомизировать веса сети
        /// </summary>
        public void Randomize()
        {
            var rand = new Random();
            for (int i = 0; i < LayersCount; i++)
            {
                for (int g = 0; g < Layers[i].NeuronsCount; g++)
                {
                    Layers[i].Neurons[g].Randomize();
                }
            }
        }
    }
}
