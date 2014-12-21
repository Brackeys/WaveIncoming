using UnityEngine;

class AudioUpdater: MonoBehaviour
{
	#region Members

	private Transform m_follow = null;
	public Transform FollowTransform
	{
		get
		{
			return m_follow;
		}
		set
		{
			m_follow = value;
		}
	}

	#endregion

	#region Messages

	public void Update()
	{
		if ((FollowTransform != null) && (gameObject != null) && (gameObject.transform != null))
		{
			gameObject.transform.position = m_follow.position;
		}
	}

	#endregion
}