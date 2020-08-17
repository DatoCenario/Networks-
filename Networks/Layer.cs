using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networks
{
    public class Layer
    {
        /// <summary>
        /// Массив нейронов слоя
        /// </summary>
        public Neuron[] Neurons { get; set; }
        /// <summary>
        /// Количество нейронов в слое
        /// </summary>
        public int NeuronsCount => Neurons == null ? 0 : Neurons.Length;
        /// <summary>
        /// Предыдущий слой (если предыдущий слой отсутствует принимает значение null)
        /// </summary>
        public Layer PreviousLayer { get; protected set; }
        ///<param name = "neuronsCount">Количество нейронов в слое</param>
        ///<param name = "previousLayer">Предыдущий слой</param>
        public Layer(int neuronsCount, Layer previousLayer)
        {
            PreviousLayer = previousLayer;
            Neurons = new Neuron[neuronsCount];
        }
    }
}
