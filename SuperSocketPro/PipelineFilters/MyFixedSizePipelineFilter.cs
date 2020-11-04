using SuperSocket.ProtoBase;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSocketIOTJiankuang.PipelineFilters
{
    public class MyFixedSizePipelineFilter : FixedSizePipelineFilter<TextPackageInfo>
    {
        public MyFixedSizePipelineFilter()
            : base(22) //传入固定的请求大小
        {

        }

        protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
        {

            var text = new TextPackageInfo();

            var reader = new SequenceReader<byte>(buffer);


            byte[] deviceMac = new byte[6];
            byte[] rfidHostResponse = new byte[7];
            byte[] rfidCardEpc = new byte[12];

            //List<byte> bytes = new List<byte>();

            deviceMac = reader.Sequence.Slice(0, 6).ToArray();
            reader.Advance(6);
            rfidHostResponse = reader.Sequence.Slice(6, 7).ToArray();
            reader.Advance(7);
            rfidCardEpc = reader.Sequence.Slice(7, 12).ToArray();
            reader.Advance(12);
            for (int i = 0; i < buffer.Length; i++)
            {
                reader.TryRead(out byte value);
                if (i < 6)
                {
                    deviceMac[i] = value;
                }
                else if(i < 13)
                {
                    rfidHostResponse[7 -i] = value;
                }
                else if (i < 25)
                {
                    rfidCardEpc[25 - i] = value;
                }
            }
            
            reader.TryRead(out byte packageKey);
            reader.Advance(2); // skip the length     

            return base.DecodePackage(ref buffer);
        }
    }
}
