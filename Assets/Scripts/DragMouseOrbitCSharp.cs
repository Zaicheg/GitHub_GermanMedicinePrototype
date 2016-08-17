using UnityEngine;
using System.Collections;

public class DragMouseOrbitCSharp : MonoBehaviour
{
	public Transform Target;
	public float Distance = 5.0f;
	public float XSpeed = 5.0f;
	public float YSpeed = 5.0f;

	public float YMinLimit;
	public float YMaxLimit = 360f;

	public float DistanceMin = 2.5f;
	public float DistanceMax = 10f;

	public float SmoothTime = 2f;
	public float PanSpeed;

	public float SpeedCoefficient;

	private float _rotationYAxis;
	private float _rotationXAxis;

	private float _velocityX;
	private float _velocityY;

	private Vector3 _position;
	private Quaternion _rotation;

	public float MaxSpeed = 1.0f;

	private float _mouseX;
	private float _mouseY;

	private const float Min = -10f;
	private const float Max = 10f;

	public float _targetZoom;
	public float ZoomSpeed = 5.0f;

	private void Start()
	{
		var angles = transform.eulerAngles;
		_rotationYAxis = angles.y;
		_rotationXAxis = angles.x;
		_velocityX = 0f;
		_velocityY = 0f;
		_targetZoom = Distance;
	}

	public void Zoom(float value)
	{
		_targetZoom -= value * ZoomSpeed;
	}

	public void Rotate(float valueX, float valueY)
	{
		_mouseX = Mathf.Clamp(valueX, Min, Max);
		_mouseY = Mathf.Clamp(valueY, Min, Max);

		_velocityX += XSpeed * _mouseX;
		_velocityY += YSpeed * _mouseY;
	}

	private void LateUpdate()
	{
		if (Target)
		{
			_rotationYAxis += _velocityX;
			_rotationXAxis -= _velocityY;

			_rotationXAxis = ClampAngle(_rotationXAxis, YMinLimit, YMaxLimit);

			//var fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			var toRotation = Quaternion.Euler(_rotationXAxis, _rotationYAxis, 0);
			_rotation = toRotation;

			_targetZoom = Mathf.Clamp(_targetZoom, DistanceMin, DistanceMax);

			Distance = Mathf.Lerp(Distance, _targetZoom, MaxSpeed * Time.deltaTime);
			Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);

			var negDistance = new Vector3(0.0f, 0.0f, -Distance);
			_position = _rotation * negDistance + Target.position;

			transform.rotation = _rotation;
			transform.position = _position;

			_velocityX = Mathf.Lerp(_velocityX, 0, Time.deltaTime * SmoothTime);
			_velocityY = Mathf.Lerp(_velocityY, 0, Time.deltaTime * SmoothTime);

			if (Input.GetMouseButton(2))
			{
				//grab the rotation of the camera so we can move in a psuedo local XY space
				Target.rotation = transform.rotation;
				Target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * PanSpeed);
				Target.Translate(transform.up * -Input.GetAxis("Mouse Y") * PanSpeed, Space.World);

				Target.transform.position = new Vector3(Mathf.Clamp(Target.transform.position.x, -3f, 0.6f), Mathf.Clamp(Target.transform.position.y, -3.5f, 2.5f), Mathf.Clamp(Target.transform.position.z, -1.8f, 1.7f));
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}
