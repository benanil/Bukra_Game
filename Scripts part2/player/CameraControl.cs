
using AnilTools;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Player
{
    public partial class CharacterMove
    {
        enum CameraState { normal, combo, minimap}
        enum MinimapCamState { moving, returning, goingUp}

        public float yaw, pitch;
        private float x , y;

        private Vector3 targetRotation;
        public LayerMask CameraBlockLayer; // for disabling camera bugs disable rays for enemies and player itself
        
        Ray cameraBckRay;
        bool cameraBlocked;
        float cameraDistanceTarget;
        static CameraState camState;
        [SerializeField] Camera MinimapCam;

        private void OnValidate()
        {
            if (!Application.isPlaying) HandleCamera();
        }

        private void FixedUpdate()  {
            CalcRay(); 
        }

        private void CalcRay() {
            
            if (camState == CameraState.normal)
            {
                cameraBckRay = new Ray(mainCam.transform.position + (mainCam.transform.forward * cameraDistanceTarget), -mainCam.transform.forward);
                cameraDistanceTarget = currentCameraDistance;
                // camera needs to be closer when wall behind of it
                cameraBlocked = Physics.Raycast(cameraBckRay, out RaycastHit hit, cameraDistanceTarget, CameraBlockLayer);
                cameraDistanceTarget = cameraBlocked && hit.distance > 1.8f ? hit.distance - 0.1f : cameraDistanceTarget; // 0.1 will fix camera getting inside of obstacles
            }
        }
        
        private void LateUpdate() {
            if (!canLook && GameMenu.PlayerOnMenu) return;
            HandleCamera();
            x = 0; y = 0;
        }

        private void HandleCamera()
        {
            //test combo camera
            // if (Input.GetKeyDown(KeyCode.C)) ComboCamera();
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (camState == CameraState.normal)
                {
                    SetMinimapCamera();
                }
                else if(camState == CameraState.minimap)
                {
                    miniCamState = MinimapCamState.returning;
                    mainCam.GetComponent<Camera>().orthographic = false;
                }
            }

            // in this state camera lerps to minimap position and player use it as minimap
            if (camState == CameraState.minimap)
            {
                MinimapState();
                return;
            }
            else if (camState == CameraState.normal && Application.isPlaying)
            { 
                x = Mathf.Lerp(x, Input.GetAxis("Mouse X"), Time.deltaTime * lookSpeed);
                y = Mathf.Lerp(y, Input.GetAxis("Mouse Y"), Time.deltaTime * lookSpeed);
                yaw += x; pitch -= y;
                pitch = Mathf.Clamp(pitch, lookYlimit.x, lookYlimit.y);

                // gta 4 camera effect reprezentation
                if (pitch < -5 && !cameraBlocked ) {
                    //var cameraDistanceTargetMin = 2;
                    cameraDistanceTarget = Mathf.Lerp(2, currentCameraDistance, Mathmatic.Remap(pitch, lookYlimit.x, -5));
                    mainCam.fieldOfView = Mathf.Lerp(85, 65, Mathmatic.Remap(pitch, lookYlimit.x, -5));
                }
                else
                {
                    cameraDistanceTarget = currentCameraDistance;
                }

                // stick camera to characters behind
                if (Horse.HorseAI.PlayerOnHorse && Horse.HorseAI.currentHorse.currentSpeed >= 8 ) // 8 = (byte)Horse.MoveState.fastWalking + 2
                {
                    yaw = Mathf.LerpAngle(yaw, transform.eulerAngles.y, Time.deltaTime * 30);
                }
            }
            else if (camState == CameraState.combo)
            {
                yaw = Mathf.Lerp(yaw, targetYaw, Time.unscaledDeltaTime * .48f);//
                cameraDistanceTarget = Mathf.Lerp(cameraDistanceTarget, comboCameraDist, Time.unscaledDeltaTime * 0.5f); 
            }

            targetRotation = new Vector3(pitch, yaw);
            mainCam.transform.eulerAngles = targetRotation;
            
            if (cameraTarget)
            { 
                mainCam.transform.position = cameraTarget.position + PositionAnchor - mainCam.transform.forward * cameraDistanceTarget + Vector3.up * CurrentCameraHeight;
            }
        }
        
        // COMBO SECTION
        float targetYaw, comboCameraDist;
        
        public void ComboCamera()
        {
            if (camState != CameraState.normal) return;

            float comboDistStart = cameraDistanceTarget;
            Vector3 beforeAnchor = PositionAnchor;

            Time.timeScale = 0.38f;
            camState = CameraState.combo;
            targetYaw    = yaw + 90;
            PositionAnchor = transform.forward * 1.57f;
            comboCameraDist = 3.5f;
            x = 0; y = 0;

            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitForSeconds(2.2f);
                Time.timeScale = .88f;
                yield return new WaitForSeconds(3.5f);
                Time.timeScale = 1f;
                PositionAnchor = beforeAnchor;
                cameraDistanceTarget = comboDistStart;
                camState = CameraState.normal;
            }
        }


        // MINI MAP SECTION
        private Vector2 minimapPos;
        private Vector3 camBeforePos; 
        private Quaternion camBeforeRot;
        private MinimapCamState miniCamState;

        public bool SetMinimapCamera()
        {
            if (camState == CameraState.combo || 
                PlayerInfo.CurrentHorse != null && PlayerInfo.CurrentHorse.currentSpeed > 4) return false;
            
            if (PlayerInfo.PlayerOnHorse)
            {
                PlayerInfo.CurrentHorse.StopHorse();
            }
            animatior.SetFloat(AnimPool.InputY, 0);
            minimapPos = Vector2.zero;
            camState = CameraState.minimap;
            miniCamState = MinimapCamState.goingUp;
            camBeforePos = mainCam.transform.position;
            camBeforeRot = mainCam.transform.rotation;
            return true;
        }

        public static bool UsingMinimap() => camState == CameraState.minimap;
        public static bool ComboCameraMode() => camState == CameraState.combo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool MoveCam(in Vector3 targetPos, in Quaternion rotation)
        {
            mainCam.transform.SetPositionAndRotation(
                                Vector3.Lerp(mainCam.transform.position, targetPos, Time.deltaTime * 3),
                                Quaternion.Slerp(mainCam.transform.rotation, rotation, Time.deltaTime * 3));

            if (Vector3.Distance(mainCam.transform.position, targetPos) < .15f &&
                Quaternion.Angle(mainCam.transform.rotation, rotation) < 1)
            {
                mainCam.transform.SetPositionAndRotation(targetPos, rotation);
                return true;
            }
            return false;
        }
        
        private void MinimapState()
        {
            Vector3 targetPos = MinimapCam.transform.position;
            targetPos.x += minimapPos.x;
            targetPos.z += minimapPos.y;

            if (miniCamState == MinimapCamState.goingUp)
            {
                if (MoveCam(targetPos, Quaternion.Euler(90, 0, 0)))
                {
                    miniCamState = MinimapCamState.moving;
                    mainCam.GetComponent<Camera>().orthographic = true;
                    mainCam.GetComponent<Camera>().orthographicSize = MinimapCam.orthographicSize / 2;
                }
            }
            else if (miniCamState == MinimapCamState.returning)
            {
                if (MoveCam(camBeforePos, camBeforeRot))
                {
                    camState = CameraState.normal;
                    if (PlayerInfo.PlayerOnHorse)
                    {
                        PlayerInfo.CurrentHorse.StartHorse();
                    }
                }
            }
            else if (miniCamState == MinimapCamState.moving)
            {
                float shiftSpeed = Input.GetKey(KeyCode.LeftShift) ? 4 : 1;
                minimapPos.x += Input.GetAxis("Horizontal") * Time.deltaTime * 8.8f * shiftSpeed;
                minimapPos.y += Input.GetAxis("Vertical"  ) * Time.deltaTime * 8.8f * shiftSpeed;
                var camera = mainCam.GetComponent<Camera>();
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - (Input.mouseScrollDelta.y * 2), 1, 800);
                mainCam.transform.position = targetPos;
            }
        }

    }
}