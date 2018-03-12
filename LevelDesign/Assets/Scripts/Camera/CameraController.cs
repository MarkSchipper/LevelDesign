using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{

    public class CameraController : MonoBehaviour
    {

        [System.Serializable]
        public class PositionSettings
        {
            public Vector3 targetPosOffset = new Vector3(0, 1, 0);
            public float lookSmooth = 100f;
            public float distance = -8;
            public float zoomSmooth = 10;
            public float maxZoom = -2;
            public float minZoom = -15;
            public bool smoothFollow = true;
            public float smooth = 0.05f;

            //[HideInInspector]
            public float newDistance = -8;

            //[HideInInspector]
            public float adjustmentDistance = -8;
        }

        [System.Serializable]
        public class OrbitSettings
        {
            public float xRotation = -20;
            public float yRotation = -180;
            public float maxXRotation = 25;
            public float minXRotation = -85;
            public float vOrbitSmooth = 150;
            public float hOrbitSmooth = 150;
        }

        [System.Serializable]
        public class InputSettings
        {
            public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
            public string ORBIT_HORIZONTAL = "OrbitHorizontal";
            public string ORBIT_VERTICAL = "OrbitVertical";
            public string ZOOM = "Mouse ScrollWheel";
        }

        [System.Serializable]
        public class DebugSettings
        {
            public bool drawDesiredCollisionLines = true;
            public bool drawAdjustedCollisionLines = true;
        }

        public PositionSettings position = new PositionSettings();
        public OrbitSettings orbit = new OrbitSettings();
        public InputSettings input = new InputSettings();
        public CollisionHandler collision = new CollisionHandler();
        public DebugSettings debug = new DebugSettings();

        private Transform _target;
        private Vector3 targetPos = Vector3.zero;
        private Vector3 destination = Vector3.zero;

        private Vector3 adjustedDestination = Vector3.zero;
        private Vector3 cameraVelocity = Vector3.zero;

        private static bool _isCameraShake = false;
        private static float _shakeIntensity;
        private static float _shakeDuration;
        private Vector3 _oldPosition;


        private bool _orbitCamera = false;

        private float vOrbitInput, hOrbitInput, zoomInput;

        private static bool _firstPerson = false;
        private float _rotateAngleX = 0f;
        private float _rotateAngleY = 0f;

        public static CameraController instance;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
           // DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            SetCameraTarget(transform.parent.transform);

            MoveToTarget();

            collision.Initialize(Camera.main);
            collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
            collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
        }

        void SetCameraTarget(Transform t)
        {
            _target = t;
            
        }

        void GetInput()
        {
            if (!_firstPerson)
            {
                if (Input.GetMouseButton(1))
                {
                    _orbitCamera = true;
                    vOrbitInput = Input.GetAxis(input.ORBIT_VERTICAL);
                    hOrbitInput = Input.GetAxis(input.ORBIT_HORIZONTAL);
                    Cursor.lockState = CursorLockMode.Locked;
                    if (!CombatSystem.PlayerController.instance.ReturnPlayerDead())
                    {
                        PlayerController.instance.RotatePlayer(-hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime);
                    }

                }
                if (Input.GetMouseButtonUp(1))
                {
                    _orbitCamera = false;
                    Cursor.lockState = CursorLockMode.None;
                }

                zoomInput = Input.GetAxisRaw(input.ZOOM);
            }
            else
            {
                vOrbitInput = Input.GetAxis(input.ORBIT_VERTICAL);
                hOrbitInput = Input.GetAxis(input.ORBIT_HORIZONTAL);
                if (!CombatSystem.PlayerController.instance.ReturnPlayerDead())
                {
                    PlayerController.instance.RotatePlayer(-hOrbitInput);
                    RotateCamera(vOrbitInput, -hOrbitInput);
                }
                Cursor.visible = false;
            }
        }

        void Update()
        {
            GetInput();
            if (!CombatSystem.PlayerController.instance.ReturnPlayerDead())
            {
            
                //OrbitTarget();
                ZoomInOnTarget();
                MoveToTarget();
                if (!_firstPerson)
                {
                    LookAtTarget();
                }
            }
            if(CombatSystem.PlayerController.instance.ReturnPlayerDead())
            {
                Cursor.visible = true;
            }
          
        }

        void FixedUpdate()
        {
            if (!CombatSystem.PlayerController.instance.ReturnPlayerDead())
            {
                // LookAtTarget();
                OrbitTarget();

                collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
                collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);


                for (int i = 0; i < 5; i++)
                {
                    if (debug.drawDesiredCollisionLines)
                    {
                        Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
                    }
                    if (debug.drawAdjustedCollisionLines)
                    {
                        Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
                    }
                }


                collision.CheckColliding(targetPos);
                if (collision.colliding)
                {

                    if (collision.ReturnCollidedWith() != null)
                    {
                        if (collision.ReturnCollidedWith().GetComponent<Renderer>() != null)
                        {
                            collision.ReturnCollidedWith().GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
                        }
                        else
                        {
                            if (collision.ReturnCollidedWith().GetComponentInParent<Renderer>() != null)
                            {
                                collision.ReturnCollidedWith().GetComponentInParent<Renderer>().material.color = new Color(1, 1, 1, 0);
                            }
                            if (collision.ReturnCollidedWith().transform.parent.GetComponentInChildren<Renderer>() != null)
                            {
                                collision.ReturnCollidedWith().transform.parent.GetComponentInChildren<Renderer>().material.color = new Color(1, 1, 1, 0);
                            }
                        }
                    }
                }
                if (!collision.colliding)
                {
                    if (collision.ReturnCollidedWith() != null)
                    {
                        if (collision.ReturnCollidedWith().GetComponent<Renderer>() != null)
                        {
                            collision.ReturnCollidedWith().GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                        }
                        else
                        {
                            if (collision.ReturnCollidedWith().GetComponentInParent<Renderer>() != null)
                            {
                                collision.ReturnCollidedWith().GetComponentInParent<Renderer>().material.color = new Color(1, 1, 1, 1);
                            }
                            if (collision.ReturnCollidedWith().transform.parent.GetComponentInChildren<Renderer>() != null)
                            {
                                collision.ReturnCollidedWith().transform.parent.GetComponentInChildren<Renderer>().material.color = new Color(1, 1, 1, 1);
                            }
                        }
                    }
                }

                if (_isCameraShake)
                {
                    ShakeIt();
                }
            }
        }

        public void SetPostProfile(string name)
        {
            if (name != string.Empty)
            {
                if (Resources.Load("PostProfiles/" + name) != null)
                {
                    GetComponentInChildren<UnityEngine.PostProcessing.PostProcessingBehaviour>().profile = Resources.Load("PostProfiles/" + name) as UnityEngine.PostProcessing.PostProcessingProfile;
                }
                else
                {
                    Debug.Log("No Post Processing Profile excists with the name " + name);
                }
            }

        }

        public void SetLevelSettings(float _clip)
        {
            Camera.main.farClipPlane = _clip;
        }

        void MoveToTarget()
        {
            if (!_firstPerson)
            {
                targetPos = _target.position + position.targetPosOffset;
                destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + _target.eulerAngles.y, 0) * -Vector3.forward * position.distance;
                destination += targetPos;
                transform.position = destination;

                if (collision.colliding)
                {
                    adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + _target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
                    adjustedDestination += targetPos;

                    if (position.smoothFollow)
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref cameraVelocity, position.smooth);

                    }
                    else
                    {
                        transform.position = adjustedDestination;
                    }
                }
                else
                {
                    if (position.smoothFollow)
                    {
                        transform.position = Vector3.SmoothDamp(transform.position, destination, ref cameraVelocity, position.smooth);
                    }
                    else
                    {
                        transform.position = destination;
                    }
                }
            }
            else
            {
                targetPos = new Vector3(_target.position.x, _target.position.y + 2.7f, _target.position.z);
                transform.position = targetPos;
            }

        }

        void LookAtTarget()
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
        }

        void RotateCamera(float x, float y)
        {
            _rotateAngleX += x;
            _rotateAngleY += y;
            transform.rotation = Quaternion.Euler(new Vector3(_rotateAngleX, _rotateAngleY, 0));
        }

        void OrbitTarget()
        {
            if (_orbitCamera)
            {
                /*
                if (hOrbitSnapInput > 0)
                {
                    orbit.yRotation = -180;
                }
                */
                orbit.xRotation += -vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime;
                //orbit.yRotation += -hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime;

                if (orbit.xRotation > orbit.maxXRotation)
                {
                    orbit.xRotation = orbit.maxXRotation;
                }

                if (orbit.xRotation < orbit.minXRotation)
                {
                    orbit.xRotation = orbit.minXRotation;
                }
            }
        }

        void OrbitToStart(int _multiplier)
        {            
            orbit.yRotation += ((-orbit.hOrbitSmooth * (Time.deltaTime / 5)) * _multiplier);
        }

        void ZoomInOnTarget()
        {
            position.distance += zoomInput * position.zoomSmooth * Time.deltaTime;

            if (position.distance > position.maxZoom)
            {
                position.distance = position.maxZoom;
            }
            if (position.distance < position.minZoom)
            {
                position.distance = position.minZoom;
            }
        }

        public static void CameraShake(float _intensity, float _duration)
        {
            _isCameraShake = !_isCameraShake;
            _shakeIntensity = _intensity;
            _shakeDuration = _duration;

        }

        void ShakeIt()
        {
            StartCoroutine(CancelCameraShake());
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x + Random.Range(_shakeIntensity * -1, _shakeIntensity), transform.position.y + Random.Range(_shakeIntensity * -1, _shakeIntensity), transform.position.z + Random.Range(_shakeIntensity * -1, _shakeIntensity)), Time.deltaTime * 1);
        }

        IEnumerator CancelCameraShake()
        {
            _oldPosition = transform.position;
            yield return new WaitForSeconds(_shakeDuration);
            _isCameraShake = false;
            transform.position = _oldPosition;
        }

        public static bool ReturnFirstPerson()
        {
            return _firstPerson;
        }

        [System.Serializable]
        public class CollisionHandler
        {
            public LayerMask collisionLayer;

            [HideInInspector]
            public bool colliding = false;
            [HideInInspector]
            public Vector3[] adjustedCameraClipPoints;

            [HideInInspector]
            public Vector3[] desiredCameraClipPoints;

            [SerializeField]
            private GameObject _collidedGameObject = null;

            Camera camera;

            public void Initialize(Camera cam)
            {
                camera = cam;
                adjustedCameraClipPoints = new Vector3[5];
                desiredCameraClipPoints = new Vector3[5];
            }

            public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
            {
                if (!camera)
                    return;

                intoArray = new Vector3[5];

                float z = camera.nearClipPlane;
                float x = Mathf.Tan(camera.fieldOfView / 2) * z;
                float y = x / camera.aspect;

                // top left
                // rotate point relative to camera
                intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;

                // top right
                intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;

                intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;

                intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;

                intoArray[4] = cameraPosition - camera.transform.forward;
            }

            bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
            {
                for (int i = 0; i < clipPoints.Length; i++)
                {
                    Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                    float distance = Vector3.Distance(clipPoints[i], fromPosition);

                    if (Physics.Raycast(ray, distance, collisionLayer))
                    {
                        return true;
                    }

                }
                return false;

            }

            public GameObject ReturnCollidedWith()
            {
                return _collidedGameObject;
            }

            public float GetAdjustedDistanceWithRay(Vector3 from)
            {
                float distance = -1f;

                for (int i = 0; i < desiredCameraClipPoints.Length; i++)
                {
                    Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {

                        Debug.Log(hit.collider.name);
                        //_collidedGameObject = hit.collider.gameObject;


                        if (distance == -1)
                        {
                            distance = hit.distance;
                        }
                        else
                        {
                            if (hit.distance < distance)
                            {
                                distance = hit.distance;

                            }
                        }
                    }
                }


                if (distance == -1)
                {
                    return 0;
                }
                else
                    return distance;

            }

            public void CheckColliding(Vector3 targetPosition)
            {
                if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
                {
                    colliding = true;
                }
                else
                {
                    colliding = false;
                }
            }

        }

    }
}
