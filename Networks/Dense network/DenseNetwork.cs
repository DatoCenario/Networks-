using Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networks
{
    /// <summary>
    ///  Представляет полносязанную нейронную сеть
    /// </summary>
    class DenseNetwork : Network
    {
        /// <summary>
        /// Инициализирует объект полносвязанной нейронной сети
        /// </summary>
        /// <param name="layersCount">Количество слоев сети</param>
        public DenseNetwork(IFunction func, int inputsCount, params int[] layers) : base(inputsCount , layers.Length)
        {
            Layers[0] = new Layer(layers[0] , null);
            for (int i = 0; i < layers[0]; i++)
            {
                Layers[0].Neurons[i] = new Neuron(func , null , inputsCount);
            }
            for (int i = 1; i < layers.Length; i++)
            {
                var layer = Layers[i] = new Layer(layers[i] , Layers[i - 1]);
                for (int g = 0; g < layer.NeuronsCount; g++)
                {
                    layer.Neurons[g] = new Neuron(func , Layers[i - 1].Neurons , layers[i - 1]);
                }
            }
        }
    }
}
