using SuperSocket.ProtoBase;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSocketIOTJiankuang.PipelineFilters
{
    public class BeginMarkAndFixedSizePipelineFilter<TPackageInfo> : PipelineFilterBase<TPackageInfo>
        where TPackageInfo : class
    {
        private readonly ReadOnlyMemory<byte> _beginMark;

        private readonly int _fixedSize;

        private bool _foundBeginMark;

        public Action<string> DeviceRegister;

        protected BeginMarkAndFixedSizePipelineFilter(ReadOnlyMemory<byte> beginMark, int fixedSize)
        {
            _beginMark = beginMark;
            _fixedSize = fixedSize;
        }

        public override TPackageInfo Filter(ref SequenceReader<byte> reader)
        {
            if (reader.Length == 6) // 检测设备接入（初次连接后发送mac）,有人USR-W610初次连接会发送6个字节的mac地址
            {
                var mac = reader.Sequence.Slice(reader.CurrentSpanIndex, 6);
                string macStr = string.Empty;
                mac.ToArray().ToList().ForEach(b =>
                {
                    macStr += b.ToString("X2").ToUpper();
                });
                DeviceRegister?.Invoke(macStr);
            }

            if (!_foundBeginMark)
            {
                var beginMark = _beginMark.Span;

                while (!reader.IsNext(beginMark, advancePast: false))
                {
                    if(reader.CurrentSpanIndex  == reader.Length)
                    {
                        return null;
                    }
                    reader.Advance(1);
                }

                _foundBeginMark = true;
            }

            if (reader.Remaining < _fixedSize)
            {
                return null;
            }

            var pack = reader.Sequence.Slice(reader.CurrentSpanIndex, _fixedSize);
            reader.Advance(_fixedSize);

            return DecodePackage(ref pack);
        }

        public override void Reset()
        {
            _foundBeginMark = false;
        }
    }
}
