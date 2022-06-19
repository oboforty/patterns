using System.Collections.Generic;
using UnityEngine;

public class DiscreteDistributionAnimSampler
{
    private ContinousDistributionAnimSampler sampler;
    private int iFrom;
    private int iTo;

    public DiscreteDistributionAnimSampler(int from, int to, AnimationCurve curve, int integrationSteps = 100)
    {
        sampler = new ContinousDistributionAnimSampler(curve, integrationSteps);
        iFrom = from;
        iTo = to;
    }

    public int Sample()
    {
        float p = sampler.Sample();

        return Mathf.FloorToInt(p * (iTo - iFrom) + iFrom);
    }

}
