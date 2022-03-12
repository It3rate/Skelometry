using SkiaSharp;
using Slugs.Agents;
using Slugs.Commands;
using Slugs.Commands.EditCommands;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Renderer;

namespace SlugTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Summary description for CommandTests
    /// </summary>
    [TestClass]
    public class CommandTests
    {
	    private static double tolerance = 0.00001;

	    private Dictionary<int, SKPoint> _changes;

        private Pad _pad;
	    private Agent _agent;
	    private Entity _entity;
	    private RenderEncoder _renderEncoder;
        private CommandStack<EditCommand> _editCommands;

        [TestInitialize()]
	    public void EntitiesInitialize()
	    {
		    _renderEncoder = new RenderEncoder(false);
            _agent = new Agent(_renderEncoder);
		    _pad = _agent.InputPad;
		    _entity = new Entity(PadKind.Input);
		    _editCommands = new CommandStack<EditCommand>(_agent);
		    _changes = new Dictionary<int, SKPoint>();
	    }

        [TestMethod]
        public void LineCommandTests()
        {
	        var traitCmd1 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 100), new SKPoint(700, 100), true);
	        var traitCmd2 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 140), new SKPoint(700, 140), true);
	        var traitCmd3 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 180), new SKPoint(700, 180), true);
	        var traitCmd4 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 220), new SKPoint(700, 220), true);
	        var traitCmd5 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 260), new SKPoint(700, 260), true);
	        var traitCmd6 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 300), new SKPoint(700, 300), true);
	        var traitCmd7 = new AddTraitCommand(_pad, TraitKind.Default, new SKPoint(100, 340), new SKPoint(700, 340), true);
	        _editCommands.Do(traitCmd1, traitCmd2, traitCmd3, traitCmd4, traitCmd5, traitCmd6, traitCmd7);

	        var index = _editCommands.StackIndex;
            _renderEncoder.Draw();
            var enc0 = _renderEncoder.EncodedFile;
	        var enc1 = _renderEncoder.EncodedFile;
            Assert.IsTrue(enc0 == enc1);
            _editCommands.Undo();
            _renderEncoder.Draw();
            enc1 = _renderEncoder.EncodedFile;
            Assert.IsFalse(enc0 == enc1);
            _editCommands.Redo();
            _renderEncoder.Draw();
            enc1 = _renderEncoder.EncodedFile;
            Assert.IsTrue(enc0 == enc1);

            var trait1 = traitCmd1.AddedTrait;
	        var trait2 = traitCmd2.AddedTrait;
	        var trait3 = traitCmd3.AddedTrait;
	        var fc1 = new AddFocalCommand(_entity, trait2, 0.1f, 0.3f);
	        var fc2 = new AddFocalCommand(_entity, trait3, 0.0f, 0.4f);
	        var fc3 = new AddFocalCommand(_entity, trait1, 0.0f, 0.2f);
	        _editCommands.Do(fc1, fc2, fc3);
	        _pad.SetUnit(fc3.AddedFocal);

	        var bc = new AddSingleBondCommand(fc1.AddedFocal, -0.3f, fc2.AddedFocal, 0.3f);
	        var db = new AddDoubleBondCommand(fc1.AddedFocal, fc2.AddedFocal);
	        _editCommands.Do(bc, db);

            _renderEncoder.Draw();
            var encFinal = _renderEncoder.EncodedFile;
            Assert.IsFalse(enc0 == encFinal);
            _editCommands.UndoToIndex(index);
            _renderEncoder.Draw();
            var enc2 = _renderEncoder.EncodedFile;
            Assert.IsTrue(enc0 == enc1);
            _editCommands.RedoAll();
            _renderEncoder.Draw();
            var enc3 = _renderEncoder.EncodedFile;
            Assert.IsTrue(encFinal == enc3);
        }
    }
}
