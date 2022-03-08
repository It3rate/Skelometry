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
		public void CreationTests()
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
			CollectionAssert.AreEqual(db0.Points, new List<IPoint> {sfp0, efp0, sfp1, efp1});
			Assert.IsTrue(db0.IsPointInside(new SKPoint(120, 380)));
			Assert.IsFalse(db0.IsPointInside(new SKPoint(150, 340)));

			Assert.AreEqual(entity.ElementKind, ElementKind.Entity);
			Assert.AreEqual(rp1.ElementKind, ElementKind.RefPoint);
			Assert.AreEqual(etp1.ElementKind, ElementKind.Terminal);
			Assert.AreEqual(trait1.ElementKind, ElementKind.Trait);
			Assert.AreEqual(sfp1.ElementKind, ElementKind.FocalPoint);
			Assert.AreEqual(focal1.ElementKind, ElementKind.Focal);
			Assert.AreEqual(bsp0.ElementKind, ElementKind.BondPoint);
			Assert.AreEqual(bep0.ElementKind, ElementKind.BondPoint);
			Assert.AreEqual(sb0.ElementKind, ElementKind.SingleBond);
			Assert.AreEqual(db0.ElementKind, ElementKind.DoubleBond);
		}

		[TestMethod]
		public void MotionTests()
		{
			Agent agent = new Agent(new RenderEncoder());
			var entity = new Entity(PadKind.Input);

            // Traits
			var stp0 = new TerminalPoint(PadKind.Input, new SKPoint(100, 400));
			var etp0 = new TerminalPoint(PadKind.Input, new SKPoint(200, 400));
			var trait0 = new Trait(TraitKind.Default, stp0, etp0);
			var rp1 = new RefPoint(PadKind.Input, stp0.Key);
			var etp1 = new TerminalPoint(PadKind.Input, new SKPoint(100, 300));
			var trait1 = new Trait(TraitKind.Default, rp1, etp1);

            // Focals
            var sfp0 = new FocalPoint(PadKind.Input, trait0.Key, 0.2f);
            var efp0 = new FocalPoint(PadKind.Input, trait0.Key, 0.8f);
            var focal0 = new Focal(entity, sfp0, efp0);
            var sfp1 = new FocalPoint(PadKind.Input, trait1.Key, -0.2f);
            var efp1 = new FocalPoint(PadKind.Input, trait1.Key, 1.2f);
            var focal1 = new Focal(entity, sfp1, efp1);

            // Bonds
            var bsp0 = new BondPoint(PadKind.Input, focal0.Key, 0.5f);
            var bep0 = new BondPoint(PadKind.Input, focal1.Key, 0.7f);
            var sb0 = new SingleBond(bsp0, bep0);
            var db0 = new DoubleBond(focal0, focal1);

            // confirm positions
            Assert.AreEqual(new SKPoint(100, 400), trait0.StartPosition);
            Assert.AreEqual(new SKPoint(200, 400), trait0.EndPosition);
            Assert.AreEqual(new SKPoint(100, 400), trait1.StartPosition);
            Assert.AreEqual(new SKPoint(100, 300), trait1.EndPosition);
            Assert.AreEqual(100, trait0.Length);
			Assert.AreEqual(new SKPoint(120, 400), sfp0.Position);
            Assert.AreEqual(new SKPoint(180, 400), efp0.Position);
            Assert.AreEqual(60, focal0.Length);
            Assert.AreEqual(1, focal0.Direction);
            Assert.AreEqual(140, focal1.Length);
            Assert.AreEqual(1, focal1.Direction);
            Assert.AreEqual(focal0.MidPosition, bsp0.Position);
            Assert.AreEqual(new SKPoint(150, 400), bsp0.Position);
            Assert.AreEqual(new SKPoint(100, 322), bep0.Position);
            Assert.AreEqual(2.33333, db0.TRatio, tolerance);

            // Move trait
            stp0.MoveTo(new SKPoint(300, 400));
            Assert.AreEqual(new SKPoint(300, 400), trait0.StartPosition);
            Assert.AreEqual(new SKPoint(300, 400), trait1.StartPosition);
            Assert.AreEqual(100, trait0.Length);
            Assert.AreEqual(60, focal0.Length);
            etp0.MoveTo(new SKPoint(300, 800));
            Assert.AreEqual(new SKPoint(300, 800), trait0.EndPosition);
            Assert.AreEqual(400, trait0.Length);
            Assert.AreEqual(240, focal0.Length);
            etp1.MoveTo(new SKPoint(700, 400));
            Assert.AreEqual(560, focal1.Length);
            rp1.MoveTo(new SKPoint(500, 400));
            Assert.AreEqual(new SKPoint(500, 400), trait0.StartPosition);
            Assert.AreEqual(new SKPoint(500, 400), trait1.StartPosition);
            Assert.AreEqual(new SKPoint(400, 600), bsp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), bep0.Position);

            // Move focal
            sfp0.T = 0.4f;
            efp0.T = 0.6f;
            Assert.AreEqual(new SKPoint(420, 560), sfp0.Position);
            Assert.AreEqual(new SKPoint(380, 640), efp0.Position);
            Assert.AreEqual(89.44272, focal0.Length, tolerance);
            Assert.AreEqual(1, focal0.Direction);
            Assert.AreEqual(new SKPoint(400, 600), bsp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), bep0.Position);
            Assert.AreEqual(6.99999, db0.TRatio, tolerance);
            sfp0.T = 0.8f;
            efp0.T = 0.2f;
            Assert.AreEqual(new SKPoint(340, 720), sfp0.Position);
            Assert.AreEqual(new SKPoint(460, 480), efp0.Position);
            Assert.AreEqual(268.32815, focal0.Length, tolerance);
            Assert.AreEqual(-1, focal0.Direction);
            Assert.AreEqual(new SKPoint(400, 600), bsp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), bep0.Position);
            Assert.AreEqual(-2.33333, db0.TRatio, tolerance);
        }
	}
}
