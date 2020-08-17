using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;

namespace Networks
{
    /// <summary>
    /// Представляет реализацию метода обратного распостранения ошибки
    /// </summary>
    public class BackPropagationLearning : ILearning
    {
        /// <summary>
        /// Изменения весов за последнюю итерацию обучения
        /// </summary>
        public Dictionary<Neuron, double[]> WeightsChanges { get; }
        /// <summary>
        /// Изменения пороговых значений за последнюю итерацию обучения
        /// </summary>
        public Dictionary<Neuron, double> TresholdChanges { get; }
        /// <summary>
        /// Ошибки нейронов за последнюю итерацию обучения
        /// </summary>
        public Dictionary<Neuron, double> NeuronMistakes { get; }
        /// <summary>
        /// Обучаемая сеть
        /// </summary>
        public Network Network { get; }
        /// <summary>
        /// Коэфицэнт скорости обучения
        /// </summary>
        public double LearningRate { get; set; }
        /// <summary>
        /// Коэфицэнт правила момента
        /// </summary>
        public double Momentum { get; set; }
        /// <summary>
        /// Инициализирует объект BackPropagationLearning
        /// </summary>
        /// <param name="net">Обучаемая сеть</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="momentum">Коэфицэнт момента</param>
        public BackPropagationLearning(Network net, double learningRate = 1, double momentum = 0)
        {
            LearningRate = learningRate;
            Momentum = momentum;
            Network = net;

            WeightsChanges = net.Layers
                .SelectMany(l => l.Neurons)
                .ToDictionary(k => k, v => new double[v.InputsCount]);

            NeuronMistakes = net.Layers
                .SelectMany(l => l.Neurons)
                .ToDictionary(k => k, v => 0.0);

            TresholdChanges = net.Layers
                .SelectMany(l => l.Neurons)
                .ToDictionary(k => k, v => 0.0);
        }

        private int neuronsTeached;
        public void Run(double[] inputs, double[] outputs)
        {
            //Обнуляем ошибки
            foreach (var key in NeuronMistakes.Keys.ToArray()) NeuronMistakes[key] = 0;
            //Вычисляем выход сети
            var result = Network.Compute(inputs);
            var lastLayer = Network.Layers.Last();
            for (int i = 0; i < result.Length; i++)
            {
                //Вычисляем ошибку i-го нейрона последнего слоя
                var mistake = result[i] - outputs[i];
                //Записываем ее в словарь ошибок
                NeuronMistakes[lastLayer.Neurons[i]] = mistake;
            }
            //Начиная с последнего слоя последовательно обрабатываем нейроны
            for (int i = Network.LayersCount - 1; i >= 0; i--)
            {
                var layer = Network.Layers[i];
                neuronsTeached = 0;
                for (int g = 0; g < layer.NeuronsCount; g++)
                {
                    //Зпускаем обработку нейрона параллельно
                    Parallel.Invoke(() => TeachNeuronAsync(i, g, inputs));
                }
                //Ждем пока все нейроны слоя не обучатся
                while (neuronsTeached != layer.NeuronsCount) ;
            }
        }

        void TeachNeuronAsync(int layerIndex, int neuronIndex, double[] inputs)
        {
            var neuron = Network.Layers[layerIndex].Neurons[neuronIndex];
            //Получаем ошибку нейрона
            double mistake = 0;
            lock (NeuronMistakes)
            {
                mistake = NeuronMistakes[neuron];
            }
            //Функция ошибки определена как 1/2(result - expected)^2
            //Ее производная по взвешенной сумме: (result - expected) * F'(Sum)
            var derivativeDelta = mistake * neuron.Function.Derivative(neuron.WeightedSum);
            //Производная функции ошибки по пороговому значению равна производной по взвешенной сумме умноженной на 1
            //К значению смешения добавляется предыдущее изменение умноженое на momentum коэфициент 
            var deltaTreshold = LearningRate * -derivativeDelta + Momentum * TresholdChanges[neuron];
            //Изменяем пороговое значение
            neuron.Treshold += deltaTreshold;
            //Записываем новое изменение
            TresholdChanges[neuron] = deltaTreshold;
            //Последовательно корректируем веса и вычисляем ошибки нейронов предыдущего слоя
            for (int i = 0; i < neuron.InputsCount; i++)
            {
                var connected = layerIndex != 0 ? neuron.ConnectedNeurons[i] : null;
                //Производная функции ошибки по i-му весу равна Mist' * F'(Sum) * Sum'(weight) 
                var mistakeDerivative = derivativeDelta * (connected == null ? inputs[i] : connected.Output);
                //Изменение в сторону противоположную производной 
                var deltaWeight = -mistakeDerivative * LearningRate + Momentum * WeightsChanges[neuron][i];
                /*Производная текущего нейрона по j-му весу предыдущего нейрона равна 
                 currentNeuronOutput'(prevNeuronOutput) * prevNeuronOutput'(weight) .
                Сообщаем предыдущему нейрону производную ошибки по нему*/
                if (connected != null)
                {
                    var newMistake = derivativeDelta * neuron.Weights[i];
                    lock (NeuronMistakes)
                    {
                        NeuronMistakes[connected] += newMistake;
                    }
                }
                //Изменяем вес и записываем новое изменение
                WeightsChanges[neuron][i] = deltaWeight;
                neuron.Weights[i] += deltaWeight;
            }
            neuronsTeached++;
        }

        public void RunEpoch(double[][] inputs, double[][] outputs, int iterationsForExample = 1)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int g = 0; g < iterationsForExample; g++)
                {
                    Run(inputs[i], outputs[i]);
                }
            }
        }
    }
}
