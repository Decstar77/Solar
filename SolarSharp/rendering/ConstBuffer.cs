using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public class ConstBuffer
    {
        private int index = 0;
        private float[] buffer;
        private DXBuffer dxBuffer;

        public ConstBuffer(DXDevice device, uint floatCount)
        {
            buffer = new float[floatCount];

            dxBuffer = device.CreateBuffer(new BufferDesc {
                BindFlags = DXBindFlag.CONSTANT_BUFFER,
                Usage = DXUsage.DEFAULT,
                ByteWidth = sizeof(float) * floatCount
            });
        }

        public ConstBuffer Reset() 
        {
            index = 0;
            return this;
        }

        public ConstBuffer Upload(DXContext context)
        {
            context.UpdateSubresource(dxBuffer, buffer);
            return this;
        }

        public ConstBuffer SetVS(DXContext context, uint slot)
        {
            context.SetVSConstBuffer(dxBuffer, slot);
            return this;
        }

        public ConstBuffer Prepare(Matrix4 m)
        {
            buffer[index++] = m.m11; buffer[index++] = m.m12; buffer[index++] = m.m13; buffer[index++] = m.m14;
            buffer[index++] = m.m21; buffer[index++] = m.m22; buffer[index++] = m.m23; buffer[index++] = m.m24;
            buffer[index++] = m.m31; buffer[index++] = m.m32; buffer[index++] = m.m33; buffer[index++] = m.m34;
            buffer[index++] = m.m41; buffer[index++] = m.m42; buffer[index++] = m.m43; buffer[index++] = m.m44;
            return this;
        }
    }

}
