using SkiaSharp;
using Slugs.Agents;
using Slugs.Commands.EditCommands;
using Slugs.Constraints;
using Slugs.Entities;
using Slugs.Pads;
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
	    private static float tolerance = 0.00001f;

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

        private Dictionary<int, SKPoint> _changes;

        [TestInitialize()]
	    public void EntitiesInitialize()
	    {
		    _agent = new Agent(new RenderEncoder());
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
        public void TestCollinear()
        {
	        // test two traits
            Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);
	        _changes.Clear();
            var coll0 = new CollinearConstraint(_trait0, _trait1);
	        _pad.Constraints.Add(coll0);
	        coll0.OnElementChanged(_trait0, _changes);
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(105, 395), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(195, 305), _trait1.EndPosition);

	        // test end point
            _changes.Clear();
	        var coll1 = new CollinearConstraint(_trait0, _trait1.EndPoint);
	        _pad.Constraints.Add(coll1);
	        coll1.OnElementChanged(_trait0, _changes);
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(105, 395), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(195, 305), _trait1.EndPosition);

	        // test start point
            Assert.AreEqual(new SKPoint(200, 400), _trait2.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait2.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 500), _trait3.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 500), _trait3.EndPosition);
	        _changes.Clear();
	        var coll2 = new CollinearConstraint(_trait2.StartPoint, _trait3);
	        _pad.Constraints.Add(coll2);
	        coll2.OnElementChanged(_trait3, _changes);
	        Assert.AreEqual(new SKPoint(200, 500), _trait2.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait2.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 500), _trait3.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 500), _trait3.EndPosition);

	        _changes.Clear();
	        var coll3 = new CollinearConstraint(_trait4.StartPoint, _trait5);
	        _pad.Constraints.Add(coll3);
	        coll3.OnElementChanged(_trait4.StartPoint, _changes);
	        Assert.AreEqual(new SKPoint(200, 400), _trait4.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait4.EndPosition);
	        Assert.AreEqual(new SKPoint(220, 400), _trait5.StartPosition);
	        Assert.AreEqual(new SKPoint(300, 400), _trait5.EndPosition);

            //var coinCommand = new AddConstraintCommand(InputPad, new CoincidentConstraint(t1.EndPointTask.IPoint, t2.StartPointTask.IPoint));
            //var midCommand = new AddConstraintCommand(InputPad, new MidpointConstraint(t2.AddedTrait, t3.EndPointTask.IPoint));
            //var parCommand = new AddConstraintCommand(InputPad, new ParallelConstraint(t3.AddedTrait, t4.AddedTrait, LengthLock.Ratio));
            //var eqCommand = new AddConstraintCommand(InputPad,
            // new EqualConstraint(t5.AddedTrait, t6.AddedTrait, LengthLock.None, DirectionLock.Perpendicular));
            //var hCommand = new AddConstraintCommand(InputPad, new HorizontalVerticalConstraint(t7.AddedTrait, true));
            //var vCommand = new AddConstraintCommand(InputPad, new HorizontalVerticalConstraint(t8.AddedTrait, false));
        }

        [TestMethod]
        public void TestCoincident()
        {
	        Assert.AreEqual(new SKPoint(100, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);

	        _changes.Clear();
	        var con0 = new CoincidentConstraint(_trait0.StartPoint, _trait1.StartPoint);
	        _pad.Constraints.Add(con0);
	        con0.OnElementChanged(_trait1.StartPoint, _changes);

	        Assert.AreEqual(new SKPoint(110, 400), _trait0.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 300), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(110, 400), _trait1.StartPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);

	        _changes.Clear();
	        con0 = new CoincidentConstraint(_trait0.EndPoint, _trait1.EndPoint);
	        _pad.Constraints.Add(con0);
	        con0.OnElementChanged(_trait1.EndPoint, _changes);
	        Assert.AreEqual(new SKPoint(200, 310), _trait0.EndPosition);
	        Assert.AreEqual(new SKPoint(200, 310), _trait1.EndPosition);
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
        }
    }
}
