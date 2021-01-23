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
using InputEnum = VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique;

[assembly: MelonInfo(typeof(NekoClient.NoClip),
        "NekoClient:NoClip",
        "1.0.0",
        "avail",
        "https://github.com/nekoclient/NekoClient-ML/releases")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace NekoClient
{
    public class NoClip : MelonMod
    {
        enum Oculus
        {
            AButton = KeyCode.JoystickButton0,
            BButton = KeyCode.JoystickButton1,
            XButton = KeyCode.JoystickButton2,
            YButton = KeyCode.JoystickButton3,
            Start = KeyCode.JoystickButton7,
            LeftThumbstickPress = KeyCode.JoystickButton8,
            RightThumbstickPress = KeyCode.JoystickButton9,
            LeftTrigger = KeyCode.JoystickButton14,
            RightTrigger = KeyCode.JoystickButton15,
            LeftThumbstickTouch = KeyCode.JoystickButton16,
            RightThumbstickTouch = KeyCode.JoystickButton17,
            LeftThumbRestTouch = KeyCode.JoystickButton18,
            RightThumbRestTouch = KeyCode.JoystickButton19
        }

        private float m_lastTime = 0;
        private bool m_airbreakActive = false;
        private Vector3 m_position = new Vector3(-1, -1, -1);
        private VRC.Player m_localPlayer;
        private float m_currentSpeed = 10.0f;
        private int m_speedIndex = 3;
        private Vector3 m_originalGravity = new Vector3(-1, -1, -1);

        private List<float> m_speeds = new List<float>()
        {
            0.5f,
            1.0f,
            5.0f,
            10.0f,
            20.0f,
            50.0f,
            100.0f
        };

        private void IgnoreCollision(bool ignore)
        {
            Object[] colliders = Object.FindObjectsOfType(Il2CppType.Of<Collider>());

            foreach (Object c in colliders)
            {
                Physics.IgnoreCollision(GameObject.FindWithTag("Player").transform.GetComponent(Il2CppType.Of<Collider>()).TryCast<Collider>(), c.TryCast<Collider>(), ignore);
            }
        }

        private void SetupAirbreak()
        {
            IgnoreCollision(true);
            m_position = m_localPlayer.transform.position;
            m_originalGravity = Physics.gravity;
            Vector3 newGravity = m_originalGravity;
            newGravity.y = 0;
            Physics.gravity = newGravity;
        }

        private void DisableAirbreak()
        {
            IgnoreCollision(false);
            Physics.gravity = m_originalGravity;
        }

        public override void OnUpdate()
        {
            if (m_localPlayer == null)
            {
                VRC.Player player = VRC.Player.prop_Player_0;

                if (player != null) // early init I guess
                {
                    m_localPlayer = player;
                }
            }

            if (m_localPlayer == null)
            {
                return; // wait until player is ready
            }

            var currentInput = VRCInputManager.field_Private_Static_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_0;

            bool controller = currentInput == InputEnum.Controller;
            bool vr = currentInput == InputEnum.Oculus || currentInput == InputEnum.Vive;
            bool desktop = (currentInput == InputEnum.Keyboard || currentInput == InputEnum.Mouse);

            bool isActiveController = controller && Input.GetKey(KeyCode.JoystickButton5);
            bool isActiveVr = vr && Input.GetKey((KeyCode)Oculus.LeftThumbstickPress);
            bool isActiveDesktop = desktop && (Input.GetKey(KeyCode.Mouse4) || Input.GetKey(KeyCode.RightControl));

            bool swapSpeedsController = controller && Input.GetKey(KeyCode.JoystickButton9);
            bool swapSpeedsVr = vr && Input.GetKey((KeyCode)Oculus.AButton);
            bool swapSpeedsKeyboard = desktop && Input.GetKey(KeyCode.LeftShift);

            bool isActive = isActiveController || isActiveVr || isActiveDesktop;
            bool swapSpeeds = swapSpeedsKeyboard || swapSpeedsController || swapSpeedsVr;

            if (isActive && Time.time - m_lastTime > 1f)
            {
                if (m_airbreakActive)
                {
                    DisableAirbreak();
                }
                else
                {
                    SetupAirbreak();
                }

                m_airbreakActive = !m_airbreakActive;

                m_lastTime = Time.time;
            }

            if (swapSpeeds && m_airbreakActive && Time.time - m_lastTime > 0.2f)
            {
                m_speedIndex += 1;

                if (m_speedIndex > m_speeds.Count() - 1)
                {
                    m_speedIndex = 0;
                }

                m_currentSpeed = m_speeds[m_speedIndex];
                m_lastTime = Time.time;
            }

            // get default fallback
            Object[] ctrls = Object.FindObjectsOfType(Il2CppType.Of<VRCVrCameraOculus>());

            Transform trans = null;

            if (ctrls.Length > 0)
            {
                trans = ctrls[0].TryCast<VRCVrCameraOculus>().transform;
            }

            // alright so
            // let's start by getting our current vrcPlayer
            VRCPlayer vrcPlayer = m_localPlayer.field_Internal_VRCPlayer_0;

            Animator animator = null;

            if (vrcPlayer == null)
            {
                animator = null;
            }
            else
            {
                // let's get our avatar manager
                VRCAvatarManager vrcAvatarManager = vrcPlayer.prop_VRCAvatarManager_0;

                if (vrcAvatarManager == null)
                {
                    animator = null;
                }
                else
                {
                    // current avatar
                    GameObject currentAvatar = vrcAvatarManager.prop_GameObject_0;
                    animator = ((currentAvatar != null) ? currentAvatar.GetComponent(Il2CppType.Of<Animator>())?.TryCast<Animator>() : null);
                }
            }

            // if the animator is not null at this stage and airbreak is enabled
            if (animator != null)
            {
                // get the head bone
                Transform tempTrans = animator.GetBoneTransform(HumanBodyBones.Head);

                // if we're humanoid
                if (tempTrans != null)
                {
                    // use the head bone's transform instead of oculus camera
                    trans = tempTrans;
                }
            }

            if (trans == null)
            {
                MelonLogger.LogError("not humanoid?");
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                m_position += trans.forward * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                m_position += (trans.right * -1) * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_position += (trans.forward * -1) * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                m_position += trans.right * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.E))
            {
                m_position += m_localPlayer.transform.up * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                m_position += (m_localPlayer.transform.up * -1) * m_currentSpeed * Time.deltaTime;
            }

            var a = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            if (a.y != 0.0f)
            {
                m_position += trans.forward * m_currentSpeed * Time.deltaTime * a.y;
            }

            if (a.x != 0.0f)
            {
                m_position += trans.right * m_currentSpeed * Time.deltaTime * a.x;
            }

            if (m_airbreakActive)
            {
                m_localPlayer.transform.position = m_position;
            }
        }
    }
}
