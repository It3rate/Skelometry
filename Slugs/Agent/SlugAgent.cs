//using System.Windows.Forms;
//using SkiaSharp;
//using SkiaSharp.Views.Desktop;
//using Slugs.Entities;
//using Slugs.Extensions;
//using Slugs.Input;
//using Slugs.Pads;
//using Slugs.Renderer;
//using Slugs.Slugs;

//namespace Slugs.Agent
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    public class SlugAgent : IAgent
//    {
//	    public static IAgent ActiveAgent { get; private set; }
//	    public readonly SlugPad WorkingPad = new SlugPad(PadKind.Working);
//	    public readonly SlugPad InputPad = new SlugPad(PadKind.Drawn);
//	    public SlugPad PadAt(int index) => _data.Pads[index];

//	    private readonly SlugRenderer _renderer;
//	    public RenderStatus RenderStatus { get; }

//        private UIData _data = new UIData();
//        public double UnitPull { get => _data.UnitPull; set => _data.UnitPull = value; }
//        public double UnitPush { get => _data.UnitPush; set => _data.UnitPush = value; }

//        public SlugAgent(SlugRenderer renderer)
//	    {
//		    ActiveAgent = this;
//		    _data.Pads[WorkingPad.PadIndex] = WorkingPad;
//		    _data.Pads[InputPad.PadIndex] = InputPad;

//            _renderer = renderer;
//            _renderer.UIData = _data;

//		    EntityPad.ActiveSlug = new Slug(_data.UnitPull, _data.UnitPush);
//		    var pl = DataMap.CreateIn(InputPad, new SKPoint(200, 100), new SKPoint(400, 300));
//		    ClearMouse();
//	    }

//	    public SKPoint this[IPointRef pointRef]
//	    {
//		    get
//		    {
//			    SKPoint result;
//			    if (!pointRef.IsEmpty)
//			    {
//				    var pad = _data.Pads[pointRef.PadIndex];
//				    var dataMap = pad.InputFromIndex(pointRef.EntityKey);
//				    result = dataMap[pointRef];
//			    }
//			    else
//			    {
//				    result = SKPoint.Empty;
//                }
//			    return result;
//		    } 
//		    set
//		    {
//			    var pad = _data.Pads[pointRef.PadIndex];
//			    var dataMap = pad.InputFromIndex(pointRef.EntityKey);
//			    dataMap[pointRef] = value;
//		    }
//	    }

//	    public void MergePointRefs(List<IPointRef> fromList, IPointRef to, SKPoint position)
//	    {
//		    to.SKPoint = position;
//		    foreach (var from in fromList)
//		    {
//			    UpdatePointRef(from, to);
//		    }
//	    }

//	    public void UpdatePointRef(IPointRef from, IPointRef to)
//	    {
//		    var pad = _data.Pads[from.PadIndex];
//		    var dataMap = pad.InputFromIndex(from.EntityKey);
//		    dataMap[from.FocalKey] = to;
//	    }

//	    public void Clear()
//        {
//        }

//        public void ClearMouse()
//        {
//	        _data.DownPoint = SKPoint.Empty;
//	        _data.CurrentPoint = SKPoint.Empty;
//	        _data.SnapPoint = SKPoint.Empty;
//            _data.DragSegment.Clear();
//	        _data.ClickData.Clear();
//            _data.DragPath.Clear();
//	        WorkingPad.Clear();
//	        _data.DragRef.Clear();

//	        _data.DownPoint = SKPoint.Empty;
//	        _data.StartHighlight = PointRef.Empty;
//        }

//        public void Draw()
//	    {
//		    _renderer.Draw();
//	    }

//	    public bool MouseDown(MouseEventArgs e)
//	    {
//		    _data.CurrentPoint = e.Location.ToSKPoint();
//            SetHighlight();
//            _data.DownPoint = _data.SnapPoint;
//            _data.DragRef.Origin = _data.CurrentPoint;
//		    if (_data.HasHighlightPoint && CurrentKey != Keys.ControlKey)
//		    {
//			    _data.DragRef.AddEntity(_data.HighlightPoints);
//		    }
//		    else if(!_data.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
//		    {
//			    _data.DragRef.AddEntity(_data.HighlightLine.StartRef, _data.HighlightLine.EndRef, true);
//		    }
//		    else
//            {
//			    _data.DragSegment.AddEntity(_data.SnapPoint);
//			    _data.DragPath.AddEntity(_data.SnapPoint);
//                _data.StartHighlight = _data.FirstHighlightPoint;
//            }

