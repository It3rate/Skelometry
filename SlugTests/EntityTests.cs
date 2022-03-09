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

		private Agent _agent;
		private Entity _entity;
		private TerminalPoint _stp0;
		private TerminalPoint _etp0;
		private Trait _trait0;
		private RefPoint _srp1;
		private TerminalPoint _etp1;
		private Trait _trait1;

		private FocalPoint _sfp0;
		private FocalPoint _efp0;
		private Focal _focal0;
		private FocalPoint _sfp1;
		private FocalPoint _efp1;
		private Focal _focal1;

		private BondPoint _sbp0;
		private BondPoint _ebp0;
		private SingleBond _sBond0;
		private DoubleBond _dBond0;

        [TestInitialize()]
		public void EntityInitialize()
		{
			_agent = new Agent(new RenderEncoder());
			_entity = new Entity(PadKind.Input);

			// Traits
			_stp0 = new TerminalPoint(PadKind.Input, new SKPoint(100, 400));
			_etp0 = new TerminalPoint(PadKind.Input, new SKPoint(200, 400));
			_trait0 = new Trait(TraitKind.Default, _stp0, _etp0);
			_srp1 = new RefPoint(PadKind.Input, _stp0.Key);
			_etp1 = new TerminalPoint(PadKind.Input, new SKPoint(100, 300));
			_trait1 = new Trait(TraitKind.Default, _srp1, _etp1);

			// Focals
			_sfp0 = new FocalPoint(PadKind.Input, _trait0.Key, 0.2f);
			_efp0 = new FocalPoint(PadKind.Input, _trait0.Key, 0.8f);
			_focal0 = new Focal(_entity, _sfp0, _efp0);
			_sfp1 = new FocalPoint(PadKind.Input, _trait1.Key, -0.2f);
			_efp1 = new FocalPoint(PadKind.Input, _trait1.Key, 1.2f);
			_focal1 = new Focal(_entity, _sfp1, _efp1);

			// Bonds
			_sbp0 = new BondPoint(PadKind.Input, _focal0.Key, 0.5f);
			_ebp0 = new BondPoint(PadKind.Input, _focal1.Key, 0.7f);
			_sBond0 = new SingleBond(_sbp0, _ebp0);
			_dBond0 = new DoubleBond(_focal0, _focal1);
		}

        [TestMethod]
		public void CreationTests()
		{
			// first trait
			Assert.AreEqual(_stp0.Position, _trait0.StartPosition);
			Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
			Assert.AreEqual(_etp0.Position, _trait0.EndPosition);
			Assert.AreEqual(new SKPoint(200, 400), _trait0.EndPosition);

			// first trait focal
			Assert.AreEqual(_focal0.StartT, 0.2f);
			Assert.AreEqual(_focal0.EndT, 0.8f);
			Assert.AreEqual(_focal0.StartPosition, new SKPoint(120, 400));
			Assert.AreEqual(_focal0.EndPosition, new SKPoint(180, 400));

			// second trait
			Assert.AreNotEqual(_srp1.Key, _stp0.Key);
			Assert.AreEqual(_srp1.TargetKey, _stp0.Key);
			Assert.AreEqual(_srp1.Position, _trait0.StartPosition);
			Assert.AreEqual(_srp1.DistanceToPoint(_trait0.StartPosition), 0);
			Assert.AreEqual(_srp1.DistanceToPoint(_trait0.EndPosition), 100);
			Assert.AreEqual(_etp1.DistanceToPoint(_trait0.EndPosition), Math.Sqrt(2.0) * 100, tolerance);

			// Second trait focal
			Assert.AreEqual(_focal1.StartT, -0.2f);
			Assert.AreEqual(_focal1.EndT, 1.2f);
			Assert.AreEqual(_focal1.StartPosition, new SKPoint(100, 420));
			Assert.AreEqual(_focal1.EndPosition, new SKPoint(100, 280));

            // Single Bonds
			Assert.AreEqual(_sbp0.T, 0.5f);
			Assert.AreEqual(_ebp0.T, 0.7f);
			Assert.AreEqual(_sbp0.Position, _focal0.MidPosition);
			Assert.AreEqual(_sBond0.LocalRatio, new Slug(0.5f, 0.7f));
			Assert.AreEqual(_sBond0.TRatio, 0.7f / 0.5f, tolerance);
			Assert.AreEqual(_sBond0.Length, 92.64988f, tolerance);

            // Double Bonds
			Assert.AreEqual(_dBond0.TRatio, 2.333333f, tolerance);
			Assert.AreEqual(_dBond0.Center, new SKPoint(125, 375));
			Assert.AreEqual(_dBond0.AllKeys.Count, 6);
			CollectionAssert.AreEqual(_dBond0.Points, new List<IPoint> {_sfp0, _efp0, _sfp1, _efp1});
			Assert.IsTrue(_dBond0.IsPointInside(new SKPoint(120, 380)));
			Assert.IsFalse(_dBond0.IsPointInside(new SKPoint(150, 340)));

            // Element Kinds
			Assert.AreEqual(_entity.ElementKind, ElementKind.Entity);
			Assert.AreEqual(_srp1.ElementKind, ElementKind.RefPoint);
			Assert.AreEqual(_etp1.ElementKind, ElementKind.Terminal);
			Assert.AreEqual(_trait1.ElementKind, ElementKind.Trait);
			Assert.AreEqual(_sfp1.ElementKind, ElementKind.FocalPoint);
			Assert.AreEqual(_focal1.ElementKind, ElementKind.Focal);
			Assert.AreEqual(_sbp0.ElementKind, ElementKind.BondPoint);
			Assert.AreEqual(_ebp0.ElementKind, ElementKind.BondPoint);
			Assert.AreEqual(_sBond0.ElementKind, ElementKind.SingleBond);
			Assert.AreEqual(_dBond0.ElementKind, ElementKind.DoubleBond);
		}

        [TestMethod]
		public void MotionTests()
		{
            // confirm positions
            Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
            Assert.AreEqual(new SKPoint(200, 400), _trait0.EndPosition);
            Assert.AreEqual(new SKPoint(100, 400), _trait1.StartPosition);
            Assert.AreEqual(new SKPoint(100, 300), _trait1.EndPosition);
            Assert.AreEqual(100, _trait0.Length);
			Assert.AreEqual(new SKPoint(120, 400), _sfp0.Position);
            Assert.AreEqual(new SKPoint(180, 400), _efp0.Position);
            Assert.AreEqual(60, _focal0.Length);
            Assert.AreEqual(1, _focal0.Direction);
            Assert.AreEqual(140, _focal1.Length);
            Assert.AreEqual(1, _focal1.Direction);
            Assert.AreEqual(_focal0.MidPosition, _sbp0.Position);
            Assert.AreEqual(new SKPoint(150, 400), _sbp0.Position);
            Assert.AreEqual(new SKPoint(100, 322), _ebp0.Position);
            Assert.AreEqual(2.33333, _dBond0.TRatio, tolerance);

            // Move trait
            _stp0.MoveTo(new SKPoint(300, 400));
            Assert.AreEqual(new SKPoint(300, 400), _trait0.StartPosition);
            Assert.AreEqual(new SKPoint(300, 400), _trait1.StartPosition);
            Assert.AreEqual(100, _trait0.Length);
            Assert.AreEqual(60, _focal0.Length);
            _etp0.MoveTo(new SKPoint(300, 800));
            Assert.AreEqual(new SKPoint(300, 800), _trait0.EndPosition);
            Assert.AreEqual(400, _trait0.Length);
            Assert.AreEqual(240, _focal0.Length);
            _etp1.MoveTo(new SKPoint(700, 400));
            Assert.AreEqual(560, _focal1.Length);
            _srp1.MoveTo(new SKPoint(500, 400));
            Assert.AreEqual(new SKPoint(500, 400), _trait0.StartPosition);
            Assert.AreEqual(new SKPoint(500, 400), _trait1.StartPosition);
            Assert.AreEqual(new SKPoint(400, 600), _sbp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), _ebp0.Position);

            // Move focal
            _sfp0.T = 0.4f;
            _efp0.T = 0.6f;
            Assert.AreEqual(new SKPoint(420, 560), _sfp0.Position);
            Assert.AreEqual(new SKPoint(380, 640), _efp0.Position);
            Assert.AreEqual(89.44272, _focal0.Length, tolerance);
            Assert.AreEqual(1, _focal0.Direction);
            Assert.AreEqual(new SKPoint(400, 600), _sbp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), _ebp0.Position);
            Assert.AreEqual(6.99999, _dBond0.TRatio, tolerance);
            _sfp0.T = 0.8f;
            _efp0.T = 0.2f;
            Assert.AreEqual(new SKPoint(340, 720), _sfp0.Position);
            Assert.AreEqual(new SKPoint(460, 480), _efp0.Position);
            Assert.AreEqual(268.32815, _focal0.Length, tolerance);
            Assert.AreEqual(-1, _focal0.Direction);
            Assert.AreEqual(new SKPoint(400, 600), _sbp0.Position);
            Assert.AreEqual(new SKPoint(656, 400), _ebp0.Position);
            Assert.AreEqual(-2.33333, _dBond0.TRatio, tolerance);
        }
	}
}
