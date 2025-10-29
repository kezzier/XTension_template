using RGiesecke.DllExport;
using System;

namespace XTensions
{
    public class Template
    {
        private static bool running;

        ///////////////////////////////////////////////////////////////
        /// X-Ways X-Tension calls
        /// 

        [DllExport]
        public static int XT_Init(CallerInformation nVersion, CallingProgramCapabilities nFlags, IntPtr hMainWnd, IntPtr lpReserved)
        {
            // If importing functions fails, we return -1 to prevent further use of the X-Tension.
            if (!ImportedMethods.Import())
            {
                HelperMethods.OutputMessage($"{DateTime.Now.TimeOfDay}: X-Tension - failed to import API methods.");
                return -1;
            }

            running = false;
            return 1;
        }

        [DllExport]
        public static int XT_About(IntPtr hParentWnd, IntPtr lpReserved)
        {
            HelperMethods.OutputMessage("");
            return 0;
        }

        /// <summary>
        /// X-Tension must be initiated through ...
        /// ...
        /// </summary>
        /// <param name="nOpType">How X-Tension is initiated.</param> 
        [DllExport]
        public static int XT_Prepare(IntPtr hVolume, IntPtr hEvidence, XTensionActionSource nOpType, IntPtr lpReserved)
        {
            running = true;
            if (nOpType == XTensionActionSource.VolumeSnapshotRefinement || nOpType == XTensionActionSource.DirectoryBrowserContextMenu)
            {
                HelperMethods.OutputMessage($"{DateTime.Now.TimeOfDay}: Starting X-Tension");

                return 1;
            }
            else
            {
                HelperMethods.OutputMessage("This X-Tension can only be run from refine volume snapshot or by right clicking a selection of items.");
                return -3;
            }
        }

        /// <summary>
        /// ...
        /// </summary>
        [DllExport]
        public static long XT_ProcessItemEx(int nItemID, IntPtr hItem, IntPtr lpReserved)
        {
            if (CommonMethods.ShouldStop() == -1) return -1;

            return 0;
        }

        /// <summary>
        /// After all items are processed.
        /// </summary>
        [DllExport]
        public static int XT_Finalize(IntPtr hVolume, IntPtr hEvidence, XTensionActionSource nOpType, IntPtr lpReserved)
        {
            return 0;
        }

        [DllExport]
        public static int XT_Done(IntPtr lpReserved)
        {
            if (running)
            {
                HelperMethods.OutputMessage($"{DateTime.Now.TimeOfDay}: X-Tension finished");
            }
            else HelperMethods.OutputMessage("[XT] Dll loaded!");
            return 0;
        }
    }
}
