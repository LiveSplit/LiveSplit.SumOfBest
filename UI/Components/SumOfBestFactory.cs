using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class SumOfBestFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Sum of Best"; }
        }

        public string Description
        {
            get { return "Displays the current sum of best segments."; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Information; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new SumOfBestComponent(state);
        }

        public string UpdateName
        {
            get { return ComponentName; }
        }

        public string XMLURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/Components/update.LiveSplit.SumOfBest.xml"; }
#else
            get { return "http://livesplit.org/update/Components/update.LiveSplit.SumOfBest.xml"; }
#endif
        }

        public string UpdateURL
        {
#if RELEASE_CANDIDATE
            get { return "http://livesplit.org/update_rc_sdhjdop/"; }
#else
            get { return "http://livesplit.org/update/"; }
#endif
        }

        public Version Version
        {
            get { return Version.Parse("1.5.0"); }
        }
    }
}
