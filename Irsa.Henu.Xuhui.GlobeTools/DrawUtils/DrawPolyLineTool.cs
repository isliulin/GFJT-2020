﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using WorldWind.PluginEngine;
using WorldWind.BaseTool;
using Qrst;

namespace DrawTools.Plugins
{
    public class DrawPolyLineTool : DrawBaseTool
    {
        DrawPolylineLayer drawLayer;

        public DrawPolyLineTool()
            : base()
        {

        }

        public override void Load()
        {
            drawLayer = new DrawPolylineLayer("tmpDrawPolylineLyr", Color.FromArgb(255, 255, 0, 0), this, ParentApplication.DrawArgs);
            drawLayer.IsOn = true;      //关闭WW自带响应事件
            ParentApplication.CurrentWorld.RenderableObjects.Add(drawLayer);

            // Subscribe events
            ParentApplication.MouseMove += new MouseEventHandler(drawLayer.MouseMove);
            ParentApplication.MouseDown += new MouseEventHandler(drawLayer.MouseDown);
            ParentApplication.MouseUp += new MouseEventHandler(drawLayer.MouseUp);
            ParentApplication.MouseDoubleClick += new MouseEventHandler(drawLayer.MouseDoubleClick);
            ParentApplication.KeyUp += new KeyEventHandler(drawLayer.KeyUp);
        }

        /// <summary>
        /// Unload our plugin
        /// </summary>
        public override void Unload()
        {
            ParentApplication.MouseMove -= new MouseEventHandler(drawLayer.MouseMove);
            ParentApplication.MouseDown -= new MouseEventHandler(drawLayer.MouseDown);
            ParentApplication.MouseUp -= new MouseEventHandler(drawLayer.MouseUp);
            ParentApplication.MouseDoubleClick -= new MouseEventHandler(drawLayer.MouseDoubleClick);
            ParentApplication.KeyUp -= new KeyEventHandler(drawLayer.KeyUp);

            ParentApplication.CurrentWorld.RenderableObjects.Remove(drawLayer);
            drawLayer.Dispose();
            drawLayer = null; 
        }

        public override void PluginLoad(QrstAxGlobeControl parent, string pluginDirectory)
        {
            
            base.PluginLoad(parent, pluginDirectory);
        }
    }
}