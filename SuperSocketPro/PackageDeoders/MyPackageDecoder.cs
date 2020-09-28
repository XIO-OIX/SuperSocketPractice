using SuperSocket.ProtoBase;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSocketPro.PackageDeoders
{
    public class MyPackageDecoder : IPackageDecoder<MyPackage>
    {
        public MyPackage Decode(ref ReadOnlySequence<byte> buffer, object context)
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
