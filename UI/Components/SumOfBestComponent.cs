using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SumOfBestComponent : IComponent
    {
        protected InfoTimeComponent InternalComponent { get; set; }
        public SumOfBestSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }

        public GraphicsCache Cache { get; set; }

        //protected IRun PreviousRun { get; set; }
        protected bool PreviousCalculationMode { get; set; }
        protected TimingMethod PreviousTimingMethod { get; set; }

        public float PaddingTop { get { return InternalComponent.PaddingTop; } }
        public float PaddingLeft { get { return InternalComponent.PaddingLeft; } }
        public float PaddingBottom { get { return InternalComponent.PaddingBottom; } }
        public float PaddingRight { get { return InternalComponent.PaddingRight; } }

        public TimeSpan? SumOfBestValue { get; set; }

        private RegularSumOfBestTimeFormatter Formatter { get; set; }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }
        
        public SumOfBestComponent(LiveSplitState state)
        {
            Formatter = new RegularSumOfBestTimeFormatter();
            InternalComponent = new InfoTimeComponent(null, null, Formatter)
            {
                InformationName = "Sum of Best Segments",
                LongestString = "Sum of Best Segments",
                AlternateNameText = new String[]
                {
                    "Sum of Best",
                    "SoB"
                }
            };
            Settings = new SumOfBestSettings();
            state.OnSplit += state_OnSplit;
            state.OnUndoSplit += state_OnUndoSplit;
            state.OnReset += state_OnReset;
            CurrentState = state;
            CurrentState.RunManuallyModified += CurrentState_RunModified;
            Cache = new GraphicsCache();
            UpdateSumOfBestValue(state);
        }

        void CurrentState_RunModified(object sender, EventArgs e)
        {
            UpdateSumOfBestValue(CurrentState);
        }

        void state_OnReset(object sender, TimerPhase e)
        {
            UpdateSumOfBestValue((LiveSplitState)sender);
        }

        void state_OnUndoSplit(object sender, EventArgs e)
        {
            UpdateSumOfBestValue((LiveSplitState)sender);
        }

        void state_OnSplit(object sender, EventArgs e)
        {
            UpdateSumOfBestValue((LiveSplitState)sender);
        }

        void UpdateSumOfBestValue(LiveSplitState state)
        {
            SumOfBestValue = SumOfBest.CalculateSumOfBest(state.Run, state.Settings.SimpleSumOfBest, state.CurrentTimingMethod);
            //PreviousRun = (IRun)(state.Run.Clone());
            PreviousCalculationMode = state.Settings.SimpleSumOfBest;
            PreviousTimingMethod = state.CurrentTimingMethod;
        }

        private bool CheckIfRunChanged(LiveSplitState state)
        {
            /*if (PreviousRun == null || PreviousRun.Count != state.Run.Count)
                return true;*/

            if (PreviousCalculationMode != state.Settings.SimpleSumOfBest)
                return true;

            if (PreviousTimingMethod != state.CurrentTimingMethod)
                return true;

            /*foreach (var segment in state.Run)
            {
                var prevSegment = PreviousRun[state.Run.IndexOf(segment)];
                if (prevSegment.PersonalBestSplitTime.RealTime != segment.PersonalBestSplitTime.RealTime ||
                    prevSegment.BestSegmentTime.RealTime != segment.BestSegmentTime.RealTime ||
                    prevSegment.PersonalBestSplitTime.GameTime != segment.PersonalBestSplitTime.GameTime ||
                    prevSegment.BestSegmentTime.GameTime != segment.BestSegmentTime.GameTime)
                    return true;
            }*/
            return false;
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.ToArgb() != Color.Transparent.ToArgb())
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawBackground(g, state, width, VerticalHeight);

            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            Formatter.Accuracy = Settings.Accuracy;

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawBackground(g, state, HorizontalWidth, height);

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            Formatter.Accuracy = Settings.Accuracy;

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public float VerticalHeight
        {
            get { return InternalComponent.VerticalHeight; }
        }

        public float MinimumWidth
        {
            get { return InternalComponent.MinimumWidth; }
        }

        public float HorizontalWidth
        {
            get { return InternalComponent.HorizontalWidth; }
        }

        public float MinimumHeight
        {
            get { return InternalComponent.MinimumHeight; }
        }

        public string ComponentName
        {
            get { return "Sum of Best"; }
        }


        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }


        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }


        public void RenameComparison(string oldName, string newName)
        {
        }


        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (CheckIfRunChanged(state))
                UpdateSumOfBestValue(state);

            InternalComponent.TimeValue = SumOfBestValue;

            Cache.Restart();
            Cache["TimeValue"] = InternalComponent.ValueLabel.Text;

            if (invalidator != null && Cache.HasChanged)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void Dispose()
        {
            CurrentState.OnSplit -= state_OnSplit;
            CurrentState.OnUndoSplit -= state_OnUndoSplit;
            CurrentState.OnReset -= state_OnReset;
        }
    }
}
