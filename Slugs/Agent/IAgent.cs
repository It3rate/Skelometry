﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Renderer;
using Slugs.Slugs;

namespace Slugs.Agent
{
    public interface IAgent
    {
        RenderStatus RenderStatus { get; }
        //SKPoint this[IPointRef pointRef] { get; set; }

        EntityPad PadAt(int index);

        //void UpdatePointRef(IPointRef from, IPointRef to);
        void Clear();
        void Draw();

        bool MouseDown(MouseEventArgs e);
        bool MouseMove(MouseEventArgs e);
        bool MouseUp(MouseEventArgs e);
        bool KeyDown(KeyEventArgs e);
        bool KeyUp(KeyEventArgs e);
    }
}
