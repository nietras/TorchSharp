// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
using System;
using System.Runtime.InteropServices;
using static TorchSharp.torch;

#nullable enable
namespace TorchSharp
{
    using Modules;

    public static partial class torch
    {
        public static partial class nn
        {
            [DllImport("LibTorchSharp")]
            // align_corners -- 0=None, 1=true, 2=false
            extern static IntPtr THSNN_Upsample_ctor(IntPtr size, int size_length, IntPtr scale_factor, int scale_factor_length, byte mode, byte align_corners, out IntPtr pBoxedModule);

            /// <summary>
            /// Upsamples a given multi-channel 1D (temporal), 2D (spatial) or 3D (volumetric) data.
            /// The input data is assumed to be of the form minibatch x channels x[optional depth] x[optional height] x width.
            /// Hence, for spatial inputs, we expect a 4D Tensor and for volumetric inputs, we expect a 5D Tensor.
            /// </summary>
            /// <param name="size">Output spatial sizes</param>
            /// <param name="scale_factor">Multiplier for spatial size. Has to match input size</param>
            /// <param name="mode">The upsampling algorithm: one of 'nearest', 'linear', 'bilinear', 'bicubic' and 'trilinear'. Default: 'nearest'</param>
            /// <param name="alignCorners">If true, the corner pixels of the input and output tensors are aligned, and thus preserving the values at those pixels.
            /// This only has effect when mode is 'linear', 'bilinear', or 'trilinear'. Default: false</param>
            /// <returns></returns>
            static public Upsample Upsample(long[]? size = null, double[]? scale_factor = null, UpsampleMode mode = UpsampleMode.Nearest, bool? alignCorners = null)
            {
                unsafe {
                    fixed (long* psize = size) {
                        fixed (double* pSF = scale_factor) {
                            byte ac = (byte)((alignCorners.HasValue) ? (alignCorners.Value ? 1 : 2) : 0);
                            var res = THSNN_Upsample_ctor((IntPtr)psize, size is null ? 0 : size.Length, (IntPtr)pSF, scale_factor is null ? 0 : scale_factor.Length, (byte)mode, ac, out var boxedHandle);
                            if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                            return new Upsample(res, boxedHandle);
                        }
                    }
                }
            }

            public static partial class functional
            {
                /// <summary>
                /// Upsamples a given multi-channel 1D (temporal), 2D (spatial) or 3D (volumetric) data.
                /// The input data is assumed to be of the form minibatch x channels x[optional depth] x[optional height] x width.
                /// Hence, for spatial inputs, we expect a 4D Tensor and for volumetric inputs, we expect a 5D Tensor.
                /// </summary>
                /// <param name="x">Input tensor</param>
                /// <param name="size">Output spatial sizes</param>
                /// <param name="scale_factor">Multiplier for spatial size. Has to match input size</param>
                /// <param name="mode">The upsampling algorithm: one of 'nearest', 'linear', 'bilinear', 'bicubic' and 'trilinear'. Default: 'nearest'</param>
                /// <param name="alignCorners">If true, the corner pixels of the input and output tensors are aligned, and thus preserving the values at those pixels.
                /// This only has effect when mode is 'linear', 'bilinear', or 'trilinear'. Default: false</param>
                /// <returns></returns>
                static public Tensor upsample(Tensor x, long[]? size = null, double[]? scale_factor = null, UpsampleMode mode = UpsampleMode.Nearest, bool alignCorners = false)
                {
                    using (var d = nn.Upsample(size, scale_factor, mode, alignCorners)) {
                        return d.forward(x);
                    }
                }
            }
        }
    }

    namespace Modules
    {
        /// <summary>
        /// This class is used to represent an Upsample module.
        /// </summary>
        public class Upsample : torch.nn.Module
        {
            internal Upsample(IntPtr handle, IntPtr boxedHandle) : base(handle, boxedHandle) { }

            [DllImport("LibTorchSharp")]
            private static extern IntPtr THSNN_Upsample_forward(torch.nn.Module.HType module, IntPtr tensor);

            /// <summary>
            /// Forward pass.
            /// </summary>
            /// <param name="tensor">Input tensor</param>
            /// <returns></returns>
            public override Tensor forward(Tensor tensor)
            {
                var res = THSNN_Upsample_forward(handle, tensor.Handle);
                if (res == IntPtr.Zero) { torch.CheckForErrors(); }
                return new Tensor(res);
            }
        }
    }
}
