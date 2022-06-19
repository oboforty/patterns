using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Playground
{
    public class TestDistributions : MonoBehaviour
    {
        public AnimationCurve distribution;
        public int iterations = 10000;
        public List<int> Result;
        public List<float> ResultDistribution;


#if UNITY_EDITOR
        [ContextMenu("Sample Discrete")]
        void __ctxSampleDiscrete()
        {
            int N = Result.Count;
            var dist = new DiscreteDistributionAnimSampler(0, N, distribution);

            Debug.Log(dist.Sample());
        }

        [ContextMenu("Sample Continous")]
        void __ctxSampleContinous()
        {
            var dist = new ContinousDistributionAnimSampler(distribution);

            Debug.Log(dist.Sample());
        }

        [ContextMenu("Test Uniform sampling at individual Distribution Samples")]
        void __ctxSampleGECHI()
        {
            int N = Result.Count;

            // Clear
            for (int i = 0; i < N; i++)
                Result[i] = 0;

            var dist = new UniformVariableFromDiscreteDistributionAnim(0, N, distribution);

            // test samples for each slot
            for (int slot = 0; slot < N; slot++)
            {
                for (int i = 0; i < iterations; i++)
                    if (dist.SampleAt(slot))
                        Result[slot]++;
            }

            // portray distribution
            ResultDistribution = new List<float>(N);
            for (int slot = 0; slot < N; slot++)
            {
                ResultDistribution.Add(dist.GetProbabilityAt(slot));
                //ResultDistribution.Add((float)Result[slot] / (float)iterations);
            }
        }


        [ContextMenu("Test DC Distribution Samplers")]
        void __ctxUpdateSpawn()
        {
            int N = Result.Count;

            // Clear
            for (int i = 0; i < N; i++)
                Result[i] = 0;

            // Sample
            var dist = new DiscreteDistributionAnimSampler(0, N, distribution);
            for (int i = 0; i < iterations; i++)
            {
                int pv = dist.Sample();
                Result[pv]++;
            }

            // portray distribution
            ResultDistribution = new List<float>(N);
            for (int i = 0; i < N; i++)
                ResultDistribution.Add((float) Result[i] / (float) iterations);
        }
#endif
    }
}
