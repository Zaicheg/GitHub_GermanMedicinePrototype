     var target : Transform;
     var distance : float= 5.0f;
     var xSpeed : float= 5.0f;
     var ySpeed : float= 5.0f;
     
     var yMinLimit : float= 0f;
     var yMaxLimit : float= 360f;
     
     var distanceMin : float= 2.5f;
     var distanceMax : float= 10f;
     
     var smoothTime : float= 2f;
	 var panSpeed : float;
	 
	 var speedCoefficient : float;
     
     private var rotationYAxis : float= 0.0f;
     private var rotationXAxis : float= 0.0f;
     
     private var velocityX : float= 0.0f;
     private var velocityY : float= 0.0f;
	 
     private var _position : Vector3;
     private var _rotation : Quaternion;
	 
	 var maxSpeed : float = 1.0f;
	 
	 private var mouseX : float;
	 private var mouseY : float;
	 
	 private var min : float = -1;
	 private var max : float = 1;
	 
	private var targetZoom : float;
	var zoomSpeed : float = 5.0f;
	
	

   function Start () 
	{
	var angles : Vector3= transform.eulerAngles;
    rotationYAxis = angles.y;
    rotationXAxis = angles.x;
	velocityX = 0;
	velocityY = 0;
	targetZoom = distance;
	
    }

     
    function LateUpdate () 
	{
    if (target)
    {
    if (Input.GetMouseButton(1))
    {
	mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), min, max);
	mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), min, max);
    velocityX += xSpeed * mouseX;
    velocityY += ySpeed * mouseY;
	
    }
     
    rotationYAxis += velocityX;
    rotationXAxis -= velocityY;
     
    rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
     
     var fromRotation : Quaternion= Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
     var toRotation : Quaternion= Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
     _rotation = toRotation;
	 
	 
	 
	
	 if(Input.GetAxis("Mouse ScrollWheel"))
	 {
	 targetZoom -= Input.GetAxis("Mouse ScrollWheel")*zoomSpeed;
	 
	 }
targetZoom = Mathf.Clamp(targetZoom, distanceMin, distanceMax);
		 
	distance = Mathf.Lerp(distance, targetZoom, maxSpeed * Time.deltaTime);
	distance = Mathf.Clamp(distance, distanceMin, distanceMax);
	
   
     var negDistance : Vector3= new Vector3(0.0f, 0.0f, -distance);
	_position = _rotation * negDistance + target.position;
     
    transform.rotation = _rotation;
    transform.position = _position;
	
     
    velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
    velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
    
	
	if (Input.GetMouseButton(2))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
			
			target.transform.position = Vector3(Mathf.Clamp(target.transform.position.x, -3, 0.6), Mathf.Clamp(target.transform.position.y, -3.5, 2.5), Mathf.Clamp(target.transform.position.z, -1.8, 1.7));
        }

	
	}
     
    }
     
    static function ClampAngle ( angle : float ,   min : float ,   max : float  ) : float 
	{
    if (angle < -360F)
    angle += 360F;
    if (angle > 360F)
    angle -= 360F;
    return Mathf.Clamp(angle, min, max);
    }

	

	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	

