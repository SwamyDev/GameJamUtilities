using UnityEngine;
using System.Collections;

namespace JamUtilities {

public class ControllerMovement : MonoBehaviour 
{
	public PlayerIdAttribute playerId;
	[SerializeField]
	private Vector3 horizontalDirection = Vector3.right;
	[SerializeField]
	private Vector3 verticalDirection = Vector3.forward;

	private IMoveable moveable;
	private string horizontalAxis;
	private string verticalAxis;

	void Awake()
	{
		moveable = gameObject.GetInterface<IMoveable>();
		horizontalAxis = string.Format("{0}Horizontal", playerId.Get().inputPrefix);
		verticalAxis = string.Format("{0}Vertical", playerId.Get().inputPrefix);
	}


	void Update()
	{
		float horizontal = Input.GetAxis(horizontalAxis);
		float vertical = Input.GetAxis(verticalAxis);

		Vector3 movement = horizontalDirection * horizontal + verticalDirection * vertical;
		moveable.Move(movement);
	}
}

}	// namespace JamUtilities