using UnityEngine;

public class UniformVariableFromDiscreteDistributionAnim
{
    protected readonly AnimationCurve _densityCurve;

    int _from;
    int _to;
    int _steps;

    public UniformVariableFromDiscreteDistributionAnim(int from, int to, AnimationCurve curve, int samplingSteps = 100)
    {
        _from = from;
        _to = to;
        _densityCurve = curve;
        _steps = samplingSteps;
    }

    /**
     * In contrast to Sample, this method returns a uniform random number
     * within the probability region of pi's value
     */
    public float GetProbabilityAt(int pi)
    {
        float p = (float)pi / (float) (_to - _from);

        float _tbegin = _densityCurve.keys[0].time;
        float _tend = _densityCurve.keys[_densityCurve.length - 1].time;

        float t = Mathf.Lerp(_tbegin, _tend, p);

        return _densityCurve.Evaluate(t);
    }

    public bool SampleAt(int slot)
    {
        return Random.value <= GetProbabilityAt(slot);
    }
}
