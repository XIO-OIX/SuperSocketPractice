using SuperSocket.ProtoBase;
using SuperSocketPro.PackageDeoders;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSocketPro.PipelineFilters
{
    public class MyPipelineFilter : FixedHeaderPipelineFilter<MyPackage>
    {
        public MyPipelineFilter()
            : base(3) // 包头的大小是3字节，所以将3传如基类的构造方法中去
        {

        }

        // 从数据包的头部返回包体的大小
        protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);
            reader.Advance(1); // skip the first byte
            reader.TryReadBigEndian(out short len);
            return len;
        }

        // 将数据包解析成 MyPackage 的实例
        protected override MyPackage DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            var package = new MyPackage();

            var reader = new SequenceReader<byte>(buffer);

            reader.TryRead(out byte packageKey);
            package.Key = packageKey;
            reader.Advance(2); // skip the length             
            package.Body = reader.ReadString();

            return package;
        }
    }
}
