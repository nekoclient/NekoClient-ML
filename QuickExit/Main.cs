using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;
using MelonLoader;
using Object = UnityEngine.Object;
using UnhollowerRuntimeLib;

[assembly: MelonInfo(typeof(NekoClient.QuickExit),
        "NekoClient:QuickExit",
        "1.0.0",
        "avail",
        "https://github.com/nekoclient/NekoClient-ML/releases")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace NekoClient
{
    public class QuickExit : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.Backspace) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
