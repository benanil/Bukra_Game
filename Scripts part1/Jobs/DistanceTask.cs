
using System;
using UnityEngine;

namespace MiddleGames.Compute
{
    [Serializable]
    public struct DistanceData
    {
        public Vector3 a;
        public Vector3 b;

        public DistanceData(Vector3 a, Vector3 b)
        {
            this.a = a;
            this.b = b;
        }
    }
    
    public static class DistanceTask
    {
        private const string DistanceBufferName = "DistanceDatas";
        private const string CalculateDistanceKarnelName = "CalculateDistances";

        public static float[] CalculateDistances(DistanceData[] distanceDatas,ComputeShader computeShader)
        {
            int karnel = computeShader.FindKernel(CalculateDistanceKarnelName);
            var buffer = new ComputeBuffer(distanceDatas.Length, sizeof(float) * 2);
            var resultBuffer = new ComputeBuffer(distanceDatas.Length, sizeof(float));

            computeShader.SetBuffer(karnel, DistanceBufferName, buffer);
            computeShader.SetBuffer(karnel, "Result", resultBuffer);

            computeShader.Dispatch(karnel, distanceDatas.Length, 1, 1);

            var results = new float[distanceDatas.Length];
            
            resultBuffer.GetData(results);

            buffer.Release();
            resultBuffer.Release();

            return results;
        }

    }
}
