using UnityEngine;
using Fusion;

namespace Class3
{
    public struct NetworkInputData : INetworkInput
    {
        private byte buttonsPressed;

        public Vector3 LookDirection { get; set; }


        public void AddInput (NetworkInputType inputType)
        {
            byte flag = (byte)(1 << (int)inputType);
            buttonsPressed |= flag;
        }

        public readonly bool IsInputDown (NetworkInputType inputType)
        {
            byte flag = (byte)(1 << (int)inputType);
            return (buttonsPressed & flag) != 0;
        }
    }
}