using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System.Buffers;
using System.Linq;

namespace SuperSocketIOTJiankuang.PipelineFilters
{
    public class MyBeginMarkAndFixedSizePipelineFilter : BeginMarkAndFixedSizePipelineFilter<TextPackageInfo>
    {
        public MyBeginMarkAndFixedSizePipelineFilter() : base(new byte[] { 0xA0, 0x13, 0x01, 0x8A }, 21)
        {
            DeviceRegister = i =>
              {
                  var session = this.Context as AppSession;
                  if (session != null)
                      session.DataContext = i;
              };
        }

        protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            var textPackageInfo = new TextPackageInfo();

            var reader = new SequenceReader<byte>(buffer);

            byte[] rfidHostResponse = new byte[7];
            byte[] rfidCardEpc = new byte[12];

            rfidHostResponse = reader.Sequence.Slice(0, 7).ToArray();
            reader.Advance(7);
            rfidCardEpc = reader.Sequence.Slice(7, 12).ToArray();
            reader.Advance(12);


            rfidCardEpc.ToList().ForEach(b =>
                {
                    textPackageInfo.Text += b.ToString("X2").ToUpper();
                });

            return textPackageInfo;
        }
    }
}
