using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;
using Slugs.Renderer;

namespace SlugTests
{
    [TestClass]
    public class EntityTests
    {
	    private static float tolerance = 0.00001f;
        [TestMethod]
        public void PointsTest()
        {
            Agent agent = new Agent(new RenderEncoder());
            var entity = new Entity(PadKind.Input);

            // first trait
            var stp0 = new TerminalPoint(PadKind.Input, new SKPoint(100, 400));
            var etp0 = new TerminalPoint(PadKind.Input, new SKPoint(200, 400));
            var trait0 = new Trait(TraitKind.Default, stp0, etp0);
            Assert.AreEqual(stp0.Position, trait0.StartPosition);
            Assert.AreEqual(new SKPoint(100, 400), trait0.StartPosition);
            Assert.AreEqual(etp0.Position, trait0.EndPosition);
            Assert.AreEqual(new SKPoint(200, 400), trait0.EndPosition);

            // first trait focal
            var sfp0 = new FocalPoint(PadKind.Input, trait0.Key, 0.2f);
            var efp0 = new FocalPoint(PadKind.Input, trait0.Key, 0.8f);
            var focal0 = new Focal(entity, sfp0, efp0);
            Assert.AreEqual(focal0.StartT, 0.2f);
            Assert.AreEqual(focal0.EndT, 0.8f);
            Assert.AreEqual(focal0.StartPosition, new SKPoint(120, 400));
            Assert.AreEqual(focal0.EndPosition, new SKPoint(180, 400));

            // second trait
            var rp1 = new RefPoint(PadKind.Input, stp0.Key);
            var etp1 = new TerminalPoint(PadKind.Input, new SKPoint(100, 300));
            var trait1 = new Trait(TraitKind.Default, rp1, etp1);
            Assert.AreNotEqual(rp1.Key, stp0.Key);
            Assert.AreEqual(rp1.TargetKey, stp0.Key);
            Assert.AreEqual(rp1.Position, trait0.StartPosition);
            Assert.AreEqual(rp1.DistanceToPoint(trait0.StartPosition), 0);
            Assert.AreEqual(rp1.DistanceToPoint(trait0.EndPosition), 100);
            Assert.AreEqual(etp1.DistanceToPoint(trait0.EndPosition), Math.Sqrt(2.0) * 100, tolerance);

            // second trait focal
            var sfp1 = new FocalPoint(PadKind.Input, trait1.Key, -0.2f);
            var efp1 = new FocalPoint(PadKind.Input, trait1.Key, 1.2f);
            var focal1 = new Focal(entity, sfp1, efp1);
            Assert.AreEqual(focal1.StartT, -0.2f);
            Assert.AreEqual(focal1.EndT, 1.2f);
            Assert.AreEqual(focal1.StartPosition, new SKPoint(100, 420));
            Assert.AreEqual(focal1.EndPosition, new SKPoint(100, 280));

            var bsp0 = new BondPoint(PadKind.Input, focal0.Key, 0.5f);
            var bep0 = new BondPoint(PadKind.Input, focal1.Key, 0.7f);
            var sb0 = new SingleBond(bsp0, bep0);
            Assert.AreEqual(bsp0.T, 0.5f);
            Assert.AreEqual(bep0.T, 0.7f);
            Assert.AreEqual(bsp0.Position, focal0.MidPosition);
            Assert.AreEqual(sb0.LocalRatio, new Slug(0.5f, 0.7f));
            Assert.AreEqual(sb0.TRatio, 0.7f / 0.5f, tolerance);
            Assert.AreEqual(sb0.Length, 92.64988f, tolerance);

            var db0 = new DoubleBond(focal0, focal1);
            Assert.AreEqual(db0.TRatio, 2.333333f, tolerance);
            Assert.AreEqual(db0.Center, new SKPoint(125, 375));
            Assert.AreEqual(db0.AllKeys.Count, 6);
            CollectionAssert.AreEqual(db0.Points, new List<IPoint>{sfp0, efp0, sfp1, efp1});
            // not implemented yet.
            //Assert.IsTrue(db0.ContainsPosition(new SKPoint(120, 380)));
            //Assert.IsFalse(db0.ContainsPosition(new SKPoint(150, 340)));
        }
    }
}