//            return true;
//	    }

//	    public bool MouseMove(MouseEventArgs e)
//	    {
//            WorkingPad.Clear();
//			  _data.CurrentPoint = e.Location.ToSKPoint();
//            SetHighlight();
//            SetDragging();
//            SetCreating();

//            return true;
//        }

//	    public bool MouseUp(MouseEventArgs e)
//	    {
//		    _data.CurrentPoint = e.Location.ToSKPoint();
//		    SetHighlight(true);
//            SetDragging();
//		    SetCreating(true);

//            ClearMouse();
//            return true;
//        }

//	    private Keys CurrentKey;
//	    public bool KeyDown(KeyEventArgs e)
//	    {
//		    CurrentKey = e.KeyCode;
//		    return true;
//        }

//	    public bool KeyUp(KeyEventArgs e)
//	    {
//		    CurrentKey = Keys.None;
//            return true;
//        }

//	    private bool SetHighlight(bool final = false)
//	    {
//		    bool hasChange = false;
//		    var snap = InputPad.GetSnapPoints(_data.CurrentPoint, _data.DragRef);
//		    if (snap.Count > 0)
//		    {
//			    hasChange = true;
//			    _data.HighlightPoints = snap;
//			    _data.HighlightLine = SegRef.Empty;
//		    }
//		    else
//		    {
//			    _data.HighlightPoints.Clear();
//			    var snapLine = InputPad.GetSnapLine(_data.CurrentPoint);
//			    if (snapLine.IsEmpty)
//			    {
//				    _data.HighlightLine = SegRef.Empty;
//			    }
//			    else
//			    {
//				    hasChange = true;
//				    _data.HighlightLine = snapLine;
//			    }
//		    }

//		    _data.SnapPoint = _data.CurrentPoint;
//		    if (_data.HasHighlightPoint)
//		    {
//			    _data.SnapPoint = _data.GetHighlightPoint();
//		    }
//            else if (_data.HasHighlightLine)
//		    {
//			    _data.SnapPoint = _data.GetHighlightLine().ProjectPointOnto(_data.CurrentPoint);
//		    }

//		    return hasChange;
//	    }

//	    private bool SetCreating(bool final = false)
//	    {
//		    var result = false;
//		    if (_data.IsDragging)
//		    {
//			    if (final)
//			    {
//				    _data.DragRef.OffsetValues(_data.SnapPoint);
//				    if (_data.IsDraggingPoint && _data.HasHighlightPoint)
//				    {
//                        MergePointRefs(_data.DragRef.PointRefs, _data.FirstHighlightPoint, _data.SnapPoint);
//				    }
//                    else if (_data.IsDraggingPoint && _data.HasHighlightLine)
//				    {
//					    var dragPoint = _data.DragRef.PointRefs[0];
//					    //var vp = _data.HighlightLine.GetVirtualPointFor(_data.SnapPoint);
//                        //UpdatePointRef(dragPoint, vp);
//					    // replace last point with Virutal SKPoint Ref for line being snapped to.
//				    }
//				    result = true;
//			    }
//		    }
//            else if (_data.IsDown)
//		    {
//			    _data.DragPath.AddEntity(_data.SnapPoint);
//			    DataMap.CreateIn(WorkingPad, _data.DownPoint, _data.SnapPoint);
//			    if (final)
//			    {
//				    _data.DragSegment.AddEntity(_data.SnapPoint);
//				    _data.ClickData.AddEntity(_data.SnapPoint);
//				    _data.DragPath.AddEntity(_data.SnapPoint);
//				    if (_data.DragSegment[0].DistanceTo(_data.DragSegment[1]) > 10)
//				    {
//					    var newDataMap = DataMap.CreateIn(InputPad, _data.DragSegment);
//					    if (!_data.StartHighlight.IsEmpty)
//					    {
//						    MergePointRefs(new List<IPointRef>() { newDataMap.FirstRef }, _data.StartHighlight, _data.StartHighlight.SKPoint);
//					    }
//					    if (_data.HasHighlightPoint)
//					    {
//						    MergePointRefs(new List<IPointRef>() { newDataMap.LastRef }, _data.FirstHighlightPoint, _data.SnapPoint);
//					    }
//                    }
//			    }
//			    result = true;
//		    }
//		    return result;
//	    }
//        private bool SetDragging()
//	    {
//		    var result = false;
//		    if (_data.IsDragging)
//		    {
//			    _data.DragRef.OffsetValues(_data.CurrentPoint);
//			    result = true;
//		    }
//            return result;
//	    }
//    }
//}
