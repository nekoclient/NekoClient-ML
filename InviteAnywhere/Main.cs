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

[assembly: MelonInfo(typeof(NekoClient.InviteAnywhere),
        "NekoClient:InviteAnywhere",
        "1.0.0",
        "avail",
        "https://github.com/nekoclient/NekoClient-ML/releases")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace NekoClient
{
    public class InviteAnywhere : MelonMod
    {
        public override void OnUpdate()
        {
        }
    }
}
