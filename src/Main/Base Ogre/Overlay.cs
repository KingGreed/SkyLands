using System;
using Mogre;


namespace Game.BaseApp
{
    public class Overlay
    {
        protected RenderWindow   mWindow;
        protected float          timeSinceLastDebugUpdate = 1;
        protected OverlayElement mGuiAvg;
        protected OverlayElement mGuiCurr;
        protected OverlayElement mGuiBest;
        protected OverlayElement mGuiWorst;
        protected OverlayElement mGuiTris;
        protected OverlayElement mModesText;
        protected string         mAdditionalInfo = "";



        public Overlay(RenderWindow window)
        {
            this.mWindow = window;

            var debugOverlay = OverlayManager.Singleton.GetByName("Core/DebugOverlay");
            debugOverlay.Show();

            this.mGuiAvg    = OverlayManager.Singleton.GetOverlayElement("Core/AverageFps");
            this.mGuiCurr   = OverlayManager.Singleton.GetOverlayElement("Core/CurrFps");
            this.mGuiBest   = OverlayManager.Singleton.GetOverlayElement("Core/BestFps");
            this.mGuiWorst  = OverlayManager.Singleton.GetOverlayElement("Core/WorstFps");
            this.mGuiTris   = OverlayManager.Singleton.GetOverlayElement("Core/NumTris");
            this.mModesText = OverlayManager.Singleton.GetOverlayElement("Core/NumBatches");
        }

        public string AdditionalInfo { set { mAdditionalInfo = value; } get { return mAdditionalInfo; } }

        public void Update(float timeFragment)
        {
            if (timeSinceLastDebugUpdate > 0.5f)
            {
                var stats = mWindow.GetStatistics();

                this.mGuiAvg.Caption    = "Average FPS: "    + stats.AvgFPS;
                this.mGuiCurr.Caption   = "Current FPS: "    + stats.LastFPS;
                this.mGuiBest.Caption   = "Best FPS: "       + stats.BestFPS  + " " + stats.BestFrameTime  + " ms";
                this.mGuiWorst.Caption  = "Worst FPS: "      + stats.WorstFPS + " " + stats.WorstFrameTime + " ms";
                this.mGuiTris.Caption   = "Triangle Count: " + stats.TriangleCount;
                mModesText.Caption = mAdditionalInfo;

                timeSinceLastDebugUpdate = 0;
            }
            else
            {
                timeSinceLastDebugUpdate += timeFragment;
            }
        }
    }
}