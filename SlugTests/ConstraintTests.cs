using SkiaSharp;
using Slugs.Agents;
using Slugs.Commands.EditCommands;
using Slugs.Constraints;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;
using Slugs.Renderer;

namespace SlugTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [TestClass]
    public class ConstraintTests
    {
	    private static double tolerance = 0.00001;

	    private Dictionary<int, SKPoint> _changes;

	    private Pad _pad;
	    private Agent _agent;
	    private Entity _entity;
	    private Trait _trait0;
	    private Trait _trait1;
	    private Trait _trait2;
	    private Trait _trait3;
	    private Trait _trait4;
	    private Trait _trait5;
	    private Trait _trait6;
	    private Trait _trait7;
	    private Trait _trait8;

        [TestInitialize()]
	    public void EntitiesInitialize()
	    {
		    _agent = new Agent(new RenderEncoder(false));
		    _pad = _agent.InputPad;
		    _entity = new Entity(PadKind.Input);

		    // Traits
		    _trait0 = new Trait(100, 400, 200, 300);
		    _trait1 = new Trait(110, 400, 200, 310);
		    _trait2 = new Trait(200, 400, 300, 400);
		    _trait3 = new Trait(220, 500, 300, 500);
		    _trait4 = new Trait(200, 400, 300, 400);
		    _trait5 = new Trait(220, 500, 300, 500);

            _trait6 = new Trait(500, 500, 500, 700);
            _trait7 = new Trait(300, 600, 700, 600);
            _trait8 = new Trait(300, 600, 700, 650);

            _changes = new Dictionary<int, SKPoint>();          
	    }

        [TestMethod]
        public void TestCoincident()
        {
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);

	        _changes.Clear();
	        var con = new CoincidentConstraint(_trait0.StartPoint, _trait1.StartPoint);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait1.StartPoint, _changes);

	        Assert.AreEqual(new SKPoint(110, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);

	        _changes.Clear();
	        con = new CoincidentConstraint(_trait0.EndPoint, _trait1.EndPoint);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait1.EndPoint, _changes);
	        Assert.AreEqual(new SKPoint(200, 310), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);
        }

        [TestMethod]
        public void TestCollinear()
        {
	        // test two traits
            Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(500, 500), _trait6.StartPosition);
	        Assert.AreEqual(new SKPoint(500, 700), _trait6.EndPosition);
	        Assert.IsFalse(_trait0.IsParallelTo(_trait6));
            _changes.Clear();
            var coll0 = new CollinearConstraint(_trait0, _trait1);
	        _pad.Constraints.Add(coll0);
	        coll0.OnElementChanged(_trait0, _changes);
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(105, 395), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(195, 305), _trait1.EndPosition);
	        Assert.IsTrue(_trait0.IsParallelTo(_trait1));
	        Assert.IsTrue(_trait0.IsCollinearTo(_trait1));

            // test end point
            _changes.Clear();
	        var coll1 = new CollinearConstraint(_trait0, _trait1.EndPoint);
	        _pad.Constraints.Add(coll1);
	        coll1.OnElementChanged(_trait0, _changes);
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(105, 395), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(195, 305), _trait1.EndPosition);
	        Assert.IsTrue(_trait0.IsParallelTo(_trait1));
	        Assert.IsTrue(_trait0.IsCollinearTo(_trait1));

            // test start point
            Assert.AreEqual(new SKPoint(200, 400), _trait2.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait2.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 500), _trait3.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 500), _trait3.EndPosition);
	        Assert.IsTrue(_trait2.IsParallelTo(_trait3));
	        Assert.IsFalse(_trait2.IsCollinearTo(_trait3));
            _changes.Clear();
	        var coll2 = new CollinearConstraint(_trait2.StartPoint, _trait3);
	        _pad.Constraints.Add(coll2);
	        coll2.OnElementChanged(_trait3, _changes);
	        Assert.AreEqual(new SKPoint(200, 500), _trait2.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait2.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 500), _trait3.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 500), _trait3.EndPosition);
	        Assert.IsFalse(_trait2.IsParallelTo(_trait3));
	        Assert.IsFalse(_trait2.IsCollinearTo(_trait3));

            _changes.Clear();
	        var coll3 = new CollinearConstraint(_trait4.StartPoint, _trait5);
	        _pad.Constraints.Add(coll3);
	        coll3.OnElementChanged(_trait4.StartPoint, _changes);
	        Assert.AreEqual(new SKPoint(200, 400), _trait4.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait4.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 400), _trait5.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait5.EndPosition);
	        Assert.IsTrue(_trait4.IsParallelTo(_trait5));
	        Assert.IsTrue(_trait4.IsCollinearTo(_trait5));
        }

        [TestMethod]
        public void TestPerpendicular()
        {
	        Assert.IsTrue(_trait6.IsPerpendicularTo(_trait7));
	        Assert.IsFalse(_trait6.IsPerpendicularTo(_trait8));

	        _changes.Clear();
	        var con = new PerpendicularConstraint(_trait6, _trait8, LengthLock.None);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait6, _changes);
	        Assert.IsTrue(_trait6.IsPerpendicularTo(_trait7));
	        Assert.IsTrue(_trait6.IsPerpendicularTo(_trait8));

	        _changes.Clear();
	        _trait6.EndPoint.Position = new SKPoint(300, 450);
	        con.OnElementChanged(_trait6, _changes);
	        Assert.IsFalse(_trait6.IsPerpendicularTo(_trait7));
	        Assert.IsTrue(_trait6.IsPerpendicularTo(_trait8));
        }

        [TestMethod]
        public void TestParallel()
        {
	        Assert.IsFalse(_trait6.IsParallelTo(_trait7));
	        Assert.IsFalse(_trait6.IsParallelTo(_trait8));

	        _changes.Clear();
	        var con = new ParallelConstraint(_trait6, _trait8, LengthLock.None);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait6, _changes);
	        Assert.IsFalse(_trait6.IsParallelTo(_trait7));
	        Assert.IsTrue(_trait6.IsParallelTo(_trait8));

	        _changes.Clear();
	        con = new ParallelConstraint(_trait6, _trait7, LengthLock.None);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait6, _changes);
	        Assert.IsTrue(_trait6.IsParallelTo(_trait7));
	        Assert.IsTrue(_trait6.IsParallelTo(_trait8));

	        _changes.Clear();
	        _trait7.EndPoint.Position = new SKPoint(300, 450);
	        con.OnElementChanged(_trait7, _changes);
	        Assert.IsTrue(_trait6.IsParallelTo(_trait7));
	        Assert.IsTrue(_trait6.IsParallelTo(_trait8));
        }
        [TestMethod]
        public void TestHorizontalVertical()
        {
	        Assert.AreEqual(new SKPoint(500, 500), _trait6.StartPosition);
	        Assert.AreEqual(new SKPoint(500, 700), _trait6.EndPosition);
	        Assert.AreEqual(new SKPoint(300, 600), _trait7.StartPosition);
	        Assert.AreEqual(new SKPoint(700, 600), _trait7.EndPosition);
	        Assert.AreEqual(new SKPoint(300, 600), _trait8.StartPosition);
	        Assert.AreEqual(new SKPoint(700, 650), _trait8.EndPosition);

	        Assert.IsFalse(_trait6.IsHorizontal());
	        Assert.IsTrue(_trait6.IsVertical());
	        Assert.IsTrue(_trait7.IsHorizontal());
	        Assert.IsFalse(_trait7.IsVertical());
	        Assert.IsFalse(_trait8.IsHorizontal());
	        Assert.IsFalse(_trait8.IsVertical());

	        _changes.Clear();
	        var con = new HorizontalVerticalConstraint(_trait8, true);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait8, _changes);
	        Assert.IsTrue(_trait8.IsHorizontal());
	        Assert.IsFalse(_trait8.IsVertical());

	        _changes.Clear();
            _trait8.EndPoint.Position = new SKPoint(300, 450);
	        con.OnElementChanged(_trait8, _changes);
	        Assert.IsTrue(_trait8.IsHorizontal());

            _changes.Clear();
	        con = new HorizontalVerticalConstraint(_trait5, false);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait5, _changes);
	        Assert.IsFalse(_trait5.IsHorizontal());
	        Assert.IsTrue(_trait5.IsVertical());

	        _changes.Clear();
            _trait5.StartPoint.Position = new SKPoint(100,150);
            con.OnElementChanged(_trait5, _changes);
            Assert.IsTrue(_trait5.IsVertical());
        }

        [TestMethod]
        public void TestMidpoint()
        {
	        Assert.IsFalse(_trait0.MidPosition == _trait5.EndPosition);

	        _changes.Clear();
	        var con = new MidpointConstraint(_trait0, _trait5.EndPoint);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait0, _changes);
	        Assert.IsTrue(_trait0.MidPosition == _trait5.EndPosition);

	        _changes.Clear();
	        _trait0.StartPoint.Position = new SKPoint(300, 450);
	        con.OnElementChanged(_trait0.StartPoint, _changes);
	        Assert.IsTrue(_trait0.MidPosition == _trait5.EndPosition);

	        _changes.Clear();
	        _trait5.EndPoint.Position = new SKPoint(500, 250);
	        con.OnElementChanged(_trait5.EndPoint, _changes);
	        Assert.IsTrue(_trait0.MidPosition == _trait5.EndPosition);


	        Assert.IsFalse(_trait1.MidPosition == _trait6.MidPosition);
	        _changes.Clear();
	        con = new MidpointConstraint(_trait1, _trait6);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait1, _changes);
	        Assert.IsTrue(_trait1.MidPosition == _trait6.MidPosition);
        }
        [TestMethod]
        public void TestEqual()
        {
	        Assert.IsFalse(_trait0.Length == _trait1.Length);
	        Assert.IsFalse(_trait6.Length == _trait7.Length);

	        _changes.Clear();
	        var con = new EqualConstraint(_trait0, _trait1, LengthLock.Equal, DirectionLock.Parallel);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait0, _changes);
	        Assert.IsTrue(_trait0.Length == _trait1.Length);
	        Assert.IsTrue(_trait0.IsParallelTo(_trait1));

	        var ratio = _trait6.Length / _trait7.Length;
	        _changes.Clear();
	        con = new EqualConstraint(_trait6, _trait7, LengthLock.Ratio, DirectionLock.None);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait6, _changes);
	        Assert.AreEqual(_trait6.Length / _trait7.Length, ratio, tolerance);
	        Assert.IsFalse(_trait6.IsParallelTo(_trait7));
	        Assert.IsTrue(_trait6.IsPerpendicularTo(_trait7));

	        _changes.Clear();
	        Assert.IsFalse(_trait5.IsPerpendicularTo(_trait8));
	        con = new EqualConstraint(_trait5, _trait8, LengthLock.Equal, DirectionLock.Perpendicular);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait5, _changes);
	        Assert.AreEqual(_trait5.Length, _trait8.Length, tolerance);
	        Assert.IsFalse(_trait5.IsParallelTo(_trait8));
	        Assert.IsTrue(_trait5.IsPerpendicularTo(_trait8));

        }
        [TestMethod]
        public void TestLock()
        {
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);

	        _trait0.EndPoint.Position = new SKPoint(500, 250);
	        Assert.AreEqual(new SKPoint(500, 250), _trait0.EndPosition);

            _changes.Clear();
	        var con = new LockConstraint(_trait0.EndPoint, true);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait0.EndPoint, _changes);
	        _trait0.EndPoint.Position = new SKPoint(300, 150);
	        Assert.AreNotEqual(new SKPoint(300, 150), _trait0.EndPosition);

	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);
            _changes.Clear();
	        con = new LockConstraint(_trait1, true);
	        _pad.Constraints.Add(con);
	        con.OnElementChanged(_trait1, _changes);
	        _trait1.EndPoint.Position = new SKPoint(500, 250);
	        Assert.AreNotEqual(new SKPoint(500, 250), _trait1.EndPosition);

        }
    }
}
