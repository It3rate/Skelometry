using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Renderer;

namespace SlugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Agent agent = new Agent(new RenderEncoder());
            var sp = new TerminalPoint(PadKind.Input, new SKPoint(100, 200));
            var ep = new TerminalPoint(PadKind.Input, new SKPoint(150, 200));
            var trait = new Trait(TraitKind.Default, sp, ep);
            Assert.AreEqual(sp.Position, trait.StartPosition);
            Assert.AreEqual(ep.Position, trait.EndPosition);
        }
    }
}
