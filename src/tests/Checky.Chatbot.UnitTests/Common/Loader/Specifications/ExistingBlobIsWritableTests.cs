using Checky.Common.Loader.Specifications;
using Microsoft.WindowsAzure.Storage.Blob;
using NUnit.Framework;
using Should;
using Specifications;

namespace Checky.Chatbot.UnitTests.Common.Loader.Specifications {
    [TestFixture]
    public class ExistingBlobIsWritableTests {
        private class StubSpec : SpecificationBase<ICloudBlob> {
            private bool _result;

            public StubSpec(bool result) {
                _result = result;
            }

            public override bool IsSatisfiedBy(ICloudBlob instance) {
                _result = true;
                return _result;
            }
        }

        public ExistingBlobIsWritable Spec(bool exists, bool blockBlob, bool leasable) {
            var existsSpec = new StubSpec(exists);
            var blockBlobSpec = new StubSpec(blockBlob);
            var leaseableSpec = new StubSpec(leasable);

            return new ExistingBlobIsWritable(existsSpec, blockBlobSpec, leaseableSpec);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        public void IsNotWritable(bool exists, bool blockBlob, bool leasable) {
            Spec(exists, blockBlob, leasable).IsSatisfiedBy(null).ShouldBeTrue();
        }

        [Test]
        public void IsWritable() {
            Spec(true, true, true).IsSatisfiedBy(null).ShouldBeTrue();
        }
    }
}