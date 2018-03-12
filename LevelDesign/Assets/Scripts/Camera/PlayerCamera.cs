using UnityEngine;
using System.Collections;

namespace CombatSystem
{

    public class PlayerCamera : MonoBehaviour
    {


        private Transform _centerPoint;

        private static bool _isCameraShake = false;
        private static float _shakeIntensity;
        private static float _shakeDuration;
        private Vector3 _oldPosition;

        // Use this for initialization
        void Start()
        {

            _centerPoint = transform.parent.transform;
            //_centerPoint = GameObject.Find("Camera_Target").transform;

        }

        // Update is called once per frame
        void Update()
        {

            transform.LookAt(_centerPoint);

        }

        void LateUpdate()
        {

            if (Input.GetKey("a"))
            {
                transform.RotateAround(_centerPoint.position, Vector3.up, 100 * Time.deltaTime);
            }

            if (Input.GetKey("d"))
            {
                transform.RotateAround(_centerPoint.position, Vector3.down, 100 * Time.deltaTime);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z + 0.2f);
                transform.Rotate(-2, 0, 0);

                //Vector3.Distance(transform.position, _centerPoint.transform.position);
                

            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z - 0.2f);
                transform.Rotate(2, 0, 0);
                //transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z - 2f), Time.deltaTime * 1);
            }

            if(_isCameraShake)
            {
                ShakeIt();
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

    }
}