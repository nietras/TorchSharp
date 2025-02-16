// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;


#nullable enable
namespace TorchSharp
{
    using Modules;

    namespace Modules
    {
        public class LSTMCell : torch.nn.Module
        {
            internal LSTMCell(IntPtr handle, IntPtr boxedHandle) : base(handle, boxedHandle) { }

            public new static LSTMCell Load(String modelPath)
            {
                var res = Module.Load(modelPath);
                return new LSTMCell(res.handle.DangerousGetHandle(), IntPtr.Zero);
            }

            [DllImport("LibTorchSharp")]
            extern static IntPtr THSNN_LSTMCell_forward(torch.nn.Module.HType module, IntPtr input, IntPtr h_0, IntPtr c_0, out IntPtr c_n);

            /// <summary>
            /// Apply the RNN cell to an input tensor.
            /// </summary>
            /// <param name="input">Tensor of shape (batch, input_size) containing the features of the input sequence.</param>
            /// <param name="h0_c0">Tensors of shape (batch, hidden_size) containing the initial hidden and cell state for each element in the batch.</param>
            /// <returns></returns>
            public (Tensor, Tensor) forward(Tensor input, (Tensor, Tensor)? h0_c0 = null)
            {
                var hN = THSNN_LSTMCell_forward(handle, input.Handle, h0_c0?.Item1.Handle ?? IntPtr.Zero, h0_c0?.Item2.Handle ?? IntPtr.Zero, out IntPtr cN);
                if (hN == IntPtr.Zero || cN == IntPtr.Zero) { torch.CheckForErrors(); }
                return (new Tensor(hN), new Tensor(cN));
            }
        }
    }

    public static partial class torch
    {
        public static partial class nn
        {
            [DllImport("LibTorchSharp")]
            private static extern IntPtr THSNN_LSTMCell_ctor(long input_size, long hidden_size, bool bias, out IntPtr pBoxedModule);

            /// <summary>
            /// A long short-term memory (LSTM) cell.
            /// </summary>
            /// <param name="inputSize">The number of expected features in the input x</param>
            /// <param name="hiddenSize">The number of features in the hidden state h</param>
            /// <param name="bias">If False, then the layer does not use bias weights b_ih and b_hh. Default: True</param>
            static public LSTMCell LSTMCell(long inputSize, long hiddenSize, bool bias = true)
            {
                var res = THSNN_LSTMCell_ctor(inputSize, hiddenSize, bias, out var boxedHandle);
                if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                return new LSTMCell(res, boxedHandle);
            }
        }
    }
}
