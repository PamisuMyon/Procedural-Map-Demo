// #if !defined(FLOW_INCLUDED)
#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float2 FlowUV(float2 uv, float2 flow,float time)
{
    float progress = frac(time);
    return uv - flow * progress;
}

float3 FlowUVW (float2 uv, float2 flow, float jump, float offset, float tiling, float time, bool flowB)
{
    float phaseOffset = flowB ? .5 : 0;
    float progress = frac(time + phaseOffset);
    float3 uvw;
    uvw.xy = uv - flow * (progress + offset);
    uvw.xy *= tiling;
    uvw.xy += phaseOffset;
    uvw.xy += (time - progress) * jump;
    uvw.z = 1 - abs(1 - 2 * progress);
    return uvw;
}

#endif