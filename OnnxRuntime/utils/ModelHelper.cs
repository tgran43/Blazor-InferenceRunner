using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor_web.OnnxRuntime.utils
{
    public static class ModelHelper
    {
        public static List<Prediction> GetPredictions(Tensor<float> input, string modelFilePath)
        {
            // Setup inputs
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("image", input)
            };

            // Run inference
            var session = new InferenceSession(modelFilePath);
            var results = session.Run(inputs).First().AsEnumerable<float>();

            // Postprocess to get softmax vector
            float sum = results.Sum(x => (float)Math.Exp(x));
            List<float> sigmoid = new List<float>();
            List<int> labels = new List<int>();
            foreach (float result in results)
            {
                sigmoid.Add(Sigmoid(result));
            }
            List<float> softmax = results.Select(x => (float)Math.Exp(x) / sum).ToList();
            foreach (float value in sigmoid)
            {
                if (value >= 0.5)
                {
                    labels.Add(1);
                }
                else
                {
                    labels.Add(0);
                }
            }
            // Extract top 10 predicted classes
            List<Prediction> top3 = labels.Select((x, i) => new Prediction { Label = LabelMap.Labels[i], Confidence = x })
                               .Take(10).ToList();
            return top3;

        }
        public static float Sigmoid(float x)
        {
            return (float)(1.0 / (1.0 + Math.Exp(-x)));
        }
    }
}
