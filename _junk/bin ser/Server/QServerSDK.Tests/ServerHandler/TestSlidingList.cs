using NUnit.Framework;
using System;

namespace QServerSDK.ServerHandler.Tests
{
    class TestSlidingList
    {
        class MyClass : IAckable
        {
            public bool Ack { get; set; }
            public int Value;

            public MyClass(int v, bool ack = false)
            {
                Value = v;
                Ack = ack;
            }

            public override string ToString()
            {
                string s = Value.ToString();
                if (Ack)
                    s += " (ack)";
                return s;
            }
        }

        SlidingList<MyClass> slide;

        [SetUp]
        public void Setup()
        {
            slide = new SlidingList<MyClass>(5);
        }

        [Test]
        public void TestCycleInsert()
        {
            Assert.AreEqual(slide.Current, null);

            slide.Insert(new MyClass(1));
            slide.Insert(new MyClass(2));
            slide.Insert(new MyClass(3));
            slide.Insert(new MyClass(4));
            slide.Insert(new MyClass(5));
            slide.Insert(new MyClass(6));

            Assert.AreEqual(slide.Current.Value, 6);
        }

        [Test]
        public void TestAckCycle()
        {
            slide.Insert(new MyClass(0));

            // Nonexistent gamestate ack
            Assert.Throws<Exception>(delegate {
                slide.Ack(1);
            });
            Assert.IsNull(slide.LastAcked);
            Assert.AreNotEqual(0, slide.LastAcked?.Value);
            Assert.AreNotEqual(1, slide.LastAcked?.Value);

            // Check acking
            slide.Insert(new MyClass(1));
            slide.Ack(1);
            Assert.AreEqual(1, slide.LastAcked.Value);

            // Acking previous state (caused by UDP non-consistent packet orders)
            slide.Ack(0);
            Assert.AreEqual(1, slide.LastAcked.Value);
            Assert.IsTrue(slide[0].Ack);

            // Test ack cycle in the end
            slide.Insert(new MyClass(2));
            slide.Insert(new MyClass(3));
            slide.Insert(new MyClass(4));

            // cycle Overflow -> clears ack
            Assert.IsTrue(slide[0].Ack);
            slide.Insert(new MyClass(5));
            Assert.IsFalse(slide[0].Ack);

        }
    }
}
